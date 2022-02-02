using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ChangeInstructionsDialog.xaml
	/// </summary>
	public partial class ChangeInstructionsDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }
		public string selectedGroup { get; set; }

		public ChangeInstructionsDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ChangeInstructions( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.enemyData;
			selectedGroup = "DG001";
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
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
			tb.Focus();
			tb.SelectAll();
		}

		private void addGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeInstructions).groupsToAdd.Add( Utils.enemyData.Where( x => x.id == selectedGroup ).First() );
		}

		private void remGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeInstructions).groupsToAdd.Remove( (sender as FrameworkElement).DataContext as DeploymentCard );
		}
	}
}
