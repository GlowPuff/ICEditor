using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Imperial_Commander_Editor
{
	public class ModifyMapEntity : EventAction, IHasEntityReference, IHasEventReference, IHasTriggerReference
	{
		public ObservableCollection<EntityModifier> entitiesToModify { get; set; } = new();

		public ModifyMapEntity()
		{

		}

		public ModifyMapEntity( string dname
			, EventActionType et ) : base( et, dname )
		{
		}

		public BrokenRefInfo NotifyEntityRemoved( Guid guid, NotifyMode mode )
		{
			var e = entitiesToModify.Where( x => x.sourceGUID == guid );
			var ranges = new List<string>();

			if ( e.Count() > 0 )
			{
				if ( mode == NotifyMode.Fix )
				{
					for ( int i = entitiesToModify.Count - 1; i >= 0; i-- )
					{
						entitiesToModify.RemoveAt( i );
					}
				}

				ranges = e.Select( x => $"'{x.entityProperties.name}'" ).ToList();

				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "REMOVED The Entity From This Event Action"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo NotifyEventRemoved( Guid guid, NotifyMode mode )
		{
			var e = entitiesToModify
				.Where( x => x.entityProperties.buttonActions.Count( x => x.eventGUID == guid ) > 0 )
				.SelectMany( x => x.entityProperties.buttonActions );

			var ranges = new List<string>();

			if ( e.Count() > 0 )
			{
				foreach ( var item in e )
				{
					if ( mode == NotifyMode.Fix )
						item.eventGUID = Guid.Empty;
				}

				ranges = e.Select( x =>
				{
					return $"'{x.buttonText}'";
				} ).ToList();

				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = $"Fixed The Event For The Following Entity Button(s): {string.Join( ", ", ranges )}"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo NotifyTriggerRemoved( Guid guid, NotifyMode mode )
		{
			var e = entitiesToModify
				.Where( x => x.entityProperties.buttonActions.Count( x => x.triggerGUID == guid ) > 0 )
				.SelectMany( x => x.entityProperties.buttonActions );

			var ranges = new List<string>();

			if ( e.Count() > 0 )
			{
				foreach ( var item in e )
				{
					if ( mode == NotifyMode.Fix )
						item.triggerGUID = Guid.Empty;
				}

				ranges = e.Select( x =>
				{
					return $"'{x.buttonText}'";
				} ).ToList();

				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = $"Fixed The Trigger For The Following Entity Button(s): {string.Join( ", ", ranges )}"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckEvents()
		{
			List<string> list = new();

			foreach ( var en in entitiesToModify )
			{
				foreach ( var item in en.entityProperties.buttonActions )//.SelectMany( x => x.entityProperties.buttonActions ) )
				{
					if ( !Utils.ValidateEvent( item.eventGUID ) )
						list.Add( $"Entity '{en.entityProperties.name}' has a missing Event from Button: '{item.buttonText}'" );
				}
			}

			if ( list.Count > 0 )
			{
				return new BrokenRefInfo()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Event,
					itemName = displayName,
					brokenGuid = Guid.Empty,
					ownerGuid = GUID,
					details = string.Join( "\n", list )
				};
			}

			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckTriggers()
		{
			List<string> strings = new();

			foreach ( var en in entitiesToModify )
			{
				foreach ( var item in en.entityProperties.buttonActions )//entitiesToModify.SelectMany( x => x.entityProperties.buttonActions ) )
				{
					if ( !Utils.ValidateTrigger( item.triggerGUID ) )
						strings.Add( $"Entity '{en.entityProperties.name}' has a missing Trigger from Button: '{item.buttonText}'" );
				}
			}

			if ( strings.Count > 0 )
			{
				return new BrokenRefInfo()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Trigger,
					itemName = displayName,
					brokenGuid = Guid.Empty,
					ownerGuid = GUID,
					details = string.Join( "\n", strings )
				};
			}

			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckEntities()
		{
			List<string> strings = new();

			foreach ( var item in entitiesToModify )
			{
				if ( !Utils.ValidateMapEntity( item.sourceGUID ) )
					strings.Add( $"Missing Entity: {item.entityProperties.name}" );
			}

			if ( strings.Count > 0 )
			{
				return new()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Entity,
					itemName = displayName,
					ownerGuid = GUID,
					brokenGuid = Guid.Empty,
					details = string.Join( "\n", strings )
				};
			}
			return new() { isBroken = false };
		}
	}
}
