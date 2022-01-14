using MaterialDesignThemes.Wpf;
using System.Windows;

namespace Imperial_Commander_Editor
{
  /// <summary>
  /// Interaction logic for CampaignEditor.xaml
  /// </summary>
  public partial class CampaignEditor : Window
  {
    public CampaignEditor()
    {
      InitializeComponent();

      var paletteHelper = new PaletteHelper();
      var theme = paletteHelper.GetTheme();
      theme.SetBaseTheme( Theme.Dark );
      paletteHelper.SetTheme( theme );
    }

    private void closeBtn_Click( object sender, RoutedEventArgs e )
    {
      var s = new StartupWindow();
      s.Show();
      Close();
    }
  }
}
