using System;
using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for OptionalDeploymentDialog.xaml
	/// </summary>
	public partial class OptionalDeploymentDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public OptionalDeploymentDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new OptionalDeployment( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.mainWindow.mission.mapEntities.Where( x => x.entityType == EntityType.DeploymentPoint );

			//verify dp still exists
			if ( !Utils.ValidateMapEntity( (eventAction as OptionalDeployment).specificDeploymentPoint ) )
			{
				(eventAction as OptionalDeployment).specificDeploymentPoint = Guid.Empty;
			}
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
