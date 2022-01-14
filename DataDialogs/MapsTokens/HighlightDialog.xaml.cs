using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for HighlightDialog.xaml
	/// </summary>
	public partial class HighlightDialog : Window, IEventActionDialog
	{
		public IEventAction eventAction { get; set; }
		public SpaceHighlight selectedSpace { get; set; }
		public List<SpaceHighlight> highlightList { get; set; } = new();
		public ObservableCollection<SpaceHighlight> addedHighlights { get; set; } = new();

		public HighlightDialog( string dname, EventActionType et, IEventAction ea = null )
		{
			InitializeComponent();

			eventAction = ea ?? new ModifyHighlight( dname, et );
			DataContext = eventAction;

			highlightList = Utils.mainWindow.mission.mapEntities.OfType<SpaceHighlight>().ToList();
		}

		private void Window_MouseDown( object sender, MouseButtonEventArgs e )
		{
			DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void remSpaceBtn_Click( object sender, RoutedEventArgs e )
		{
			(eventAction as ModifyHighlight).highlightList.Remove( ((sender as FrameworkElement).DataContext as SpaceHighlight).GUID );
			for ( int i = addedHighlights.Count - 1; i >= 0; i-- )
			{
				if ( addedHighlights[i].GUID == ((SpaceHighlight)(sender as FrameworkElement).DataContext).GUID )
					addedHighlights.RemoveAt( i );
			}
		}

		private void addTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedSpace != null && !(eventAction as ModifyHighlight).highlightList.Contains( selectedSpace.GUID ) )
			{
				(eventAction as ModifyHighlight).highlightList.Add( selectedSpace.GUID );
				addedHighlights.Add( selectedSpace );
			}
		}
	}
}
