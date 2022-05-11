namespace Imperial_Commander_Editor
{
	public class ChangeReposition : EventAction
	{
		string _theText, _groupID;
		bool _useSpecific;

		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public string groupID { get { return _groupID; } set { _groupID = value; PC(); } }
		public bool useSpecific { get { return _useSpecific; } set { _useSpecific = value; } }

		public ChangeReposition()
		{

		}

		public ChangeReposition( string dname
			, EventActionType et ) : base( et, dname )
		{
			_useSpecific = false;
			_groupID = "DG001";
		}
	}
}
