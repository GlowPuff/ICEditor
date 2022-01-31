using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for SymbolsWindow.xaml
	/// </summary>
	public partial class SymbolsWindow : Window
	{
		public SymbolsWindow()
		{
			InitializeComponent();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}
	}
}
