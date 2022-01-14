using System;
using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ModifyHighlight : EventAction
	{
		public ObservableCollection<Guid> highlightList { get; set; } = new();

		public ModifyHighlight()
		{

		}

		public ModifyHighlight( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
