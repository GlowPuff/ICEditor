﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class FileManager
	{
		public FileManager() { }

		/// <summary>
		/// Checks if the base save folder exists (Documents/ImperialCommander) and creates it if not
		/// </summary>
		private static bool CreateBaseDirectory()
		{
			string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

			if ( !Directory.Exists( basePath ) )
			{
				var di = Directory.CreateDirectory( basePath );
				if ( di == null )
				{
					MessageBox.Show( "Could not create the base project folder.\r\nTried to create: " + basePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// saves a mission to base project folder
		/// </summary>
		public static bool Save( Mission mission, bool saveAs )
		{
			string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

			if ( !CreateBaseDirectory() )
				return false;

			string filePath;//full path to the file, including filename
			if ( saveAs || string.IsNullOrEmpty( mission.fileName ) )
			{
				SaveFileDialog saveFileDialog = new();
				saveFileDialog.DefaultExt = ".json";
				saveFileDialog.Title = "Save Mission";
				saveFileDialog.Filter = "Mission File (*.json)|*.json";
				saveFileDialog.InitialDirectory = basePath;
				if ( saveFileDialog.ShowDialog() == true )
				{
					filePath = saveFileDialog.FileName;
					mission.fullPathToFile = filePath;
					//mission.relativePath = Path.GetRelativePath( basePath, new DirectoryInfo( filePath ).FullName );
				}
				else
					return false;
			}
			else
				filePath = mission.fullPathToFile;

			FileInfo fi = new( filePath );
			mission.fileName = fi.Name;
			mission.fullPathToFile = filePath;
			mission.saveDate = DateTime.Now.ToString( "M/d/yyyy" );
			mission.timeTicks = DateTime.Now.Ticks;
			mission.fileVersion = Utils.formatVersion;

			string output = JsonConvert.SerializeObject( mission, Formatting.Indented );
			Utils.Log( mission.fullPathToFile );
			try
			{
				using ( var stream = File.CreateText( mission.fullPathToFile ) )
				{
					stream.Write( output );
				}
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not save the Mission.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return false;
			}
			return true;
		}

		/// <summary>
		/// Loads a mission from its .json.
		/// Supply the FULL PATH with the filename
		/// </summary>
		public static Mission LoadMission( string filename )
		{
			string json = "";
			string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

			try
			{
				using ( StreamReader sr = new( filename ) )
				{
					json = sr.ReadToEnd();
				}

				var m = JsonConvert.DeserializeObject<Mission>( json );
				//overwrite fileName, relativePath and fileVersion properties so they are up-to-date
				FileInfo fi = new FileInfo( filename );
				m.fileName = fi.Name;
				m.fullPathToFile = fi.FullName;
				//m.relativePath = Path.GetRelativePath( basePath, new DirectoryInfo( filename ).FullName );
				m.fileVersion = Utils.formatVersion;
				return m;
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not load the Mission.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return null;
			}
		}

		public static Mission LoadMissionFromString( string json )
		{
			//make sure it's a mission, simple check for a property in the text
			if ( !json.Contains( "missionGUID" ) )
				return null;

			try
			{
				var m = JsonConvert.DeserializeObject<Mission>( json );
				//Utils.Log( "LoadMissionFromString: " + m.missionProperties.missionID );
				return m;
			}
			catch ( Exception e )
			{
				MessageBox.Show( "LoadMissionFromString()::Could not load the Mission.\n\nException:\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return null;
			}
		}

		/// <summary>
		/// "filename" is a full path, returns null on failure
		/// </summary>
		public static ProjectItem CreateProjectItem( string filename )
		{
			ProjectItem projectItem = new ProjectItem();

			var mission = LoadMissionFromString( File.ReadAllText( filename ) );
			if ( mission != null )
			{
				projectItem.fullPathWithFilename = filename;
				projectItem.fileName = new FileInfo( filename ).Name;
				projectItem.Title = mission.missionProperties.missionName;
				projectItem.Date = mission.saveDate;
				projectItem.fileVersion = mission.fileVersion;
				projectItem.timeTicks = mission.timeTicks;
				projectItem.Description = mission.missionProperties.missionDescription;
			}
			else
				return null;

			return projectItem;

			//ProjectItem projectItem = new();
			//FileInfo fi = new FileInfo( filename );
			//string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

			//string[] text = File.ReadAllLines( filename );
			//foreach ( var line in text )
			//{
			//	//manually parse each line
			//	string[] split = line.Split( ':' );
			//	if ( split.Length == 2 )
			//	{
			//		projectItem.fileName = fi.Name;
			//		projectItem.relativePath = Path.GetRelativePath( basePath, new DirectoryInfo( filename ).FullName );

			//		split[0] = split[0].Replace( "\"", "" ).Replace( ",", "" ).Trim();
			//		split[1] = split[1].Replace( "\"", "" ).Replace( ",", "" ).Trim();
			//		if ( split[0] == "missionName" )
			//			projectItem.Title = split[1];
			//		if ( split[0] == "saveDate" )
			//			projectItem.Date = split[1];
			//		if ( split[0] == "fileVersion" )
			//			projectItem.fileVersion = split[1];
			//		if ( split[0] == "timeTicks" )
			//			projectItem.timeTicks = long.Parse( split[1] );
			//	}
			//	else if ( split.Length > 2 )//mission name with a colon
			//	{
			//		for ( int i = 0; i < split.Length; i++ )
			//			split[i] = split[i].Replace( "\"", "" ).Replace( ",", "" ).Trim();
			//		if ( split[0] == "missionName" )
			//		{
			//			int idx = line.IndexOf( ':' );
			//			int c = line.LastIndexOf( ',' );
			//			string mname = line.Substring( idx + 1, c - idx - 1 ).Replace( "\"", "" ).Trim();
			//			projectItem.Title = mname;
			//		}
			//	}
			//}
			//return projectItem;
		}


		/// <summary>
		/// Return ProjectItem info for missions in Project folder
		/// </summary>
		public static IEnumerable<ProjectItem> GetProjects()
		{
			string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

			//make sure the project folder exists
			if ( !Directory.Exists( basePath ) )
			{
				var dinfo = Directory.CreateDirectory( basePath );
				if ( dinfo == null )
				{
					MessageBox.Show( "Could not create the Mission project folder.\r\nTried to create: " + basePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					return null;
				}
			}

			List<ProjectItem> items = new();
			DirectoryInfo di = new( basePath );
			FileInfo[] files = di.GetFiles().Where( file => file.Extension == ".json" ).ToArray();

			try
			{
				//find mission files
				foreach ( FileInfo fi in files )
				{
					var pi = CreateProjectItem( fi.FullName );
					if ( pi != null )
						items.Add( pi );
				}
				items.Sort();
				return items;
			}
			catch ( Exception )
			{
				return null;
			}
		}

		/// <summary>
		/// Load a Resource asset embedded in the app
		/// </summary>
		public static T LoadAsset<T>( string assetName )
		{
			try
			{
				string json = "";
				var assembly = Assembly.GetExecutingAssembly();
				var resourceName = assembly.GetManifestResourceNames().Single( str => str.EndsWith( assetName ) );
				using ( Stream stream = assembly.GetManifestResourceStream( resourceName ) )
				using ( StreamReader reader = new StreamReader( stream ) )
				{
					json = reader.ReadToEnd();
				}

				return JsonConvert.DeserializeObject<T>( json );
			}
			catch ( JsonReaderException e )
			{
				Utils.Log( $"FileManager::LoadData() ERROR:\r\nError parsing {assetName}" );
				Utils.Log( e.Message );
				throw new Exception( $"FileManager::LoadData() ERROR:\r\nError parsing {assetName}" );
			}
		}

		/// <summary>
		/// Handles full Toon export experience, returns bool for success
		/// </summary>
		public static bool ExportCharacter( CustomToon toon )
		{
			string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

			if ( !CreateBaseDirectory() )
				return false;

			string filePath;//full path to the file, including filename
			SaveFileDialog saveFileDialog = new();
			saveFileDialog.DefaultExt = ".json";
			saveFileDialog.Title = "Export Custom Character";
			saveFileDialog.Filter = "Character File (*.json)|*.json";
			saveFileDialog.InitialDirectory = basePath;
			if ( saveFileDialog.ShowDialog() == true )
			{
				filePath = saveFileDialog.FileName;
			}
			else
				return false;

			string output = JsonConvert.SerializeObject( toon, Formatting.Indented );
			try
			{
				using ( var stream = File.CreateText( filePath ) )
				{
					stream.Write( output );
				}
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not Export the Character.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return false;
			}

			return true;
		}

		public static CustomToon ImportCharacter()
		{
			string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

			CreateBaseDirectory();

			OpenFileDialog openFileDialog = new();
			openFileDialog.DefaultExt = ".json";
			openFileDialog.Title = "Import Custom Character";
			openFileDialog.Filter = "Character File (*.json)|*.json";
			openFileDialog.InitialDirectory = basePath;
			if ( openFileDialog.ShowDialog() == true )
			{
				try
				{
					string json = "";
					using ( StreamReader sr = new( openFileDialog.FileName ) )
					{
						json = sr.ReadToEnd();
					}
					var m = JsonConvert.DeserializeObject<CustomToon>( json );
					if ( m != null )
						return m;
					else
						return null;
				}
				catch ( Exception e )
				{
					MessageBox.Show( "Could not Import the Character.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					return null;
				}
			}
			else
				return null;
		}
	}
}
