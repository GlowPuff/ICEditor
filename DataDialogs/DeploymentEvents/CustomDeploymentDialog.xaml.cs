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
	/// Interaction logic for CustomDeploymentDialog.xaml
	/// </summary>
	public partial class CustomDeploymentDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		Guid selectedDP, initialDP;

		public IEventAction eventAction { get; set; }
		public ObservableCollection<DeploymentPoint> deploymentPoints { get; set; } = new();

		public ObservableCollection<DeploymentColor> deploymentColors
		{
			get { return Utils.deploymentColors; }
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public CustomDeploymentDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new CustomEnemyDeployment( dname, et );
			DataContext = eventAction;

			var ced = eventAction as CustomEnemyDeployment;

			initialDP = ced.specificDeploymentPoint;
			enemyCB.ItemsSource = Utils.enemyData;
			allyCB.ItemsSource = Utils.allyData;

			triggersCB.ItemsSource = Utils.mainWindow.localTriggers;
			eventsCB.ItemsSource = Utils.mainWindow.localEvents;

			deploymentPoints.Add( new() { GUID = Guid.Empty, name = "Active Deployment Point" } );
			deploymentPoints.Add( new() { GUID = Utils.GUIDOne, name = "None" } );

			foreach ( var e in Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>() )
			{
				deploymentPoints.Add( e );
			}

			instructionsBtn.Foreground = string.IsNullOrEmpty( ced.enemyGroupData.customText ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
			bonusBtn.Foreground = string.IsNullOrEmpty( ced.bonuses ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
			abilityBtn.Foreground = string.IsNullOrEmpty( ced.abilities ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
			surgeBtn.Foreground = string.IsNullOrEmpty( ced.surges ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
			keywordsBtn.Foreground = string.IsNullOrEmpty( ced.keywords ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );

			//verify trigger/event and dp still exist
			List<string> strings = new();
			if ( !Utils.ValidateMapEntity( ced.specificDeploymentPoint ) )
			{
				strings.Add( "Missing Specific Deployment Point" );
				//ced.specificDeploymentPoint = Guid.Empty;
			}
			for ( int i = 0; i < ced.enemyGroupData.pointList.Count; i++ )
			{
				if ( !Utils.ValidateMapEntity( ced.enemyGroupData.pointList[i].GUID ) )
				{
					strings.Add( "Missing Deployment Point" );
					//ced.enemyGroupData.pointList[i].GUID = Guid.Empty;
				}
			}
			if ( !Utils.ValidateTrigger( ced.enemyGroupData.defeatedTrigger ) )
			{
				strings.Add( "Missing 'On Defeated' Trigger" );
				//ced.enemyGroupData.defeatedTrigger = Guid.Empty;
			}
			if ( !Utils.ValidateEvent( ced.enemyGroupData.defeatedEvent ) )
			{
				strings.Add( "Missing 'On Defeated' Event" );
				//ced.enemyGroupData.defeatedTrigger = Guid.Empty;
			}
			if ( strings.Count > 0 )
				MessageBox.Show( $"This Event Action is referencing an Event, Trigger, or Deployment Point that no longer exist in the Mission.\n\n{string.Join( "\n", strings )}", "Missing Reference(s) Found" );
		}

		private void editDP_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as CustomEnemyDeployment).UpdateDP();

			EditDPDialog dialog = new EditDPDialog( (eventAction as CustomEnemyDeployment).enemyGroupData );
			dialog.ShowDialog();
		}

		private void dpCB_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			selectedDP = (eventAction as CustomEnemyDeployment).specificDeploymentPoint;
			if ( initialDP != selectedDP )
			{
				initialDP = selectedDP;
				(eventAction as CustomEnemyDeployment).enemyGroupData.SetDP( (eventAction as CustomEnemyDeployment).specificDeploymentPoint );
			}
		}

		private void nameTB_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void targetBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new PriorityTraitsDialog( (eventAction as CustomEnemyDeployment).enemyGroupData.groupPriorityTraits );
			dlg.ShowDialog();
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == MouseButtonState.Pressed )
				DragMove();
		}

		private void instructionsBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT INSTRUCTIONS", (eventAction as CustomEnemyDeployment).enemyGroupData.customText );
			dlg.ShowDialog();
			(eventAction as CustomEnemyDeployment).enemyGroupData.customText = dlg.theText;
			instructionsBtn.Foreground = string.IsNullOrEmpty( dlg.theText ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void bonusBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT BONUSES", (eventAction as CustomEnemyDeployment).bonuses );
			dlg.ShowDialog();
			(eventAction as CustomEnemyDeployment).bonuses = dlg.theText;
			bonusBtn.Foreground = string.IsNullOrEmpty( dlg.theText ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void abilityBtn_Click( object sender, RoutedEventArgs e )
		{
			//this one needs special formatting
			var dlg = new GenericTextDialog( "EDIT ABILITIES", (eventAction as CustomEnemyDeployment).abilities );
			dlg.ShowDialog();
			(eventAction as CustomEnemyDeployment).abilities = dlg.theText;
			abilityBtn.Foreground = string.IsNullOrEmpty( dlg.theText ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void surgeBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT SURGES", (eventAction as CustomEnemyDeployment).surges );
			dlg.ShowDialog();
			(eventAction as CustomEnemyDeployment).surges = dlg.theText;
			surgeBtn.Foreground = string.IsNullOrEmpty( dlg.theText ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void keywordsBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT KEYWORDS", (eventAction as CustomEnemyDeployment).keywords );
			dlg.ShowDialog();
			(eventAction as CustomEnemyDeployment).keywords = dlg.theText;
			keywordsBtn.Foreground = string.IsNullOrEmpty( dlg.theText ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void sizeText_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
				Utils.LoseFocus( sender as Control );
			(eventAction as CustomEnemyDeployment).UpdateDP();
		}

		private void sizeText_LostFocus( object sender, RoutedEventArgs e )
		{
			(eventAction as CustomEnemyDeployment).UpdateDP();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as CustomEnemyDeployment).UpdateDP();

			var ced = eventAction as CustomEnemyDeployment;
			//if ( !ced.enemyGroupData.useGenericMugshot && ced.customType == MarkerType.Rebel )
			//	ced.enemyGroupData.cardID = ced.thumbnailGroupRebel;
			//else if ( !ced.enemyGroupData.useGenericMugshot && ced.customType == MarkerType.Imperial )
			//	ced.enemyGroupData.cardID = ced.thumbnailGroupImperial;

			eventAction.displayName = "Custom Deployment: " + ced.enemyGroupData.cardName;

			Close();
		}
	}
}
