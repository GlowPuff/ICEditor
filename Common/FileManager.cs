using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class FileManager
	{
		public FileManager() { }

		/// <summary>
		/// saves a mission to base project folder
		/// </summary>
		public static bool Save( Mission mission, bool saveAs )
		{
			string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

			if ( !Directory.Exists( basePath ) )
			{
				var di = Directory.CreateDirectory( basePath );
				if ( di == null )
				{
					MessageBox.Show( "Could not create the Mission project folder.\r\nTried to create: " + basePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					return false;
				}
			}

			string filePath;
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
					mission.relativePath = Path.GetRelativePath( basePath, new DirectoryInfo( filePath ).FullName );
				}
				else
					return false;
			}
			else
				filePath = Path.Combine( basePath, mission.relativePath, mission.fileName );

			//just use the filename, not the whole path
			FileInfo fi = new( filePath );
			mission.fileName = fi.Name;
			mission.saveDate = DateTime.Now.ToString( "M/d/yyyy" );
			mission.timeTicks = DateTime.Now.Ticks;
			mission.fileVersion = Utils.formatVersion;

			string output = JsonConvert.SerializeObject( mission, Formatting.Indented );
			string outpath = Path.Combine( basePath, mission.relativePath );
			Utils.Log( outpath );
			try
			{
				using ( var stream = File.CreateText( outpath ) )
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
				m.relativePath = Path.GetRelativePath( basePath, new DirectoryInfo( filename ).FullName );
				m.fileVersion = Utils.formatVersion;
				return m;
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not load the Mission.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return null;
			}
		}

		public static ProjectItem CreateProjectItem( string filename )
		{
			ProjectItem projectItem = new();
			FileInfo fi = new FileInfo( filename );
			string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

			string[] text = File.ReadAllLines( filename );
			foreach ( var line in text )
			{
				//manually parse each line
				string[] split = line.Split( ':' );
				if ( split.Length == 2 )
				{
					projectItem.fileName = fi.Name;
					projectItem.relativePath = Path.GetRelativePath( basePath, new DirectoryInfo( filename ).FullName );

					split[0] = split[0].Replace( "\"", "" ).Replace( ",", "" ).Trim();
					split[1] = split[1].Replace( "\"", "" ).Replace( ",", "" ).Trim();
					if ( split[0] == "missionName" )
						projectItem.Title = split[1];
					if ( split[0] == "saveDate" )
						projectItem.Date = split[1];
					if ( split[0] == "fileVersion" )
						projectItem.fileVersion = split[1];
					if ( split[0] == "timeTicks" )
						projectItem.timeTicks = long.Parse( split[1] );
				}
				else if ( split.Length > 2 )//mission name with a colon
				{
					for ( int i = 0; i < split.Length; i++ )
						split[i] = split[i].Replace( "\"", "" ).Replace( ",", "" ).Trim();
					if ( split[0] == "missionName" )
					{
						int idx = line.IndexOf( ':' );
						int c = line.LastIndexOf( ',' );
						string mname = line.Substring( idx + 1, c - idx - 1 ).Replace( "\"", "" ).Trim();
						projectItem.Title = mname;
					}
				}
			}
			return projectItem;
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
		/// Load a mission from its .json file
		/// </summary>
		/// <param name="relativePath">The RELATIVE PATH within basePath</param>
		/// <returns></returns>
		public static Mission LoadMissionRelativePath( string relativePath )
		{
			//missionID is the folder name containing the mission
			//if ( missionID == "Saves" )
			//	return null;

			try
			{
				//combines into (XX is the mission number):
				//../MyDocs/ImperialCommander/MISSIONXX.json
				string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );
				string json = "";
				using ( StreamReader sr = new( Path.Combine( basePath, relativePath ) ) )
				{
					json = sr.ReadToEnd();
				}

				var m = JsonConvert.DeserializeObject<Mission>( json, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate } );
				//overwrite fileName, relativePath and fileVersion properties so they are up-to-date
				FileInfo fi = new FileInfo( Path.Combine( basePath, relativePath ) );
				m.fileName = fi.Name;
				m.relativePath = Path.GetRelativePath( basePath, new DirectoryInfo( Path.Combine( basePath, relativePath ) ).FullName );
				m.fileVersion = Utils.formatVersion;

				return m;
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not load the Mission.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return null;
			}
		}
	}
}
