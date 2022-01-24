using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

		private bool _nothingSelected, _tilePropsEnabled, _entityPropsEnabled, _filterBySection;
		private IPropertyModel _propModel;
		private MapTile _selectedMapTile;
		private UIElement mcap;
		private IMapEntity _selectedEntity;
		private MapSection _selectedMapSection;

		public double mScale = 1;
		public MainWindow parent
		{
			get
			{
				return (MainWindow)Window.GetWindow( this );
			}
		}

		public bool tilePropsEnabled { get { return _tilePropsEnabled; } set { _tilePropsEnabled = value; PC(); } }
		public bool filterBySection { get { return _filterBySection; } set { _filterBySection = value; PC(); } }
		public bool entityPropsEnabled { get { return _entityPropsEnabled; } set { _entityPropsEnabled = value; PC(); } }
		public bool nothingSelected { get { return _nothingSelected; } set { _nothingSelected = value; PC(); } }
		public IPropertyModel propModel { get { return _propModel; } set { _propModel = value; PC(); nothingSelected = _propModel == null; } }
		public MapSection selectedMapSection { get { return _selectedMapSection; } set { _selectedMapSection = value; PC(); } }
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
		public ObservableCollection<IMapEntity> mapEntities { get; set; } = new();

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
			selectedEntity = null;
			selectedMapTile = null;
			filterBySection = false;
		}

		public void UpdateUI( MapSection ms = null )
		{
			selectedMapSection = ms ?? selectedMapSection;
			if ( filterBySection )
			{
				IMapEntity prev = selectedEntity;
				mapEntities.Clear();
				foreach ( var item in parent.mission.mapEntities.Where( x => x.mapSectionOwner == selectedMapSection.GUID ) )
				{
					mapEntities.Add( item );
				}
				mapEntities.Sort<IMapEntity>();
				selectedEntity = prev;
			}
			else
			{
				IMapEntity prev = selectedEntity;
				mapEntities.Clear();
				foreach ( var item in parent.mission.mapEntities )
				{
					mapEntities.Add( item );
				}
				mapEntities.Sort<IMapEntity>();
				selectedEntity = prev;
			}
		}

		public void SetSelectedPropertyPanel()
		{
			if ( selectedEntity is Crate )
				propModel = new CrateProps( selectedEntity as Crate );
			else if ( selectedEntity is DeploymentPoint )
				propModel = new DeploymentProps( selectedEntity as DeploymentPoint );
			else if ( selectedEntity is Token )
				propModel = new TokenProps( selectedEntity as Token );
			else if ( selectedEntity is Console )
				propModel = new ComputerProps( selectedEntity as Console );
			else if ( selectedEntity is SpaceHighlight )
				propModel = new HighlightProps( selectedEntity as SpaceHighlight );
			else if ( selectedEntity is Door )
				propModel = new DoorProps( selectedEntity as Door );
			else if ( selectedEntity is MapTile )
				propModel = new TileProps( selectedEntity as MapTile );
			else
				propModel = null;
		}

		public void InsertDuplicateEntity( IMapEntity item )
		{
			item.entityPosition = new( item.entityPosition.X + 10, item.entityPosition.Y + 10 );
			Vector p = item.entityPosition;
			item.BuildRenderer( MainCanvas, p, mScale );
			item.mapRenderer.SetPosition( p );
			item.mapRenderer.SetRotation( item.entityRotation );
			parent.mission.mapEntities.Add( item );
			selectedEntity = item;
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
			Point mousePosition = e.GetPosition( MainCanvas );
			//Actual Zoom
			double zoomNow = Math.Round( MainCanvas.RenderTransform.Value.M11, 1 );

			//ZoomScale
			double zoomScale = 0.1;
			if ( (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift )
				zoomScale = .025f;
			double zoomCorrected = zoomScale * scaleTransform.ScaleX;
			//Positive or negative zoom
			double valZoom = e.Delta > 0 ? zoomScale + zoomCorrected : -(zoomScale + zoomCorrected);

			if ( zoomNow + valZoom >= 7 || zoomNow + valZoom <= .5 )
				return;

			Zoom( new Point( mousePosition.X, mousePosition.Y ), zoomNow + valZoom );
		}

		private void Zoom( Point point, double scale )
		{
			scale = Math.Clamp( scale, .5d, 7d );
			mScale = scale;
			//calculate centers
			//double centerX = (point.X - translateTransform.X) / scaleTransform.ScaleX;
			//double centerY = (point.Y - translateTransform.Y) / scaleTransform.ScaleY;
			Utils.Log( "==============" );
			//Utils.Log( $"CENTER: {centerX}, {centerY}" );
			Utils.Log( $"SCALE: {mScale}" );

			var w = Utils.mainWindow.ActualWidth;
			var h = Utils.mainWindow.ActualHeight;
			double cx = 0, cy = 0;
			if ( selectedEntity != null && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control )
			{
				cx = (-selectedEntity.entityPosition.X * mScale) + (w / 2) - 225;
				cy = (-selectedEntity.entityPosition.Y * mScale) + (h / 2) - 125;
				translateTransform.X = cx;
				translateTransform.Y = cy;
			}
			else
			{
				cx = (-1000 * mScale) + (w / 2) - 225;//(-point.X * mScale) + (w / 2) - 225;
				cy = (-1000 * mScale) + (h / 2) - 125;//(-point.Y * mScale) + (h / 2) - 125;
				CenterMap( new( point.X, point.Y ) );
			}
			Utils.Log( $"CX/Y: {cx}, {cy}" );

			scaleTransform.ScaleX = scale;
			scaleTransform.ScaleY = scale;

			//translateTransform.X = point.X - centerX * scaleTransform.ScaleX;
			//translateTransform.Y = point.Y - centerY * scaleTransform.ScaleY;

			//translateTransform.X = cx;
			//translateTransform.Y = cy;

			Utils.Log( $"TRANS: {translateTransform.X}, {translateTransform.Y}" );
			Utils.Log( $"MOUSE: {point.X}, {point.Y}" );
		}

		private void panelToggleButton_Click( object sender, RoutedEventArgs e )
		{
			//showPanel = !showPanel;
			//if ( !showPanel )
			//	panelToggleButton.Content = new PackIcon() { Kind = PackIconKind.ArrowLeft };
			//else
			//	panelToggleButton.Content = new PackIcon() { Kind = PackIconKind.ArrowRight };
		}

		private void AddEntity( IMapEntity e )
		{
			e.BuildRenderer( MainCanvas, new Vector( translateTransform.X, translateTransform.Y ), mScale );
			parent.mission.mapEntities.Add( e );
			selectedEntity = e;
			UpdateUI();
		}

		private void addCrateButton_Click( object sender, RoutedEventArgs e )
		{
			AddEntity( new Crate( Utils.mainWindow.activeSection.GUID ) );
		}

		private void addDoorButton_Click( object sender, RoutedEventArgs e )
		{
			AddEntity( new Door( Utils.mainWindow.activeSection.GUID ) );
		}

		private void addDeploymentButton_Click( object sender, RoutedEventArgs e )
		{
			AddEntity( new DeploymentPoint( Utils.mainWindow.activeSection.GUID ) );
		}

		private void addTileButton_Click( object sender, RoutedEventArgs e )
		{
			OnAddTile();
		}

		private void addConsoleButton_Click( object sender, RoutedEventArgs e )
		{
			AddEntity( new Console( Utils.mainWindow.activeSection.GUID ) );
		}

		private void addTokenButton_Click( object sender, RoutedEventArgs e )
		{
			AddEntity( new Token( Utils.mainWindow.activeSection.GUID ) );
		}

		private void addHighlightButton_Click( object sender, RoutedEventArgs e )
		{
			AddEntity( new SpaceHighlight( Utils.mainWindow.activeSection.GUID ) );
		}

		private void centerButton_Click( object sender, RoutedEventArgs e )
		{
			CenterMap( new( 1000, 1000 ) );
		}

		private void CenterMap( Vector e )
		{
			var w = Utils.mainWindow.ActualWidth;
			var h = Utils.mainWindow.ActualHeight;
			var cx = (-e.X * mScale) + (w / 2) - 225;
			var cy = (-e.Y * mScale) + (h / 2) - 125;
			translateTransform.X = cx;
			translateTransform.Y = cy;
			//mScale = 1;
			//scaleTransform.ScaleX = mScale;
			//scaleTransform.ScaleY = mScale;
			//translateTransform.X = -1000 + (w / 2) - 225;
			//translateTransform.Y = -1000 + (h / 2) - 125;

			//Utils.Log( $"{translateTransform.X}, {translateTransform.Y}" );
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
				UpdateUI();
			}
		}

		private void CenterSelection( IMapEntity e )
		{
			if ( e != null )
			{
				var w = Utils.mainWindow.ActualWidth;
				var h = Utils.mainWindow.ActualHeight;
				var cx = (-e.entityPosition.X * mScale) + (w / 2) - 225;
				var cy = (-e.entityPosition.Y * mScale) + (h / 2) - 125;
				translateTransform.X = cx;
				translateTransform.Y = cy;
			}
		}

		public void ProcessKey( KeyEventArgs e )
		{
			IInputElement focusedControl = Keyboard.FocusedElement;
			if ( focusedControl != null && focusedControl is TextBox )
				return;

			if ( e.Key == Key.M )
			{
				CenterMap( new( 1000, 1000 ) );
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
			else if ( e.Key == Key.X )
			{
				if ( selectedEntity != null )
				{
					selectedEntity?.mapRenderer.RemoveVisual();
					if ( !(selectedEntity is MapTile) )
					{
						parent.mission.mapEntities.Remove( selectedEntity );
						UpdateUI();
					}
					else
						selectedMapSection.mapTiles.Remove( selectedEntity as MapTile );
					selectedEntity = null;
					this.Focus();
				}
			}
			else if ( e.Key == Key.D )
			{
				if ( selectedEntity != null && !(selectedEntity is MapTile) )
				{
					var item = selectedEntity.Duplicate();
					InsertDuplicateEntity( item );
					UpdateUI();
				}
			}
		}

		private void UserControl_PreviewKeyDown( object sender, KeyEventArgs e ) { }

		private void OnAddTile()
		{
			var tg = new TileGallery();
			var result = tg.ShowDialog();
			if ( result == true )
			{
				if ( !tg.addQueue )
				{
					GalleryTile gt = tg.selectedTile;
					MapTile t = new( gt.tileNumber.ToString(), gt.selectedExpansion, gt.tileSide );
					t.BuildRenderer( MainCanvas, new Vector( translateTransform.X, translateTransform.Y ), mScale );
					parent.activeSection.mapTiles.Add( t );
					selectedEntity = t;
				}
				else
				{
					foreach ( var item in tg.tileQueue )
					{
						GalleryTile gt = item;
						MapTile t = new( gt.tileNumber.ToString(), gt.selectedExpansion, gt.tileSide );
						t.BuildRenderer( MainCanvas, new Vector( translateTransform.X, translateTransform.Y ), mScale );
						parent.activeSection.mapTiles.Add( t );
						selectedEntity = t;
					}
				}
			}
		}

		private void filterCheck_Click( object sender, RoutedEventArgs e )
		{
			UpdateUI();
		}

		private void switchSectionBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedEntity != null )
			{
				if ( Utils.mainWindow.mission.mapSections.Any( x => x.GUID == selectedEntity.mapSectionOwner ) )
				{
					Utils.mainWindow.activeSection = Utils.mainWindow.mission.mapSections.First( x => x.GUID == selectedEntity.mapSectionOwner );
				}
			}
		}

		private void TextBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		public void LoadMap()
		{
			foreach ( var s in Utils.mainWindow.mission.mapSections )
			{
				foreach ( var tile in s.mapTiles )
				{
					Vector p = tile.entityPosition;
					((MapTile)tile).BuildRenderer( MainCanvas, p, mScale );
					tile.mapRenderer.SetPosition( p );
					tile.mapRenderer.SetRotation( tile.entityRotation );
					selectedEntity = tile;
				}
			}
			//foreach ( var section in Utils.mainWindow.mission.mapSections )
			//{
			foreach ( var item in Utils.mainWindow.mission.mapEntities )
			{
				Vector p = item.entityPosition;
				item.BuildRenderer( MainCanvas, p, mScale );
				item.mapRenderer.SetPosition( p );
				item.mapRenderer.SetRotation( item.entityRotation );
				selectedEntity = item;
			}
			//}
		}

		public void OnWindowLoaded()
		{
			CenterMap( new( 1000, 1000 ) );
		}
	}
}
