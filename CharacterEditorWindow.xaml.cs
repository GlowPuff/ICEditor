using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for CharacterEditorWindow.xaml
	/// </summary>
	public partial class CharacterEditorWindow : Window
	{
		public CharacterEditorWindow()
		{
			InitializeComponent();
		}

		private void Window_PreviewKeyDown( object sender, KeyEventArgs e )
		{

		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_Closed( object sender, System.EventArgs e )
		{
			StartupWindow sw = new();
			sw.Show();
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			toonEditor.SetStandalone();
		}
	}
}
