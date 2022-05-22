using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

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
			var section = ((Button)sender).DataContext as MapSection;
			//sanity check
			if ( parent.mission.mapSections.IndexOf( section ) == 0 )
				return;//can't remove default section
			var dlg = new ConfirmSectionRemoveDialog( section.name );
			bool result = dlg.ShowDialog().Value;

			//var res = MessageBox.Show( $"Are you sure you want to remove this Map Section?\r\n\r\n{(((Button)sender).DataContext as MapSection).name}", "Remove Map Section", MessageBoxButton.YesNo, MessageBoxImage.Warning );
			if ( result )//res == MessageBoxResult.Yes )
			{
				var m = ((Button)sender).DataContext as MapSection;
				if ( m.GUID != System.Guid.Empty )
				{
					if ( dlg.removeChildren )
						Utils.RemoveMapSectionObjects( m );
					else//otherwise set objects' owner to default map section
						Utils.SetOwnerToDefaultSection( m );
					parent.activeSection = parent.mission.mapSections[0];
					parent.mission.mapSections.Remove( m );
				}
			}
		}
	}
}
