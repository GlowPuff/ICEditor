namespace Imperial_Commander_Editor
{
	public class ModifyRoundLimit : EventAction
	{
		int _roundLimitModifier;
		public int roundLimitModifier { get => _roundLimitModifier; set { _roundLimitModifier = value; PC(); } }

		public ModifyRoundLimit()
		{

		}

		public ModifyRoundLimit( string dname
	, EventActionType et ) : base( et, dname )
		{
			roundLimitModifier = 0;
		}
	}
}
