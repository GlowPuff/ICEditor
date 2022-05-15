using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
//using System.Windows.Media;
//using MaterialDesignColors;
//using MaterialDesignThemes.Wpf;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for StartupWindow.xaml
	/// </summary>
	public partial class StartupWindow : Window
	{
		public ObservableCollection<ProjectItem> projectCollection;
		public event PropertyChangedEventHandler PropertyChanged;
		public bool scrollVisible { get; set; }

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
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
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
			var project = FileManager.LoadMissionRelativePath( item.relativePath );
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
			string basePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander", pitem.relativePath );

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
	}
}
