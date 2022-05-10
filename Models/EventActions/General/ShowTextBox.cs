namespace Imperial_Commander_Editor
{
	public class ShowTextBox : EventAction
	{
		string _theText;
		public string theText { get { return _theText; } set { _theText = value; PC(); } }

		public ShowTextBox()
		{

		}

		public ShowTextBox( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
