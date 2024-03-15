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
using System.Windows.Media.Imaging;

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

			enemyCB.ItemsSource = Utils.enemyData;
			triggersCB.ItemsSource = Utils.mainWindow.allMissionTriggers;
			eventsCB.ItemsSource = Utils.mainWindow.allMissionEvents;
			thumbListCB.ItemsSource = Utils.thumbnailData.Filter( ThumbType.All );
			thumbListCB.SelectedItem = (eventAction as EnemyDeployment).thumbnail;

			deploymentPoints.Add( new() { GUID = Guid.Empty, name = "Active Deployment Point" } );
			deploymentPoints.Add( new() { GUID = Utils.GUIDOne, name = "None" } );

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

			//verify trigger/event and dp still exist
			List<string> strings = new();
			if ( !Utils.ValidateMapEntity( (eventAction as EnemyDeployment).specificDeploymentPoint ) )
			{
				strings.Add( "Missing Specific Deployment Point" );
				//(eventAction as EnemyDeployment).specificDeploymentPoint = Guid.Empty;
			}
			for ( int i = 0; i < (eventAction as EnemyDeployment).enemyGroupData.pointList.Count; i++ )
			{
				if ( !Utils.ValidateMapEntity( (eventAction as EnemyDeployment).enemyGroupData.pointList[i].GUID ) )
				{
					strings.Add( "Missing Deployment Point" );
					//(eventAction as EnemyDeployment).enemyGroupData.pointList[i].GUID = Guid.Empty;
				}
			}
			if ( !Utils.ValidateTrigger( (eventAction as EnemyDeployment).enemyGroupData.defeatedTrigger ) )
			{
				strings.Add( "Missing 'On Defeated' Trigger" );
				//(eventAction as EnemyDeployment).enemyGroupData.defeatedTrigger = Guid.Empty;
			}
			if ( !Utils.ValidateEvent( (eventAction as EnemyDeployment).enemyGroupData.defeatedEvent ) )
			{
				strings.Add( "Missing 'On Defeated' Event" );
			}
			if ( strings.Count > 0 )
				MessageBox.Show( $"This Event Action is referencing an Event, Trigger, or Deployment Point that no longer exist in the Mission.\n\n{string.Join( "\n", strings )}", "Missing Reference(s) Found" );
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			//if ( (eventAction as EnemyDeployment).sourceType == SourceType.Hand )
			//eventAction.displayName = "Deploy: From Hand";
			if ( string.IsNullOrEmpty( (eventAction as EnemyDeployment).deploymentGroup ) )
				eventAction.displayName = "Deploy: INVALID SELECTION";
			else
				eventAction.displayName = "Deploy: " + (eventAction as EnemyDeployment).deploymentGroup + "/" + (eventAction as EnemyDeployment).enemyGroupData.cardName;

			Close();
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
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

		private void nameTB_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
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
			if ( e.AddedItems.Count > 0 )
				(eventAction as EnemyDeployment).UpdateCard( e.AddedItems[0] as DeploymentCard );
			//(eventAction as EnemyDeployment).enemyGroupData.cardName = card.name;
			//(eventAction as EnemyDeployment).enemyGroupData.cardID = card.id;
		}

		private void filterBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				filterBox.TextChanged -= TextBox_TextChanged;
				filterBox.Text = "";
				filterBox.TextChanged += TextBox_TextChanged;
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
			(eventAction as EnemyDeployment).repositionInstructions = dlg.theText;
			repInfo.Foreground = string.IsNullOrEmpty( dlg.theText ) ? new SolidColorBrush( Colors.Red ) : new SolidColorBrush( Colors.LawnGreen );
		}

		private void resetBtn_Click( object sender, RoutedEventArgs e )
		{
			//   WHAT IS THIS FOR
		}

		private void filterAllButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.All );
		}

		private void filterRebelButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.Rebel );
		}

		private void filterImperialButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.Imperial );
		}

		private void filterMercButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.Mercenary );
		}

		private void filterStockImperialButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.StockImperial );
		}

		private void filterHeroButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.StockHero );
		}

		private void filterVillainButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.StockVillain );
		}

		private void filterAllyButton_Click( object sender, RoutedEventArgs e )
		{
			SetThumbSource( ThumbType.StockAlly );
		}

		private void iconFilterBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			if ( string.IsNullOrEmpty( iconFilterBox.Text ) )
				return;

			var fthumbs = Utils.thumbnailData.Filter( ThumbType.All ).Where( x => x.Name.ToLower().Contains( iconFilterBox.Text.ToLower() ) );
			if ( fthumbs == null )
				return;

			//set custom filtered CB source
			thumbListCB.SelectionChanged -= thumbListCB_SelectionChanged;
			thumbListCB.ItemsSource = fthumbs;
			thumbListCB.SelectionChanged += thumbListCB_SelectionChanged;

			//select first one found
			if ( fthumbs.FirstOr( null ) != null )
			{
				thumbListCB.SelectionChanged -= thumbListCB_SelectionChanged;
				thumbListCB.SelectedItem = fthumbs.First();
				thumbListCB.SelectionChanged += thumbListCB_SelectionChanged;

				(eventAction as EnemyDeployment).thumbnail = fthumbs.First();
				SetThumbnailImage();
			}
		}

		private void thumbListCB_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			(eventAction as EnemyDeployment).thumbnail = thumbListCB.SelectedItem as Thumbnail;
			SetThumbnailImage();
			iconFilterBox.Text = "";
			SetThumbSource( ThumbType.All );
		}

		private void iconFilterBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( string.IsNullOrEmpty( iconFilterBox.Text ) )
				return;

			var fthumbs = Utils.thumbnailData.Filter( ThumbType.All ).Where( x => x.Name.ToLower().Contains( iconFilterBox.Text.ToLower() ) );
			if ( fthumbs == null )
				return;

			//set custom filtered CB source
			thumbListCB.SelectionChanged -= thumbListCB_SelectionChanged;
			thumbListCB.ItemsSource = fthumbs;
			thumbListCB.SelectionChanged += thumbListCB_SelectionChanged;

			//select first one found
			if ( fthumbs.FirstOr( null ) != null )
			{
				thumbListCB.SelectionChanged -= thumbListCB_SelectionChanged;
				thumbListCB.SelectedItem = fthumbs.First();
				thumbListCB.SelectionChanged += thumbListCB_SelectionChanged;
				(eventAction as EnemyDeployment).thumbnail = fthumbs.First();
				SetThumbnailImage();
			}
		}

		private void SetThumbSource( ThumbType ttype )
		{
			iconFilterBox.Text = "";
			thumbListCB.SelectionChanged -= thumbListCB_SelectionChanged;
			thumbListCB.ItemsSource = Utils.thumbnailData.Filter( ttype );
			thumbListCB.SelectionChanged += thumbListCB_SelectionChanged;
		}

		public void SetThumbnailImage()
		{
			var item = Utils.thumbnailData.Filter( ThumbType.All ).Where( x => x.ID == (eventAction as EnemyDeployment).thumbnail.ID ).FirstOrDefault();
			thumbListCB.SelectedItem = item;
			thumbPreview.Source = new BitmapImage( new Uri( $"pack://application:,,,/Imperial Commander Editor;component/Assets/Thumbnails/{(eventAction as EnemyDeployment).thumbnail.ID.ThumbFolder()}/{(eventAction as EnemyDeployment).thumbnail.ID}.png" ) );
		}

		private void useDefaultIconButton_Click( object sender, RoutedEventArgs e )
		{
			thumbListCB.ItemsSource = Utils.thumbnailData.Filter( ThumbType.All );
			(eventAction as EnemyDeployment).thumbnail = Utils.thumbnailData.NoneThumb;
			thumbListCB.SelectedItem = (eventAction as EnemyDeployment).thumbnail;
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( t != null )
			{
				triggersCB.ItemsSource = Utils.mainWindow.allMissionTriggers;
				(eventAction as EnemyDeployment).enemyGroupData.defeatedTrigger = t.GUID;
			}
		}

		private void addNewEventButton_Click( object sender, RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null )
			{
				eventsCB.ItemsSource = Utils.mainWindow.allMissionEvents;
				(eventAction as EnemyDeployment).enemyGroupData.defeatedEvent = me.GUID;
			}
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
