using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for SetNextMissionDialog.xaml
	/// </summary>
	public partial class SetNextMissionDialog : Window, IEventActionDialog
	{
		//sets the next STORY Mission
		public bool enabledStatus { get; set; }

		public IEventAction eventAction { get; set; }

		public SetNextMissionDialog( string dname, EventActionType et, IEventAction ea = null, bool fromPackageManager = false )
		{
			InitializeComponent();

			eventAction = ea ?? new CampaignSetNextMission( dname, et );
			DataContext = eventAction;

			var custom = new MissionNameData() { id = "Custom", name = "Custom" };
			var names = Utils.missionNames;
			names.Insert( 0, custom );
			missionIDCB.ItemsSource = names;
			enabledStatus = !fromPackageManager;

			if ( fromPackageManager )
			{
				instructions.Text = "Copy/Paste the Custom Mission Identifier of the Mission you want to set as the next Story Mission.";
				customIDTB.Focus();
				customIDTB.SelectAll();
			}
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

		private void nameTB_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}
	}
}
