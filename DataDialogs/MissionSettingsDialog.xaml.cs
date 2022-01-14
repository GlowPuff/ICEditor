using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for MissionSettings.xaml
	/// </summary>
	public partial class MissionSettingsDialog : Window
	{
		public Mission mission { get; set; }

		public MissionSettingsDialog( Mission m )
		{
			InitializeComponent();

			DataContext = m.missionProperties;

			mission = m;

			allyCB.ItemsSource = Utils.allyData;
			fixedAllyCB.ItemsSource = Utils.allyData;
			heroCB.ItemsSource = Utils.heroData;
			var items = from e in mission.globalEvents where e.isGlobal select e;
			eventCB.ItemsSource = null;

			ciInfo.Text = string.IsNullOrEmpty( mission.missionProperties.customInstructionText ) ? "Text Not Set" : "Text Set";
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}

		private void ciButton_Click( object sender, RoutedEventArgs e )
		{
			GenericTextDialog dlg = new GenericTextDialog( "Custom Instructions", mission.missionProperties.customInstructionText );
			dlg.textHint = "Leave this completely empty to UNSET Custom Instructions.";
			dlg.ShowDialog();

			mission.missionProperties.customInstructionText = dlg.theText;

			ciInfo.Text = string.IsNullOrEmpty( mission.missionProperties.customInstructionText ) ? "Text Not Set" : "Text Set";
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			mnameTB.Focus();
			mnameTB.SelectAll();
		}
	}
}
