using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for AddGroupDialog.xaml
	/// </summary>
	public partial class AddGroupDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		string _selectedGroup;
		public IEventAction eventAction { get; set; }
		public string selectedGroup { get { return _selectedGroup; } set { _selectedGroup = value; PC(); } }

		public AddGroupDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new AddGroupDeployment( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.enemyData;
			selectedGroup = "DG001";
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		private void remGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as AddGroupDeployment).groupsToAdd.Remove( (sender as FrameworkElement).DataContext as DeploymentCard );
		}

		private void addGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as AddGroupDeployment).groupsToAdd.Add( Utils.enemyData.Where( x => x.id == selectedGroup ).First() );
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

		private void TextBox_TextChanged( object sender, System.Windows.Controls.TextChangedEventArgs e )
		{
			var s = Utils.enemyData.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );
			if ( s != null )
				selectedGroup = s.id;
			else
				selectedGroup = null;

			//try id
			if ( s == null )
			{
				var ss = Utils.enemyData.Where( x => x.id.Contains( filterBox.Text ) ).FirstOr( null );
				if ( ss != null )
					selectedGroup = ss.id;
				else
					selectedGroup = null;
			}
		}

		private void filterBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				if ( !string.IsNullOrEmpty( selectedGroup ) )
				{
					addGroupButton_Click( null, null );
					filterBox.Text = "";
				}
			}
		}
	}
}
