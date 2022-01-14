using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ChangeGroupStatusDialog.xaml
	/// </summary>
	public partial class ChangeGroupStatusDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }
		public string selectedReadyGroup { get; set; }
		public string selectedExhaustGroup { get; set; }

		public ChangeGroupStatusDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ChangeGroupStatus( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.enemyData;
			dp2CB.ItemsSource = Utils.enemyData;
			selectedExhaustGroup = selectedReadyGroup = "DG001";
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void addExhaustButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeGroupStatus).exhaustGroups.Add( Utils.enemyData.Where( x => x.id == selectedExhaustGroup ).First() );
		}

		private void addReadyButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeGroupStatus).readyGroups.Add( Utils.enemyData.Where( x => x.id == selectedReadyGroup ).First() );
		}

		private void remReadyButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeGroupStatus).readyGroups.Remove( (sender as FrameworkElement).DataContext as DeploymentCard );
		}

		private void remExhaustButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeGroupStatus).exhaustGroups.Remove( (sender as FrameworkElement).DataContext as DeploymentCard );
		}
	}
}
