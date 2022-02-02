using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ChangeMissionInfoDialog.xaml
	/// </summary>
	public partial class ChangeMissionInfoDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public ChangeMissionInfoDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ChangeMissionInfo( dname, et );
			DataContext = eventAction;
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

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			textbox.Focus();
			textbox.SelectAll();
		}
	}
}
