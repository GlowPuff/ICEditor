using System;

namespace Imperial_Commander_Editor
{
	public class OptionalDeployment : EventAction, IHasEntityReference
	{
		DeploymentSpot _deploymentPoint;
		int _threatCost;
		bool _useThreat, _isOnslaught;
		Guid _specificDeploymentPoint;

		public DeploymentSpot deploymentPoint { get { return _deploymentPoint; } set { _deploymentPoint = value; PC(); } }
		public int threatCost { get { return _threatCost; } set { _threatCost = value; PC(); } }
		public bool useThreat { get { return _useThreat; } set { _useThreat = value; PC(); } }
		public Guid specificDeploymentPoint { get { return _specificDeploymentPoint; } set { _specificDeploymentPoint = value; PC(); } }
		public bool isOnslaught { get { return _isOnslaught; } set { _isOnslaught = value; PC(); } }

		public OptionalDeployment()
		{

		}

		public OptionalDeployment( string dname
			, EventActionType et ) : base( et, dname )
		{
			_deploymentPoint = DeploymentSpot.Active;
			_threatCost = 0;
			_useThreat = true;
			_specificDeploymentPoint = Guid.Empty;
			_isOnslaught = false;
		}

		public BrokenRefInfo NotifyEntityRemoved( Guid guid, NotifyMode mode )
		{
			if ( specificDeploymentPoint == guid )
			{
				if ( mode == NotifyMode.Fix )
				{
					specificDeploymentPoint = Guid.Empty;
					deploymentPoint = DeploymentSpot.Active;
				}
				return new()
				{
					itemName = displayName,
					isBroken = deploymentPoint == DeploymentSpot.Specific,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "Specific Deployment Point reset to Active Deployment Point"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckEntities()
		{
			if ( !Utils.ValidateMapEntity( specificDeploymentPoint ) )
			{
				return new()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Entity,
					itemName = displayName,
					ownerGuid = GUID,
					brokenGuid = Guid.Empty,
					details = "Missing Specific Deployment Point"
				};
			}
			return new() { isBroken = false };
		}
	}
}
