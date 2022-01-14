using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class AddGroupDeployment : EventAction
	{
		public ObservableCollection<DeploymentCard> groupsToAdd { get; set; } = new();

		public AddGroupDeployment()
		{

		}

		public AddGroupDeployment( string dname
			, EventActionType et ) : base( et, dname )
		{
		}
	}
}
