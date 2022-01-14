using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for TokenProps.xaml
	/// </summary>
	public partial class TokenProps : UserControl, IPropertyModel
	{
		public TokenProps()
		{
			InitializeComponent();
		}

		private void editPropsBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			EditEntityProperties dlg = new( (DataContext as Token).entityProperties );
		}
	}
}
