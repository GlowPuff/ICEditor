using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for AddEventDialog.xaml
	/// </summary>
	public partial class AddEventDialog : Window, INotifyPropertyChanged
	{
		private IEventAction _selectedEventAction;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool showCancelBtn { get; set; } = false;
		public MissionEvent missionEvent { get; set; }
		public List<Trigger> allTriggers { get; set; }
		public IEventAction selectedEventAction { get { return _selectedEventAction; } set { _selectedEventAction = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public AddEventDialog( MissionEvent me = null )
		{
			InitializeComponent();

			showCancelBtn = me == null;
			missionEvent = me ?? (new());
			RebuildTriggerMenu();
			DataContext = missionEvent;

			etInfo.Text = string.IsNullOrEmpty( missionEvent.eventText ) ? "Text Not Set" : "Text Set";
			SolidColorBrush brush = new( string.IsNullOrEmpty( missionEvent.eventText ) ? Colors.Red : Colors.LawnGreen );
			etInfo.Foreground = brush;

			alliesCB.ItemsSource = Utils.allyData;
			activationCB.ItemsSource = Utils.enemyData;
			woundedCB.ItemsSource = withdrawsCB.ItemsSource = Utils.heroData;

			if ( missionEvent.eventActions.Count > 0 )
				selectedEventAction = missionEvent.eventActions[0];

			//validate additional triggers (name change, still exists)
			for ( int i = 0; i < missionEvent.additionalTriggers.Count; i++ )
			{
				if ( !Utils.mainWindow.mission.TriggerExists( missionEvent.additionalTriggers[i].triggerGUID ) )
				{
					missionEvent.additionalTriggers[i].triggerGUID = Guid.Empty;
					missionEvent.additionalTriggers[i].triggerName = "None";
				}
			}
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = true;
			Close();
		}

		private void addAdditionalTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new AddTriggerDialog();
			if ( dlg.ShowDialog().Value )
			{
				if ( dlg.trigger.isGlobal )
					Utils.mainWindow.mission.globalTriggers.Add( dlg.trigger );
				else
					Utils.mainWindow.activeSection.triggers.Add( dlg.trigger );
				Utils.mainWindow.leftPanel.triggersCB.SelectedItem = dlg.trigger;
				//add to this event's trigger list
				missionEvent.additionalTriggers.Add( new TriggeredBy( dlg.trigger ) );
				RebuildTriggerMenu();
			}
		}

		private void removeAdditionalTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			var t = (sender as Button).DataContext as TriggeredBy;
			missionEvent.additionalTriggers.Remove( t );
		}

		private void removeActionButton_Click( object sender, RoutedEventArgs e )
		{
			missionEvent.eventActions.Remove( eventActionsCB.SelectedItem as EventAction );
			if ( missionEvent.eventActions.Count > 0 )
				selectedEventAction = missionEvent.eventActions[0];
			else
				selectedEventAction = null;
		}

		private void modifyActionButton_Click( object sender, RoutedEventArgs e )
		{
			EditEventAction();
		}

		private Window GetDialog( string dname, EventActionType et, IEventAction ea = null ) => et switch
		{
			EventActionType.G1 => new MissionManagementDialog( dname, et, ea ),
			EventActionType.G2 => new ChangeMissionInfoDialog( dname, et, ea ),
			EventActionType.G3 => new ChangeObjectiveDialog( dname, et, ea ),
			EventActionType.G4 => new ModifyVariableDialog( dname, et, ea ),
			EventActionType.G5 => new ModifyThreatDialog( dname, et, ea ),
			EventActionType.G6 => new QuestionPromptDialog( dname, et, ea ),
			EventActionType.D1 => new EnemyDeploymentDialog( dname, et, ea ),
			EventActionType.D2 => new AllyDeploymentDialog( dname, et, ea ),
			EventActionType.D3 => new OptionalDeploymentDialog( dname, et, ea ),
			EventActionType.D4 => new RandomDeploymentDialog( dname, et, ea ),
			EventActionType.D5 => new AddGroupDialog( dname, et, ea ),
			EventActionType.GM1 => new ChangeInstructionsDialog( dname, et, ea ),
			EventActionType.GM2 => new ChangeTargetDialog( dname, et, ea ),
			EventActionType.GM3 => new ChangeGroupStatusDialog( dname, et, ea ),
			EventActionType.M1 => new MapManagementDialog( dname, et, ea ),
			EventActionType.M2 => new ModifyMapEntityDialog( dname, et, ea ),
			_ => null
		};

		private void MenuItem_Click( object sender, RoutedEventArgs e )
		{
			string dname = (sender as MenuItem).Header as string;
			if ( Enum.TryParse( (string)(sender as MenuItem).Tag, out EventActionType tag ) )
			{
				var dlg = GetDialog( dname, tag );
				dlg?.ShowDialog();

				missionEvent.eventActions.Add( (dlg as IEventActionDialog).eventAction );
				selectedEventAction = (dlg as IEventActionDialog).eventAction;
			}
		}

		private void RebuildTriggerMenu()
		{
			allTriggers = new();
			allTriggers = allTriggers.Concat( Utils.mainWindow.mission.globalTriggers.Where( x => x.GUID != Guid.Empty ) ).ToList();
			//collect all triggers from all map sections
			var f = from m in Utils.mainWindow.mission.mapSections from t in m.triggers where t.GUID != Guid.Empty select t;
			allTriggers = allTriggers.Concat( f ).ToList();

			existingTriggersLB.ItemsSource = allTriggers;
		}

		private void existingTriggersLB_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			if ( e.AddedItems.Count > 0 )
			{
				missionEvent.additionalTriggers.Add( new TriggeredBy( e.AddedItems[0] as Trigger ) );// e.AddedItems[0] as Trigger );
				existingTriggersLB.SelectedIndex = -1;
			}
		}

		private void Window_ContentRendered( object sender, EventArgs e )
		{
			nameTB.Focus();
			nameTB.SelectAll();
		}

		private void editTextButton_Click( object sender, RoutedEventArgs e )
		{
			GenericTextDialog dlg = new( "Event Text", missionEvent.eventText );
			dlg.ShowDialog();
			missionEvent.eventText = dlg.theText;
			etInfo.Text = string.IsNullOrEmpty( missionEvent.eventText ) ? "Text Not Set" : "Text Set";
			SolidColorBrush brush = new( string.IsNullOrEmpty( dlg.theText ) ? Colors.Red : Colors.LawnGreen );
			etInfo.Foreground = brush;
		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = false;
			Close();
		}

		private void nameTB_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void eventActionsCB_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			EditEventAction();
		}

		private void EditEventAction()
		{
			if ( selectedEventAction != null )
			{
				Window dlg = GetDialog( selectedEventAction.displayName, selectedEventAction.eventActionType, selectedEventAction );
				dlg?.ShowDialog();
			}
		}

		private void upBtn_Click( object sender, RoutedEventArgs e )
		{
			IEventAction ea = (sender as Control).DataContext as IEventAction;
			int idx = missionEvent.eventActions.IndexOf( ea );
			missionEvent.eventActions.Move( idx, Math.Max( 0, idx - 1 ) );
		}

		private void downBtn_Click( object sender, RoutedEventArgs e )
		{
			IEventAction ea = (sender as Control).DataContext as IEventAction;
			int idx = missionEvent.eventActions.IndexOf( ea );
			missionEvent.eventActions.Move( idx, Math.Min( missionEvent.eventActions.Count - 1, idx + 1 ) );
		}
	}
}
