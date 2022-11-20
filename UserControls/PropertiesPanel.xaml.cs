using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for PropertiesPanel.xaml
	/// </summary>
	public partial class PropertiesPanel : UserControl, INotifyPropertyChanged
	{
		bool _editEnabled, _editEGEnabled;
		EventGroup _selectedEventGroup;
		EntityGroup _selectedEntityGroup;
		int _mapEntityCount;
		string _eventGroupCount, _entityGroupCount;

		public string eventGroupCount { get => _eventGroupCount; set { _eventGroupCount = value; PC(); } }
		public string entityGroupCount { get => _entityGroupCount; set { _entityGroupCount = value; PC(); } }
		public bool editEnabled { get { return _editEnabled; } set { _editEnabled = value; PC(); } }
		public bool editEGEnabled { get { return _editEGEnabled; } set { _editEGEnabled = value; PC(); } }
		public ObservableCollection<EventGroup> eventGroups { get { return Utils.mainWindow.mission.eventGroups; } }
		public ObservableCollection<EntityGroup> entityGroups { get { return Utils.mainWindow.mission.entityGroups; } }
		public int mapEntityCount { get { return _mapEntityCount; } set { _mapEntityCount = value; PC(); } }
		public EventGroup selectedEventGroup
		{
			get { return _selectedEventGroup; }
			set
			{
				_selectedEventGroup = value;
				PC();
				editEnabled = _selectedEventGroup != null;
			}
		}
		public EntityGroup selectedEntityGroup
		{
			get { return _selectedEntityGroup; }
			set
			{
				_selectedEntityGroup = value;
				PC();
				editEGEnabled = _selectedEntityGroup != null;
			}
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public PropertiesPanel()
		{
			InitializeComponent();
			rightCard.DataContext = this;
		}

		public void UpdateUI( MapSection ms = null )
		{
			leftCard.DataContext = ms ?? leftCard.DataContext;
			meTB.DataContext = Utils.mainWindow.mission;

			mapEntityCount = Utils.mainWindow.mission.mapEntities.Where( x => x.mapSectionOwner == Utils.mainWindow.activeSection.GUID ).Count();

			eventGroupCount = $"Event Groups, {Utils.mainWindow.mission.eventGroups.Count()} in Mission";
			entityGroupCount = $"Random Map Entity Groups, {Utils.mainWindow.mission.entityGroups.Count()} in Mission";

			//start section can't be toggled
			//invisToggle.IsEnabled = Utils.mainWindow.activeSection.GUID != Guid.Parse( "11111111-1111-1111-1111-111111111111" );
			//if it's the start section, make sure this setting is set to NO
			//if ( Utils.mainWindow.activeSection.GUID == Guid.Parse( "11111111-1111-1111-1111-111111111111" ) )
			//	Utils.mainWindow.activeSection.invisibleUntilActivated = false;
		}

		private void TextBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void editGroupBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			if ( selectedEventGroup == null )
				return;

			var dlg = new EventGroupDialog( selectedEventGroup );
			dlg.ShowDialog();
		}

		private void newGroupBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new EventGroupDialog();
			dlg.ShowDialog();
			if ( dlg.DialogResult == true )
			{
				Utils.mainWindow.mission.eventGroups.Add( dlg.eventGroup );
				selectedEventGroup = dlg.eventGroup;
			}
		}

		private void remGroupBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			if ( selectedEventGroup == null )
				return;
			Utils.mainWindow.mission.eventGroups.Remove( selectedEventGroup );
			selectedEventGroup = null;
		}

		private void remTokenGroupBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			if ( selectedEntityGroup == null )
				return;
			Utils.mainWindow.mission.entityGroups.Remove( selectedEntityGroup );
			selectedEntityGroup = null;
		}

		private void editTokenGroupBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			if ( selectedEntityGroup == null )
				return;

			var dlg = new EntityGroupDialog( selectedEntityGroup );
			dlg.ShowDialog();
		}

		private void newTokenGroupBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new EntityGroupDialog();
			dlg.ShowDialog();
			if ( dlg.DialogResult == true )
			{
				Utils.mainWindow.mission.entityGroups.Add( dlg.entityGroup );
				selectedEntityGroup = dlg.entityGroup;
			}
		}
	}
}
