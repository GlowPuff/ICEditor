using CommunityToolkit.Mvvm.ComponentModel;

namespace Imperial_Commander_Editor
{
	public class CustomCampaign : ObservableObject
	{
		string _campaignName;

		public string CampaignName { get => _campaignName; set { SetProperty( ref _campaignName, value ); } }

		public CustomCampaign()
		{

		}
	}
}
