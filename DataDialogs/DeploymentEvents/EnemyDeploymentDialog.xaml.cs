using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EnemyDeploymentDialog.xaml
	/// </summary>
	public partial class EnemyDeploymentDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		Guid selectedDP, initialDP;

		public IEventAction eventAction { get; set; }
		public ObservableCollection<DeploymentPoint> deploymentPoints { get; set; } = new();

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

			initialDP = (eventAction as EnemyDeployment).specificDeploymentPoint;

			//if ( (eventAction as EnemyDeployment).sourceType == SourceType.InitialReserved )
			//{
			//	var items = new List<DeploymentCard>();
			//	items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.initialDeploymentGroups.Any( y => y.cardID == x.id ) ) ).ToList();
			//	items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.reservedDeploymentGroups.Any( y => y.cardID == x.id ) ) ).ToList();

			//	enemyCB.ItemsSource = items;
			//}
			//else
			enemyCB.ItemsSource = Utils.enemyData;

			triggersCB.ItemsSource = Utils.mainWindow.localTriggers;
			eventsCB.ItemsSource = Utils.mainWindow.localEvents;

			deploymentPoints.Add( new() { GUID = Guid.Empty, name = "None" } );
			foreach ( var e in Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>() )
			{
				deploymentPoints.Add( e );
			}

			if ( string.IsNullOrEmpty( (eventAction as EnemyDeployment).repositionInstructions ) )
			{
				repInfo.Foreground = new SolidColorBrush( Colors.Red );
				repInfo.Text = "Not Set";
			}
			else
			{
				repInfo.Foreground = new SolidColorBrush( Colors.LawnGreen );
				repInfo.Text = "Text Set";
			}

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
			if ( (eventAction as EnemyDeployment).sourceType == SourceType.Hand )
				eventAction.displayName = "Deploy: From Hand";
			else if ( string.IsNullOrEmpty( (eventAction as EnemyDeployment).deploymentGroup ) )
				eventAction.displayName = "Deploy: INVALID SELECTION";
			else
				eventAction.displayName = "Deploy: " + (eventAction as EnemyDeployment).deploymentGroup + "/" + (eventAction as EnemyDeployment).enemyGroupData.cardName;

			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void initialRB_Click( object sender, RoutedEventArgs e )
		{
			if ( initialRB.IsChecked == true )
			{
				var items = new List<DeploymentCard>();
				items = items.Concat( Utils.enemyData.Where( x => Utils.mainWindow.mission.reservedDeploymentGroups.Any( y => y.cardID == x.id ) ) ).ToList();

				enemyCB.ItemsSource = items;
			}
			else
				enemyCB.ItemsSource = Utils.enemyData;
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
			EditDPDialog dialog = new EditDPDialog( (eventAction as EnemyDeployment).enemyGroupData );
			dialog.ShowDialog();
		}

		private void enemyCB_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			//DeploymentCard card = Utils.enemyData.First( x => x.id == (eventAction as EnemyDeployment).deploymentGroup );
			(eventAction as EnemyDeployment).UpdateCard( e.AddedItems[0] as DeploymentCard );
			//(eventAction as EnemyDeployment).enemyGroupData.cardName = card.name;
			//(eventAction as EnemyDeployment).enemyGroupData.cardID = card.id;
		}

		private void filterBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
			}
		}

		private void TextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			DeploymentCard dc;

			if ( initialRB.IsChecked == true )
			{
				var items = new List<DeploymentCard>();
				items = Utils.enemyData.Where( x => Utils.mainWindow.mission.reservedDeploymentGroups.Any( y => y.cardID == x.id ) ).ToList();
				//try id
				dc = items.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );
			}
			else
			{
				dc = Utils.enemyData.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );
				//try id
				if ( dc == null )
					dc = Utils.enemyData.Where( x => x.id.Contains( filterBox.Text ) ).FirstOr( null );
			}

			if ( dc != null )
				(eventAction as EnemyDeployment).deploymentGroup = dc.id;
			else
				(eventAction as EnemyDeployment).deploymentGroup = null;
		}

		private void targetBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new PriorityTraitsDialog( (eventAction as EnemyDeployment).enemyGroupData.groupPriorityTraits );
			dlg.ShowDialog();
		}

		private void repositionBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT REPOSITION INSTRUCTIONS", (eventAction as EnemyDeployment).repositionInstructions );
			dlg.ShowDialog();
			(eventAction as EnemyDeployment).repositionInstructions = dlg.theText.Trim();
			repInfo.Foreground = string.IsNullOrEmpty( dlg.theText ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void resetBtn_Click( object sender, RoutedEventArgs e )
		{

		}

		private void dpCB_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			selectedDP = (eventAction as EnemyDeployment).specificDeploymentPoint;
			if ( initialDP != selectedDP )
			{
				initialDP = selectedDP;
				(eventAction as EnemyDeployment).enemyGroupData.SetDP( (eventAction as EnemyDeployment).specificDeploymentPoint );
			}
		}
	}
}
