using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for CampaignPackager.xaml
	/// </summary>
	public partial class CampaignPackager : Window
	{
		public CampaignPackager()
		{
			InitializeComponent();
		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_Closed( object sender, System.ComponentModel.CancelEventArgs e )
		{
			StartupWindow sw = new();
			sw.Show();
		}

		private void loadButton_Click( object sender, RoutedEventArgs e )
		{

		}

		private void packageButton_Click( object sender, RoutedEventArgs e )
		{

		}
	}
}
