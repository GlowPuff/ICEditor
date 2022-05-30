using System;
using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class InputPrompt : EventAction
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
	}
}
