using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
			DeploymentCard dc;
			dc = Utils.enemyData.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );

			//try id
			if ( dc == null )
				dc = Utils.enemyData.Where( x => x.id.Contains( filterBox.Text ) ).FirstOr( null );

			if ( dc != null )
				(eventAction as ChangeReposition).groupID = dc.id;
			else
				(eventAction as ChangeReposition).groupID = null;
		}

		private void filterBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				filterBox.TextChanged -= TextBox_TextChanged;
				filterBox.Text = "";
				filterBox.TextChanged += TextBox_TextChanged;
				Utils.LoseFocus( sender as Control );
			}
		}
	}
}
