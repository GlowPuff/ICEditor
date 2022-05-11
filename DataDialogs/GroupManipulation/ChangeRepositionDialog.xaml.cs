using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ChangeRepositionDialog.xaml
	/// </summary>
	public partial class ChangeRepositionDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }

		SymbolsWindow symbolsWindow;
		FormattingWindow formattingWindow;

		public ChangeRepositionDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ChangeReposition( dname, et );
			DataContext = this;

			dpCB.ItemsSource = Utils.enemyData;
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

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			textbox.Focus();
			textbox.Select( textbox.Text.Length, 0 );
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
	}
}
