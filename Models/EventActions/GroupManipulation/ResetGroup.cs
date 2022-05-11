using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ResetGroup : EventAction
	{
		bool _resetAll;
		public ObservableCollection<DCPointer> groupsToAdd { get; set; } = new();

		public bool resetAll { get { return _resetAll; } set { _resetAll = value; PC(); } }

		public ResetGroup()
		{

		}

		public ResetGroup( string dname
			, EventActionType et ) : base( et, dname )
		{
			resetAll = true;
		}
	}
}
