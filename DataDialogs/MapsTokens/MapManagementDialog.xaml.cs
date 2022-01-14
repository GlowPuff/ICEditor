using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
  /// <summary>
  /// Interaction logic for MapManagementDialog.xaml
  /// </summary>
  public partial class MapManagementDialog : Window, IEventActionDialog
  {
    public IEventAction eventAction { get; set; }

    public MapManagementDialog( string dname, EventActionType et, IEventAction ea = null )
    {
      InitializeComponent();

      eventAction = ea ?? new MapManagement( dname, et );
      DataContext = eventAction;

      //get map sections that start invisible
      List<MapSection> sections = new();
      MapSection section = new() { name = "None", GUID = Guid.Empty };
      section.Init();
      sections.Add( section );
      sections = sections.Concat( Utils.mainWindow.mission.mapSections.Where( x => x.invisibleUntilActivated ) ).ToList();
      msCB.ItemsSource = sections;

      //for removal, list ALL sections
      MapSection mapSection = new() { name = "None", GUID = Guid.Empty };
      mapSection.Init();
      ms2CB.ItemsSource = new MapSection[] { mapSection }.Concat( Utils.mainWindow.mission.mapSections );

      //get tiles that start NOT active
      List<MapTile> tiles = new();
      tiles.Add( new( "None" ) { name = "None", GUID = Guid.Empty } );
      tiles = tiles.Concat( Utils.mainWindow.activeSection.mapTiles.Where( x => !x.entityProperties.isActive ) ).ToList();
      tileCB.ItemsSource = tiles;

      //for removal, list ALL tiles
      tile2CB.ItemsSource = new MapTile[] { new( "None" ) { name = "None", GUID = Guid.Empty } }.Concat( Utils.mainWindow.activeSection.mapTiles );
    }

    private void Window_MouseDown( object sender, MouseButtonEventArgs e )
    {
      DragMove();
    }

    private void okButton_Click( object sender, RoutedEventArgs e )
    {
      Close();
    }
  }
}
