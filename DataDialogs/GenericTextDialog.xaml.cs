using System.ComponentModel;
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
			Close();
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			textbox.Focus();
			textbox.Select( textbox.Text.Length, 0 );
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}
	}
}
