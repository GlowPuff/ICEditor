using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for MapSections.xaml
	/// </summary>
	public partial class MapSections : UserControl
	{
		MainWindow parent
		{
			get
			{
				return (MainWindow)Window.GetWindow( this );
			}
		}

		public MapSections()
		{
			InitializeComponent();
		}

		private void editSection_Click( object sender, RoutedEventArgs e )
		{
			parent.onEditSection( ((Button)sender).DataContext as MapSection );
		}

		private void editMap_Click( object sender, RoutedEventArgs e )
		{
			parent.onEditMap( ((Button)sender).DataContext as MapSection );
		}

		private void Card_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			parent.activeSection = ((Card)sender).DataContext as MapSection;
		}

		private void removeButton_Click( object sender, RoutedEventArgs e )
		{
			var m = ((Button)sender).DataContext as MapSection;
			if ( m.GUID != System.Guid.Empty )
			{
				parent.activeSection = parent.mission.mapSections[0];
				parent.mission.mapSections.Remove( m );
			}
		}
	}
}
