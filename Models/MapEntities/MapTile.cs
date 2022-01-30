using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imperial_Commander_Editor
{
	public class MapTile : INotifyPropertyChanged, IMapEntity
	{
		string _tileID, _tileSide, _name;
		Expansion _expansion;
		Guid _mapSectionOwner;

		public Guid GUID { get; set; }
		public string name
		{
			get { return _name; }
			set
			{
				if ( !string.IsNullOrEmpty( value ) )
					_name = value;
				else
					_name = $"{_expansion}{_tileID}{_tileSide}";
				PC();
			}
		}
		public EntityType entityType { get; set; }
		public Vector entityPosition { get; set; }
		public double entityRotation { get; set; }

		//tile props
		public string tileID { get { return _tileID; } set { _tileID = value; PC(); } }
		public string tileSide { get { return _tileSide; } set { _tileSide = value.ToUpper(); SetSide(); PC(); } }
		public Expansion expansion { get { return _expansion; } set { _expansion = value; PC(); } }
		public EntityProperties entityProperties { get; set; }
		public Guid mapSectionOwner { get { return _mapSectionOwner; } set { _mapSectionOwner = value; PC(); } }
		[JsonIgnore]
		public EntityRenderer mapRenderer { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public MapTile() { }

		public MapTile( string id, string exp = "Core", string side = "A" )
		{
			GUID = Guid.NewGuid();
			_tileSide = side;
			_tileID = id;
			_expansion = (Expansion)Enum.Parse( typeof( Expansion ), exp, true );
			_name = $"{_expansion}{_tileID}{_tileSide}";
			entityType = EntityType.Tile;
			entityProperties = new() { name = name };
			mapSectionOwner = Utils.mainWindow.activeSection.GUID;
		}

		public IMapEntity Duplicate()
		{
			return null;
		}

		public void BuildRenderer( Canvas c, Vector where, double scale )
		{
			var desc = Utils.tileData
				.Where(
				x => x.expansion.ToLower() == expansion.ToString().ToLower()
				&& x.id.ToString() == _tileID
				).First();

			mapRenderer = new( this, where, scale, new( desc.width, desc.height ) )
			{
				selectedZ = 100,
				unselectedStrokeWidth = .25d,
			};
			mapRenderer.BuildShape( TokenShape.Square );
			mapRenderer.BuildImage( $"pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/{expansion}/{expansion}_{tileID + tileSide}.jpg" );
			c.Children.Add( mapRenderer.entityImage );
			c.Children.Add( mapRenderer.entityShape );
		}

		public void SetSide()
		{
			if ( mapRenderer != null )
			{
				ImageSource image = new BitmapImage( new Uri( $"pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/{expansion}/{expansion}_{tileID + tileSide}.jpg" ) );
				mapRenderer.entityImage.Source = image;
			}
		}

		public bool Validate()
		{
			return true;
		}

		public void Dim( Guid guid )
		{
			mapRenderer.Dim( mapSectionOwner != guid );
		}
	}
}
