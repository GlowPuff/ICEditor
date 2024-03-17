using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for CampaignPackager.xaml
	/// </summary>
	public partial class CampaignPackager : Window, INotifyPropertyChanged
	{
		//the previous path to saved missions
		string lastMissionPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );
		//the previous path to mission translations
		string lastTranslationPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );
		string defaultPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

		CampaignPackage _campaignPackage { get; set; } = new();
		CampaignMissionItem _selectedMissionItem { get; set; } = null;
		CampaignStructure _selectedStructure { get; set; } = null;
		CampaignTranslationItem _selectedTranslationItem { get; set; } = null;

		public CampaignPackage campaignPackage { get => _campaignPackage; set { _campaignPackage = value; PC(); } }
		public CampaignMissionItem selectedMissionItem { get => _selectedMissionItem; set { _selectedMissionItem = value; PC(); } }
		public CampaignStructure selectedStructure { get => _selectedStructure; set { _selectedStructure = value; PC(); } }
		public CampaignTranslationItem selectedTranslationItem { get => _selectedTranslationItem; set { _selectedTranslationItem = value; PC(); } }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public CampaignPackager()
		{
			InitializeComponent();
			DataContext = campaignPackage;

			instructionBtn.Foreground = new SolidColorBrush( Colors.Red );
			dropNotice.Visibility = Visibility.Visible;
			tabControl.SelectedIndex = 1;
		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_Closed( object sender, CancelEventArgs e )
		{
			StartupWindow sw = new();
			sw.Show();
		}

		/// <summary>
		/// Load a campaign package (zip) via button
		/// </summary>
		private void loadButton_Click( object sender, RoutedEventArgs e )
		{
			OpenFileDialog ofd = new() { Title = "Load Campaign Package", InitialDirectory = defaultPath, Filter = "Campaign Package|*.zip" };
			var res = ofd.ShowDialog();
			if ( res.Value == true )
			{
				LoadCampaignPackage( ofd.FileName );
			}
		}

		/// <summary>
		/// Load a campaign package (zip) via drag/drop
		/// </summary>
		private void loadButton_Drop( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				//grab the filename
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				if ( filename.Length == 1 )
				{
					LoadCampaignPackage( filename[0] );
				}
			}
		}

		private void LoadCampaignPackage( string filename )
		{
			selectedMissionItem = null;
			selectedTranslationItem = null;
			campaignPackage = null;

			try
			{
				campaignPackage = FileManager.LoadCampaignPackage( filename );
				if ( campaignPackage != null )
				{
					dropNotice.Visibility = campaignPackage.campaignMissionItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
					dropTranslationNotice.Visibility = campaignPackage.campaignTranslationItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

					instructionBtn.Foreground = string.IsNullOrEmpty( campaignPackage?.campaignInstructions ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.Lime );
					DataContext = campaignPackage;
					MessageBox.Show( $"Package successfully imported.", "Import Successful", MessageBoxButton.OK, MessageBoxImage.Information );
				}
				else
				{
					campaignPackage = new();
					DataContext = campaignPackage;
				}
			}
			catch ( Exception ee )
			{
				campaignPackage = new();
				DataContext = campaignPackage;
				MessageBox.Show( $"Could not load the Campaign Package.\n\n{ee.Message}", "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
			}
		}

		/// <summary>
		/// Zip and save the package
		/// </summary>
		private void packageButton_Click( object sender, RoutedEventArgs e )
		{
			var save = new SaveFileDialog() { Title = "Export the Campaign Package", InitialDirectory = defaultPath, Filter = "Campaign Package (.zip)|*.zip" };
			var res = save.ShowDialog();
			if ( res.Value == true )
			{
				if ( FileManager.SaveCampaignPackage( save.FileName, campaignPackage ) )
				{
					MessageBox.Show( $"Package successfully exported to\n{save.FileName}.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information );
				}
			}
		}

		private void instructionBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT CAMPAIGN INSTRUCTIONS", campaignPackage.campaignInstructions );
			dlg.ShowDialog();

			campaignPackage.campaignInstructions = dlg.theText;
			instructionBtn.Foreground = string.IsNullOrEmpty( campaignPackage.campaignInstructions ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.Lime );
		}

		private void upBtn_Click( object sender, RoutedEventArgs e )
		{
			int idx = structureLV.SelectedIndex;
			if ( idx != -1 )
				campaignPackage.campaignStructure.Move( idx, Math.Max( 0, idx - 1 ) );
		}

		private void downBtn_Click( object sender, RoutedEventArgs e )
		{
			int idx = structureLV.SelectedIndex;
			if ( idx != -1 )
				campaignPackage.campaignStructure.Move( idx, Math.Min( campaignPackage.campaignStructure.Count - 1, idx + 1 ) );
		}

		private void missionLV_DragEnter( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				bool valid = true;
				foreach ( var item in filename )
				{
					if ( Path.GetExtension( item ) != ".json" )
						valid = false;
				}
				if ( valid )
					e.Effects = DragDropEffects.All;
			}
		}

		/// <summary>
		/// Add mission via + button
		/// </summary>
		private void addMissionBtn_Click( object sender, RoutedEventArgs e )
		{
			selectedMissionItem = null;

			OpenFileDialog ofd = new() { Title = "Add a Mission", InitialDirectory = lastMissionPath, Filter = "IC2 Missions|*.json" };
			var res = ofd.ShowDialog();
			if ( res.Value == true )
			{
				lastMissionPath = new FileInfo( ofd.FileName ).DirectoryName;
				var m = FileManager.LoadMission( ofd.FileName );
				if ( m != null )
				{
					selectedMissionItem = campaignPackage.AddMission( ofd.SafeFileName, m );
					dropNotice.Visibility = campaignPackage.campaignMissionItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
				}
				else
					MessageBox.Show( "Loaded Mission is null.", "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
			}
		}

		/// <summary>
		/// Add mission via drag/drop
		/// </summary>
		private void missionLV_Drop( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				//grab the filenames
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				try
				{
					foreach ( var item in filename )
					{
						var mission = FileManager.LoadMission( item );
						if ( mission != null )
						{
							FileInfo fi = new FileInfo( item );
							lastMissionPath = fi.DirectoryName;
							selectedMissionItem = campaignPackage.AddMission( fi.Name, mission );
							dropNotice.Visibility = campaignPackage.campaignMissionItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
						}
						else
							throw new( $"Loaded Mission is null.\n{item}" );
					}
				}
				catch ( Exception ee )
				{
					MessageBox.Show( $"Could not load the Mission.\n\n{ee.Message}", "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					selectedMissionItem = null;
				}
			}
		}

		private void removeBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedMissionItem != null )
			{
				campaignPackage.RemoveMission( selectedMissionItem );
				//if a structure slot is using this removed mission, reset it
				campaignPackage.ValidateMissions();
				selectedMissionItem = null;
			}
			dropNotice.Visibility = campaignPackage.campaignMissionItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private void loadButton_DragEnter( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				if ( Path.GetExtension( filename[0] ) == ".zip" )
					e.Effects = DragDropEffects.All;
			}
		}

		private void swapMissionBtn_Click( object sender, RoutedEventArgs e )
		{
			campaignPackage.RemoveMission( selectedMissionItem );
			selectedMissionItem = null;
			addMissionBtn_Click( sender, e );
		}

		private void TB_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
			}
		}

		private void tabControl_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			if ( e.OriginalSource is TabControl )
			{
				if ( ((TabControl)sender).SelectedIndex == 0 )
				{
					structurePanel.Visibility = Visibility.Visible;
					poolPanel.Visibility = Visibility.Collapsed;
					translationPanel.Visibility = Visibility.Collapsed;
				}
				else if ( ((TabControl)sender).SelectedIndex == 1 )
				{
					structurePanel.Visibility = Visibility.Collapsed;
					poolPanel.Visibility = Visibility.Visible;
					translationPanel.Visibility = Visibility.Collapsed;
				}
				else
				{
					structurePanel.Visibility = Visibility.Collapsed;
					poolPanel.Visibility = Visibility.Collapsed;
					translationPanel.Visibility = Visibility.Visible;
				}
			}
		}

		private void addStructureBtn_Click( object sender, RoutedEventArgs e )
		{
			campaignPackage.AddStructure();
		}

		private void removeStructureBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedStructure != null )
			{
				campaignPackage.RemoveStructure( selectedStructure );
				selectedStructure = null;
			}
		}

		private void missionsPopupLB_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			if ( e.AddedItems.Count > 0 )
			{
				selectedStructure.projectItem.Title = ((CampaignMissionItem)e.AddedItems[0]).missionName;
				selectedStructure.missionID = ((CampaignMissionItem)e.AddedItems[0]).missionGUID.ToString();
				selectedStructure.missionSource = MissionSource.Embedded;
				selectedStructure.projectItem.missionGUID = ((CampaignMissionItem)e.AddedItems[0]).mission.missionGUID.ToString();
				selectedStructure.customMissionIdentifier = ((CampaignMissionItem)e.AddedItems[0]).customMissionIdentifier;

				var eventActions = ((CampaignMissionItem)e.AddedItems[0]).mission.GetAllEvents().SelectMany( x => x.eventActions );

				selectedStructure.mission = ((CampaignMissionItem)e.AddedItems[0]).mission;
				selectedStructure.hasCustomSetNextEventActions = eventActions.Any( x => x.eventActionType == EventActionType.CM4 && ((CampaignSetNextMission)x).missionID == "Custom" );

				missionsPopupLB.SelectedIndex = -1;
			}
		}

		private void missionResetBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedStructure != null )
			{
				selectedStructure.Reset();
			}
		}

		private void exportButton_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedMissionItem != null )
			{
				var save = new SaveFileDialog() { Title = "Export the Selected Mission", InitialDirectory = defaultPath, Filter = "IC2 Missions (.json)|*.json" };
				var res = save.ShowDialog();
				if ( res.Value == true )
				{
					try
					{
						string json = JsonConvert.SerializeObject( selectedMissionItem.mission, Formatting.Indented );
						File.WriteAllText( save.FileName, json );
						MessageBox.Show( $"Mission successfully exported:\n{save.FileName}.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information );
					}
					catch ( Exception ee )
					{
						MessageBox.Show( $"There was an error trying to export the Mission:\n{ee.Message}", "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					}
				}
			}
		}

		private void iconBtn_Click( object sender, RoutedEventArgs e )
		{
			OpenFileDialog ofd = new OpenFileDialog()
			{
				Title = "Load Campaign Icon",
				InitialDirectory = defaultPath,
				Filter = "PNG Image|*.png"
			};
			var res = ofd.ShowDialog();
			if ( res.Value == true )
			{
				bool goodFile = true;
				//check the size and format
				if ( CheckPNGFormat( ofd.FileName ) )
				{
					if ( !isCorrectDimensions( ofd.FileName ) )
					{
						goodFile = false;
						Utils.ShowError( "Is the image size 64x64 or 128x128?", "Wrong File Size" );
					}
				}
				else
				{
					goodFile = false;
					Utils.ShowError( "Is the image a PNG?", "Wrong File Type" );
				}

				if ( goodFile )
				{
					campaignPackage.SetIcon( ofd.FileName );
					campaignPackage.campaignIconName = new FileInfo( ofd.FileName ).Name;
				}
			}
		}

		private bool CheckPNGFormat( string path )
		{
			byte[] buffer = new byte[8];
			byte[] bufferEnd = new byte[2];
			//PNG "\x89PNG\x0D\0xA\0x1A\0x0A"
			var png = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

			try
			{
				using ( FileStream fs = new FileStream( path, FileMode.Open, FileAccess.Read ) )
				{
					if ( fs.Length > buffer.Length )
					{
						fs.Read( buffer, 0, buffer.Length );
						fs.Position = (int)fs.Length - bufferEnd.Length;
						fs.Read( bufferEnd, 0, bufferEnd.Length );
					}
					fs.Close();
				}

				if ( this.ByteArrayStartsWith( buffer, png ) )
					return true;
			}
			catch ( Exception e )
			{
				Utils.ShowError( $"CheckPNG()::There was an error checking the image type:\n{e.Message}", "" );
			}

			return false;
		}

		private bool ByteArrayStartsWith( byte[] a, byte[] b )
		{
			if ( a.Length < b.Length )
			{
				return false;
			}

			for ( int i = 0; i < b.Length; i++ )
			{
				if ( a[i] != b[i] )
				{
					return false;
				}
			}

			return true;
		}

		private bool isCorrectDimensions( string path )
		{
			using ( var imageStream = new FileStream( path, FileMode.Open, FileAccess.Read ) )
			{
				var decoder = BitmapDecoder.Create( imageStream, BitmapCreateOptions.IgnoreColorProfile,
								BitmapCacheOption.Default );
				var width = decoder.Frames[0].PixelWidth;
				var height = decoder.Frames[0].PixelHeight;
				if ( width == 64 && height == 64 )
					return true;
				else if ( width == 128 && height == 128 )
					return true;
			}
			return false;
		}

		private void copyCustomID_Click( object sender, RoutedEventArgs e )
		{
			Clipboard.SetText( customMissionIDText.Text, TextDataFormat.Text );
		}

		private void copyPoolCustomID_Click( object sender, RoutedEventArgs e )
		{
			Clipboard.SetText( customMissionPoolIDText.Text, TextDataFormat.Text );
		}

		private void editSetNextBtn_Click( object sender, RoutedEventArgs e )
		{
			var validActions = selectedStructure.mission.GetAllEvents().SelectMany( x => x.eventActions ).Where( x => x.eventActionType == EventActionType.CM4 ).ToList();

			//if there is only one 'set next mission' event action, show the edit window directly
			if ( validActions.Count() == 1 )
			{
				var ea = new CampaignSetNextMission() { missionID = "Custom", customMissionID = ((CampaignSetNextMission)validActions[0]).customMissionID };
				var dlg = new SetNextMissionDialog( "Set Next Mission", EventActionType.CM4, ea, true );
				dlg.ShowDialog();
			}
			else if ( validActions.Count() > 1 )
			{
				//otherwise show the selector window so each one can be edited independently
				var chooser = new ChooseSetNextMissionWindow( selectedStructure.mission.GetAllEvents() );
				chooser.ShowDialog();
			}
		}

		private void addTranslationBtn_Click( object sender, RoutedEventArgs e )
		{
			selectedTranslationItem = null;

			OpenFileDialog ofd = new() { Title = "Add a Translation", InitialDirectory = lastTranslationPath, Filter = "Translations|*.json;*.txt" };
			var res = ofd.ShowDialog();
			if ( res.Value == true )
			{
				FileInfo fi = new FileInfo( ofd.FileName );
				lastTranslationPath = fi.DirectoryName;

				if ( Path.GetExtension( fi.FullName ) == ".json" )//mission translation
				{
					TranslatedMission tm = FileManager.LoadJSON<TranslatedMission>( ofd.FileName );
					if ( tm != null )
					{
						//verify it's a translation
						string stringified = File.ReadAllText( fi.FullName );
						if ( stringified.Contains( "languageID" )
							&& stringified.Contains( "events" )
							&& stringified.Contains( "mapEntities" )
							&& stringified.Contains( "initialGroups" ) )
						{
							selectedTranslationItem = campaignPackage.AddMissionTranslation( tm, ofd.SafeFileName );
							dropTranslationNotice.Visibility = campaignPackage.campaignTranslationItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
						}
						else
							MessageBox.Show( "The file doesn't appear to be a Mission Translation.", "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					}
					else
						MessageBox.Show( "Loaded Translation is null.", "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				}
				else if ( Path.GetExtension( fi.FullName ) == ".txt" )//campaign instruction
				{
					string instructionText = File.ReadAllText( fi.FullName );
					selectedTranslationItem = campaignPackage.AddCampaignInfoTranslation( instructionText, fi.Name );
					dropTranslationNotice.Visibility = campaignPackage.campaignTranslationItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
				}
			}
		}

		private void removeTranslationBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedTranslationItem != null )
			{
				campaignPackage.RemoveTranslation( selectedTranslationItem );
				selectedTranslationItem = null;
			}
			dropTranslationNotice.Visibility = campaignPackage.campaignTranslationItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private void translationLV_Drop( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				//grab the filenames
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				try
				{
					foreach ( var item in filename )
					{
						FileInfo fi = new FileInfo( item );

						if ( Path.GetExtension( item ) == ".json" )//mission translation
						{
							var translation = FileManager.LoadJSON<TranslatedMission>( item );
							if ( translation != null )
							{
								lastTranslationPath = fi.DirectoryName;
								//verify it's a translation
								string stringified = File.ReadAllText( fi.FullName );
								if ( stringified.Contains( "languageID" )
									&& stringified.Contains( "events" )
									&& stringified.Contains( "mapEntities" )
									&& stringified.Contains( "initialGroups" ) )
								{
									selectedTranslationItem = campaignPackage.AddMissionTranslation( translation, fi.Name );
									dropTranslationNotice.Visibility = campaignPackage.campaignTranslationItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
								}
								else
									throw new Exception( "The file doesn't appear to be a Mission Translation." );
							}
							else
								throw new( $"Loaded Translation is null.\n{item}" );
						}
						else if ( Path.GetExtension( item ) == ".txt" )//campaign instructions
						{
							lastTranslationPath = fi.DirectoryName;
							string instructionText = File.ReadAllText( fi.FullName );
							selectedTranslationItem = campaignPackage.AddCampaignInfoTranslation( instructionText, fi.Name );
							dropTranslationNotice.Visibility = campaignPackage.campaignTranslationItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
						}
					}
				}
				catch ( Exception ee )
				{
					MessageBox.Show( $"Could not load the Translation.\n\n{ee.Message}", "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					selectedMissionItem = null;
				}
			}
		}

		private void translationDrop_DragEnter( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				bool valid = true;
				foreach ( var item in filename )
				{
					if ( Path.GetExtension( item ) != ".json" || Path.GetExtension( item ) != ".txt" )
						valid = false;
				}
				if ( valid )
					e.Effects = DragDropEffects.All;
			}
		}

		private void updateStructureBtn_Click( object sender, RoutedEventArgs e )
		{
			OpenFileDialog ofd = new() { Title = "Update/Replace a Mission", InitialDirectory = lastMissionPath, Filter = "IC2 Missions|*.json" };
			var res = ofd.ShowDialog();
			if ( res.Value == true )
			{
				lastMissionPath = new FileInfo( ofd.FileName ).DirectoryName;
				var m = FileManager.LoadMission( ofd.FileName );
				if ( m != null )
				{
					string oldName = selectedMissionItem.missionName;
					selectedMissionItem = campaignPackage.ReplaceMission( selectedMissionItem, ofd.SafeFileName, m );
					dropNotice.Visibility = campaignPackage.campaignMissionItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
					MessageBox.Show( $"Replaced/updated [{oldName}] with [{selectedMissionItem.missionName}]", "Mission Replaced/Updated" );
				}
				else
					MessageBox.Show( "Loaded Mission is null.", "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
			}
		}
	}
}
