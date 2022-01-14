﻿using System;

namespace Imperial_Commander_Editor
{
	public class RandomDeployment : EventAction
	{
		ThreatModifierType _threatType;
		int _fixedValue, _threatLevel;
		DeploymentSpot _deploymentPoint;
		Guid _specificDeploymentPoint;

		public ThreatModifierType threatType { get { return _threatType; } set { _threatType = value; PC(); } }
		public int fixedValue { get { return _fixedValue; } set { _fixedValue = value; PC(); } }
		public int threatLevel { get { return _threatLevel; } set { _threatLevel = value; PC(); } }
		public DeploymentSpot deploymentPoint { get { return _deploymentPoint; } set { _deploymentPoint = value; PC(); } }
		public Guid specificDeploymentPoint { get { return _specificDeploymentPoint; } set { _specificDeploymentPoint = value; PC(); } }


		public RandomDeployment()
		{

		}

		public RandomDeployment( string dname
			, EventActionType et ) : base( et, dname )
		{
			_threatType = ThreatModifierType.Fixed;
			_fixedValue = 0;
			_threatLevel = 1;
			_deploymentPoint = DeploymentSpot.Active;
			_specificDeploymentPoint = Guid.Empty;
		}
	}
}
