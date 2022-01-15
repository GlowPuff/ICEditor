using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
  /// <summary>
  /// Interaction logic for DeploymentProps.xaml
  /// </summary>
  public partial class DeploymentProps : UserControl, IPropertyModel
  {
    public List<DeploymentColor> deploymentColors { get { return Utils.deploymentColors.ToList(); } }

    public DeploymentProps()
    {
      InitializeComponent();
    }
  }
}
