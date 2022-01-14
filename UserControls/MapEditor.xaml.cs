using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Imperial_Commander_Editor
{
  /// <summary>
  /// Interaction logic for MapEditor.xaml
  /// </summary>
  public partial class MapEditor : UserControl, INotifyPropertyChanged
  {
    private Point pointOnClick; // Click Position for panning
    private Point lastMTx;//last mouse position
    private readonly ScaleTransform scaleTransform;
    private readonly TranslateTransform translateTransform;
    private readonly RotateTransform rotateTransform;
    private readonly TransformGroup transformGroup;
    private bool canvasDown = false;
    private bool shapeDown = false;
    private IMapEntity shapeDragging = null;

    private bool _showPanel, _nothingSelected, _tilePropsEnabled, _entityPropsEnabled;
    private IPropertyModel _propModel;
    private MapTile _selectedMapTile;
    private UIElement mcap;
    private IMapEntity _selectedEntity;

    public double mScale = 1;
    public MainWindow parent
    {
      get
      {
        return (MainWindow)Window.GetWindow( this );
      }
    }

    public bool showPanel { get { return _showPanel; } set { _showPanel = value; PC(); } }
    public bool tilePropsEnabled { get { return _tilePropsEnabled; } set { _tilePropsEnabled = value; PC(); } }
    public bool entityPropsEnabled { get { return _entityPropsEnabled; } set { _entityPropsEnabled = value; PC(); } }
    public bool nothingSelected { get { return _nothingSelected; } set { _nothingSelected = value; PC(); } }
    public IPropertyModel propModel { get { return _propModel; } set { _propModel = value; PC(); nothingSelected = _propModel == null; } }
    public MapSection selectedMapSection { get { return parent.activeSection; } }
    public IMapEntity selectedEntity
    {
      get { return _selectedEntity; }
      set
      {
        _selectedEntity?.mapRenderer.Unselect();
        _selectedEntity = value;
        _selectedEntity?.mapRenderer.Select();
        PC();
        SetSelectedPropertyPanel();
        tilePropsEnabled = _selectedEntity is MapTile;
        entityPropsEnabled = !(_selectedEntity is MapTile) && _selectedEntity != null;
        if ( _selectedEntity is MapTile )
          selectedMapTile = _selectedEntity as MapTile;
        else
          selectedMapTile = null;
      }
    }
    public MapTile selectedMapTile { get { return _selectedMapTile; } set { _selectedMapTile = value; PC(); } }

    public event PropertyChangedEventHandler PropertyChanged;
    public void PC( [CallerMemberName] string n = "" )
    {
      if ( !string.IsNullOrEmpty( n ) )
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
    }

    public MapEditor()
    {
      InitializeComponent();

      DataContext = this;

      translateTransform = new TranslateTransform();
      scaleTransform = new ScaleTransform();
      rotateTransform = new RotateTransform();

      transformGroup = new TransformGroup();
      transformGroup.Children.Add( scaleTransform );
      transformGroup.Children.Add( translateTransform );
      transformGroup.Children.Add( rotateTransform );

      MainCanvas.RenderTransform = transformGroup;

      nothingSelected = true;
      showPanel = true;
      selectedEntity = null;
      selectedMapTile = null;
    }

    public void SetSelectedPropertyPanel()
    {
      if ( selectedEntity is Crate )
        propModel = new CrateProps() { DataContext = selectedEntity };
      else if ( selectedEntity is DeploymentPoint )
        propModel = new DeploymentProps() { DataContext = selectedEntity };
      else if ( selectedEntity is Token )
        propModel = new TokenProps() { DataContext = selectedEntity };
      else if ( selectedEntity is Console )
        propModel = new ComputerProps() { DataContext = selectedEntity };
      else if ( selectedEntity is SpaceHighlight )
        propModel = new HighlightProps() { DataContext = selectedEntity };
      else if ( selectedEntity is Door )
        propModel = new DoorProps() { DataContext = selectedEntity };
      else
        propModel = null;
    }

    private void MoveObject( MouseEventArgs e )
    {
      double txCorrected = 1f / mScale;
      Point currentPosition = e.GetPosition( this );
      Point delta = (Point)((currentPosition - lastMTx) * txCorrected);

      if ( shapeDragging != null )
      {
        shapeDragging.mapRenderer.Highlight();
        shapeDragging.mapRenderer.Update( delta );
      }

      lastMTx = e.GetPosition( (FrameworkElement)MainCanvas.Parent );
    }

    private void CanvasPan( MouseEventArgs e )
    {
      //Return if mouse is not captured
      if ( /*!MainCanvas.IsMouseCaptured ||*/ !canvasDown )
        return;
      //Point on move from Parent
      Point pointOnMove = e.GetPosition( (FrameworkElement)MainCanvas.Parent );
      //set TranslateTransform
      translateTransform.X = MainCanvas.RenderTransform.Value.OffsetX - (pointOnClick.X - pointOnMove.X);
      translateTransform.Y = MainCanvas.RenderTransform.Value.OffsetY - (pointOnClick.Y - pointOnMove.Y);
      //clamp to bounds

      //Update pointOnClick
      pointOnClick = e.GetPosition( (FrameworkElement)MainCanvas.Parent );
    }

    private void MainCanvas_MouseRightButtonDown( object sender, MouseButtonEventArgs e )
    {
      var s = e.OriginalSource;
      if ( s is Shape shape )
      {

        if ( shape.DataContext is MapTile )
        {
          selectedEntity = shape.DataContext as MapTile;
          (shape.DataContext as MapTile).mapRenderer.Rotate( 1 );
        }
        else if ( shape.DataContext is Door )
        {
          selectedEntity = shape.DataContext as IMapEntity;
          (shape.DataContext as Door).mapRenderer.Rotate( 1 );
        }
      }
    }

    private void MainCanvas_MouseWheel( object sender, MouseWheelEventArgs e )
    {
      //Point de la souris
      Point mousePosition = e.GetPosition( MainCanvas );
      //Actual Zoom
      double zoomNow = Math.Round( MainCanvas.RenderTransform.Value.M11, 1 );
      //ZoomScale
      double zoomScale = 0.1;
      double zoomCorrected = zoomScale * scaleTransform.ScaleX;
      //Positive or negative zoom
      double valZoom = e.Delta > 0 ? zoomScale + zoomCorrected : -(zoomScale + zoomCorrected);

      //Point de la souris pour le panning et zoom/dezoom
      //Point pointOnMove = e.GetPosition( (FrameworkElement)MainCanvas.Parent );
      //RenderTransformOrigin (doesn't fully working)
      //MainCanvas.RenderTransformOrigin = new Point( mousePosition.X / MainCanvas.ActualWidth, mousePosition.Y / MainCanvas.ActualHeight );
      //Appel du zoom
      Zoom( new Point( mousePosition.X, mousePosition.Y ), zoomNow + valZoom );
    }

    private void Zoom( Point point, double scale )
    {
      scale = Math.Clamp( scale, .5d, 7d );
      mScale = scale;
      //calculate centers
      double centerX = (point.X - translateTransform.X) / scaleTransform.ScaleX;
      double centerY = (point.Y - translateTransform.Y) / scaleTransform.ScaleY;

      scaleTransform.ScaleX = scale;
      scaleTransform.ScaleY = scale;

      translateTransform.X = point.X - centerX * scaleTransform.ScaleX;
      translateTransform.Y = point.Y - centerY * scaleTransform.ScaleY;
    }

    private void panelToggleButton_Click( object sender, RoutedEventArgs e )
    {
      showPanel = !showPanel;
      if ( !showPanel )
        panelToggleButton.Content = new PackIcon() { Kind = PackIconKind.ArrowLeft };
      else
        panelToggleButton.Content = new PackIcon() { Kind = PackIconKind.ArrowRight };
    }

    private void AddEntity( IMapEntity e )
    {
      e.BuildRenderer( MainCanvas, new Vector( translateTransform.X, translateTransform.Y ), showPanel, mScale );
      parent.mission.mapEntities.Add( e );
      selectedEntity = e;
    }

    private void addCrateButton_Click( object sender, RoutedEventArgs e )
    {
      AddEntity( new Crate() );
    }

    private void addDoorButton_Click( object sender, RoutedEventArgs e )
    {
      AddEntity( new Door() );
    }

    private void addDeploymentButton_Click( object sender, RoutedEventArgs e )
    {
      AddEntity( new DeploymentPoint( Utils.deploymentColors ) );
    }

    private void addTileButton_Click( object sender, RoutedEventArgs e )
    {
      OnAddTile();
    }

    private void addConsoleButton_Click( object sender, RoutedEventArgs e )
    {
      AddEntity( new Console() );
    }

    private void addTokenButton_Click( object sender, RoutedEventArgs e )
    {
      AddEntity( new Token() );
    }

    private void addHighlightButton_Click( object sender, RoutedEventArgs e )
    {
      AddEntity( new SpaceHighlight( Utils.deploymentColors ) );
    }

    private void centerButton_Click( object sender, RoutedEventArgs e )
    {
      CenterMap();
    }

    private void CenterMap()
    {
      var w = Utils.mainWindow.Width;
      var h = Utils.mainWindow.Height;
      mScale = 1;
      scaleTransform.ScaleX = mScale;
      scaleTransform.ScaleY = mScale;
      translateTransform.X = -1000 + (w / 2) - (showPanel ? 225 : 0);
      translateTransform.Y = -1000 + (h / 2) - 125;
    }

    private void MainCanvas_MouseDown( object sender, MouseButtonEventArgs e )
    {
      if ( e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && !(e.OriginalSource is Shape)) )
      {
        canvasDown = true;
      }
      else if ( e.ChangedButton == MouseButton.Left && e.OriginalSource is Shape shape )
      {
        shapeDragging = shape.DataContext as IMapEntity;
        if ( shapeDragging is IMapEntity )
        {
          selectedEntity = shapeDragging;
          selectedEntity.mapRenderer.Select();
        }
        shapeDown = true;
      }

      mcap = (UIElement)e.OriginalSource;
      mcap.CaptureMouse();

      //Store click position relation to Parent of the canvas
      pointOnClick = e.GetPosition( (FrameworkElement)MainCanvas.Parent );
      lastMTx = pointOnClick;
    }

    private void MainCanvas_MouseUp( object sender, MouseButtonEventArgs e )
    {
      if ( e.ChangedButton == MouseButton.Middle )
      {
      }
      else if ( e.ChangedButton == MouseButton.Left )
      {
        if ( shapeDragging != null )
        {
          shapeDragging.mapRenderer.RoundPosition();
          shapeDragging.mapRenderer.Select();
          shapeDragging = null;
        }
      }

      mcap?.ReleaseMouseCapture();
      //Set cursor by default
      Mouse.OverrideCursor = null;
      canvasDown = shapeDown = false;
    }

    private void MainCanvas_MouseMove( object sender, MouseEventArgs e )
    {
      //showToolTip( e );

      if ( canvasDown )
        CanvasPan( e );
      if ( shapeDown )
        MoveObject( e );
    }

    private void centerSelectedButton_Click( object sender, RoutedEventArgs e )
    {
      if ( selectedEntity != null )
      {
        CenterSelection( selectedEntity );
      }
    }

    private void centerTileButton_Click( object sender, RoutedEventArgs e )
    {
      if ( selectedMapTile != null )
        CenterSelection( selectedMapTile );
    }

    private void removeTileButton_Click( object sender, RoutedEventArgs e )
    {
      if ( selectedEntity != null )
      {
        selectedEntity?.mapRenderer.RemoveVisual();
        if ( !(selectedEntity is MapTile) )
          parent.mission.mapEntities.Remove( selectedEntity );
        else
          selectedMapSection.mapTiles.Remove( selectedEntity as MapTile );
        selectedEntity = null;
      }
    }

    private void tileGalleryBtn_Click( object sender, RoutedEventArgs e )
    {
      OnAddTile();
    }

    private void removeEntityButton_Click( object sender, RoutedEventArgs e )
    {
      if ( !(selectedEntity is MapTile) && selectedEntity != null )
      {
        selectedEntity?.mapRenderer.RemoveVisual();
        parent.mission.mapEntities.Remove( selectedEntity );
      }
    }

    private void CenterSelection( IMapEntity e )
    {
      if ( e != null )
      {
        var w = Utils.mainWindow.Width;
        var h = Utils.mainWindow.Height;
        mScale = 1;
        scaleTransform.ScaleX = mScale;
        scaleTransform.ScaleY = mScale;

        var sx = Canvas.GetLeft( e.mapRenderer.entityShape );
        var sy = Canvas.GetTop( e.mapRenderer.entityShape );
        sx = 0 - sx;
        sy = 0 - sy;
        translateTransform.X = sx + (w / 2) - (showPanel ? 225 : 0);
        translateTransform.Y = sy + (h / 2) - 125;
      }
    }

    private void UserControl_PreviewKeyDown( object sender, KeyEventArgs e )
    {
      if ( e.Key == Key.M )
      {
        CenterMap();
      }
      if ( e.Key == Key.S && selectedEntity != null )
      {
        CenterSelection( selectedEntity );
      }
      else if ( (selectedEntity is MapTile || selectedEntity is Door) && e.Key == Key.OemOpenBrackets )
      {
        selectedEntity?.mapRenderer.Rotate( 1 );
      }
      else if ( (selectedEntity is MapTile || selectedEntity is Door) && e.Key == Key.OemCloseBrackets )
      {
        selectedEntity?.mapRenderer.Rotate( -1 );
      }
      else if ( e.Key == Key.X )//&& (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control )
      {
        if ( selectedEntity != null )
        {
          selectedEntity?.mapRenderer.RemoveVisual();
          if ( !(selectedEntity is MapTile) )
            parent.mission.mapEntities.Remove( selectedEntity );
          else
            selectedMapSection.mapTiles.Remove( selectedEntity as MapTile );
          selectedEntity = null;
        }
      }
    }

    private void OnAddTile()
    {
      var tg = new TileGallery();
      var result = tg.ShowDialog();
      if ( result == true )
      {
        GalleryTile gt = tg.selectedTile;
        MapTile t = new( gt.tileNumber.ToString(), gt.selectedExpansion, gt.tileSide );
        t.BuildRenderer( MainCanvas, new Vector( translateTransform.X, translateTransform.Y ), showPanel, mScale );
        parent.activeSection.mapTiles.Add( t );
        selectedEntity = t;
      }
    }

    public void LoadMap()
    {
      foreach ( var s in Utils.mainWindow.mission.mapSections )
      {
        foreach ( var tile in s.mapTiles )
        {
          ((MapTile)tile).BuildRenderer( MainCanvas, tile.entityPosition, true, mScale );
          selectedEntity = tile;
        }
      }
      foreach ( var item in Utils.mainWindow.mission.mapEntities )
      {
        item.BuildRenderer( MainCanvas, item.entityPosition, showPanel, mScale );
        selectedEntity = item;
      }
    }
  }
}
