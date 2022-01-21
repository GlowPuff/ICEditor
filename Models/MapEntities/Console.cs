using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public class Console : INotifyPropertyChanged, IMapEntity
	{
		string _name, _ownerName;
		Guid _mapSectionOwner;

		//common props
		public Guid GUID { get; set; }
		public string name
		{
			get { return _name; }
			set
			{
				_name = string.IsNullOrEmpty( value ) ? "New Console" : value;
				PC();
			}
		}
		public EntityType entityType { get; set; }
		public Vector entityPosition { get; set; }
		public double entityRotation { get; set; }
		[JsonIgnore]
		public EntityRenderer mapRenderer { get; set; }
		public EntityProperties entityProperties { get; set; }
		public Guid mapSectionOwner
		{
			get { return _mapSectionOwner; }
			set
			{
				_mapSectionOwner = value;
				PC();
				ownerName = Utils.mainWindow?.mission.mapSections.First( x => x.GUID == _mapSectionOwner ).name;
			}
		}
		public string ownerName { get { return _ownerName; } set { _ownerName = value; PC(); } }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public Console()
		{
		}

		public Console( Guid ownderGUID )
		{
			GUID = Guid.NewGuid();
			name = "New Console";
			entityType = EntityType.Console;
			entityProperties = new();
			mapSectionOwner = ownderGUID;
		}

		public IMapEntity Duplicate()
		{
			var dupe = new Console();
			dupe.GUID = Guid.NewGuid();
			dupe.name = name + " (Duplicate)";
			dupe.entityType = entityType;
			dupe.entityProperties = new();
			dupe.entityProperties.CopyFrom( this );
			dupe.entityPosition = entityPosition;
			dupe.entityRotation = entityRotation;
			dupe.mapSectionOwner = mapSectionOwner;
			return dupe;
		}

		public void BuildRenderer( Canvas c, Vector where, double scale )
		{
			mapRenderer = new( this, where, scale, new( 1, 1 ) )
			{
				selectedZ = 300,
				unselectedBGColor = new( Colors.SkyBlue ),
				selectedBGColor = new( Colors.SkyBlue )
			};
			mapRenderer.BuildShape( TokenShape.Square );
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
	}
}
