using System;

namespace Imperial_Commander_Editor
{
	public class ModifyRoundLimit : EventAction, IHasEventReference
	{
		int _roundLimitModifier, _setLimitTo;
		Guid _eventGUID;
		bool _disableRoundLimit, _setRoundLimit;

		public int roundLimitModifier { get => _roundLimitModifier; set { _roundLimitModifier = value; PC(); } }
		public int setLimitTo { get => _setLimitTo; set { _setLimitTo = value; PC(); } }
		public Guid eventGUID { get => _eventGUID; set { _eventGUID = value; PC(); } }
		public bool disableRoundLimit { get => _disableRoundLimit; set { _disableRoundLimit = value; PC(); } }
		public bool setRoundLimit { get => _setRoundLimit; set { _setRoundLimit = value; PC(); } }

		public ModifyRoundLimit()
		{

		}

		public ModifyRoundLimit( string dname
	, EventActionType et ) : base( et, dname )
		{
			roundLimitModifier = 0;
			setLimitTo = 0;
			eventGUID = Guid.Empty;
			disableRoundLimit = false;
			setRoundLimit = false;
		}

		public BrokenRefInfo NotifyEventRemoved( Guid guid, NotifyMode mode )
		{
			if ( eventGUID == guid )
			{
				if ( mode == NotifyMode.Fix )
					eventGUID = Guid.Empty;
				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "Fixed 'Round Limit' Event"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckEvents()
		{
			if ( !Utils.ValidateEvent( eventGUID ) )
			{
				return new()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Event,
					itemName = displayName,
					ownerGuid = GUID,
					brokenGuid = eventGUID,
					details = "Missing 'Round Limit' Event"
				};
			}
			return new() { isBroken = false };
		}
	}
}
