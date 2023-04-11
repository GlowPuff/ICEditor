using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for PriorityTraitsDialog.xaml
	/// </summary>
	public partial class PriorityTraitsDialog : Window
	{
		public GroupPriorityTraits priorityTraits;

		public PriorityTraitsDialog( GroupPriorityTraits traits, bool hideUseDefault = false )
		{
			InitializeComponent();

			priorityTraits = traits;
			DataContext = priorityTraits;

			if ( hideUseDefault )
				useDefaultCard.Visibility = Visibility.Collapsed;
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void checkBtn_Click( object sender, RoutedEventArgs e )
		{
			priorityTraits.CheckAll();
		}

		private void clearBtn_Click( object sender, RoutedEventArgs e )
		{
			priorityTraits.ClearAll();
		}
	}
}
