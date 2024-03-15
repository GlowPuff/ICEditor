using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for SetCountdownDialog.xaml
	/// </summary>
	public partial class SetCountdownDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public SetCountdownDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new SetCountdown( dname, et );
			DataContext = eventAction;

			eventCB.ItemsSource = Utils.mainWindow.allMissionEvents;
			triggerCB.ItemsSource = Utils.mainWindow.allMissionTriggers;

			//make sure events still exist
			List<string> strings = new();
			var e = Utils.mainWindow.mission.GetEventFromGUID( ((SetCountdown)eventAction).eventGUID );
			if ( e == null )
			{
				((SetCountdown)eventAction).eventGUID = Guid.Empty;
				strings.Add( "Missing Event" );
			}

			//make sure trigger still exists
			if ( Utils.mainWindow.mission.GetTriggerFromGUID( ((SetCountdown)eventAction).triggerGUID ) == null )
			{
				((SetCountdown)eventAction).triggerGUID = Guid.Empty;
				strings.Add( "Missing Trigger" );
			}

			if ( strings.Count > 0 )
				MessageBox.Show( $"This Event Action is referencing one or more items that no longer exist in the Mission.  The following will be set to 'None'.\n\n{string.Join( "\n", strings )}", "Missing Reference(s) Found" );
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			//trim the value of whitespace
			(eventAction as SetCountdown).countdownTimerName = (eventAction as SetCountdown).countdownTimerName.Trim();

			if ( string.IsNullOrEmpty( ((SetCountdown)eventAction).countdownTimerName.Trim() ) )
				MessageBox.Show( "The countdown name is required.", "Missing Countdown Name" );
			else
				Close();
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			timerNAme.Focus();
			timerNAme.SelectAll();
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger trigger = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( trigger != null )
			{
				triggerCB.ItemsSource = Utils.mainWindow.allMissionTriggers;
				((SetCountdown)eventAction).triggerGUID = trigger.GUID;
			}
		}

		private void addNewEventButton_Click( object sender, RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null )
			{
				eventCB.ItemsSource = Utils.mainWindow.allMissionEvents;
				((SetCountdown)eventAction).eventGUID = me.GUID;
			}
		}

		private void inputChanged_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}
	}
}
