using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for HighlightProps.xaml
	/// </summary>
	public partial class HighlightProps : UserControl, IPropertyModel
	{
		public ObservableCollection<DeploymentColor> deploymentColors
		{
			get { return Utils.deploymentColors; }
		}

		public HighlightProps()
		{
			InitializeComponent();
		}

		private void editPropsBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			EditEntityProperties dlg = new( (DataContext as SpaceHighlight).entityProperties );
			dlg.ShowDialog();
		}

		private void applyBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			(DataContext as SpaceHighlight).Rebuild();
		}

		private void dupeBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dupe = (DataContext as SpaceHighlight).Duplicate();
			Utils.mainWindow.mapEditor.InsertDuplicateEntity( dupe );
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
			}
		}
	}
}
