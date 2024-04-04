using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for AddCampaignRewardDialog.xaml
	/// </summary>
	public partial class AddCampaignRewardDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		CampaignItem _selectedItem;
		CampaignReward _selectedReward;
		DeploymentCard _selectedVillain;
		DeploymentCard _selectedAlly;

		public CampaignItem selectedItem { get => _selectedItem; set { _selectedItem = value; PC(); } }
		public CampaignReward selectedReward { get => _selectedReward; set { _selectedReward = value; PC(); } }
		public DeploymentCard selectedVillain { get => _selectedVillain; set { _selectedVillain = value; PC(); } }
		public DeploymentCard selectedAlly { get => _selectedAlly; set { _selectedAlly = value; PC(); } }

		//UI
		public ObservableCollection<CampaignItem> addedItems { get; set; } = new();
		public ObservableCollection<CampaignReward> addedRewards { get; set; } = new();
		public ObservableCollection<DeploymentCard> addedVillains { get; set; } = new();
		public ObservableCollection<DeploymentCard> addedAllies { get; set; } = new();

		public IEventAction eventAction { get; set; }

		public AddCampaignRewardDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new AddCampaignReward( dname, et );
			DataContext = eventAction;

			itemListCB.ItemsSource = Utils.campaignItemList.OrderBy( x => x.name ).ToList();
			rewardListCB.ItemsSource = Utils.campaignRewardsList.OrderBy( x => x.name ).ToList();
			villainListCB.ItemsSource = Utils.villainData.OrderBy( x => x.name ).ToList();
			alliesListCB.ItemsSource = Utils.allyRebelData.OrderBy( x => x.name ).ToList();

			selectedItem = Utils.campaignItemList.OrderBy( x => x.name ).ToList()[0];
			selectedReward = Utils.campaignRewardsList.OrderBy( x => x.name ).ToList()[0];
			selectedVillain = Utils.villainData.OrderBy( x => x.name ).ToList()[0];
			selectedAlly = Utils.allyRebelData.OrderBy( x => x.name ).ToList()[0];

			//populate UI lists
			foreach ( var item in (eventAction as AddCampaignReward).campaignItems )
				addedItems.Add( Utils.campaignItemList.First( x => x.id == item ) );
			foreach ( var item in (eventAction as AddCampaignReward).campaignRewards )
				addedRewards.Add( Utils.campaignRewardsList.First( x => x.id == item ) );
			foreach ( var item in (eventAction as AddCampaignReward).earnedVillains )
				addedVillains.Add( Utils.villainData.First( x => x.id == item ) );
			foreach ( var item in (eventAction as AddCampaignReward).earnedAllies )
				addedAllies.Add( Utils.allyRebelData.First( x => x.id == item ) );
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void removeItemBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( (string)(sender as FrameworkElement).Tag == "itemsLB" )
			{
				(eventAction as AddCampaignReward).campaignItems.Remove( ((sender as FrameworkElement).DataContext as CampaignItem).id );
				addedItems.Remove( (sender as FrameworkElement).DataContext as CampaignItem );
			}
			else if ( (string)(sender as FrameworkElement).Tag == "rewardsLB" )
			{
				(eventAction as AddCampaignReward).campaignRewards.Remove( ((sender as FrameworkElement).DataContext as CampaignReward).id );
				addedRewards.Remove( (sender as FrameworkElement).DataContext as CampaignReward );
			}
			else if ( (string)(sender as FrameworkElement).Tag == "villainsLB" )
			{
				(eventAction as AddCampaignReward).earnedVillains.Remove( ((sender as FrameworkElement).DataContext as DeploymentCard).id );
				addedVillains.Remove( (sender as FrameworkElement).DataContext as DeploymentCard );
			}
			else if ( (string)(sender as FrameworkElement).Tag == "alliesLB" )
			{
				(eventAction as AddCampaignReward).earnedAllies.Remove( ((sender as FrameworkElement).DataContext as DeploymentCard).id );
				addedAllies.Remove( (sender as FrameworkElement).DataContext as DeploymentCard );
			}
		}

		private void ScrollViewer_PreviewMouseWheel( object sender, System.Windows.Input.MouseWheelEventArgs e )
		{
			ScrollViewer scv = (ScrollViewer)sender;
			scv.ScrollToVerticalOffset( scv.VerticalOffset - e.Delta );
			e.Handled = true;
		}

		private void addItemBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedItem != null )
			{
				(eventAction as AddCampaignReward).campaignItems.Add( selectedItem.id );
				addedItems.Add( selectedItem );
			}
		}

		private void addRewardBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedReward != null )
			{
				(eventAction as AddCampaignReward).campaignRewards.Add( selectedReward.id );
				addedRewards.Add( selectedReward );
			}
		}

		private void addVillainBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedVillain != null )
			{
				(eventAction as AddCampaignReward).earnedVillains.Add( selectedVillain.id );
				addedVillains.Add( selectedVillain );
			}
		}

		private void addAllyBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedAlly != null )
			{
				(eventAction as AddCampaignReward).earnedAllies.Add( selectedAlly.id );
				addedAllies.Add( selectedAlly );
			}
		}

		private void inputChanged_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}
	}
}
