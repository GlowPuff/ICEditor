using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ModifyMapEntity : EventAction
	{
		public ObservableCollection<EntityModifier> entitiesToModify { get; set; } = new();

		public ModifyMapEntity()
		{

		}

		public ModifyMapEntity( string dname
			, EventActionType et ) : base( et, dname )
		{
		}
	}
}
