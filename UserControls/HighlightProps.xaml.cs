using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
  /// <summary>
  /// Interaction logic for HighlightProps.xaml
  /// </summary>
  public partial class HighlightProps : UserControl, IPropertyModel
  {
    public List<DeploymentColor> deploymentColors { get { return Utils.deploymentColors.ToList(); } }

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
  }
}
