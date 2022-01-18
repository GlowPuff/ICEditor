using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EventGroupDialog.xaml
	/// </summary>
	public partial class EventGroupDialog : Window, INotifyPropertyChanged
	{
		bool _buttonEnabled;
		MissionEvent _selectedEvent;

		public bool buttonEnabled { get { return _buttonEnabled; } set { _buttonEnabled = value; PC(); } }
		public EventGroup eventGroup { get; set; }
		public MissionEvent selectedEvent
		{
			get { return _selectedEvent; }
			set
			{
				_selectedEvent = value;
				PC();
				buttonEnabled = _selectedEvent != null;
			}
		}
		public bool showCancelBtn { get; set; }
		public ObservableCollection<MissionEvent> addedEvents { get; set; } = new();

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public EventGroupDialog( EventGroup eg = null )
		{
			InitializeComponent();
			DataContext = this;

			eventsCB.ItemsSource = Utils.mainWindow.localEvents;
			tCB.ItemsSource = Utils.mainWindow.localTriggers;

			this.eventGroup = eg ?? new();
			showCancelBtn = eg == null;
			buttonEnabled = false;

			//make sure events still exist
			for ( int i = eventGroup.missionEvents.Count - 1; i >= 0; i-- )
			{
				var e = Utils.mainWindow.mission.GetEventFromGUID( eventGroup.missionEvents[i] );
				if ( e != null )
					addedEvents.Add( e );
				else
					eventGroup.missionEvents.RemoveAt( i );
			}
			//make sure trigger still exists
			if ( Utils.mainWindow.mission.GetTriggerFromGUID( eventGroup.triggerGUID ) == null )
				eventGroup.triggerGUID = Guid.Empty;
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			if ( !string.IsNullOrEmpty( eventGroup.name ) )
			{
				DialogResult = true;
				Close();
			}
		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = false;
			Close();
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void addGroupBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedEvent == null )
				return;
			eventGroup.missionEvents.Add( selectedEvent.GUID );
			addedEvents.Add( selectedEvent );
			selectedEvent = null;
		}

		private void remButton_Click( object sender, RoutedEventArgs e )
		{
			eventGroup.missionEvents.Remove( ((sender as FrameworkElement).DataContext as MissionEvent).GUID );
			addedEvents.Remove( (sender as FrameworkElement).DataContext as MissionEvent );
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( t != null )
			{
				tCB.ItemsSource = Utils.mainWindow.localTriggers;
				eventGroup.triggerGUID = t.GUID;
			}
		}

		private void tCB_GotFocus( object sender, RoutedEventArgs e )
		{
			tCB.GotFocus -= tCB_GotFocus;
			tCB.ItemsSource = Utils.mainWindow.localTriggers;
			tCB.GotFocus += tCB_GotFocus;
		}
	}
}
