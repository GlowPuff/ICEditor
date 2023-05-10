using System;

namespace Imperial_Commander_Editor
{
	public class QueryGroup : EventAction
	{
		Guid _foundTrigger, _foundEvent;

		public DCPointer groupEnemyToQuery { get; set; } = null;
		public DCPointer groupRebelToQuery { get; set; } = null;
		public Guid foundTrigger { get { return _foundTrigger; } set { _foundTrigger = value; PC(); } }
		public Guid foundEvent { get { return _foundEvent; } set { _foundEvent = value; PC(); } }

		public QueryGroup()
		{

		}

		public QueryGroup( string dname
	, EventActionType et ) : base( et, dname )
		{
			foundTrigger = Guid.Empty;
			foundEvent = Guid.Empty;
		}
	}
}
