using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ModifyDeployment : EventAction
	{
		public ObservableCollection<DeploymentModifier> deploymentModifiers { get; set; } = new();

		public ModifyDeployment()
		{

		}

		public ModifyDeployment( string dname
			, EventActionType et ) : base( et, dname )
		{

		}
	}
}
