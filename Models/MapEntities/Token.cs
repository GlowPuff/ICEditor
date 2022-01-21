﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public class Token : INotifyPropertyChanged, IMapEntity
	{
		string _name, _ownerName;
		string _tokenColor;
		MarkerType _markerType;
		Guid _mapSectionOwner;

		//common props
		public Guid GUID { get; set; }
		public string name
		{
			get { return _name; }
			set
			{
				_name = string.IsNullOrEmpty( value ) ? "New Marker" : value;
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
		public string ownerName { get { return _ownerName; } set { _ownerName = value; PC(); } }

		//token props
		public string tokenColor
		{
			get { return _tokenColor; }
			set
			{
				_tokenColor = value;
				PC();
				if ( mapRenderer != null )
				{
					Color c = Utils.ColorFromName( _tokenColor ).color;
					mapRenderer.entityShape.Fill = new SolidColorBrush( c );
					mapRenderer.unselectedBGColor = new SolidColorBrush( c );
					mapRenderer.selectedBGColor = new SolidColorBrush( c );
				}
			}
		}
		public MarkerType markerType
		{
			get { return _markerType; }
			set
			{
				_markerType = value;
				PC();
				Color c = Utils.ColorFromName( _tokenColor ).color;
				Color ol = Colors.Gray;
				if ( markerType == MarkerType.Rebel )
					ol = Colors.CornflowerBlue;
				else if ( markerType == MarkerType.Imperial )
					ol = Colors.Red;
				if ( mapRenderer != null )
					mapRenderer.unselectedStrokeColor = new( ol );
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public Token()
		{
		}

		public Token( Guid ownderGUID )
		{
			GUID = Guid.NewGuid();
			name = "New Marker";
			entityType = EntityType.Token;
			entityProperties = new();
			mapSectionOwner = ownderGUID;
			ownerName = Utils.mainWindow?.mission.mapSections.First( x => x.GUID == mapSectionOwner ).name;

			tokenColor = "Gray";
			markerType = MarkerType.Neutral;
		}

		public IMapEntity Duplicate()
		{
			var dupe = new Token();
			dupe.GUID = Guid.NewGuid();
			dupe.name = name + " (Duplicate)";
			dupe.entityType = entityType;
			dupe.entityProperties = new();
			dupe.entityProperties.CopyFrom( this );
			dupe.entityPosition = entityPosition;
			dupe.entityRotation = entityRotation;
			dupe.mapSectionOwner = mapSectionOwner;
			dupe.ownerName = ownerName;
			dupe.tokenColor = tokenColor;
			return dupe;
		}

		public void BuildRenderer( Canvas canvas, Vector where, double scale )
		{
			Color c = Utils.ColorFromName( _tokenColor ).color;

			mapRenderer = new( this, where, scale, new( 1, 1 ) )
			{
				selectedZ = 300,
				selectedBGColor = new( c ),
				unselectedBGColor = new( c ),
				unselectedStrokeColor = new( Colors.Gray )
			};
			mapRenderer.BuildShape( TokenShape.Circle );
			canvas.Children.Add( mapRenderer.entityShape );
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
