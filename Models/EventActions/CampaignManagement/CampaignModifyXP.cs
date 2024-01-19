namespace Imperial_Commander_Editor
{
	public class CampaignModifyXP : EventAction
	{
		int _xpToAdd;
		public int xpToAdd { get { return _xpToAdd; } set { _xpToAdd = value; PC(); } }

		public CampaignModifyXP()
		{

		}

		public CampaignModifyXP( string dname, EventActionType et ) : base( et, dname )
		{
			xpToAdd = 0;
		}
	}
}
