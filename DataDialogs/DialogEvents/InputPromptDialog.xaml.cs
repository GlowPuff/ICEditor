using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for InputPromptDialog.xaml
	/// </summary>
	public partial class InputPromptDialog : Window, IEventActionDialog
	{
		SymbolsWindow symbolsWindow;
		FormattingWindow formattingWindow;

		public static MainWindow mainWindow { get { return Utils.mainWindow; } }
		public IEventAction eventAction { get; set; }

		public InputPromptDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new InputPrompt( dname, et );
			DataContext = eventAction;

			//validate eventAction's button triggers and events (GUID) exist
			List<string> strings = new();
			for ( int i = 0; i < (eventAction as InputPrompt).inputList.Count; i++ )
			{
				if ( !Utils.ValidateEvent( (eventAction as InputPrompt).inputList[i].eventGUID ) )
				{
					strings.Add( $"Missing Input Range Event, From {(eventAction as InputPrompt).inputList[i].fromValue} to {(eventAction as InputPrompt).inputList[i].toValue}" );
				}
				if ( !Utils.ValidateTrigger( (eventAction as InputPrompt).inputList[i].triggerGUID ) )
				{
					strings.Add( $"Missing Input Range Trigger, From {(eventAction as InputPrompt).inputList[i].fromValue} to {(eventAction as InputPrompt).inputList[i].toValue}" );
				}
				//if ( !Utils.ValidateTrigger( (eventAction as InputPrompt).inputList[i].triggerGUID ) )
				//	(eventAction as InputPrompt).inputList[i].triggerGUID = Guid.Empty;
				//if ( !Utils.ValidateEvent( (eventAction as InputPrompt).inputList[i].eventGUID ) )
				//	(eventAction as InputPrompt).inputList[i].eventGUID = Guid.Empty;
			}
			if ( strings.Count > 0 )
				MessageBox.Show( $"This Event Action is referencing Events and/or Triggers that no longer exist in the Mission.\n\n{string.Join( "\n", strings )}", "Missing Reference(s) Found" );
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.LeftButton == MouseButtonState.Pressed )
				DragMove();
		}

		private void Window_ContentRendered( object sender, EventArgs e )
		{
			textbox.Focus();
			textbox.SelectAll();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			symbolsWindow?.Close();
			formattingWindow?.Close();
			Close();
		}

		private void remInputButton_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as InputPrompt).inputList.Remove( ((FrameworkElement)sender).DataContext as InputRange );
		}

		private void formatBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( !IsWindowOpen<FormattingWindow>() )
			{
				formattingWindow = new FormattingWindow();
				formattingWindow.Show();
			}
		}

		private void infoBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( !IsWindowOpen<SymbolsWindow>() )
			{
				symbolsWindow = new SymbolsWindow();
				symbolsWindow.Show();
			}
		}

		private static bool IsWindowOpen<T>( string name = "" ) where T : Window
		{
			return string.IsNullOrEmpty( name )
				 ? Application.Current.Windows.OfType<T>().Any()
				 : Application.Current.Windows.OfType<T>().Any( w => w.Name.Equals( name ) );
		}

		private void editBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new EditInputPrompt( (sender as FrameworkElement).DataContext as InputRange );
			dlg.ShowDialog();
			triggersCB.ItemsSource = mainWindow.localTriggers;
			eventsCB.ItemsSource = mainWindow.localEvents;
		}

		private void editFailBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "Edit Failure Text", (eventAction as InputPrompt).failText );
			dlg.ShowDialog();
			(eventAction as InputPrompt).failText = dlg.theText;
		}

		private void addNewRangeBtn_Click( object sender, RoutedEventArgs e )
		{
			InputRange ir = new InputRange();
			(eventAction as InputPrompt).inputList.Add( ir );
		}
	}
}
