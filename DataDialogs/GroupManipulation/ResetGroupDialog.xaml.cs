using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ResetGroupDialog.xaml
	/// </summary>
	public partial class ResetGroupDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		public string selectedGroup { get; set; }
		public IEventAction eventAction { get; set; }

		public ResetGroupDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ResetGroup( dname, et );
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
			(eventAction as ResetGroup).groupsToAdd.Add( new( Utils.enemyData.Where( x => x.id == selectedGroup ).First() ) );
		}

		private void remGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeInstructions).groupsToAdd.Remove( (sender as FrameworkElement).DataContext as DCPointer );
		}
	}
}
