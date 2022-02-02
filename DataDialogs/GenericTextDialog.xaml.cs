using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for GenericTextDialog.xaml
	/// </summary>
	public partial class GenericTextDialog : Window, INotifyPropertyChanged
	{
		string _theText, _header, _textHint;
		SymbolsWindow symbolsWindow;
		FormattingWindow formattingWindow;

		public Mission mission { get; set; }
		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public string header { get { return _header; } set { _header = value; PC(); } }
		public string textHint { get { return _textHint; } set { _textHint = value; PC(); } }

		public event PropertyChangedEventHandler PropertyChanged;

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public GenericTextDialog( string title, string text )
		{
			InitializeComponent();

			DataContext = this;
			header = title;
			theText = text;
			textHint = "";
		}

		private void clearButton_Click( object sender, RoutedEventArgs e )
		{
			theText = "";
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			symbolsWindow?.Close();
			formattingWindow?.Close();
			Close();
		}

		private void infoBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( !IsWindowOpen<SymbolsWindow>() )
			{
				symbolsWindow = new SymbolsWindow();
				symbolsWindow.Show();
			}
		}

		private void formatBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( !IsWindowOpen<FormattingWindow>() )
			{
				formattingWindow = new FormattingWindow();
				formattingWindow.Show();
			}
		}

		private static bool IsWindowOpen<T>( string name = "" ) where T : Window
		{
			return string.IsNullOrEmpty( name )
				 ? Application.Current.Windows.OfType<T>().Any()
				 : Application.Current.Windows.OfType<T>().Any( w => w.Name.Equals( name ) );
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			textbox.Focus();
			textbox.Select( textbox.Text.Length, 0 );
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}
	}
}
