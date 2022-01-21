using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for MissionPropsPanel.xaml
	/// </summary>
	public partial class MissionPropsPanel : UserControl
	{
		public MissionPropsPanel()
		{
			InitializeComponent();
			DataContext = null;
		}

		private void ciButton_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			GenericTextDialog dlg = new GenericTextDialog( "Custom Instructions", Utils.mainWindow.mission.missionProperties.customInstructionText );
			dlg.textHint = "Leave this completely empty to UNSET Custom Instructions.";
			dlg.ShowDialog();

			Utils.mainWindow.mission.missionProperties.customInstructionText = dlg.theText;

			ciInfo.Text = string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.customInstructionText ) ? "Text Not Set" : "Text Set";
		}

		public void Refresh()
		{
			DataContext = Utils.mainWindow.mission.missionProperties;

			allyCB.ItemsSource = Utils.allyData;
			fixedAllyCB.ItemsSource = Utils.allyData;
			bannedAllyCB.ItemsSource = Utils.allyData;
			heroCB.ItemsSource = Utils.heroData;
			var items = from e in Utils.mainWindow.mission.globalEvents where e.isGlobal select e;
			eventCB.ItemsSource = Utils.mainWindow.localEvents;

			ciInfo.Text = string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.customInstructionText ) ? "Text Not Set" : "Text Set";
		}

		private void Validate_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				//Utils.mainWindow.tabControl.SelectedIndex = 1;
				//Utils.mainWindow.tabControl.SelectedIndex = 0;
			}
		}

		private void eventCB_GotFocus( object sender, System.Windows.RoutedEventArgs e )
		{
			eventCB.GotFocus -= eventCB_GotFocus;
			eventCB.ItemsSource = Utils.mainWindow.localEvents;
			eventCB.GotFocus += eventCB_GotFocus;
		}

		private void descriptionBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "Mission Description", Utils.mainWindow.mission.missionProperties.missionDescription );
			dlg.ShowDialog();
			Utils.mainWindow.mission.missionProperties.missionDescription = dlg.theText;
		}
	}
}
