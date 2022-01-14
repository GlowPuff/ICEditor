using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Imperial_Commander_Editor
{
  /// <summary>
  /// Interaction logic for TileGallery.xaml
  /// </summary>
  public partial class TileGallery : Window, INotifyPropertyChanged
  {
    //string filterText;
    string _selectedExpansion;
    bool _selectedTileSideA, _tileIsSelected;
    GalleryTile _selectedTile;

    public ObservableCollection<GalleryTile> tileObserverA { get; set; } = new();
    public ObservableCollection<GalleryTile> tileObserverB { get; set; } = new();
    public ObservableCollection<string> expansionsList { get; set; } = new();
    public string selectedExpansion
    {
      get { return _selectedExpansion; }
      set
      {
        _selectedExpansion = value;
        PC();
        LoadTiles();
        selectedTileSideA = true;
      }
    }
    public bool selectedTileSideA
    {
      get { return _selectedTileSideA; }
      set
      {
        _selectedTileSideA = value;
        PC();
        if ( _selectedTileSideA )
          items.ItemsSource = tileObserverA;
        else
          items.ItemsSource = tileObserverB;
      }
    }
    public bool tileIsSelected { get { return _tileIsSelected; } set { _tileIsSelected = value; PC(); } }
    public GalleryTile selectedTile { get { return _selectedTile; } set { _selectedTile = value; PC(); } }

    public TileGallery()
    {
      InitializeComponent();
      DataContext = this;

      expansionsList.Add( "CORE" );

      _selectedTileSideA = true;
      selectedExpansion = "CORE";

      tileIsSelected = false;
    }

    static int tileCount( string expansion ) => expansion.ToUpper() switch
    {
      "CORE" => 39,
      _ => 0,
    };

    async void LoadTiles()
    {
      await Task.Run( () =>
      {
        for ( int i = 1; i <= tileCount( selectedExpansion ); i++ )
        {
          App.Current.Dispatcher.Invoke( () =>
          {
            GalleryTile galleryTileA = new GalleryTile( i, selectedExpansion, "A" );
            galleryTileA.source = new BitmapImage( new Uri( $"pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/{selectedExpansion}/{selectedExpansion}_{i}A.jpg" ) );

            GalleryTile galleryTileB = new GalleryTile( i, selectedExpansion, "B" );
            galleryTileB.source = new BitmapImage( new Uri( $"pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/{selectedExpansion}/{selectedExpansion}_{i}B.jpg" ) );

            tileObserverA.Add( galleryTileA );
            tileObserverB.Add( galleryTileB );
          } );
        }
      } );
    }

    public void PC( [CallerMemberName] string n = "" )
    {
      if ( !string.IsNullOrEmpty( n ) )
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
    }
    public event PropertyChangedEventHandler PropertyChanged;

    private void tileButton_Click( object sender, RoutedEventArgs e )
    {
      selectedTile = (sender as FrameworkElement).DataContext as GalleryTile;
      tileIsSelected = true;
    }

    private void tileButton_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
    {
      selectedTile = (sender as FrameworkElement).DataContext as GalleryTile;
      tileIsSelected = true;
      DialogResult = true;
      Close();
    }

    private void addSelectedBtn_Click( object sender, RoutedEventArgs e )
    {
      DialogResult = true;
      Close();
    }

    private void filterTB_TextChanged( object sender, System.Windows.Controls.TextChangedEventArgs e )
    {
      //filterText = filterTB.Text;
    }

    private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
    {
      DragMove();
    }

    private async void Window_Loaded( object sender, RoutedEventArgs e )
    {
      //await Task.Run( () =>
      //{
      //  for ( int i = 1; i <= tileCount( selectedExpansion ); i++ )
      //  {
      //    App.Current.Dispatcher.Invoke( () =>
      //    {
      //      GalleryTile galleryTileA = new GalleryTile( i, selectedExpansion, "A" );
      //      galleryTileA.source = new BitmapImage( new Uri( $"pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/{selectedExpansion}/{selectedExpansion}_{i}A.jpg" ) );

      //      GalleryTile galleryTileB = new GalleryTile( i, selectedExpansion, "B" );
      //      galleryTileB.source = new BitmapImage( new Uri( $"pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/{selectedExpansion}/{selectedExpansion}_{i}B.jpg" ) );

      //      tileObserverA.Add( galleryTileA );
      //      tileObserverB.Add( galleryTileB );
      //    } );
      //  }
      //} );
    }

    private void closeBtn_Click( object sender, RoutedEventArgs e )
    {
      DialogResult = false;
      Close();
    }
  }
}
