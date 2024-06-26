﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
//using System.Windows.Media;
//using MaterialDesignColors;
//using MaterialDesignThemes.Wpf;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for StartupWindow.xaml
	/// </summary>
	public partial class StartupWindow : Window, INotifyPropertyChanged
	{
		bool _isBusy;
		private GitHubResponse gitHubResponse = null;

		public bool scrollVisible { get; set; }
		public ObservableCollection<ProjectItem> projectCollection;
		public bool isBusy { get { return _isBusy; } set { _isBusy = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public StartupWindow()
		{
			InitializeComponent();

			Utils.LoadAllCardData();
			Utils.InitColors();

			DataContext = this;
			scrollVisible = false;

			PopulateRecents();

			formatVersion.Text = "Mission Format: " + Utils.formatVersion;
			appVersion.Text = "Editor Version: " + Utils.appVersion;

			//var paletteHelper = new PaletteHelper();
			//var theme = paletteHelper.GetTheme();
			////theme.SetBaseTheme( Theme.Dark );
			//theme.SetBaseTheme( (BaseTheme)Theme.GetSystemTheme() );//.Dark );
			//paletteHelper.SetTheme( theme );

			busyStatus.Text = "Checking...";
			if ( NetworkInterface.GetIsNetworkAvailable() )
				Task.Run( StartVersionCheck );
			else
			{
				busyStatus.Text = "Error Checking For Update";
				DoStatus( 0 );
			}
		}

		private void PopulateRecents()
		{
			projectCollection = new ObservableCollection<ProjectItem>();
			projectLV.ItemsSource = projectCollection;

			//poll Project folder for files and populate Recent list, sorted by newest first
			var projects = FileManager.GetMRUProjects();
			if ( projects != null )
			{
				foreach ( ProjectItem pi in projects )
				{
					projectCollection.Add( pi );
				}
			}
			else
			{
				//throw new Exception( "Could not properly load mission projects." );
				MessageBox.Show( "Could not load mission projects.", "Error::PopulateRecents()" );
			}
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == MouseButtonState.Pressed )
				DragMove();
		}

		private void newMissionButton_Click( object sender, RoutedEventArgs e )
		{
			MainWindow mainWindow = new();
			mainWindow.Show();
			Close();
		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			Application.Current.Shutdown();
		}

		private void projectLV_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			ProjectItem item = ((ListView)e.Source).SelectedItem as ProjectItem;
			var project = FileManager.LoadMission( item.fullPathWithFilename );
			if ( project != null )
			{
				MainWindow mainWindow = new( project );
				mainWindow.Show();
				Close();
			}
		}

		private void loadMissionButton_Click( object sender, RoutedEventArgs e )
		{
			OpenFileDialog od = new();
			od.InitialDirectory = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );
			od.Filter = "Mission File|*.json";
			od.Title = "Open Mission";
			if ( od.ShowDialog() == true )
			{
				var filePath = od.FileName;
				var project = FileManager.LoadMission( filePath );
				if ( project != null )
				{
					MainWindow mainWindow = new( project );
					mainWindow.Show();
					Close();
				}
			}
		}

		private void removeButton_Click( object sender, RoutedEventArgs e )
		{
			ProjectItem pitem = (sender as Button).DataContext as ProjectItem;
			string basePath = pitem.fullPathWithFilename;

			var r = MessageBox.Show( $"Are you sure you want to PERMANENTLY DELETE this mission OR remove it from the Recent Mission List?\n\nPath: {basePath}\n\nYES = PERMANENTLY DELETE THE MISSION\nNO = ONLY Remove the Mission from the Recent Mission List\nCANCEL = Don't do ANYTHING, Cancel!", "Permanently Delete Mission OR Remove from Recent Mission List", MessageBoxButton.YesNoCancel );
			if ( r == MessageBoxResult.Yes )
			{
				try
				{
					FileManager.RemoveFromMRU( basePath );
					File.Delete( basePath );
					PopulateRecents();
				}
				catch ( Exception ee )
				{
					MessageBox.Show( $"Could not delete file '{basePath}'.\r\r{ee.InnerException}", "Error Deleting File", MessageBoxButton.OK, MessageBoxImage.Error );
				}
			}
			else if ( r == MessageBoxResult.No )
			{
				FileManager.RemoveFromMRU( basePath );
				PopulateRecents();
			}
		}

		private async Task CheckVersion()
		{
			// /repos/{owner}/{repo}/releases
			var web = new HttpClient();
			web.DefaultRequestHeaders.Add( "User-Agent", "request" );
			var result = await web.GetAsync( "https://api.github.com/repos/GlowPuff/ICEditor/releases/latest" );

			if ( !result.IsSuccessStatusCode )
			{
				//Debug.Log( "network error" );
				DoStatus( 0 );
				gitHubResponse = null;
			}
			else
			{
				var response = await result.Content.ReadAsStringAsync();
				//parse JSON response
				gitHubResponse = JsonConvert.DeserializeObject<GitHubResponse>( response );

				//remove beginning "v." and "." from tag_name
				int ghVersion = int.Parse( gitHubResponse.tag_name.GetDigits() );
				int v = int.Parse( Utils.appVersion.GetDigits() );

				if ( ghVersion == v )
					DoStatus( 1 );
				else if ( v > ghVersion )
					DoStatus( 3 );
				else
					DoStatus( 2, gitHubResponse.tag_name );
			}
		}

		private async void StartVersionCheck()
		{
			//first check if internet is available
			var ping = new Ping();
			var reply = ping.Send( new IPAddress( new byte[] { 8, 8, 8, 8 } ), 5000 );
			if ( reply.Status == IPStatus.Success )
			{
				//internet available, check for latest version
				await CheckVersion();
			}
			else
			{
				DoStatus( 0 );
				gitHubResponse = null;
			}
		}

		/// <summary>
		/// 0=red, 1=green, 2=yellow, 3=green(using a beta)
		/// </summary>
		private void DoStatus( int s, string version = "" )
		{
			Dispatcher.Invoke( () =>
			{
				//hide animated icon
				busyIcon.Visibility = Visibility.Collapsed;

#if DEBUG
				s = 3;
#endif

				if ( s == 0 )
				{
					busyStatus.Text = "Error Checking For Update";
					busyIconRed.Visibility = Visibility.Visible;
					busyIconGreen.Visibility = Visibility.Collapsed;
					busyIconYellow.Visibility = Visibility.Collapsed;
				}
				else if ( s == 1 )
				{
					busyStatus.Text = "Latest Version";
					busyIconRed.Visibility = Visibility.Collapsed;
					busyIconGreen.Visibility = Visibility.Visible;
					busyIconYellow.Visibility = Visibility.Collapsed;
				}
				else if ( s == 2 )
				{
					busyStatus.Text = $"Update Available: {version}";
					busyIconRed.Visibility = Visibility.Collapsed;
					busyIconGreen.Visibility = Visibility.Collapsed;
					busyIconYellow.Visibility = Visibility.Visible;
				}
				else if ( s == 3 )
				{
					busyStatus.Text = "BETA";
					busyIconRed.Visibility = Visibility.Collapsed;
					busyIconGreen.Visibility = Visibility.Visible;
					busyIconYellow.Visibility = Visibility.Collapsed;
				}
			} );
		}

		private void toonEditorButton_Click( object sender, RoutedEventArgs e )
		{
			CharacterEditorWindow ced = new();
			ced.Show();
			Close();
		}

		private void packageButton_Click( object sender, RoutedEventArgs e )
		{
			CampaignPackager packager = new();
			packager.Show();
			Close();
		}

		private void Window_DragEnter( object sender, DragEventArgs e )
		{
			dropStatusText.Text = "DRAG AND DROP MISSION FILE";

			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				if ( filename.Length == 1 && Path.GetExtension( filename[0] ) == ".json" )
				{
					e.Effects = DragDropEffects.Copy;
					dropNotice.Visibility = Visibility.Visible;
				}
			}
			else
			{
				e.Effects = DragDropEffects.None;
				dropNotice.Visibility = Visibility.Hidden;
			}
			e.Handled = true;
		}

		private void Window_Drop( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				if ( filename.Length == 1 && Path.GetExtension( filename[0] ) == ".json" )
				{
					dropStatusText.Text = "OPENING MISSION...";

					var task = new Task( () =>
					{
						var filePath = filename[0];
						var project = FileManager.LoadMission( filePath );
						if ( project != null )
						{
							Dispatcher.Invoke( () =>
							{
								MainWindow mainWindow = new( project );
								mainWindow.Show();
								Close();
							} );
						}
						else
						{
							Dispatcher.Invoke( () =>
							{
								dropNotice.Visibility = Visibility.Hidden;
							} );
						}
					} );
					task.Start();
				}
			}
			e.Handled = true;
		}

		private void Window_DragLeave( object sender, DragEventArgs e )
		{
			dropNotice.Visibility = Visibility.Hidden;
			e.Handled = true;
		}

		private void Window_DragOver( object sender, DragEventArgs e )
		{
			string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
			if ( filename.Length == 1 && Path.GetExtension( filename[0] ) == ".json" )
			{
				e.Effects = DragDropEffects.Copy;
				dropNotice.Visibility = Visibility.Visible;
			}
			else
			{
				e.Effects = DragDropEffects.None;
				dropNotice.Visibility = Visibility.Hidden;
			}
			e.Handled = true;
		}

		private void clearMRUBtn_Click( object sender, RoutedEventArgs e )
		{
			FileManager.ClearMRU();
			PopulateRecents();
		}
	}
}
