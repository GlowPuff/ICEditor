using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for LeftPanel.xaml
	/// </summary>
	public partial class LeftPanel : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public LeftPanel()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Called from MainWindow after a Mission has been loaded/created
		/// </summary>
		public void SetListSources()
		{
			triggersCB.ItemsSource = Utils.mainWindow.mission.globalTriggers;
			eventsCB.ItemsSource = Utils.mainWindow.mission.globalEvents;
			triggersCB.SelectedIndex = 0;
			eventsCB.SelectedIndex = 0;
		}

		private void editTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			EditTrigger();
		}

		public void addTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new AddTriggerDialog();
			if ( dlg.ShowDialog().Value )
			{
				if ( dlg.trigger.isGlobal )
					Utils.mainWindow.mission.globalTriggers.Add( dlg.trigger );
				else
					Utils.mainWindow.activeSection.triggers.Add( dlg.trigger );
				triggersCB.SelectedItem = dlg.trigger;
				Utils.mainWindow.SetStatus( "Trigger Added" );
			}
		}

		public Trigger addNewTrigger()
		{
			var dlg = new AddTriggerDialog();
			if ( dlg.ShowDialog().Value )
			{
				if ( dlg.trigger.isGlobal )
					Utils.mainWindow.mission.globalTriggers.Add( dlg.trigger );
				else
					Utils.mainWindow.activeSection.triggers.Add( dlg.trigger );
				triggersCB.SelectedItem = dlg.trigger;
				Utils.mainWindow.SetStatus( "Trigger Added" );
				return dlg.trigger;
			}
			return null;
		}

		public MissionEvent AddNewEvent()
		{
			var dlg = new AddEventDialog();
			if ( dlg.ShowDialog().Value )
			{
				if ( dlg.missionEvent.isGlobal )
					Utils.mainWindow.mission.globalEvents.Add( dlg.missionEvent );
				else
					Utils.mainWindow.activeSection.missionEvents.Add( dlg.missionEvent );
				eventsCB.SelectedItem = dlg.missionEvent;
				Utils.mainWindow.SetStatus( "Event Added" );
				return dlg.missionEvent;
			}
			return null;
		}

		private void remTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			var t = triggersCB.SelectedItem as Trigger;
			MessageBoxResult res = MessageBoxResult.Yes;

			if ( t.GUID == Guid.Empty )
			{
				Utils.mainWindow.SetStatus( "Can't Remove 'None'" );
				return;
			}

			var report = HealthReportHelper.CheckAndNotifyTriggerRemoved( t.GUID, NotifyMode.Report, t.name );
			if ( report.isBroken )
			{
				res = MessageBox.Show( "Deleting this Trigger will break references to it made from other Events, Triggers, or Map Entities.\n\nAre you sure you want to delete it?\n\nChoosing YES will delete the Trigger and fix any broken references, along with displaying a report showing you where all the fixes were made.\n\nChoosing NO will delete the Trigger but will NOT fix the broken references left behind.\n\nChoosing CANCEL will cancel this whole operation.\n\nNOTE: Fixing this broken reference will update all affected Buttons, Input Ranges, and any other items within the data.", "Warning - Deleting Will Create One Or More Broken References", MessageBoxButton.YesNoCancel, MessageBoxImage.Question );
			}

			if ( res == MessageBoxResult.Cancel )
				return;

			if ( t.GUID != Guid.Empty )
			{
				Utils.mainWindow.mission.globalTriggers.Remove( t );
				triggersCB.SelectedIndex = 0;
				Utils.mainWindow.SetStatus( "Trigger Removed" );
				//fix the broken references
				if ( report.isBroken && res == MessageBoxResult.Yes )
				{
					var dlg = new BrokenRefWindow( NotifyType.Trigger, $"{report.detailsMessage}\n\nThese broken Trigger references will be changed to 'None (Global)'.  However, for some affected items, these broken Events will be REMOVED from them entirely.\n\nNOTE: This will affect Buttons, Input Ranges, and any other items within affected data.", report.brokenList, t.GUID, t.name );
					dlg.ShowDialog();
				}
			}
		}

		public void addEventButton_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new AddEventDialog();
			if ( dlg.ShowDialog().Value )
			{
				if ( dlg.missionEvent.isGlobal )
					Utils.mainWindow.mission.globalEvents.Add( dlg.missionEvent );
				else
					Utils.mainWindow.activeSection.missionEvents.Add( dlg.missionEvent );
				eventsCB.SelectedItem = dlg.missionEvent;
				Utils.mainWindow.mapEditor.SetSelectedPropertyPanel();
				Utils.mainWindow.SetStatus( "Event Added" );
			}
		}

		private void remEventButton_Click( object sender, RoutedEventArgs e )
		{
			var t = eventsCB.SelectedItem as MissionEvent;
			MessageBoxResult res = MessageBoxResult.Yes;

			if ( t.GUID == Guid.Empty )
			{
				Utils.mainWindow.SetStatus( "Can't Remove 'None'" );
				return;
			}

			var report = HealthReportHelper.CheckAndNotifyEventRemoved( t.GUID, NotifyMode.Report, t.name );
			if ( report.isBroken )
			{
				res = MessageBox.Show( "Deleting this Event will break references to it made from other Events, Triggers, or Map Entities.\n\nAre you sure you want to delete it?\n\nChoosing YES will delete the Event and fix any broken references, along with displaying a report showing you where all the fixes were made.\n\nChoosing NO will delete the Event but will NOT fix the broken references left behind.\n\nChoosing CANCEL will cancel this whole operation.\n\nNOTE: Fixing this broken reference will update all affected Buttons, Input Ranges, and any other items within the data.", "Warning - Deleting Will Create One Or More Broken References", MessageBoxButton.YesNoCancel, MessageBoxImage.Question );
			}

			if ( res == MessageBoxResult.Cancel )
				return;

			if ( t.GUID != Guid.Empty )
			{
				Utils.mainWindow.mission.globalEvents.Remove( t );
				eventsCB.SelectedIndex = 0;
				Utils.mainWindow.mapEditor.SetSelectedPropertyPanel();
				Utils.mainWindow.SetStatus( "Event Removed" );
				//fix the broken references
				if ( report.isBroken && res == MessageBoxResult.Yes )
				{
					var dlg = new BrokenRefWindow( NotifyType.Event, $"{report.detailsMessage}\n\nThese broken Event references will be changed to 'None (Global)'.  However, for some affected items, these broken Events will be REMOVED from them entirely.\n\nNOTE: This will affect Buttons, Input Ranges, and any other items within affected data.", report.brokenList, t.GUID, t.name );
					dlg.ShowDialog();
				}
			}
		}

		private void editEventButton_Click( object sender, RoutedEventArgs e )
		{
			EditEvent();
		}

		private void eventsCB_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			EditEvent();
		}

		private void triggersCB_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			EditTrigger();
		}

		private void EditTrigger()
		{
			var t = triggersCB.SelectedItem as Trigger;
			if ( t.GUID == Guid.Empty )
				return;

			var index = triggersCB.SelectedIndex;
			var dlg = new AddTriggerDialog( t );
			if ( dlg.ShowDialog().Value )
			{
				//make sure edited trigger is same TYPE
				if ( dlg.trigger.isGlobal && dlg.editedTrigger.isGlobal )
				{
					var idx = Utils.mainWindow.mission.globalTriggers.IndexOf( dlg.editedTrigger );
					Utils.mainWindow.mission.globalTriggers[idx] = dlg.trigger;
					triggersCB.SelectedIndex = index;
				}
				else if ( !dlg.trigger.isGlobal && !dlg.editedTrigger.isGlobal )
				{
					var idx = Utils.mainWindow.activeSection.triggers.IndexOf( dlg.editedTrigger );
					Utils.mainWindow.activeSection.triggers[idx] = dlg.trigger;
					triggersCB.SelectedIndex = index;
				}
				else if ( dlg.trigger.isGlobal && !dlg.editedTrigger.isGlobal )
				{
					Utils.mainWindow.mission.globalTriggers.Add( dlg.trigger );
					Utils.mainWindow.activeSection.triggers.Remove( dlg.editedTrigger );
					triggersCB.SelectedItem = dlg.trigger;
				}
				else if ( !dlg.trigger.isGlobal && dlg.editedTrigger.isGlobal )
				{
					Utils.mainWindow.mission.globalTriggers.Remove( dlg.editedTrigger );
					Utils.mainWindow.activeSection.triggers.Add( dlg.trigger );
					triggersCB.SelectedIndex = 0;
				}
			}
		}

		private void EditEvent()
		{
			var t = eventsCB.SelectedItem as MissionEvent;
			if ( t.GUID == Guid.Empty )
				return;

			bool previousGlobal = t.isGlobal;
			var index = eventsCB.SelectedIndex;
			var dlg = new AddEventDialog( t );
			if ( dlg.ShowDialog().Value )
			{
				if ( dlg.missionEvent.isGlobal && previousGlobal )
				{ eventsCB.SelectedIndex = index; }
				else if ( !dlg.missionEvent.isGlobal && !previousGlobal )
				{ eventsCB.SelectedIndex = index; }
				else if ( dlg.missionEvent.isGlobal && !previousGlobal )
				{
					Utils.mainWindow.mission.globalEvents.Add( dlg.missionEvent );
					Utils.mainWindow.activeSection.missionEvents.Remove( dlg.missionEvent );
					eventsCB.SelectedItem = dlg.missionEvent;
				}
				else if ( !dlg.missionEvent.isGlobal && previousGlobal )
				{
					Utils.mainWindow.mission.globalEvents.Remove( dlg.missionEvent );
					Utils.mainWindow.activeSection.missionEvents.Add( dlg.missionEvent );
					eventsCB.SelectedIndex = 0;
				}
				Utils.mainWindow.mapEditor.SetSelectedPropertyPanel();
			}
		}

		private void ScrollViewer_PreviewMouseWheel( object sender, System.Windows.Input.MouseWheelEventArgs e )
		{
			ScrollViewer scv = (ScrollViewer)sender;
			scv.ScrollToVerticalOffset( scv.VerticalOffset - e.Delta );
			e.Handled = true;
		}

		private void dupeEventBtn_Click( object sender, RoutedEventArgs e )
		{
			var t = eventsCB.SelectedItem as MissionEvent;
			if ( t.GUID != Guid.Empty )
			{
				var clone = t.Clone();
				if ( t.isGlobal )
					Utils.mainWindow.mission.globalEvents.Add( clone as MissionEvent );
				else
					Utils.mainWindow.activeSection.missionEvents.Add( clone as MissionEvent );
				eventsCB.SelectedItem = clone;
			}
		}

		public void SelectTrigger( Trigger t )
		{
			triggersCB.SelectedItem = t;
			EditTrigger();
		}

		public void SelectEvent( MissionEvent ev )
		{
			eventsCB.SelectedItem = ev;
			EditEvent();
		}
	}
}
