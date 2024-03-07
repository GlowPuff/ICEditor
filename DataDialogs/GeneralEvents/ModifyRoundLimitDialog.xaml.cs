using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ModifyRoundLimitDialog.xaml
	/// </summary>
	public partial class ModifyRoundLimitDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		public ModifyRoundLimitDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ModifyRoundLimit( dname, et );
			DataContext = eventAction;

			eventCB.ItemsSource = Utils.mainWindow.localEvents;

			//make sure events still exist
			List<string> strings = new();
			var e = Utils.mainWindow.mission.GetEventFromGUID( ((ModifyRoundLimit)eventAction).eventGUID );
			if ( e == null )
			{
				((SetCountdown)eventAction).eventGUID = Guid.Empty;
				strings.Add( "Missing Event" );
			}
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

		private void inputChanged_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void addNewEventButton_Click( object sender, RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null )
			{
				eventCB.ItemsSource = Utils.mainWindow.localEvents;
				((ModifyRoundLimit)eventAction).eventGUID = me.GUID;
			}
		}
	}
}
