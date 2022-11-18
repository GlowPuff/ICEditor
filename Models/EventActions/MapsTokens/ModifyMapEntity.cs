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

			foreach ( var item in entitiesToModify.SelectMany( x => x.entityProperties.buttonActions ) )
			{
				if ( !Utils.ValidateEvent( item.eventGUID ) )
					list.Add( $"Missing Event from Button '{item.buttonText}'" );
			}

			if ( list.Count > 0 )
			{
				return new BrokenRefInfo()
				{
					isBroken = true,
					notifyType = NotifyType.Event,
					itemName = displayName,
					brokenGuid = Guid.Empty,
					ownerGuid = GUID,
					details = string.Join( "\n", list )
				};
			}

			return new() { isBroken = false };
		}
	}
}
