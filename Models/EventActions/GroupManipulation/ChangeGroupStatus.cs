using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ChangeGroupStatus : EventAction
	{
		public ObservableCollection<DCPointer> readyGroups { get; set; } = new();
		public ObservableCollection<DCPointer> exhaustGroups { get; set; } = new();

		public ChangeGroupStatus()
		{

		}

		public ChangeGroupStatus( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
