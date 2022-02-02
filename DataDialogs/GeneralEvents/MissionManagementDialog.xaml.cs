using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for MissionManagementDialog.xaml
	/// </summary>
	public partial class MissionManagementDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public MissionManagementDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new MissionManagement( dname, et );
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
	}
}
