using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
  /// <summary>
  /// Interaction logic for EditInitialGroupDialog.xaml
  /// </summary>
  public partial class EditInitialGroupDialog : Window
  {
    public InitialGroupData initialGroupData { get; set; }
    public DeploymentPoint selectedPoint { get; set; }
    public Guid selectedGUID { get; set; }

    public List<DeploymentPoint> deploymentPoints
    {
      get { return Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>().ToList(); }
    }

    public EditInitialGroupDialog( InitialGroupData data )
    {
      InitializeComponent();
      DataContext = this;

      initialGroupData = data;
    }

    private void okButton_Click( object sender, RoutedEventArgs e )
    {
      Close();
    }

    private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
    {
      DragMove();
    }
  }
}
