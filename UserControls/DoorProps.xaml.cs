using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for DoorProps.xaml
	/// </summary>
	public partial class DoorProps : UserControl, IPropertyModel
	{
		public DoorProps()
		{
			InitializeComponent();
		}

		private void editPropsBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			EditEntityProperties dlg = new( (DataContext as Door).entityProperties );
			dlg.ShowDialog();
		}
	}
}
