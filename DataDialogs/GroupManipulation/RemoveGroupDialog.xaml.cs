using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for RemoveGroupDialog.xaml
	/// </summary>
	public partial class RemoveGroupDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		DeploymentCard _selectedGroup, _selectedAllyGroup;
		public DeploymentCard selectedGroup { get { return _selectedGroup; } set { _selectedGroup = value; PC(); } }
		public DeploymentCard selectedAllyGroup { get { return _selectedAllyGroup; } set { _selectedAllyGroup = value; PC(); } }
		public IEventAction eventAction { get; set; }

		public RemoveGroupDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new RemoveGroup( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.enemyData;
			selectedGroup = Utils.enemyData.First( x => x.id == "DG001" );

			dpCBAlly.ItemsSource = Utils.allyData;
			selectedAllyGroup = Utils.allyData.First( x => x.id == "A001" );
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

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

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == MouseButtonState.Pressed )
				DragMove();
		}

		private void TextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			selectedGroup = Utils.enemyData.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );

			//try id
			if ( selectedGroup == null )
				selectedGroup = Utils.enemyData.Where( x => x.id.Contains( filterBox.Text ) ).FirstOr( null );
		}

		private void filterBoxA_TextChanged( object sender, TextChangedEventArgs e )
		{
			selectedAllyGroup = Utils.allyData.Where( x => x.name.ToLower().Contains( filterBoxA.Text.ToLower() ) ).FirstOr( null );

			//try id
			if ( selectedAllyGroup == null )
				selectedAllyGroup = Utils.allyData.Where( x => x.id.Contains( filterBoxA.Text ) ).FirstOr( null );
		}

		private void remGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as RemoveGroup).groupsToRemove.Remove( (sender as FrameworkElement).DataContext as DCPointer );
		}

		private void addGroupButtonAlly_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as RemoveGroup).allyGroupsToRemove.Add( new( Utils.allyData.Where( x => x.id == selectedAllyGroup.id ).First() ) );
		}

		private void remAllyGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as RemoveGroup).allyGroupsToRemove.Remove( (sender as FrameworkElement).DataContext as DCPointer );
		}

		private void addGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as RemoveGroup).groupsToRemove.Add( new( Utils.enemyData.Where( x => x.id == selectedGroup.id ).First() ) );
		}
	}
}
