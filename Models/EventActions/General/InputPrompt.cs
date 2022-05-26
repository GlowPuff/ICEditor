using System;

namespace Imperial_Commander_Editor
{
	public class InputPrompt : EventAction
	{
		string _theText;
		int _fromValue, _toValue;
		Guid _triggerGUID, _eventGUID;

		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public int fromValue { get { return _fromValue; } set { _fromValue = value; PC(); } }
		public int toValue { get { return _toValue; } set { _toValue = value; PC(); } }
		public Guid triggerGUID { get { return _triggerGUID; } set { _triggerGUID = value; PC(); } }
		public Guid eventGUID { get { return _eventGUID; } set { _eventGUID = value; PC(); } }

		public InputPrompt()
		{

		}

		public InputPrompt( string dname
			, EventActionType et ) : base( et, dname )
		{
			triggerGUID = Guid.Empty;
			eventGUID = Guid.Empty;
		}
	}
}
