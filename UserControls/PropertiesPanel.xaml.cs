using System.Collections.ObjectModel;
using System.ComponentModel;
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
		//private MapSection _mapSection;
		//public MapSection mapSection { get { return _mapSection; } set { _mapSection = value; PC(); } }
		bool _editEnabled;
		EventGroup _selectedEventGroup;

		public bool editEnabled { get { return _editEnabled; } set { _editEnabled = value; PC(); } }
		public ObservableCollection<EventGroup> eventGroups { get { return Utils.mainWindow.mission.eventGroups; } }
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

		public void Populate( MapSection ms )
		{
			leftCard.DataContext = ms;
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
	}
}
