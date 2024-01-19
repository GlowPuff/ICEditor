namespace Imperial_Commander_Editor
{
	public class CampaignModifyFameAwards : EventAction
	{
		int _fameToAdd, _awardsToAdd;
		string _customIdentifier;
		public int fameToAdd { get => _fameToAdd; set { _fameToAdd = value; PC(); } }
		public int awardsToAdd { get => _awardsToAdd; set { _awardsToAdd = value; PC(); } }
		public string customIdentifier { get => _customIdentifier; set { _customIdentifier = value; PC(); } }

		public CampaignModifyFameAwards()
		{

		}

		public CampaignModifyFameAwards( string dname, EventActionType et ) : base( et, dname )
		{
			fameToAdd = awardsToAdd = 0;
			customIdentifier = "";
		}
	}
}
