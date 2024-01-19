using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ModifyMapEntity.xaml
	/// </summary>
	public partial class ModifyMapEntityDialog : Window, IEventActionDialog
	{
		public IMapEntity selectedEntity { get; set; }
		public ObservableCollection<DeploymentColor> deploymentColors
		{
			get { return Utils.deploymentColors; }
		}
		public IEventAction eventAction { get; set; }

		public ModifyMapEntityDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ModifyMapEntity( dname, et );
			DataContext = eventAction;

			elist.ItemsSource = Utils.mainWindow.mission.mapEntities;

			//verify entities to modify still exist
			List<string> strings = new();
			for ( int i = (eventAction as ModifyMapEntity).entitiesToModify.Count - 1; i >= 0; i-- )
			{
				if ( !Utils.ValidateMapEntity( (eventAction as ModifyMapEntity).entitiesToModify[i].sourceGUID ) )
				{
					strings.Add( $"{(eventAction as ModifyMapEntity).entitiesToModify[i].entityProperties.name}" );
					//(eventAction as ModifyMapEntity).entitiesToModify.RemoveAt( i );
				}
			}
			if ( strings.Count > 0 )
				MessageBox.Show( $"This Event Action is referencing one or more Entities that no longer exist in the Mission.\n\n{string.Join( "\n", strings )}", "Missing Reference(s) Found" );
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

		private void addModifyEntityButton_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedEntity != null )
			{
				EntityProperties props = new();
				props.CopyFrom( selectedEntity );
				(eventAction as ModifyMapEntity).entitiesToModify.Add( new()
				{
					sourceGUID = selectedEntity.GUID,
					entityProperties = props,
					hasColor = selectedEntity.hasColor,
					hasProperties = selectedEntity.hasProperties,
				} );
			}
		}

		private void editPropsBtn_Click( object sender, RoutedEventArgs e )
		{
			EditEntityProperties dlg = new( (((FrameworkElement)sender).DataContext as EntityModifier).entityProperties, false );
			dlg.ShowDialog();
		}

		private void remBtn_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ModifyMapEntity).entitiesToModify.Remove( ((FrameworkElement)sender).DataContext as EntityModifier );
		}
	}
}
