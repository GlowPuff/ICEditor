using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for MultipleBannedAlliesDialog.xaml
	/// </summary>
	public partial class MultipleBannedAlliesDialog : Window
	{
		public MultipleBannedAlliesDialog()
		{
			InitializeComponent();

			DataContext = this;

			//List<DeploymentCard> alist = new();
			//foreach ( var item in Utils.allyData )
			//{
			//	alist.Add( $"{item.name} [{item.id}]" );
			//}

			allyItems.ItemsSource = Utils.allyData.Select( x => new { name = $"{x.name} ({x.id})", id = x.id } );
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Utils.mainWindow.mission.missionProperties.multipleBannedAllies.Clear();

			for ( int i = 0; i < allyItems.Items.Count; i++ )
			{
				var pres = (ContentPresenter)allyItems.ItemContainerGenerator.ContainerFromIndex( i );
				CheckBox cb = (CheckBox)VisualTreeHelper.GetChild( pres, 0 );
				//var grid = (Grid)VisualTreeHelper.GetChild( pres, 0 );
				//CheckBox cb = grid.Children[0] as CheckBox;
				if ( cb.IsChecked == true )
					Utils.mainWindow.mission.missionProperties.multipleBannedAllies.Add( Utils.allyData[i].id );
			}

			DialogResult = true;
			Close();
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			for ( int i = 0; i < Utils.allyData.Count; i++ )
			{
				var pres = (ContentPresenter)allyItems.ItemContainerGenerator.ContainerFromIndex( i );
				CheckBox cb = (CheckBox)VisualTreeHelper.GetChild( pres, 0 );
				//var grid = (Grid)VisualTreeHelper.GetChild( pres, 0 );
				//CheckBox cb = grid.Children[0] as CheckBox;
				cb.IsChecked = Utils.mainWindow.mission.missionProperties.multipleBannedAllies.Contains( Utils.allyData[i].id );
			}
		}
	}
}
