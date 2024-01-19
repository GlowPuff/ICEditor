using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ModifyCreditsDialog.xaml
	/// </summary>
	public partial class ModifyCreditsDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public ModifyCreditsDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new CampaignModifyCredits( dname, et );
			DataContext = eventAction;
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

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			modifyInput.Focus();
		}
	}
}
