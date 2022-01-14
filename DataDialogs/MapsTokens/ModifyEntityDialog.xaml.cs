using System;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ModifyEntityDialog.xaml
	/// </summary>
	public partial class ModifyEntityDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }
		public IMapEntity selectedEntity { get; set; }
		public IMapEntity selectedEntity2 { get; set; }

		public ModifyEntityDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ModifyEntity( dname, et );
			DataContext = eventAction;

			elist.ItemsSource = Utils.mainWindow.mission.mapEntities;
			elist2.ItemsSource = Utils.mainWindow.mission.mapEntities;

			//validate triggers in "entitiesToModify"
			for ( int i = 0; i < (eventAction as ModifyEntity).entitiesToModify.Count; i++ )
			{
				for ( int ii = 0; ii < (eventAction as ModifyEntity).entitiesToModify[i].buttonActions.Count; ii++ )
				{
					if ( !Utils.mainWindow.mission.TriggerExists( (eventAction as ModifyEntity).entitiesToModify[i].buttonActions[ii].triggerGUID ) )
						(eventAction as ModifyEntity).entitiesToModify[i].buttonActions[ii].triggerGUID = Guid.Empty;
					(eventAction as ModifyEntity).entitiesToModify[i].buttonActions[ii].triggerName = Utils.mainWindow.mission.GetTriggerFromGUID( (eventAction as ModifyEntity).entitiesToModify[i].buttonActions[ii].triggerGUID ).name;
				}
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

		private void addActivateEntityButton_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedEntity != null )
			{
				EntityProperties props = new();
				props.CopyFrom( selectedEntity );
				(eventAction as ModifyEntity).entitiesToActivate.Add( props );
			}
		}

		private void addModifyEntityButton_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedEntity2 != null )
			{
				EntityProperties props = new();
				props.CopyFrom( selectedEntity2 );
				(eventAction as ModifyEntity).entitiesToModify.Add( props );
			}
		}

		private void editButton_Click( object sender, RoutedEventArgs e )
		{
			EditEntityProperties dlg = new( (sender as FrameworkElement).DataContext as EntityProperties );
			dlg.ShowDialog();
		}

		private void remModButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ModifyEntity).entitiesToModify.Remove( (sender as FrameworkElement).DataContext as EntityProperties );
		}

		private void remActButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ModifyEntity).entitiesToActivate.Remove( (sender as FrameworkElement).DataContext as EntityProperties );
		}
	}
}
