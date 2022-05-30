using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EditInputPrompt.xaml
	/// </summary>
	public partial class EditInputPrompt : Window
	{
		public static MainWindow mainWindow { get { return Utils.mainWindow; } }
		public InputRange inputRange { get; set; }

		public EditInputPrompt( InputRange irange )
		{
			InitializeComponent();

			inputRange = irange;
			DataContext = inputRange;
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void addTriggerBtn_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( t != null )
			{
				triggersCB.ItemsSource = mainWindow.localTriggers;
				inputRange.triggerGUID = t.GUID;
			}
		}

		private void addEventBtn_Click( object sender, RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null )
			{
				eventsCB.ItemsSource = mainWindow.localEvents;
				inputRange.eventGUID = me.GUID;
			}
		}

		private void TextBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
			}
		}
	}
}
