using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for TileProps.xaml
	/// </summary>
	public partial class TileProps : UserControl, IPropertyModel
	{
		public TileProps()
		{
			InitializeComponent();
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void ownerChangeBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			MapTile tile = (sender as FrameworkElement).DataContext as MapTile;
			tile.mapSectionOwner = Utils.mainWindow.activeSection.GUID;
			tile.ownerName = Utils.mainWindow.mission.mapSections.First( x => x.GUID == tile.mapSectionOwner ).name;
			Utils.mainWindow.SetStatus( $"Owner Set To '{Utils.mainWindow.activeSection.name}'" );
		}
	}
}
