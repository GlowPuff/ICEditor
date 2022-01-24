using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
//using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		MapSection _activeSection;
		string _mainTitle, _infoText;
		Timer infoTimer;
		SolidColorBrush _statusColor;

		public event PropertyChangedEventHandler PropertyChanged;

		public Mission mission { get; set; }
		public string infoText { get { return _infoText; } set { _infoText = value; PC(); } }
		public SolidColorBrush statusColor { get { return _statusColor; } set { _statusColor = value; PC(); } }
		public MapSection activeSection
		{
			get { return _activeSection; }
			set
			{
				if ( _activeSection != null )
					_activeSection.isActive = false;
				_activeSection = value;
				_activeSection.isActive = true;
				properties.UpdateUI( _activeSection );
				mapEditor.UpdateUI( _activeSection );
				if ( !leftPanel.showGlobal )
				{
					leftPanel.triggersCB.ItemsSource = activeSection.triggers;
					leftPanel.triggersCB.SelectedIndex = 0;//first trigger is None
					leftPanel.eventsCB.ItemsSource = activeSection.missionEvents;
					leftPanel.eventsCB.SelectedIndex = 0;//first trigger is None
				}
				PC();
			}
		}
		public List<MissionEvent> localEvents
		{
			get
			{
				var events = new List<MissionEvent>();
				events = events.Concat( mission.globalEvents ).ToList();
				events = events.Concat( activeSection.missionEvents.Where( x => x.GUID != Guid.Empty ) ).ToList();
				return events;
			}
		}
		public List<Trigger> localTriggers
		{
			get
			{
				var trigs = new List<Trigger>();
				trigs = trigs.Concat( mission.globalTriggers ).ToList();//.Where( x => x.GUID != Guid.Empty ) ).ToList();
				trigs = trigs.Concat( activeSection.triggers.Where( x => x.GUID != Guid.Empty ) ).ToList();
				return trigs;
			}
		}
		//public List<IMapEntity> mapEntities
		//{
		//	get
		//	{
		//		var ents = new List<IMapEntity>();
		//		foreach ( var section in mission.mapSections )
		//		{
		//			ents = ents.Concat( section.mapEntities ).ToList();
		//		}
		//		return ents;
		//	}
		//}
		public string mainTitle { get { return _mainTitle; } set { _mainTitle = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		//public MainWindow( string missionID ) : this( null )
		//{
		//	mission.missionID = missionID;
		//}

		public MainWindow( Mission s = null )
		{
			Utils.Init( this );

			InitializeComponent();

			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

			var paletteHelper = new PaletteHelper();
			var theme = paletteHelper.GetTheme();
			theme.SetBaseTheme( Theme.Dark );
			paletteHelper.SetTheme( theme );

			infoTimer = new Timer( 3000 );
			infoTimer.AutoReset = false;
			infoTimer.Elapsed += infoTimerFunc;
			infoText = "Status OK";

			mission = s ?? new();
			if ( s == null )
			{
				mission.InitDefaultData();
				mainTitle = "Imperial Commander Mission Editor - New Mission (unsaved)";
			}
			else
			{
				mapEditor.LoadMap();
				mainTitle = $"Imperial Commander Mission Editor - {mission.fileName}";
			}

			DataContext = this;

			///set default data connections
			//default active section is "start", item 0
			activeSection = mission.mapSections[0];
			mapSections.mapSectionItems.ItemsSource = mission.mapSections;
			///set left panel data
			leftPanel.triggersCB.ItemsSource = activeSection.triggers;
			leftPanel.triggersCB.SelectedIndex = 0;//first trigger is None
			leftPanel.eventsCB.ItemsSource = activeSection.missionEvents;
			leftPanel.eventsCB.SelectedIndex = 0;//first trigger is None

			appVersion.Text = Utils.appVersion;
			formatVersion.Text = Utils.formatVersion;

			leftPanel.showGlobal = true;
		}

		public void SetStatus( string s )
		{
			infoTimer.Stop();
			infoText = s;
			statusColor = new SolidColorBrush( Colors.Purple );
			infoTimer.Start();
		}

		private void infoTimerFunc( Object source, ElapsedEventArgs e )
		{
			Dispatcher.Invoke( () =>
			{
				statusColor = new SolidColorBrush( Color.FromRgb( 48, 48, 48 ) );
			} );
			infoText = "Status OK";
		}

		private void openMissionButton_Click( object sender, RoutedEventArgs e )
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
					//mission = project;
					MainWindow mainWindow = new( project );
					mainWindow.Show();
					Close();
					SetStatus( "Mission Loaded" );
				}
			}
		}

		private void saveMissionButton_Click( object sender, RoutedEventArgs e )
		{
			if ( FileManager.Save( mission, false ) )
			{
				mainTitle = $"Imperial Commander Mission Editor - {mission.fileName}";
				SetStatus( "Mission Saved" );
			}
			else
				SetStatus( "Error Saving Mission" );
		}

		private void saveMissionAsButton_Click( object sender, RoutedEventArgs e )
		{
			if ( FileManager.Save( mission, true ) )
				mainTitle = $"Imperial Commander Mission Editor - {mission.fileName}";
		}

		private void addTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			leftPanel.addTriggerButton_Click( sender, e );
		}

		private void addEventButton_Click( object sender, RoutedEventArgs e )
		{
			leftPanel.addEventButton_Click( sender, e );
		}

		private void addBlockButton_Click( object sender, RoutedEventArgs e )
		{
			var n = new MapSection();
			n.Init();
			mission.mapSections.Add( n );
			activeSection = n;
		}

		public void onEditSection( MapSection m )
		{
			tabControl.SelectedIndex = 4;
			activeSection.isActive = false;
			activeSection = m;
			activeSection.isActive = true;
		}

		public void onEditMap( MapSection m )
		{
			tabControl.SelectedIndex = 2;
			activeSection.isActive = false;
			activeSection = m;
			activeSection.isActive = true;
		}

		private void projectManagerButton_Click( object sender, RoutedEventArgs e )
		{
			var r = MessageBox.Show( "Are you sure you want to close this Mission and open the Project Manager?\r\rAny unsaved changes will be lost.", "Are you sure?", MessageBoxButton.YesNo );
			if ( r == MessageBoxResult.Yes )
			{
				var dlg = new StartupWindow();
				dlg.Show();
				Close();
			}
		}

		private void tabControl_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			if ( (sender as TabControl).SelectedIndex == 4 )
				properties.UpdateUI( activeSection );
			else if ( (sender as TabControl).SelectedIndex == 3 )
				encounters.UpdateUI();
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			missionProps.Refresh();
			mapEditor.OnWindowLoaded();
		}

		private void Window_PreviewKeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.F5 || (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) )
			{
				if ( FileManager.Save( mission, false ) )
				{
					mainTitle = $"Imperial Commander Mission Editor - {mission.fileName}";
					SetStatus( "Mission Saved" );
				}
			}
			if ( e.Key == Key.F9 || (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) )
			{
				openMissionButton_Click( null, null );
			}

			if ( tabControl.SelectedIndex == 2 )
				mapEditor.ProcessKey( e );
		}
	}
}
