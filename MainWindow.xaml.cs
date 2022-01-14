using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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
    string _mainTitle;

    public event PropertyChangedEventHandler PropertyChanged;

    public Mission mission { get; set; }
    public MapSection activeSection
    {
      get { return _activeSection; }
      set
      {
        if ( _activeSection != null )
          _activeSection.isActive = false;
        _activeSection = value;
        _activeSection.isActive = true;
        properties.Populate( _activeSection );
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
      InitializeComponent();

      System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
      System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
      System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

      var paletteHelper = new PaletteHelper();
      var theme = paletteHelper.GetTheme();
      theme.SetBaseTheme( Theme.Dark );
      paletteHelper.SetTheme( theme );

      Utils.Init( this );

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
    }

    private void Button_Click( object sender, RoutedEventArgs e )
    {
      FileManager.Save( mission );
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
          //mapSections.infoSnackbar.MessageQueue?.Enqueue( "Mission Loaded" );
        }
      }
    }

    private void saveMissionButton_Click( object sender, RoutedEventArgs e )
    {
      if ( FileManager.Save( mission ) )
        mainTitle = $"Imperial Commander Mission Editor - {mission.fileName}";
      //  mapSections.infoSnackbar.MessageQueue?.Enqueue( "Mission Saved" );
      //else
      //  mapSections.infoSnackbar.MessageQueue?.Enqueue( "Error Saving Mission" );
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
      mission.mapSections.Add( n );
      activeSection = n;
    }

    private void missionSettingsButton_Click( object sender, RoutedEventArgs e )
    {
      MissionSettingsDialog dlg = new( mission );
      dlg.ShowDialog();
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
        properties.Populate( activeSection );
      else if ( (sender as TabControl).SelectedIndex == 3 )
        encounters.Populate();
    }

    private void Window_Loaded( object sender, RoutedEventArgs e )
    {
      missionProps.Refresh();
    }
  }
}
