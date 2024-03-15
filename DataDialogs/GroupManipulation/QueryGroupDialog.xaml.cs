using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for QueryGroupDialog.xaml
	/// </summary>
	public partial class QueryGroupDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		DeploymentCard _selectedGroup, _selectedAllyGroup;
		public DeploymentCard selectedGroup { get { return _selectedGroup; } set { _selectedGroup = value; PC(); } }
		public DeploymentCard selectedAllyGroup { get { return _selectedAllyGroup; } set { _selectedAllyGroup = value; PC(); } }

		public IEventAction eventAction { get; set; }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public QueryGroupDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new QueryGroup( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.enemyData;
			selectedGroup = Utils.enemyData.First( x => x.id == "DG001" );

			dpCBAlly.ItemsSource = Utils.allyRebelHeroData;
			selectedAllyGroup = Utils.allyRebelHeroData.First( x => x.id == "A001" );

			triggersCB.ItemsSource = Utils.mainWindow.allMissionTriggers;
			eventsCB.ItemsSource = Utils.mainWindow.allMissionEvents;

			var g = (eventAction as QueryGroup).groupEnemyToQuery;
			var a = (eventAction as QueryGroup).groupRebelToQuery;
			resultText.Text = "No Group Selected";
			if ( (eventAction as QueryGroup).groupEnemyToQuery != null )
				resultText.Text = $"Query: {g.name} [{g.id}]";
			else if ( (eventAction as QueryGroup).groupRebelToQuery != null )
				resultText.Text = $"Query: {a.name} [{a.id}]";
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void addGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as QueryGroup).groupEnemyToQuery = new() { id = selectedGroup.id, name = selectedGroup.name };
			(eventAction as QueryGroup).groupRebelToQuery = null;

			resultText.Text = $"Query: {selectedGroup.name} [{selectedGroup.id}]";
		}

		private void addGroupButtonAlly_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as QueryGroup).groupEnemyToQuery = null;
			(eventAction as QueryGroup).groupRebelToQuery = new() { id = selectedAllyGroup.id, name = selectedAllyGroup.name };

			resultText.Text = $"Query: {selectedAllyGroup.name} [{selectedAllyGroup.id}]";
		}

		private void filterBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				if ( selectedGroup != null )
				{
					addGroupButton_Click( null, null );
					filterBox.Text = "";
				}
			}
		}

		private void TextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			selectedGroup = Utils.enemyData.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );

			//try id
			if ( selectedGroup == null )
				selectedGroup = Utils.enemyData.Where( x => x.id.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );
		}

		private void filterBoxA_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				if ( selectedAllyGroup != null )
				{
					addGroupButtonAlly_Click( null, null );
					filterBoxA.Text = "";
				}
			}
		}

		private void filterBoxA_TextChanged( object sender, TextChangedEventArgs e )
		{
			selectedAllyGroup = Utils.allyRebelHeroData.Where( x => x.name.ToLower().Contains( filterBoxA.Text.ToLower() ) ).FirstOr( null );

			//try id
			if ( selectedAllyGroup == null )
				selectedAllyGroup = Utils.allyRebelHeroData.Where( x => x.id.ToLower().Contains( filterBoxA.Text.ToLower() ) ).FirstOr( null );
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( t != null )
			{
				triggersCB.ItemsSource = Utils.mainWindow.allMissionTriggers;
				(eventAction as QueryGroup).foundTrigger = t.GUID;
			}
		}

		private void addNewEventButton_Click( object sender, RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null )
			{
				eventsCB.ItemsSource = Utils.mainWindow.allMissionEvents;
				(eventAction as QueryGroup).foundEvent = me.GUID;
			}
		}
	}
}
