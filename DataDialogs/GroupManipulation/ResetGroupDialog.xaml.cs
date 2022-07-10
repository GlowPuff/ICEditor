using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ResetGroupDialog.xaml
	/// </summary>
	public partial class ResetGroupDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		DeploymentCard _selectedGroup;

		public DeploymentCard selectedGroup { get { return _selectedGroup; } set { _selectedGroup = value; PC(); } }
		public IEventAction eventAction { get; set; }

		public ResetGroupDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ResetGroup( dname, et );
			DataContext = eventAction;

			dpCB.ItemsSource = Utils.enemyData;
			selectedGroup = Utils.enemyData.First( x => x.id == "DG001" );
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

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
			(eventAction as ResetGroup).groupsToAdd.Add( new( Utils.enemyData.Where( x => x.id == selectedGroup.id ).First() ) );
		}

		private void remGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ResetGroup).groupsToAdd.Remove( (sender as FrameworkElement).DataContext as DCPointer );
		}

		private void TextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			selectedGroup = Utils.enemyData.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );

			//try id
			if ( selectedGroup == null )
				selectedGroup = Utils.enemyData.Where( x => x.id.Contains( filterBox.Text ) ).FirstOr( null );
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
	}
}
