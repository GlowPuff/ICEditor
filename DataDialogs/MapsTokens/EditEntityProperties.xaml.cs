using System;
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

			//verify triggers/events still exist
			entityProperties.ValidateTriggers();
			entityProperties.ValidateEvents();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void remQuestionButton_Click( object sender, RoutedEventArgs e )
		{
			entityProperties.buttonActions.Remove( (sender as FrameworkElement).DataContext as ButtonAction );
		}

		private void addEventBtn_Click( object sender, RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null && entityProperties.buttonActions.Count < 5 )
			{
				entityProperties.buttonActions.Add( new() { buttonText = "Button Text", triggerGUID = Guid.Empty, eventGUID = me.GUID } );
			}
		}

		private void addTriggerBtn_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( t != null && entityProperties.buttonActions.Count < 5 )
			{
				entityProperties.buttonActions.Add( new() { buttonText = "Button Text", triggerGUID = t.GUID, eventGUID = Guid.Empty } );
			}
		}

		private void addButtonBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( entityProperties.buttonActions.Count < 5 )
			{
				entityProperties.buttonActions.Add( new() { buttonText = "Button Text", triggerGUID = Guid.Empty, eventGUID = Guid.Empty } );
			}
		}

		private void btnText_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
			}
		}
	}
}
