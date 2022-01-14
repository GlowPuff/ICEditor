using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for AddGroupDialog.xaml
	/// </summary>
	public partial class AddGroupDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }
		public string selectedGroup { get; set; }

		public AddGroupDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new AddGroupDeployment( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.enemyData;
			selectedGroup = "DG001";
		}

		private void remGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as AddGroupDeployment).groupsToAdd.Remove( (sender as FrameworkElement).DataContext as DeploymentCard );
		}

		private void addGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as AddGroupDeployment).groupsToAdd.Add( Utils.enemyData.Where( x => x.id == selectedGroup ).First() );
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
