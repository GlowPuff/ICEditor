using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ChangeObjectiveDialog.xaml
	/// </summary>
	public partial class ChangeObjectiveDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public ChangeObjectiveDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ChangeMissionInfo( dname, et );
			DataContext = eventAction;
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			textbox.Focus();
			textbox.SelectAll();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}
	}
}
