﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public static class Utils
	{
		public const string formatVersion = "21";
		public const string appVersion = "1.0.30";

		public static List<DeploymentCard> allyData;
		public static List<DeploymentCard> enemyData;
		public static List<DeploymentCard> villainData;
		public static List<DeploymentCard> heroData;
		public static List<TileDescriptor> tileData;
		public static ThumbnailData thumbnailData;

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

			thumbnailData = FileManager.LoadAsset<ThumbnailData>( "thumbnails.json" );
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
			allyData = FileManager.LoadAsset<List<DeploymentCard>>( "allies.json" );
			enemyData = FileManager.LoadAsset<List<DeploymentCard>>( "enemies.json" );
			villainData = FileManager.LoadAsset<List<DeploymentCard>>( "villains.json" );
			heroData = FileManager.LoadAsset<List<DeploymentCard>>( "heroes.json" );

			enemyData = enemyData.Concat( villainData ).ToList();
		}

		public static void AddCustomToon( DeploymentCard card )
		{
			enemyData.Add( card );
		}

		public static void RemoveCustomToon( DeploymentCard card )
		{
			enemyData.Remove( card );
		}

		public static string GetAvailableCustomToonID()
		{
			var usedIDs = enemyData.Where( x => x.id.Contains( "TC" ) ).Select( x => int.Parse( x.id.Substring( 2 ) ) ).ToList();
			int highest = 1;
			if ( usedIDs.Count() >= 1 )
			{
				for ( int i = 0; i < usedIDs.Count(); i++ )
				{
					if ( !usedIDs.Contains( highest + i + 1 ) )//start at 1
					{
						highest += i + 1;
						break;
					}
				}
			}
			return $"TC{highest}";
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

		public static HealthReport CheckAndNotifyEventRemoved( Guid guid, NotifyMode notifyMode, string sourceName )
		{
			//notify all Triggers, Entities, and Event Actions that are IHasEventReference
			var brokenStartEvent = Guid.Empty;
			var brokenEAList = new List<Guid>();
			var brokenTriggerList = new List<Guid>();
			var brokenEntityList = new List<Guid>();
			var brokenEventGroupList = new List<Guid>();
			var brokenInitialGroupList = new List<Guid>();
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
					topLevelNotifyType = NotifyType.StartingEvent,
					brokenGuid = guid,
					ownerGuid = Guid.Empty,
					details = "Event from [Starting Event]"
				};
				allBrokenList.Add( info );
			}

			//initial groups (defeated Event)
			foreach ( var group in mainWindow.mission.initialDeploymentGroups )
			{
				var info = group.NotifyEventRemoved( guid, notifyMode );
				if ( info.isBroken )
				{
					info.topOwnerName = group.cardName;
					info.topLevelNotifyType = NotifyType.InitialGroup;
					allBrokenList.Add( info );
					brokenInitialGroupList.Add( ((EnemyGroupData)group).GUID );
				}
			}

			//event groups
			foreach ( var group in mainWindow.mission.eventGroups )
			{
				var info = group.NotifyEventRemoved( guid, notifyMode );
				if ( info.isBroken )
				{
					info.topOwnerName = group.name;
					info.topLevelNotifyType = NotifyType.EventGroup;
					allBrokenList.Add( info );
					brokenEventGroupList.Add( ((EventGroup)group).GUID );
				}
			}

			//event actions
			foreach ( var ev in mainWindow.mission.GetAllEvents() )
			{
				foreach ( var ea in ev.eventActions.OfType<IHasEventReference>() )
				{
					var info = ea.NotifyEventRemoved( guid, notifyMode );
					if ( info.isBroken )
					{
						info.topOwnerName = ev.name;
						info.topLevelNotifyType = NotifyType.Event;
						allBrokenList.Add( info );
						brokenEAList.Add( ((EventAction)ea).GUID );
					}
				}
			}
			//triggers
			foreach ( var trigger in mainWindow.mission.GetAllTriggers().OfType<IHasEventReference>() )
			{
				var info = trigger.NotifyEventRemoved( guid, notifyMode );
				if ( info.isBroken )
				{
					info.topOwnerName = ((Trigger)trigger).name;
					info.topLevelNotifyType = NotifyType.Trigger;
					allBrokenList.Add( info );
					brokenTriggerList.Add( ((Trigger)trigger).GUID );
				}
			}
			//entities
			foreach ( var entity in mainWindow.mission.mapEntities.OfType<IHasEventReference>() )
			{
				var info = entity.NotifyEventRemoved( guid, notifyMode );
				if ( info.isBroken )
				{
					info.topOwnerName = ((IMapEntity)entity).name;
					info.topLevelNotifyType = NotifyType.Entity;
					allBrokenList.Add( info );
					brokenEntityList.Add( ((IMapEntity)entity).GUID );
				}
			}

			List<string> infoMsg = new();
			if ( brokenStartEvent != Guid.Empty )
				infoMsg.Add( "Found a broken Event reference to the Mission Starting Event." );
			if ( brokenInitialGroupList.Count > 0 )
				infoMsg.Add( $"Found {brokenInitialGroupList.Count} broken Event reference(s) in this Mission's Initial Enemy Groups." );
			if ( brokenEventGroupList.Count > 0 )
				infoMsg.Add( $"Found {brokenEventGroupList.Count} broken Event reference(s) in this Mission's Event Groups." );
			if ( brokenEAList.Count > 0 )
				infoMsg.Add( $"Found {brokenEAList.Count} broken Event reference(s) in this Mission's Event Actions." );
			if ( brokenTriggerList.Count > 0 )
				infoMsg.Add( $"Found {brokenTriggerList.Count} broken Event reference(s) in this Mission's Triggers." );
			if ( brokenEntityList.Count > 0 )
				infoMsg.Add( $"Found {brokenEntityList.Count} broken Event reference(s) in this Mission's Map Entities." );

			if ( notifyMode == NotifyMode.Report )
			{
				if ( allBrokenList.Count > 0 )
					return new()
					{
						isBroken = true,
						detailsMessage = string.Join( "\n", infoMsg ),
						brokenList = allBrokenList,
						startEvent = brokenStartEvent != Guid.Empty ? 0 : 1,
						initialGroups = brokenInitialGroupList.Count,
						eventGroups = brokenEventGroupList.Count,
						eventActions = brokenEAList.Count,
						triggers = brokenTriggerList.Count,
						entities = brokenEntityList.Count
					};
			}

			return new() { isBroken = false };
		}

		public static HealthReport CheckAndNotifyTriggerRemoved( Guid guid, NotifyMode notifyMode, string sourceName )
		{
			//notify all Entities, and Event Actions that are IHasTriggerReference
			var brokenEAList = new List<Guid>();
			var brokenEntityList = new List<Guid>();
			var brokenEventList = new List<Guid>();
			var brokenInitialGroupList = new List<Guid>();
			var allBrokenList = new List<BrokenRefInfo>();

			//initial groups (defeated Trigger)
			foreach ( var group in mainWindow.mission.initialDeploymentGroups )
			{
				var info = group.NotifyTriggerRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = group.cardName;
					info.topLevelNotifyType = NotifyType.InitialGroup;
					allBrokenList.Add( info );
					brokenInitialGroupList.Add( ((EnemyGroupData)group).GUID );
				}
			}

			//events - additional triggers
			foreach ( var ev in mainWindow.mission.GetAllEvents() )
			{
				var info = ev.NotifyTriggerRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = ev.name;
					info.topLevelNotifyType = NotifyType.Event;
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
						info.topLevelNotifyType = NotifyType.Event;
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
					info.topLevelNotifyType = NotifyType.Entity;
					allBrokenList.Add( info );
					brokenEntityList.Add( ((IMapEntity)entity).GUID );
				}
			}

			List<string> infoMsg = new();
			if ( brokenInitialGroupList.Count > 0 )
				infoMsg.Add( $"Found {brokenInitialGroupList.Count} broken Trigger reference(s) in all of this Mission's Initial Enemy Groups." );
			if ( brokenEventList.Count > 0 )
				infoMsg.Add( $"Found {brokenEventList.Count} broken Additional Trigger reference(s) in all of this Mission's Events." );
			if ( brokenEAList.Count > 0 )
				infoMsg.Add( $"Found {brokenEAList.Count} broken Trigger reference(s) in all of this Mission's Event Actions." );
			if ( brokenEntityList.Count > 0 )
				infoMsg.Add( $"\nFound {brokenEntityList.Count} broken Trigger reference(s) in all of this Mission's Map Entities." );

			if ( notifyMode == NotifyMode.Report )
			{
				if ( allBrokenList.Count > 0 )
					return new()
					{
						isBroken = true,
						detailsMessage = string.Join( "\n", infoMsg ),
						brokenList = allBrokenList,
						events = brokenEventList.Count,
						initialGroups = brokenInitialGroupList.Count,
						eventActions = brokenEAList.Count,
						entities = brokenEntityList.Count
					};
			}

			return new() { isBroken = false };
		}

		public static HealthReport CheckAndNotifyEntityRemoved( Guid guid, NotifyMode notifyMode, string sourceName )
		{
			//notify all Event Actions that are IHasEntityReference
			var brokenEAList = new List<Guid>();
			var brokenEntityGroupList = new List<Guid>();
			var brokenInitialGroupList = new List<Guid>();
			var allBrokenList = new List<BrokenRefInfo>();

			//initial deployment groups (DPs)
			foreach ( var group in mainWindow.mission.initialDeploymentGroups )
			{
				var info = group.NotifyEntityRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = group.cardName;
					info.topLevelNotifyType = NotifyType.InitialGroup;
					allBrokenList.Add( info );
					brokenInitialGroupList.Add( ((EnemyGroupData)group).GUID );
				}
			}

			//entity groups
			foreach ( var group in mainWindow.mission.entityGroups )
			{
				var info = group.NotifyEntityRemoved( guid, NotifyMode.Report );
				if ( info.isBroken )
				{
					info.topOwnerName = group.name;
					info.topLevelNotifyType = NotifyType.EntityGroup;
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
						info.topLevelNotifyType = NotifyType.Event;
						allBrokenList.Add( info );
						brokenEAList.Add( ((EventAction)ea).GUID );
					}
				}
			}

			List<string> infoMsg = new();
			if ( brokenInitialGroupList.Count > 0 )
				infoMsg.Add( $"Found {brokenInitialGroupList.Count} broken Entity reference(s) in all of this Mission's Initial Groups." );
			if ( brokenEntityGroupList.Count > 0 )
				infoMsg.Add( $"Found {brokenEntityGroupList.Count} broken Entity reference(s) in all of this Mission's Entity Groups." );
			if ( brokenEAList.Count > 0 )
				infoMsg.Add( $"Found {brokenEAList.Count} broken Entity reference(s) in all of this Mission's Event Actions." );

			if ( notifyMode == NotifyMode.Report )
			{
				if ( allBrokenList.Count > 0 )
					return new()
					{
						isBroken = true,
						detailsMessage = string.Join( "\n", infoMsg ),
						brokenList = allBrokenList,
						initialGroups = brokenInitialGroupList.Count,
						eventActions = brokenEAList.Count,
						entities = brokenEntityGroupList.Count,
					};
			}

			return new() { isBroken = false };
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

		public static DiceColor[] ParseCustomDice( string[] dice )
		{
			List<DiceColor> diceColors = new List<DiceColor>();
			var regex = new Regex( @"\d\w+", RegexOptions.IgnoreCase );

			try
			{
				foreach ( var diceItem in dice )
				{
					var m = regex.Matches( diceItem );
					foreach ( var match in regex.Matches( diceItem ) )
					{
						int count = int.Parse( match.ToString()[0].ToString() );

						for ( int i = 0; i < count; i++ )
							diceColors.Add( (DiceColor)Enum.Parse( typeof( DiceColor ), match.ToString().Substring( 1 ) ) );
					}
				}
				return diceColors.ToArray();
			}
			catch ( Exception e )
			{
				MessageBox.Show( $"Check your formatting.\n\nException:\n{e.Message}", "Error Parsing Dice Values" );
				return new DiceColor[0];
			}
		}
	}
}
