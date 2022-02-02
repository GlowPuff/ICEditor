using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ModifyThreatDialog.xaml
	/// </summary>
	public partial class ModifyThreatDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }
		public MainWindow mainWindow { get { return Utils.mainWindow; } }

		public ModifyThreatDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ModifyThreat( dname, et );
			DataContext = this;
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}
	}
}
