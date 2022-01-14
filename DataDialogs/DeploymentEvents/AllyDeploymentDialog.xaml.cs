using System;
using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for AllyDeploymentDialog.xaml
	/// </summary>
	public partial class AllyDeploymentDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public AllyDeploymentDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new AllyDeployment( dname, et );
			DataContext = eventAction;

			allyCB.ItemsSource = Utils.allyData;
			triggersCB.ItemsSource = Utils.mainWindow.localTriggers;
			dpCB.ItemsSource = Utils.mainWindow.mission.mapEntities.Where( x => x.entityType == EntityType.DeploymentPoint );

			//verify trigger and dp still exist
			if ( !Utils.mainWindow.mission.EntityExists( (eventAction as AllyDeployment).specificDeploymentPoint ) )
			{
				(eventAction as AllyDeployment).specificDeploymentPoint = Guid.Empty;
			}
			if ( !Utils.mainWindow.mission.TriggerExists( (eventAction as AllyDeployment).setTrigger ) )
			{
				(eventAction as AllyDeployment).setTrigger = Guid.Empty;
			}
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}
	}
}
