using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for FormattingWindow.xaml
	/// </summary>
	public partial class FormattingWindow : Window
	{
		public FormattingWindow()
		{
			InitializeComponent();
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
	}
}
