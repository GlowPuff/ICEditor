using System;

namespace Imperial_Commander_Editor
{
	public class OptionalDeployment : EventAction
	{
		DeploymentSpot _deploymentPoint;
		int _threatCost;
		bool _useThreat;
		Guid _specificDeploymentPoint;

		public DeploymentSpot deploymentPoint { get { return _deploymentPoint; } set { _deploymentPoint = value; PC(); } }
		public int threatCost { get { return _threatCost; } set { _threatCost = value; PC(); } }
		public bool useThreat { get { return _useThreat; } set { _useThreat = value; PC(); } }
		public Guid specificDeploymentPoint { get { return _specificDeploymentPoint; } set { _specificDeploymentPoint = value; PC(); } }

		public OptionalDeployment()
		{

		}

		public OptionalDeployment( string dname
			, EventActionType et ) : base( et, dname )
		{
			_deploymentPoint = DeploymentSpot.Active;
			_threatCost = 0;
			_useThreat = false;
			_specificDeploymentPoint = Guid.Empty;
		}
	}
}
