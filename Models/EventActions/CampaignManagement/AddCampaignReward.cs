using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class AddCampaignReward : EventAction
	{
		int _medpacsToModify, _threatToModify;

		//store the ID
		public ObservableCollection<string> campaignItems { get; set; }
		public ObservableCollection<string> campaignRewards { get; set; }
		public ObservableCollection<string> earnedVillains { get; set; }
		public ObservableCollection<string> earnedAllies { get; set; }
		public int medpacsToModify { get => _medpacsToModify; set { _medpacsToModify = value; PC(); } }
		public int threatToModify { get => _threatToModify; set { _threatToModify = value; PC(); } }

		public AddCampaignReward()
		{

		}

		public AddCampaignReward( string dname, EventActionType et ) : base( et, dname )
		{
			campaignItems = new();
			campaignRewards = new();
			earnedVillains = new();
			earnedAllies = new();
			medpacsToModify = 0;
			threatToModify = 0;
		}
	}
}
