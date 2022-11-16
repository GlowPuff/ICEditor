using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public static class Utils
	{
		public const string formatVersion = "19";
		public const string appVersion = "1.0.26";

		public static List<DeploymentCard> allyData;
		public static List<DeploymentCard> enemyData;
		public static List<DeploymentCard> villainData;
		public static List<DeploymentCard> heroData;
		public static List<TileDescriptor> tileData;

		public static ObservableCollection<DeploymentColor> deploymentColors;

		public static MainWindow mainWindow
		{
			get { return Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(); }
		}

		public static Guid GUIDOne { get { return Guid.Parse( "11111111-1111-1111-1111-111111111111" ); } }

		public static void InitColors()
		{
			deploymentColors = new ObservableCollection<DeploymentColor>();
			deploymentColors.Add( new( "Gray", ColorFromFloats( .3301887f, .3301887f, .3301887f ) ) );
			deploymentColors.Add( new( "Purple", ColorFromFloats( .6784314f, 0f, 1f ) ) );
			deploymentColors.Add( new( "Black", ColorFromFloats( 0, 0, 0 ) ) );
			deploymentColors.Add( new( "Blue", ColorFromFloats( 0, 0.3294118f, 1 ) ) );
			deploymentColors.Add( new( "Green", ColorFromFloats( 0, 0.735849f, 0.1056484f ) ) );
			deploymentColors.Add( new( "Red", ColorFromFloats( 1, 0, 0 ) ) );
			deploymentColors.Add( new( "Yellow", ColorFromFloats( 1, 202f / 255f, 40f / 255f ) ) );
		}

		//public static void Init( MainWindow mw )
		//{
		//	mainWindow = mw;
		//}

		public static void LoadAllCardData()
		{
			LoadCardData();
			tileData = TileDescriptor.LoadData();
		}

		public static DeploymentColor ColorFromName( string n )
		{
			if ( string.IsNullOrEmpty( n ) )
				return deploymentColors[0];
			return deploymentColors.Where( x => x.name == n ).First();
		}

		public static Color ColorFromFloats( float r, float g, float b )
		{
			return Color.FromRgb(
				(byte)(r * 255f),
				(byte)(g * 255f),
				(byte)(b * 255f) );
		}

		public static void LoseFocus( Control element )
		{
			FrameworkElement parent = (FrameworkElement)element.Parent;
			while ( parent != null && parent is IInputElement && !((IInputElement)parent).Focusable )
			{
				parent = (FrameworkElement)parent.Parent;
			}

			DependencyObject scope = FocusManager.GetFocusScope( element );
			FocusManager.SetFocusedElement( scope, parent );
		}

		public static void Log( string s )
		{
			Debug.WriteLine( s );
		}

		public static void LoadCardData()
		{
			allyData = DeploymentCard.LoadData( "allies.json" );
			enemyData = DeploymentCard.LoadData( "enemies.json" );
			villainData = DeploymentCard.LoadData( "villains.json" );
			heroData = DeploymentCard.LoadData( "heroes.json" );

			enemyData = enemyData.Concat( villainData ).ToList();
		}

		/// <summary>
		/// Check if a Trigger exists in the mission
		/// </summary>
		public static bool ValidateTrigger( Guid guid )
		{
			return mainWindow.mission.TriggerExists( guid );
		}

		/// <summary>
		/// Check if an Event exists in the mission
		/// </summary>
		public static bool ValidateEvent( Guid guid )
		{
			return mainWindow.mission.EventExists( guid );
		}

		/// <summary>
		/// Check if a map entity exists in the mission
		/// </summary>
		public static bool ValidateMapEntity( Guid guid )
		{
			return mainWindow.mission.EntityExists( guid ) || guid == Utils.GUIDOne;
		}

		/// <summary>
		/// Remove all tiles/entities/events/triggers associated with this Section
		/// </summary>
		public static void RemoveMapSectionObjects( MapSection ms )
		{
			ms.triggers.Clear();
			ms.missionEvents.Clear();
			//map entities
			for ( int i = mainWindow.mission.mapEntities.Count - 1; i >= 0; i-- )
			{
				if ( mainWindow.mission.mapEntities[i].mapSectionOwner == ms.GUID )
				{
					mainWindow.mapEditor.RemoveEntityFromMap( mainWindow.mission.mapEntities[i] );
					mainWindow.mission.mapEntities.RemoveAt( i );
				}
			}
			//tiles
			for ( int i = ms.mapTiles.Count - 1; i >= 0; i-- )
			{
				mainWindow.mapEditor.RemoveEntityFromMap( ms.mapTiles[i] );
			}
			ms.mapTiles.Clear();

			mainWindow.mapEditor.UpdateUI();
		}

		/// <summary>
		/// Given map section "ms", set it's child objects' owner to default section
		/// </summary>
		public static void SetOwnerToDefaultSection( MapSection ms )
		{
			foreach ( var item in ms.triggers )
				if ( item.GUID != Guid.Empty )
					mainWindow.mission.globalTriggers.Add( item );
			foreach ( var item in ms.missionEvents )
				if ( item.GUID != Guid.Empty )
					mainWindow.mission.globalEvents.Add( item );
			//entities
			var entities = mainWindow.mission.mapEntities.Where( x => x.mapSectionOwner == ms.GUID ).ToList();
			foreach ( var entity in entities )
				entity.mapSectionOwner = Guid.Parse( "11111111-1111-1111-1111-111111111111" );
			//tiles
			foreach ( var item in ms.mapTiles )
				item.mapSectionOwner = Guid.Parse( "11111111-1111-1111-1111-111111111111" );
			mainWindow.mapEditor.UpdateUI();
		}

		public static bool CheckAndNotifyEventRemoved( Guid guid, NotifyMode notifyMode, string sourceName )
		{
			//notify all Triggers, Entities, and Event Actions that are IHasEventReference
			var brokenStartEvent = Guid.Empty;
			var brokenEAList = new List<Guid>();
			var brokenTriggerList = new List<Guid>();
			var brokenEntityList = new List<Guid>();
			var brokenEventGroupList = new List<Guid>();
			var allBrokenList = new List<BrokenRefInfo>();

			//mission starting event
			if ( mainWindow.mission.missionProperties.startingEvent == guid )
			{
				brokenStartEvent = guid;
				var info = new BrokenRefInfo()
				{
					isBroken = true,
					topOwnerName = "Mission Properties Tab",
					itemName = "Mission Properties",
					notifyType = NotifyType.StartingEvent,
					brokenGuid = guid,
					ownerGuid = Guid.Empty,
					details = "Event from [Starting Event]"
				};
				allBrokenList.Add( info );
			}

			//event groups
			foreach ( var group in mainWindow.mission.eventGroups )
			{
				var info = group.NotifyEventRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = group.name;
					info.notifyType = NotifyType.EventGroup;
					allBrokenList.Add( info );
					brokenEventGroupList.Add( ((EventGroup)group).GUID );
				}
			}

			//event actions
			foreach ( var ev in mainWindow.mission.GetAllEvents() )
			{
				foreach ( var ea in ev.eventActions.OfType<IHasEventReference>() )
				{
					var info = ea.NotifyEventRemoved( guid, NotifyMode.Report );
					if ( info.isBroken )
					{
						info.topOwnerName = ev.name;
						info.notifyType = NotifyType.Event;
						allBrokenList.Add( info );
						brokenEAList.Add( ((EventAction)ea).GUID );
					}
				}
			}
			//triggers
			foreach ( var trigger in mainWindow.mission.GetAllTriggers().OfType<IHasEventReference>() )
			{
				var info = trigger.NotifyEventRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = ((Trigger)trigger).name;
					info.notifyType = NotifyType.Trigger;
					allBrokenList.Add( info );
					brokenTriggerList.Add( ((Trigger)trigger).GUID );
				}
			}
			//entities
			foreach ( var entity in mainWindow.mission.mapEntities.OfType<IHasEventReference>() )
			{
				var info = entity.NotifyEventRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = ((IMapEntity)entity).name;
					info.notifyType = NotifyType.Entity;
					allBrokenList.Add( info );
					brokenEntityList.Add( ((IMapEntity)entity).GUID );
				}
			}

			if ( notifyMode == NotifyMode.Report && allBrokenList.Count > 0 )
				return true;

			List<string> infoMsg = new();
			if ( brokenStartEvent != Guid.Empty )
				infoMsg.Add( "Found a broken Event reference to the Mission Starting Event." );
			if ( brokenEventGroupList.Count > 0 )
				infoMsg.Add( $"Found {brokenEventGroupList.Count} broken Event reference(s) in all of this Mission's Event Groups." );
			if ( brokenEAList.Count > 0 )
				infoMsg.Add( $"Found {brokenEAList.Count} broken Event reference(s) in all of this Mission's Event Actions." );
			if ( brokenTriggerList.Count > 0 )
				infoMsg.Add( $"Found {brokenTriggerList.Count} broken Event reference(s) in all of this Mission's Triggers." );
			if ( brokenEntityList.Count > 0 )
				infoMsg.Add( $"Found {brokenEntityList.Count} broken Event reference(s) in all of this Mission's Map Entities." );

			if ( infoMsg.Count > 0 )
			{
				//string generalMsg = "Deleting this Event has created broken references to it elsewhere in the Mission.";

				BrokenRefWindow broken = new BrokenRefWindow( NotifyType.Event, $"{string.Join( "\n", infoMsg )}\n\nThese broken Event references will be changed to 'None (Global)'.  However, for some affected items, these broken Events will be REMOVED from them entirely.\n\nNOTE: This will affect Buttons, Input Ranges, and any other items within affected data.", allBrokenList, guid, sourceName );
				broken.ShowDialog();
			}
			return false;
		}

		public static bool CheckAndNotifyTriggerRemoved( Guid guid, NotifyMode notifyMode, string sourceName )
		{
			//notify all Entities, and Event Actions that are IHasTriggerReference
			var brokenEAList = new List<Guid>();
			var brokenEntityList = new List<Guid>();
			var brokenEventList = new List<Guid>();
			var allBrokenList = new List<BrokenRefInfo>();

			//events - additional triggers
			foreach ( var ev in mainWindow.mission.GetAllEvents() )
			{
				var info = ev.NotifyTriggerRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = ev.name;
					info.notifyType = NotifyType.Event;
					allBrokenList.Add( info );
					brokenEventList.Add( ev.GUID );
				}
			}

			//event actions
			foreach ( var ev in mainWindow.mission.GetAllEvents() )
			{
				foreach ( var ea in ev.eventActions.OfType<IHasTriggerReference>() )
				{
					var info = ea.NotifyTriggerRemoved( guid, NotifyMode.Report );
					if ( info.isBroken )
					{
						info.topOwnerName = ev.name;
						info.notifyType = NotifyType.Event;
						allBrokenList.Add( info );
						brokenEAList.Add( ((EventAction)ea).GUID );
					}
				}
			}
			//entities
			foreach ( var entity in mainWindow.mission.mapEntities.OfType<IHasTriggerReference>() )
			{
				var info = entity.NotifyTriggerRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = ((IMapEntity)entity).name;
					info.notifyType = NotifyType.Entity;
					allBrokenList.Add( info );
					brokenEntityList.Add( ((IMapEntity)entity).GUID );
				}
			}

			if ( notifyMode == NotifyMode.Report && allBrokenList.Count > 0 )
				return true;

			List<string> infoMsg = new();
			if ( brokenEventList.Count > 0 )
				infoMsg.Add( $"Found {brokenEventList.Count} broken Additional Trigger reference(s) in all of this Mission's Events." );
			if ( brokenEAList.Count > 0 )
				infoMsg.Add( $"Found {brokenEAList.Count} broken Trigger reference(s) in all of this Mission's Event Actions." );
			if ( brokenEntityList.Count > 0 )
				infoMsg.Add( $"\nFound {brokenEntityList.Count} broken Trigger reference(s) in all of this Mission's Map Entities." );

			if ( infoMsg.Count > 0 )
			{
				//string generalMsg = "Deleting this Trigger will create broken references to it elsewhere in the Mission.";

				BrokenRefWindow broken = new BrokenRefWindow( NotifyType.Trigger, $"{string.Join( "\n", infoMsg )}\n\nThese broken Trigger references will be updated to 'None (Global)'.  However, for some affected items, these broken Triggers will be REMOVED from them entirely.\n\nNOTE: This will affect Buttons, Input Ranges, and any other items within affected data.", allBrokenList, guid, sourceName );
				broken.ShowDialog();
			}
			return false;
		}

		public static bool CheckAndNotifyEntityRemoved( Guid guid, NotifyMode notifyMode, string sourceName )
		{
			//notify all Event Actions that are IHasEntityReference
			var brokenEAList = new List<Guid>();
			var brokenEntityGroupList = new List<Guid>();
			var allBrokenList = new List<BrokenRefInfo>();

			//entity groups
			foreach ( var group in mainWindow.mission.entityGroups )
			{
				var info = group.NotifyEntityRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = group.name;
					info.notifyType = NotifyType.EntityGroup;
					allBrokenList.Add( info );
					brokenEntityGroupList.Add( ((EntityGroup)group).GUID );
				}
			}

			//event actions
			foreach ( var ev in mainWindow.mission.GetAllEvents() )
			{
				foreach ( var ea in ev.eventActions.OfType<IHasEntityReference>() )
				{
					var info = ea.NotifyEntityRemoved( guid, NotifyMode.Report );
					if ( info.isBroken )
					{
						info.topOwnerName = ev.name;
						info.notifyType = NotifyType.Event;
						allBrokenList.Add( info );
						brokenEAList.Add( ((EventAction)ea).GUID );
					}
				}
			}

			if ( notifyMode == NotifyMode.Report && allBrokenList.Count > 0 )
				return true;

			List<string> infoMsg = new();
			if ( brokenEntityGroupList.Count > 0 )
				infoMsg.Add( $"Found {brokenEntityGroupList.Count} broken Entity reference(s) in all of this Mission's Entity Groups." );
			if ( brokenEAList.Count > 0 )
				infoMsg.Add( $"Found {brokenEAList.Count} broken Entity reference(s) in all of this Mission's Event Actions." );

			if ( infoMsg.Count > 0 )
			{
				//string generalMsg = "Deleting this Entity will create broken references to it elsewhere in the Mission.";

				BrokenRefWindow broken = new BrokenRefWindow( NotifyType.Entity, $"{string.Join( "\n", infoMsg )}\n\nThese broken Entity references will be updated to 'None (Global)'.  However, for some affected items, these broken Entities will be REMOVED from them entirely.", allBrokenList, guid, sourceName );
				broken.ShowDialog();
			}
			return false;
		}

		///EXTENSIONS
		public static double RoundOff( this double i, double value )
		{
			return ((double)Math.Round( i / value )) * value;
		}

		public static ObservableCollection<IMapEntity> Sort<T>( this ObservableCollection<IMapEntity> collection )
		{
			ObservableCollection<IMapEntity> temp;
			temp = new ObservableCollection<IMapEntity>( collection.OrderBy( p => p.name ) );
			collection.Clear();
			foreach ( IMapEntity j in temp )
				collection.Add( j );
			return collection;
		}
	}
}
