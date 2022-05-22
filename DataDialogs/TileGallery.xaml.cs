using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
				tileObserverA.Clear();
				tileObserverB.Clear();
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
		public ObservableCollection<GalleryTile> tileQueue { get; set; } = new();
		public bool addQueue = false;

		public TileGallery()
		{
			InitializeComponent();
			DataContext = this;

			expansionsList.Add( "CORE" );
			expansionsList.Add( "BESPIN" );
			expansionsList.Add( "EMPIRE" );
			expansionsList.Add( "HOTH" );
			expansionsList.Add( "JABBA" );
			expansionsList.Add( "LOTHAL" );
			expansionsList.Add( "TWIN" );

			_selectedTileSideA = true;
			selectedExpansion = "CORE";

			tileIsSelected = false;
		}

		static int tileCount( string expansion )
		{
			return Utils.tileData.Where( x => x.expansion.ToLower() == expansion.ToLower() ).Count();
		}

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

		private void tileButton_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.ChangedButton == System.Windows.Input.MouseButton.Right )
			{
				tileQueue.Add( (sender as FrameworkElement).DataContext as GalleryTile );
			}
		}

		private void addSelectedBtn_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = true;
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.ChangedButton == System.Windows.Input.MouseButton.Left )
				DragMove();
		}

		private void addToQueueBtn_Click( object sender, RoutedEventArgs e )
		{
			tileQueue.Add( selectedTile );
		}

		private void remQueueBtn_Click( object sender, RoutedEventArgs e )
		{
			tileQueue.Remove( (sender as FrameworkElement).DataContext as GalleryTile );
		}

		private void insertQueueBtn_Click( object sender, RoutedEventArgs e )
		{
			addQueue = true;
			DialogResult = true;
			Close();
		}

		private void closeBtn_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = false;
			Close();
		}
	}
}
