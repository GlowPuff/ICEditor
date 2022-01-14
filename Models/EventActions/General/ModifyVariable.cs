using System;
using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
  public class ModifyVariable : EventAction
  {
    public ObservableCollection<Guid> triggerList { get; set; } = new();

    public ModifyVariable()
    {

    }

    public ModifyVariable( string dname
      , EventActionType et ) : base( et, dname )
    {
    }
  }
}
