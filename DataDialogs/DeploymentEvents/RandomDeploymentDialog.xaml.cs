﻿using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for RandomDeploymentDialog.xaml
	/// </summary>
	public partial class RandomDeploymentDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public RandomDeploymentDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new RandomDeployment( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.mainWindow.mission.mapEntities.Where( x => x.entityType == EntityType.DeploymentPoint );

			//verify dp still exists
			if ( !Utils.ValidateMapEntity( (eventAction as RandomDeployment).specificDeploymentPoint ) )
			{
				MessageBox.Show( "This Event Action is referencing a Deployment Point that no longer exists in the Mission.", "Missing Reference(s) Found" );
				//(eventAction as RandomDeployment).specificDeploymentPoint = Guid.Empty;
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
