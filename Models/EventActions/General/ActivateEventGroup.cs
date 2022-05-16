using System;

namespace Imperial_Commander_Editor
{
	public class ActivateEventGroup : EventAction
	{
		Guid _eventGroupGUID;

		public Guid eventGroupGUID { get { return _eventGroupGUID; } set { _eventGroupGUID = value; PC(); } }

		public ActivateEventGroup()
		{

		}

		public ActivateEventGroup( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
