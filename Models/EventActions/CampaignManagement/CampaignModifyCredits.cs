namespace Imperial_Commander_Editor
{
	public class CampaignModifyCredits : EventAction
	{
		int _creditsToModify;
		public int creditsToModify { get { return _creditsToModify; } set { _creditsToModify = value; PC(); } }

		public CampaignModifyCredits()
		{

		}

		public CampaignModifyCredits( string dname, EventActionType et ) : base( et, dname )
		{
			creditsToModify = 0;
		}
	}
}
