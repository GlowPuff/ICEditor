using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class AddCampaignReward : EventAction
	{
		//store the ID
		public ObservableCollection<string> campaignItems { get; set; }
		public ObservableCollection<string> campaignRewards { get; set; }
		public ObservableCollection<string> earnedVillains { get; set; }
		public ObservableCollection<string> earnedAllies { get; set; }

		public AddCampaignReward()
		{

		}

		public AddCampaignReward( string dname, EventActionType et ) : base( et, dname )
		{
			campaignItems = new();
			campaignRewards = new();
			earnedVillains = new();
			earnedAllies = new();
		}
	}
}
