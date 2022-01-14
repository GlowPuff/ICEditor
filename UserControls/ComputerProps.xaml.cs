using System.Windows.Controls;

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
	}
}
