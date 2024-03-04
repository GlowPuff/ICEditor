namespace Imperial_Commander_Editor
{
	public class CampaignModifyFameAwards : EventAction
	{
		int _fameToAdd, _awardsToAdd;

		public int fameToAdd { get => _fameToAdd; set { _fameToAdd = value; PC(); } }
		public int awardsToAdd { get => _awardsToAdd; set { _awardsToAdd = value; PC(); } }

		public CampaignModifyFameAwards()
		{

		}

		public CampaignModifyFameAwards( string dname, EventActionType et ) : base( et, dname )
		{
			fameToAdd = awardsToAdd = 0;
		}
	}
}
