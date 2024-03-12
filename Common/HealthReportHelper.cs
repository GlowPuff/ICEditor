using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	public static class HealthReportHelper
	{
		public static MainWindow mainWindow
		{
			get { return Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(); }
		}

		public static HealthReport CheckAndNotifyEventRemoved( Guid guid, NotifyMode notifyMode, string sourceName )
		{
			//notify all Triggers, Entities, and Event Actions that are IHasEventReference
			var brokenStartEvent = Guid.Empty;
			var brokenRoundLimitEvent = Guid.Empty;
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
					itemName = "Mission Start Section",
					topLevelNotifyType = NotifyType.StartingEvent,
					brokenGuid = guid,
					ownerGuid = Guid.Empty,
					details = "Event from [Starting Event]"
				};
				allBrokenList.Add( info );
			}

			//mission round limit event
			if ( mainWindow.mission.missionProperties.roundLimitEvent == guid )
			{
				brokenRoundLimitEvent = guid;
				var info = new BrokenRefInfo()
				{
					isBroken = true,
					topOwnerName = "Mission Properties Tab",
					itemName = "Round Limit Section",
					topLevelNotifyType = NotifyType.RoundLimitEvent,
					brokenGuid = guid,
					ownerGuid = Guid.Empty,
					details = "Event from [Round Limit Event]"
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
				infoMsg.Add( "Found a broken Event reference in the Mission Starting Event." );
			if ( brokenRoundLimitEvent != Guid.Empty )
				infoMsg.Add( "Found a broken Event reference in the Mission Round Limit Event." );
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
	}
}
