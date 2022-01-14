using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for CrateProps.xaml
	/// </summary>
	public partial class CrateProps : UserControl, IPropertyModel
	{
		public CrateProps()
		{
			InitializeComponent();
		}

		private void editPropsBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			EditEntityProperties dlg = new( (DataContext as Crate).entityProperties );
			dlg.ShowDialog();
		}
	}
}
