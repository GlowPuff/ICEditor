using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
		string defaultPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

		CampaignPackage _campaignPackage { get; set; } = new();
		CampaignMissionItem _selectedMissionItem { get; set; } = null;
		CampaignStructure _selectedStructure { get; set; } = null;

		public CampaignPackage campaignPackage { get => _campaignPackage; set { _campaignPackage = value; PC(); } }
		public CampaignMissionItem selectedMissionItem { get => _selectedMissionItem; set { _selectedMissionItem = value; PC(); } }
		public CampaignStructure selectedStructure { get => _selectedStructure; set { _selectedStructure = value; PC(); } }

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
			OpenFileDialog ofd = new() { Title = "Load Campaign Package", InitialDirectory = defaultPath, Filter = "Campaign Package (.zip)|*.zip" };
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
			campaignPackage = null;

			try
			{
				campaignPackage = FileManager.LoadCampaignPackage( filename );
				if ( campaignPackage != null )
				{
					dropNotice.Visibility = campaignPackage.campaignMissionItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
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

			OpenFileDialog ofd = new() { Title = "Add a Mission", InitialDirectory = lastMissionPath, Filter = "IC2 Missions (.json)|*.json" };
			var res = ofd.ShowDialog();
			if ( res.Value == true )
			{
				lastMissionPath = new FileInfo( ofd.FileName ).DirectoryName;
				var m = FileManager.LoadMission( ofd.FileName );
				if ( m != null )
				{
					selectedMissionItem = campaignPackage.AddMission( m );
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
							lastMissionPath = new FileInfo( item ).DirectoryName;
							selectedMissionItem = campaignPackage.AddMission( mission );
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
				}
				else
				{
					structurePanel.Visibility = Visibility.Collapsed;
					poolPanel.Visibility = Visibility.Visible;
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
	}
}
