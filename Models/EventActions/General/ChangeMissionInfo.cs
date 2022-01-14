namespace Imperial_Commander_Editor
{
	public class ChangeMissionInfo : EventAction
	{
		string _theText;
		public string theText { get { return _theText; } set { _theText = value; PC(); } }

		public ChangeMissionInfo()
		{

		}
		public ChangeMissionInfo( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
