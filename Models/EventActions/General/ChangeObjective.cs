namespace Imperial_Commander_Editor
{
	public class ChangeObjective : EventAction
	{
		string _theText;
		public string theText { get { return _theText; } set { _theText = value; PC(); } }

		public ChangeObjective()
		{

		}
		public ChangeObjective( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
