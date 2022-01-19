using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for DeploymentProps.xaml
	/// </summary>
	public partial class DeploymentProps : UserControl, IPropertyModel
	{
		public ObservableCollection<DeploymentColor> deploymentColors
		{
			get { return Utils.deploymentColors; }
		}

		public DeploymentProps()
		{
			InitializeComponent();
		}

		private void eName_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
			}
		}

		private void dupeBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dupe = (DataContext as DeploymentPoint).Duplicate();
			Utils.mainWindow.mapEditor.InsertDuplicateEntity( dupe );
		}

		private void ownerChangeBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			((sender as FrameworkElement).DataContext as DeploymentPoint).mapSectionOwner = Utils.mainWindow.activeSection.GUID;
			Utils.mainWindow.SetStatus( $"Owner Set To '{Utils.mainWindow.activeSection.name}'" );
		}
	}
}
