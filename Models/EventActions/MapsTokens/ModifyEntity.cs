using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ModifyEntity : EventAction
	{
		public string name { get; set; }
		public ObservableCollection<EntityProperties> entitiesToActivate { get; set; } = new();
		public ObservableCollection<EntityProperties> entitiesToModify { get; set; } = new();

		public ModifyEntity()
		{

		}

		public ModifyEntity( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
