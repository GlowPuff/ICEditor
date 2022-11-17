using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Imperial_Commander_Editor
{
	public class InputPrompt : EventAction, IHasEventReference, IHasTriggerReference
	{
		string _theText, _failText;
		Guid _failTriggerGUID, _failEventGUID;

		public string failText { get { return _failText; } set { _failText = value; PC(); } }
		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public Guid failTriggerGUID { get { return _failTriggerGUID; } set { _failTriggerGUID = value; PC(); } }
		public Guid failEventGUID { get { return _failEventGUID; } set { _failEventGUID = value; PC(); } }
		public ObservableCollection<InputRange> inputList { get; set; } = new();

		public InputPrompt()
		{

		}

		public InputPrompt( string dname
			, EventActionType et ) : base( et, dname )
		{
			_theText = "";
			_failText = "";
			_failTriggerGUID = Guid.Empty;
			_failEventGUID = Guid.Empty;
		}

		public BrokenRefInfo NotifyEventRemoved( Guid guid, NotifyMode mode )
		{
			if ( failEventGUID == guid || inputList.Any( x => x.eventGUID == guid ) )
			{
				var msg = "";
				var msg2 = "";
				var ranges = new List<string>();

				if ( mode == NotifyMode.Update )
				{
					if ( failEventGUID == guid )
						failEventGUID = Guid.Empty;
					//check input ranges
					foreach ( var item in inputList )
					{
						if ( item.eventGUID == guid )
							item.eventGUID = Guid.Empty;
					}
				}

				ranges = inputList.Where( x => x.eventGUID == guid ).Select( x => $"[{x.fromValue}-{x.toValue}]" ).ToList();

				if ( failEventGUID == guid )
					msg = "Fixed 'Default Handler' Event";
				if ( ranges.Count > 0 )
					msg2 = "Fixed The Event For The Following Input Range(s): " + string.Join( ", ", ranges );

				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = msg + (!string.IsNullOrEmpty( msg ) ? "\n" + msg2 : msg2)
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo NotifyTriggerRemoved( Guid guid, NotifyMode mode )
		{
			if ( failTriggerGUID == guid || inputList.Any( x => x.triggerGUID == guid ) )
			{
				var msg = "";
				var msg2 = "";
				var ranges = new List<string>();

				if ( mode == NotifyMode.Update )
				{
					if ( failTriggerGUID == guid )
						failTriggerGUID = Guid.Empty;
					//check input ranges
					foreach ( var item in inputList )
					{
						if ( item.triggerGUID == guid )
							item.triggerGUID = Guid.Empty;
					}
				}

				ranges = inputList.Where( x => x.triggerGUID == guid ).Select( x =>
				{
					return $"[{x.fromValue}-{x.toValue}]";
				} ).ToList();

				if ( failTriggerGUID == guid )
					msg = "Fixed 'Default Handler' Trigger";
				if ( ranges.Count > 0 )
					msg2 = "Fixed The Trigger For The Following Input Range(s): " + string.Join( ", ", ranges );

				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = $"{msg + (!string.IsNullOrEmpty( msg ) ? '\n' + msg2 : msg2)}"
				};
			}
			return new() { isBroken = false };
		}
	}
}
