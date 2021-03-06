using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for AllyDeploymentDialog.xaml
	/// </summary>
	public partial class AllyDeploymentDialog : Window, IEventActionDialog
	{
		public ObservableCollection<DeploymentPoint> deploymentPoints { get; set; } = new();

		public IEventAction eventAction { get; set; }

		public AllyDeploymentDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new AllyDeployment( dname, et );
			DataContext = eventAction;

			deploymentPoints.Add( new() { GUID = Guid.Empty, name = "Active Deployment Point" } );
			deploymentPoints.Add( new() { GUID = Utils.GUIDOne, name = "None" } );
			foreach ( var e in Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>() )
			{
				deploymentPoints.Add( e );
			}

			allyCB.ItemsSource = Utils.allyData;
			triggersCB.ItemsSource = Utils.mainWindow.localTriggers;
			//dpCB.ItemsSource = Utils.mainWindow.mission.mapEntities.Where( x => x.entityType == EntityType.DeploymentPoint );

			//verify trigger and dp still exist
			if ( !Utils.ValidateMapEntity( (eventAction as AllyDeployment).specificDeploymentPoint ) )
			{
				(eventAction as AllyDeployment).specificDeploymentPoint = Guid.Empty;
			}
			if ( !Utils.ValidateTrigger( (eventAction as AllyDeployment).setTrigger ) )
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
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}
	}
}
