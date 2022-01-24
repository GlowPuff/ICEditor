using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EnemyDeploymentDialog.xaml
	/// </summary>
	public partial class EnemyDeploymentDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		public IEventAction eventAction { get; set; }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public EnemyDeploymentDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new EnemyDeployment( dname, et );
			DataContext = eventAction;

			if ( (eventAction as EnemyDeployment).sourceType == SourceType.InitialReserved )
			{
				var items = new List<DeploymentCard>();
				items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.initialDeploymentGroups.Any( y => y.cardID == x.id ) ) ).ToList();
				items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.reservedDeploymentGroups.Any( y => y.cardID == x.id ) ) ).ToList();

				enemyCB.ItemsSource = items;
			}
			else
				enemyCB.ItemsSource = Utils.enemyData;

			triggersCB.ItemsSource = Utils.mainWindow.localTriggers;
			dpCB.ItemsSource = Utils.mainWindow.mission.mapEntities.Where( x => x.entityType == EntityType.DeploymentPoint );

			//verify trigger and dp still exist
			if ( !Utils.ValidateMapEntity( (eventAction as EnemyDeployment).specificDeploymentPoint ) )
			{
				(eventAction as EnemyDeployment).specificDeploymentPoint = Guid.Empty;
			}
			for ( int i = 0; i < (eventAction as EnemyDeployment).enemyGroupData.pointList.Count; i++ )
			{
				if ( !Utils.ValidateMapEntity( (eventAction as EnemyDeployment).enemyGroupData.pointList[i].GUID ) )
					(eventAction as EnemyDeployment).enemyGroupData.pointList[i].GUID = Guid.Empty;
			}
			if ( !Utils.ValidateTrigger( (eventAction as EnemyDeployment).setTrigger ) )
			{
				(eventAction as EnemyDeployment).setTrigger = Guid.Empty;
			}
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			eventAction.displayName = "Deploy: " + (eventAction as EnemyDeployment).deploymentGroup + "/" + (eventAction as EnemyDeployment).enemyGroupData.cardName;
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}

		private void initialRB_Click( object sender, RoutedEventArgs e )
		{
			var items = new List<DeploymentCard>();
			items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.initialDeploymentGroups.Any( y => y.cardID == x.id ) ) ).ToList();
			items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.reservedDeploymentGroups.Any( y => y.cardID == x.id ) ) ).ToList();

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

		private void nameTB_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void editGroup_Click( object sender, RoutedEventArgs e )
		{
			EditInitialGroupDialog dialog = new EditInitialGroupDialog( (eventAction as EnemyDeployment).enemyGroupData );
			dialog.ShowDialog();
		}

		private void enemyCB_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			if ( Utils.enemyData.Any( x => x.id == (eventAction as EnemyDeployment).deploymentGroup ) )
			{
				DeploymentCard card = Utils.enemyData.First( x => x.id == (eventAction as EnemyDeployment).deploymentGroup );
				(eventAction as EnemyDeployment).enemyGroupData.cardName = card.name;
				(eventAction as EnemyDeployment).enemyGroupData.cardID = card.id;
			}
		}
	}
}
