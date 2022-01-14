using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EditEntityProperties.xaml
	/// </summary>
	public partial class EditEntityProperties : Window
	{
		public EntityProperties entityProperties { get; set; }
		public static MainWindow mainWindow { get { return Utils.mainWindow; } }
		public Trigger selectedTrigger { get; set; }

		public EditEntityProperties( EntityProperties ep )
		{
			InitializeComponent();
			DataContext = this;

			entityProperties = ep;
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}

		private void triggersCB_GotFocus( object sender, RoutedEventArgs e )
		{
			(sender as ComboBox).GotFocus -= triggersCB_GotFocus;
			(sender as ComboBox).ItemsSource = Utils.mainWindow.localTriggers;
			(sender as ComboBox).GotFocus += triggersCB_GotFocus;
		}

		private void remQuestionButton_Click( object sender, RoutedEventArgs e )
		{
			entityProperties.buttonActions.Remove( (sender as FrameworkElement).DataContext as ButtonAction );
		}

		private void addTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedTrigger != null /*&& !entityProperties.buttonActions.Any( x => x.triggerGUID == selectedTrigger.GUID ) */)
				entityProperties.buttonActions.Add( new() { triggerGUID = selectedTrigger.GUID, triggerName = selectedTrigger.name } );
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( t != null )
				entityProperties.buttonActions.Add( new() { triggerGUID = t.GUID, triggerName = t.name } );

			//refresh comboboxes with updated trigger list collection
			tlist.ItemsSource = Utils.mainWindow.localTriggers;
		}
	}
}
