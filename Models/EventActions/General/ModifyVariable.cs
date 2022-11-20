using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Imperial_Commander_Editor
{
	public class ModifyVariable : EventAction, IHasTriggerReference
	{
		public ObservableCollection<TriggerModifier> triggerList { get; set; } = new();

		public ModifyVariable()
		{

		}

		public ModifyVariable( string dname
			, EventActionType et ) : base( et, dname )
		{
		}

		public BrokenRefInfo NotifyTriggerRemoved( Guid guid, NotifyMode mode )
		{
			var e = triggerList.Where( x => x.triggerGUID == guid ).ToList();

			if ( e.Count() > 0 )
			{
				if ( mode == NotifyMode.Fix )
				{
					for ( int i = triggerList.Count - 1; i >= 0; i-- )
					{
						if ( triggerList[i].triggerGUID == guid )
							triggerList.RemoveAt( i );
					}
				}

				var ranges = e.Select( x => $"'{x.triggerName}'" ).ToList();

				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = $"Trigger REMOVED from 'Trigger Modifiers': {string.Join( ", ", ranges )}"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckTriggers()
		{
			List<string> strings = new();

			foreach ( var item in triggerList )
			{
				if ( !Utils.ValidateTrigger( item.triggerGUID ) )
					strings.Add( $"Missing Trigger: {item.triggerName}" );
			}

			if ( strings.Count > 0 )
			{
				return new()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Trigger,
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
