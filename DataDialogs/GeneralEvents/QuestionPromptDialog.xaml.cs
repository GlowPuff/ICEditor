using System;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for QuestionPromptDialog.xaml
	/// </summary>
	public partial class QuestionPromptDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }
		public static MainWindow mainWindow { get { return Utils.mainWindow; } }

		public QuestionPromptDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new QuestionPrompt( dname, et );
			DataContext = this;

			//validate eventAction's button triggers and events (GUID) exist
			for ( int i = 0; i < (eventAction as QuestionPrompt).buttonList.Count; i++ )
			{
				if ( !Utils.ValidateTrigger( (eventAction as QuestionPrompt).buttonList[i].triggerGUID ) )
					(eventAction as QuestionPrompt).buttonList[i].triggerGUID = Guid.Empty;
				if ( !Utils.ValidateEvent( (eventAction as QuestionPrompt).buttonList[i].eventGUID ) )
					(eventAction as QuestionPrompt).buttonList[i].eventGUID = Guid.Empty;
			}
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void Window_ContentRendered( object sender, EventArgs e )
		{
			textbox.Focus();
			textbox.SelectAll();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void remQuestionButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as QuestionPrompt).buttonList.Remove( (sender as FrameworkElement).DataContext as ButtonAction );
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( t != null && (eventAction as QuestionPrompt).buttonList.Count < 5 )
			{
				(eventAction as QuestionPrompt).buttonList.Add( new() { buttonText = "Button Text", triggerGUID = t.GUID, eventGUID = Guid.Empty } );
			}
		}

		private void addButtonBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( (eventAction as QuestionPrompt).buttonList.Count < 5 )
			{
				(eventAction as QuestionPrompt).buttonList.Add( new() { buttonText = "Button Text", triggerGUID = Guid.Empty, eventGUID = Guid.Empty } );
			}
		}

		private void addEventBtn_Click( object sender, RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null && (eventAction as QuestionPrompt).buttonList.Count < 5 )
			{
				(eventAction as QuestionPrompt).buttonList.Add( new() { buttonText = "Button Text", triggerGUID = Guid.Empty, eventGUID = me.GUID } );
			}
		}
	}
}
