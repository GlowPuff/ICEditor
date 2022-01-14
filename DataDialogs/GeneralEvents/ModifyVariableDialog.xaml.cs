using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
  /// <summary>
  /// Interaction logic for ModifyVariableDialog.xaml
  /// </summary>
  public partial class ModifyVariableDialog : Window, IEventActionDialog
  {
    public static MainWindow mainWindow { get { return Utils.mainWindow; } }
    public IEventAction eventAction { get; set; }
    public Trigger selectedTrigger { get; set; }
    public ObservableCollection<Trigger> selectedTriggers { get; set; } = new();

    public ModifyVariableDialog( string dname, EventActionType et, IEventAction ea = null )
    {
      InitializeComponent();

      eventAction = ea ?? new ModifyVariable( dname, et );
      DataContext = this;

      //validate triggers
      for ( int i = (eventAction as ModifyVariable).triggerList.Count - 1; i >= 0; i-- )
      {
        if ( !Utils.mainWindow.mission.TriggerExists( (eventAction as ModifyVariable).triggerList[i] ) )
          (eventAction as ModifyVariable).triggerList[i] = Guid.Empty;
      }

      foreach ( var item in (eventAction as ModifyVariable).triggerList )
      {
        selectedTriggers.Add( Utils.mainWindow.mission.GetTriggerFromGUID( item ) );
      }
    }

    private void Window_MouseDown( object sender, MouseButtonEventArgs e )
    {
      DragMove();
    }

    private void okButton_Click( object sender, RoutedEventArgs e )
    {
      Close();
    }

    private void addTriggerButton_Click( object sender, RoutedEventArgs e )
    {
      if ( selectedTrigger != null && !(eventAction as ModifyVariable).triggerList.Contains( selectedTrigger.GUID ) )
      {
        (eventAction as ModifyVariable).triggerList.Add( selectedTrigger.GUID );
        selectedTriggers.Add( selectedTrigger );
      }
    }

    private void remTriggerButton_Click( object sender, RoutedEventArgs e )
    {
      (eventAction as ModifyVariable).triggerList.Remove( ((sender as FrameworkElement).DataContext as Trigger).GUID );
      selectedTriggers.Remove( (sender as FrameworkElement).DataContext as Trigger );
    }

    private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
    {
      Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
      if ( !(eventAction as ModifyVariable).triggerList.Contains( t.GUID ) )
      {
        (eventAction as ModifyVariable).triggerList.Add( t.GUID );
        selectedTriggers.Add( t );
      }
    }

    private void tlist_GotFocus( object sender, RoutedEventArgs e )
    {
      tlist.GotFocus -= tlist_GotFocus;
      tlist.ItemsSource = Utils.mainWindow.localTriggers;
      tlist.GotFocus += tlist_GotFocus;
    }
  }
}
