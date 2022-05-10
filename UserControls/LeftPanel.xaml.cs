﻿using System;
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
		bool _showGlobal;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool showGlobal
		{
			get { return _showGlobal; }
			set
			{
				_showGlobal = value;
				if ( _showGlobal )
				{
					triggersCB.ItemsSource = Utils.mainWindow.mission.globalTriggers;
					eventsCB.ItemsSource = Utils.mainWindow.mission.globalEvents;
				}
				else
				{
					triggersCB.ItemsSource = Utils.mainWindow.activeSection.triggers;
					eventsCB.ItemsSource = Utils.mainWindow.activeSection.missionEvents;
				}
				localText.Visibility = _showGlobal ? Visibility.Collapsed : Visibility.Visible;
				globalText.Visibility = _showGlobal ? Visibility.Visible : Visibility.Collapsed;
				triggersCB.SelectedIndex = 0;
				eventsCB.SelectedIndex = 0;
				PC();
			}
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public LeftPanel()
		{
			InitializeComponent();

			showGlobalToggle.DataContext = this;
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
			if ( t.GUID != Guid.Empty )
			{
				if ( showGlobal )
					Utils.mainWindow.mission.globalTriggers.Remove( t );
				else
					Utils.mainWindow.activeSection.triggers.Remove( t );
				triggersCB.SelectedIndex = 0;
				Utils.mainWindow.SetStatus( "Trigger Removed" );
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
			if ( t.GUID != Guid.Empty )
			{
				if ( showGlobal )
					Utils.mainWindow.mission.globalEvents.Remove( t );
				else
					Utils.mainWindow.activeSection.missionEvents.Remove( t );
				eventsCB.SelectedIndex = 0;
				Utils.mainWindow.mapEditor.SetSelectedPropertyPanel();
				Utils.mainWindow.SetStatus( "Event Removed" );
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
					if ( showGlobal )
						triggersCB.SelectedItem = dlg.trigger;
					else
						triggersCB.SelectedIndex = 0;
				}
				else if ( !dlg.trigger.isGlobal && dlg.editedTrigger.isGlobal )
				{
					Utils.mainWindow.mission.globalTriggers.Remove( dlg.editedTrigger );
					Utils.mainWindow.activeSection.triggers.Add( dlg.trigger );
					if ( showGlobal )
						triggersCB.SelectedIndex = 0;
					else
						triggersCB.SelectedItem = dlg.trigger;
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
					if ( showGlobal )
						eventsCB.SelectedItem = dlg.missionEvent;
					else
						eventsCB.SelectedIndex = 0;
				}
				else if ( !dlg.missionEvent.isGlobal && previousGlobal )
				{
					Utils.mainWindow.mission.globalEvents.Remove( dlg.missionEvent );
					Utils.mainWindow.activeSection.missionEvents.Add( dlg.missionEvent );
					if ( showGlobal )
						eventsCB.SelectedIndex = 0;
					else
						eventsCB.SelectedItem = dlg.missionEvent;
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
	}
}
