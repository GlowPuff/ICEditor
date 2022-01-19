using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ModifyDeploymentDialog.xaml
	/// </summary>
	public partial class ModifyDeploymentDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }
		public DeploymentPoint selectedDP { get; set; }

		public ObservableCollection<DeploymentColor> deploymentColors
		{
			get { return Utils.deploymentColors; }
		}

		public ModifyDeploymentDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ModifyDeployment( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.mainWindow.mission.mapEntities.Where( x => x.entityType == EntityType.DeploymentPoint );

			//verify dp still exists
			for ( int i = (eventAction as ModifyDeployment).deploymentModifiers.Count - 1; i >= 0; i-- )
			{
				if ( !Utils.ValidateMapEntity( (eventAction as ModifyDeployment).deploymentModifiers[i].GUID ) )
					(eventAction as ModifyDeployment).deploymentModifiers.RemoveAt( i );
			}
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void addDPButton_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedDP == null )
				return;

			(eventAction as ModifyDeployment).deploymentModifiers.Add( new()
			{
				name = selectedDP.name,
				GUID = selectedDP.GUID,
				isActive = selectedDP.entityProperties.isActive,
				deploymentColor = selectedDP.deploymentColor
			} );
		}

		private void remDPButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ModifyDeployment).deploymentModifiers.Remove( (sender as FrameworkElement).DataContext as DeploymentModifier );
		}
	}
}
