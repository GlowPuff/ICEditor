using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ChangeTargetDialog.xaml
	/// </summary>
	public partial class ChangeTargetDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }
		public string selectedGroup { get; set; }

		public ChangeTargetDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ChangeTarget( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.enemyData;
			hCB.ItemsSource = Utils.heroData;
			aCB.ItemsSource = Utils.allyData;
			selectedGroup = "DG001";
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

		private void addGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeTarget).groupsToAdd.Add( Utils.enemyData.Where( x => x.id == selectedGroup ).First() );
		}

		private void remGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeTarget).groupsToAdd.Remove( (sender as FrameworkElement).DataContext as DeploymentCard );
		}
	}
}
