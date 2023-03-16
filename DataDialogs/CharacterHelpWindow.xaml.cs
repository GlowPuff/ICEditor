using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for CharacterHelpWindow.xaml
	/// </summary>
	public partial class CharacterHelpWindow : Window
	{
		public CharacterHelpWindow( bool isStandalone )
		{
			InitializeComponent();

			standaloneMessage.Text = isStandalone ? "You are using the standalone Character Editor. Characters you create must be EXPORTED for use by players in their game. Exported Characters can also be Imported into the Mission Editor for use in your Custom Missions." : "You are using the in-Mission Character Editor. Characters you create are saved inside the Mission file and do not need to be Exported, unless you also want the Character available to players outside of this Mission.";
		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}
	}
}
