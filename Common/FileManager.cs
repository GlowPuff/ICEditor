using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	public class FileManager
	{
		//public string fileName;
		//public string fileVersion;
		//public Guid missionID;//usually set to MISSIONXX where XX is the mission number
		//public string missionName;
		//public string saveDate;

		public FileManager() { }

		//public FileManager( Mission source )
		//{
		//	fileName = source.fileName;
		//	fileVersion = source.fileVersion;
		//	missionID = source.missionID;
		//	missionName = source.missionName;
		//	saveDate = source.saveDate;
		//}

		/// <summary>
		/// saves a mission to base project folder
		/// </summary>
		public static bool Save( Mission mission, bool saveAs )
		{
			//if ( missionID == Guid.Empty )
			//	return Save( false, Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "Your Journey" ) );

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
					mission.relativePath = mission.relativePath = Path.GetRelativePath( basePath, new DirectoryInfo( filePath ).FullName );
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
			try
			{
				using ( StreamReader sr = new( filename ) )
				{
					json = sr.ReadToEnd();
				}

				var m = JsonConvert.DeserializeObject<Mission>( json );
				return m;
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not load the Mission.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return null;
			}
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

			//find mission files
			foreach ( FileInfo fi in files )
			{
				//Debug.Log( fi.FullName );
				Mission s = LoadMission( fi.FullName );
				if ( s != null )
					items.Add( new ProjectItem() { Title = s.missionProperties.missionName, Date = s.saveDate, fileName = s.fileName, relativePath = s.relativePath, fileVersion = s.fileVersion } );
			}
			return items;
		}

		/// <summary>
		/// Load a mission from its .json file
		/// </summary>
		/// <param name="relativePath">The RELATIVE PATH withing basePath</param>
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

				var c = JsonConvert.DeserializeObject<Mission>( json, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate } );
				return c;
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not load the Mission.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return null;
			}
		}
	}
}
