using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class Door : INotifyPropertyChanged, IMapEntity, IHasEventReference
	{
		string _name;
		Guid _mapSectionOwner;

		//common props
		public Guid GUID { get; set; }
		public string name
		{
			get { return _name; }
			set
			{
				_name = string.IsNullOrEmpty( value ) ? "New Door" : value;
				PC();
			}
		}
		public EntityType entityType { get; set; }
		public Vector entityPosition { get; set; }
		public double entityRotation { get; set; }
		[JsonIgnore]
		public EntityRenderer mapRenderer { get; set; }
		public EntityProperties entityProperties { get; set; }
		public Guid mapSectionOwner { get { return _mapSectionOwner; } set { _mapSectionOwner = value; PC(); } }
		public bool hasProperties { get { return true; } }
		public bool hasColor { get { return false; } }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public Door()
		{
		}

		public Door( Guid ownderGUID )
		{
			GUID = Guid.NewGuid();
			name = "New Door";
			entityType = EntityType.Door;
			entityProperties = new() { name = name, isActive = false };//default closed (isActive=false)
			mapSectionOwner = ownderGUID;
		}

		public IMapEntity Duplicate()
		{
			var dupe = new Door();
			dupe.GUID = Guid.NewGuid();
			dupe.name = name + " (Duplicate)";
			dupe.entityType = entityType;
			dupe.entityProperties = new();
			dupe.entityProperties.CopyFrom( this );
			dupe.entityProperties.name = dupe.name;
			dupe.entityPosition = entityPosition;
			dupe.entityRotation = entityRotation;
			dupe.mapSectionOwner = mapSectionOwner;
			return dupe;
		}

		public void BuildRenderer( Canvas c, Vector where, double scale )
		{
			mapRenderer = new( this, where, scale, new( 2, 2 ) )
			{
				unselectedBGColor = new SolidColorBrush( Colors.Transparent ),
				selectedBGColor = new SolidColorBrush( Colors.Transparent ),
				unselectedStrokeColor = new SolidColorBrush( Colors.Transparent ),
				selectedImageZ = 305,
				selectedZ = 300,
				unselectedStrokeWidth = .25f
			};
			mapRenderer.BuildShape( TokenShape.Square );
			mapRenderer.BuildImage( "pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/door.png" );
			c.Children.Add( mapRenderer.entityImage );
			c.Children.Add( mapRenderer.entityShape );
		}

		public bool Validate()
		{
			if ( !Utils.mainWindow.mission.EntityExists( GUID ) )
			{
				if ( GUID != Guid.Empty )
					name += " (NO LONGER VALID)";
				GUID = Guid.Empty;
				return false;
			}
			return true;
		}

		public void Dim( Guid guid )
		{
			mapRenderer.Dim( mapSectionOwner != guid );
		}

		public BrokenRefInfo NotifyEventRemoved( Guid guid, NotifyMode mode )
		{
			return entityProperties.NotifyEventRemoved( name, guid, GUID, mode );
		}
	}
}
