using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class RemoveGroup : EventAction
	{
		public ObservableCollection<DCPointer> groupsToRemove { get; set; } = new();
		public ObservableCollection<DCPointer> allyGroupsToRemove { get; set; } = new();

		public RemoveGroup()
		{

		}

		public RemoveGroup( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
