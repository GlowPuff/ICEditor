namespace Imperial_Commander_Editor
{
	public class ChangeObjective : EventAction
	{
		string _theText, _longText;
		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public string longText { get { return _longText; } set { _longText = value; PC(); } }

		public ChangeObjective()
		{

		}
		public ChangeObjective( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
