using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ComputerProps.xaml
	/// </summary>
	public partial class ComputerProps : UserControl, IPropertyModel
	{
		public ComputerProps()
		{
			InitializeComponent();
		}

		private void editPropsBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			EditEntityProperties dlg = new( (DataContext as Console).entityProperties );
			dlg.ShowDialog();
		}

		private void dupeBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dupe = (DataContext as Console).Duplicate();
			Utils.mainWindow.mapEditor.InsertDuplicateEntity( dupe );
		}

		private void TextBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
			}
		}
	}
}
