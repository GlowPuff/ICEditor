using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for QuestionPromptDialog.xaml
	/// </summary>
	public partial class QuestionPromptDialog : Window, IEventActionDialog
	{
		private bool dirtyList, dirtyList2;

		public IEventAction eventAction { get; set; }
		public static MainWindow mainWindow { get { return Utils.mainWindow; } }
		public Trigger selectedTrigger { get; set; }

		public QuestionPromptDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new QuestionPrompt( dname, et );
			DataContext = this;

			selectedTrigger = mainWindow.localTriggers[0];

			//validate eventAction's buttonList.trigger (GUID) exists
			for ( int i = 0; i < (eventAction as QuestionPrompt).buttonList.Count; i++ )
			{
				if ( !Utils.ValidateTrigger( (eventAction as QuestionPrompt).buttonList[i].triggerGUID ) )
					(eventAction as QuestionPrompt).buttonList[i].triggerGUID = Guid.Empty;
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
			(eventAction as QuestionPrompt).buttonList.Remove( (sender as FrameworkElement).DataContext as Question );
		}

		private void addTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			if ( (eventAction as QuestionPrompt).buttonList.Count < 5 )
				(eventAction as QuestionPrompt).buttonList.Add( new() { buttonText = "Button Text", triggerGUID = selectedTrigger.GUID } );
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			dirtyList = dirtyList2 = true;
			if ( t != null && (eventAction as QuestionPrompt).buttonList.Count < 5 )
			{
				(eventAction as QuestionPrompt).buttonList.Add( new() { buttonText = "Button Text", triggerGUID = t.GUID } );
			}
		}

		private void tlist_GotFocus( object sender, RoutedEventArgs e )
		{
			if ( dirtyList )
			{
				dirtyList = false;
				tlist.ItemsSource = Utils.mainWindow.localTriggers;
			}
		}
		private void triggersCB_GotFocus( object sender, RoutedEventArgs e )
		{
			if ( dirtyList2 )
			{
				dirtyList2 = false;
				(sender as ComboBox).ItemsSource = Utils.mainWindow.localTriggers;
			}
		}
	}
}
