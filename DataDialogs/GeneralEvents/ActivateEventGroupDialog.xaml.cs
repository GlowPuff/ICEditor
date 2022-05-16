using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ActivateEventGroupDialog.xaml
	/// </summary>
	public partial class ActivateEventGroupDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public ActivateEventGroupDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ActivateEventGroup( dname, et );
			DataContext = eventAction;

			egCB.ItemsSource = Utils.mainWindow.mission.eventGroups;
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}
	}
}
