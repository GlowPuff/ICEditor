using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EnemyDeploymentDialog.xaml
	/// </summary>
	public partial class EnemyDeploymentDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public EnemyDeploymentDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new EnemyDeployment( dname, et );
			DataContext = eventAction;

			if ( (eventAction as EnemyDeployment).sourceType == SourceType.InitialReserved )
			{
				var items = new List<DeploymentCard>();
				items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.initialDeploymentGroups.Any( y => y.card.id == x.id ) ) ).ToList();
				items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.reservedDeploymentGroups.Any( y => y.id == x.id ) ) ).ToList();

				enemyCB.ItemsSource = items;
			}
			else
				enemyCB.ItemsSource = Utils.enemyData;

			triggersCB.ItemsSource = Utils.mainWindow.localTriggers;
			dpCB.ItemsSource = Utils.mainWindow.mission.mapEntities.Where( x => x.entityType == EntityType.DeploymentPoint );

			//verify trigger and dp still exist
			if ( !Utils.mainWindow.mission.EntityExists( (eventAction as EnemyDeployment).specificDeploymentPoint ) )
			{
				(eventAction as EnemyDeployment).specificDeploymentPoint = Guid.Empty;
			}
			if ( !Utils.mainWindow.mission.TriggerExists( (eventAction as EnemyDeployment).setTrigger ) )
			{
				(eventAction as EnemyDeployment).setTrigger = Guid.Empty;
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

		private void initialRB_Click( object sender, RoutedEventArgs e )
		{
			var items = new List<DeploymentCard>();
			items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.initialDeploymentGroups.Any( y => y.card.id == x.id ) ) ).ToList();
			items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.reservedDeploymentGroups.Any( y => y.id == x.id ) ) ).ToList();

			enemyCB.ItemsSource = items;
		}

		private void manualRB_Click( object sender, RoutedEventArgs e )
		{
			enemyCB.ItemsSource = Utils.enemyData;
		}

		private void handRB_Click( object sender, RoutedEventArgs e )
		{
			enemyCB.ItemsSource = Utils.enemyData;
		}
	}
}
