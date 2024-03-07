using System;

namespace Imperial_Commander_Editor
{
	public class SetCountdown : EventAction, IHasEventReference, IHasTriggerReference
	{
		int _countdownTimer;
		Guid _eventGUID, _triggerGUID;
		bool _showPlayerCountdown;//whether to show a number in IC2 so players know how many rounds remain
		string _countdownTimerName;

		public string countdownTimerName { get => _countdownTimerName; set { _countdownTimerName = value; PC(); } }
		public int countdownTimer { get => _countdownTimer; set { _countdownTimer = value; PC(); } }
		public Guid eventGUID { get => _eventGUID; set { _eventGUID = value; PC(); } }
		public Guid triggerGUID { get => _triggerGUID; set { _triggerGUID = value; PC(); } }
		public bool showPlayerCountdown { get => _showPlayerCountdown; set { _showPlayerCountdown = value; PC(); } }

		public SetCountdown()
		{

		}

		public SetCountdown( string dname
	, EventActionType et ) : base( et, dname )
		{
			countdownTimer = 0;
			eventGUID = Guid.Empty;
			triggerGUID = Guid.Empty;
			showPlayerCountdown = false;
			countdownTimerName = "Timer1";
		}

		public BrokenRefInfo NotifyEventRemoved( Guid guid, NotifyMode mode )
		{
			if ( guid == eventGUID )
			{
				if ( mode == NotifyMode.Fix )
					eventGUID = Guid.Empty;

				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "Fixed The Countdown Event"
				};
			}

			return new() { isBroken = false };
		}

		public BrokenRefInfo NotifyTriggerRemoved( Guid guid, NotifyMode mode )
		{
			if ( guid == triggerGUID )
			{
				if ( mode == NotifyMode.Fix )
					triggerGUID = Guid.Empty;

				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "Fixed The Countdown Trigger"
				};
			}

			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckEvents()
		{
			if ( !Utils.ValidateEvent( eventGUID ) )
			{
				return new BrokenRefInfo()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Event,
					itemName = displayName,
					brokenGuid = Guid.Empty,
					ownerGuid = GUID,
					details = $"Missing Countdown Event: '{eventGUID}'"
				};
			}

			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckTriggers()
		{
			if ( !Utils.ValidateTrigger( triggerGUID ) )
			{
				return new BrokenRefInfo()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Trigger,
					itemName = displayName,
					brokenGuid = Guid.Empty,
					ownerGuid = GUID,
					details = $"Missing Countdown Trigger: '{triggerGUID}'"
				};
			}

			return new() { isBroken = false };
		}
	}
}