using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ModifyFameAwardsDialog.xaml
	/// </summary>
	public partial class ModifyFameAwardsDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public ModifyFameAwardsDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new CampaignModifyFameAwards( dname, et );
			DataContext = eventAction;
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			modifyInput.Focus();
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
	}
}
