using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ChangeRepositionDialog.xaml
	/// </summary>
	public partial class ChangeRepositionDialog : Window, IEventActionDialog, INotifyPropertyChanged
	{
		//DeploymentCard _selectedGroupAdd;

		//public DeploymentCard selectedGroupAdd { get { return _selectedGroupAdd; } set { _selectedGroupAdd = value; PC(); } }
		string _selectedGroup;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public IEventAction eventAction { get; set; }
		public string selectedGroup { get { return _selectedGroup; } set { _selectedGroup = value; PC(); } }

		SymbolsWindow symbolsWindow;
		FormattingWindow formattingWindow;

		public event PropertyChangedEventHandler PropertyChanged;

		public ChangeRepositionDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ChangeReposition( dname, et );
			DataContext = this;

			dpCB.ItemsSource = Utils.enemyData;
			selectedGroup = "DG001";
			//selectedGroupAdd = Utils.enemyData.First( x => x.id == "DG001" );
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			symbolsWindow?.Close();
			formattingWindow?.Close();
			Close();
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			textbox.Focus();
			textbox.SelectAll();
		}
		private void formatBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( !IsWindowOpen<FormattingWindow>() )
			{
				formattingWindow = new FormattingWindow();
				formattingWindow.Show();
			}
		}
		private void infoBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( !IsWindowOpen<SymbolsWindow>() )
			{
				symbolsWindow = new SymbolsWindow();
				symbolsWindow.Show();
			}
		}

		private static bool IsWindowOpen<T>( string name = "" ) where T : Window
		{
			return string.IsNullOrEmpty( name )
				 ? Application.Current.Windows.OfType<T>().Any()
				 : Application.Current.Windows.OfType<T>().Any( w => w.Name.Equals( name ) );
		}

		private void clearButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeReposition).theText = "";
		}

		private void TextBox_TextChanged( object sender, System.Windows.Controls.TextChangedEventArgs e )
		{
			DeploymentCard selectedGroupAdd;
			selectedGroupAdd = Utils.enemyData.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );

			//try id
			if ( selectedGroupAdd == null )
				selectedGroupAdd = Utils.enemyData.Where( x => x.id.Contains( filterBox.Text ) ).FirstOr( null );

			if ( selectedGroupAdd != null )
				selectedGroup = selectedGroupAdd.id;
			//	(eventAction as ChangeReposition).groupID = dc.id;
			//else
			//	(eventAction as ChangeReposition).groupID = null;
		}

		private void filterBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				if ( selectedGroup != null )
					addGroupBtn_Click( null, null );
				selectedGroup = "DG001";
				filterBox.TextChanged -= TextBox_TextChanged;
				filterBox.Text = "";
				filterBox.TextChanged += TextBox_TextChanged;
				Utils.LoseFocus( sender as Control );
			}
		}

		private void addGroupBtn_Click( object sender, RoutedEventArgs e )
		{
			var dc = Utils.enemyData.Where( x => x.id.Contains( selectedGroup ) ).FirstOr( null );
			if ( dc != null )
				(eventAction as ChangeReposition).repoGroups.Add( new() { id = dc.id, name = dc.name } );
		}

		private void remGroupButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ChangeReposition).repoGroups.Remove( (sender as FrameworkElement).DataContext as DCPointer );
		}
	}
}
