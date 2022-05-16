using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ChangeReposition : EventAction
	{
		string _theText;
		bool _useSpecific;

		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public bool useSpecific { get { return _useSpecific; } set { _useSpecific = value; } }
		public ObservableCollection<DCPointer> repoGroups { get; set; } = new();

		public ChangeReposition()
		{

		}

		public ChangeReposition( string dname
			, EventActionType et ) : base( et, dname )
		{
			_useSpecific = false;
		}
	}
}
