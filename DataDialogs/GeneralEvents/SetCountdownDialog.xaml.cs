﻿using System;
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

			eventCB.ItemsSource = Utils.mainWindow.localEvents;
			triggerCB.ItemsSource = Utils.mainWindow.localTriggers;

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
			Close();
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			modifyInput.Focus();
			modifyInput.SelectAll();
		}

		private void eventCB_GotFocus( object sender, RoutedEventArgs e )
		{
			eventCB.GotFocus -= eventCB_GotFocus;
			eventCB.ItemsSource = Utils.mainWindow.localEvents;
			eventCB.GotFocus += eventCB_GotFocus;
		}

		private void triggerCB_GotFocus( object sender, RoutedEventArgs e )
		{
			triggerCB.GotFocus -= triggerCB_GotFocus;
			triggerCB.ItemsSource = Utils.mainWindow.localTriggers;
			triggerCB.GotFocus += triggerCB_GotFocus;
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger trigger = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( trigger != null )
			{
				triggerCB.ItemsSource = Utils.mainWindow.localTriggers;
				((SetCountdown)eventAction).triggerGUID = trigger.GUID;
			}
		}

		private void addNewEventButton_Click( object sender, RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null )
			{
				eventCB.ItemsSource = Utils.mainWindow.localEvents;
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