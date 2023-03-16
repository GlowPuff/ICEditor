using System;
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
using MaterialDesignThemes.Wpf;
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

			var paletteHelper = new PaletteHelper();
			var theme = paletteHelper.GetTheme();
			theme.SetBaseTheme( Theme.Dark );
			paletteHelper.SetTheme( theme );

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
			var projects = FileManager.GetProjects();
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

		private void ScrollViewer_MouseEnter( object sender, MouseEventArgs e )
		{
			scrollVisible = true;
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( scrollVisible ) ) );
		}

		private void ScrollViewer_MouseLeave( object sender, MouseEventArgs e )
		{
			scrollVisible = false;
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( scrollVisible ) ) );
		}

		private void ScrollViewer_PreviewMouseWheel( object sender, MouseWheelEventArgs e )
		{
			ScrollViewer scv = (ScrollViewer)sender;
			scv.ScrollToVerticalOffset( scv.VerticalOffset - e.Delta / 10 );
			e.Handled = true;
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
			od.Filter = "Mission File (*.json)|*.json";
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
			//string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander", pitem.relativePath );
			string basePath = pitem.fullPathWithFilename;

			var r = MessageBox.Show( $"Are you sure you want to PERMANENTLY DELETE this mission?\r\rPath: {basePath}", "Permanently Delete This Mission?", MessageBoxButton.YesNo );
			if ( r == MessageBoxResult.Yes )
			{
				try
				{
					File.Delete( basePath );
					PopulateRecents();
				}
				catch ( Exception ee )
				{
					MessageBox.Show( $"Could not delete file '{basePath}'.\r\r{ee.InnerException}", "Error Deleting File", MessageBoxButton.OK, MessageBoxImage.Error );
				}
			}
		}

		private void editorButton_Click( object sender, RoutedEventArgs e )
		{
			new CampaignEditor().Show();
			Close();
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

				if ( gitHubResponse.tag_name.Substring( 2 ) == Utils.appVersion )//remove beginning "v."
				{
					DoStatus( 1 );
				}
				else
				{
					DoStatus( 2, gitHubResponse.tag_name );
				}
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
		/// 0=red, 1=green, 2=yellow
		/// </summary>
		private void DoStatus( int s, string version = "" )
		{
			Dispatcher.Invoke( () =>
			{
				//hide animated icon
				busyIcon.Visibility = Visibility.Collapsed;

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
				else
				{
					busyStatus.Text = $"Update Available: {version}";
					busyIconRed.Visibility = Visibility.Collapsed;
					busyIconGreen.Visibility = Visibility.Collapsed;
					busyIconYellow.Visibility = Visibility.Visible;
				}
			} );
		}

		private void toonEditorButton_Click( object sender, RoutedEventArgs e )
		{
			CharacterEditorWindow ced = new();
			ced.Show();
			Close();
		}
	}
}
