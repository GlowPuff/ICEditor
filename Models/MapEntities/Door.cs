using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public class Door : INotifyPropertyChanged, IMapEntity
	{
		string _name;

		//common props
		public Guid GUID { get; set; }
		public string name { get { return _name; } set { _name = value; PC(); } }
		public EntityType entityType { get; set; }
		public Vector entityPosition { get; set; }
		public double entityRotation { get; set; }
		[JsonIgnore]
		public EntityRenderer mapRenderer { get; set; }
		public EntityProperties entityProperties { get; set; }
		public Guid mapSectionOwner { get; set; }

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
			entityProperties = new();
			mapSectionOwner = ownderGUID;
		}

		public IMapEntity Duplicate()
		{
			var dupe = new Door();
			dupe.GUID = Guid.NewGuid();
			dupe.name = name + " (Duplicate)";
			dupe.entityType = entityType;
			dupe.entityPosition = entityPosition;
			dupe.entityRotation = entityRotation;
			dupe.mapSectionOwner = mapSectionOwner;
			return dupe;
		}

		public void BuildRenderer( Canvas c, Vector where, bool showPanel, double scale )
		{
			mapRenderer = new( this, where, showPanel, scale, new( 2, 2 ) )
			{
				unselectedBGColor = new SolidColorBrush( Colors.Transparent ),
				selectedBGColor = new SolidColorBrush( Colors.Transparent ),
				unselectedStrokeColor = new SolidColorBrush( Colors.Transparent ),
				selectedZ = 300
			};
			mapRenderer.BuildShape( TokenShape.Square );
			mapRenderer.BuildImage( "pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/door.png" );
			c.Children.Add( mapRenderer.entityImage );
			c.Children.Add( mapRenderer.entityShape );
		}
	}
}
