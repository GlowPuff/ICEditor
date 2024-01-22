using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ChooseSetNextMissionWindow.xaml
	/// </summary>
	public partial class ChooseSetNextMissionWindow : Window
	{
		public ObservableCollection<ChooseNextMissionEvent> eventActionList { get; set; }

		public ChooseSetNextMissionWindow( List<MissionEvent> evList )
		{
			InitializeComponent();

			eventActionList = new();

			foreach ( var item in evList )
			{
				string evName = item.name;
				foreach ( var ea in item.eventActions.Where( x => x.eventActionType == EventActionType.CM4 ) )
				{
					var ev = new ChooseNextMissionEvent()
					{
						eventAction = ea as CampaignSetNextMission,
						eventName = evName,
						identifier = ((CampaignSetNextMission)ea).customMissionID
					};
					eventActionList.Add( ev );
				}
			}

			DataContext = this;
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

		private void structureLV_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			var item = ((ListView)sender).SelectedItem as ChooseNextMissionEvent;
			var dlg = new SetNextMissionDialog( "Set Next Mission", EventActionType.CM4, item.eventAction, true );
			dlg.ShowDialog();
			item.identifier = item.eventAction.customMissionID;
		}

		private void editEvent_Click( object sender, RoutedEventArgs e )
		{
			var item = ((Button)sender).DataContext as ChooseNextMissionEvent;
			var dlg = new SetNextMissionDialog( "Set Next Mission", EventActionType.CM4, item.eventAction, true );
			dlg.ShowDialog();
			item.identifier = item.eventAction.customMissionID;
		}
	}

	/// <summary>
	/// Bindable construct to hold the event action to display in the window's listbox
	/// </summary>
	public class ChooseNextMissionEvent : ObservableObject
	{
		string _identifier;

		public string eventName { get; set; }
		public string identifier { get => _identifier; set { SetProperty( ref _identifier, value ); } }
		public CampaignSetNextMission eventAction { get; set; }
	}
}
