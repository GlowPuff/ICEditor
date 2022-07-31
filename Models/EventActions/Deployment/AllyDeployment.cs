using System;

namespace Imperial_Commander_Editor
{
	public class AllyDeployment : EventAction
	{
		string _allyName, _allyID;
		Guid _setTrigger, _specificDeploymentPoint, _setEvent;
		DeploymentSpot _deploymentPoint;
		int _threatCost;
		bool _useThreat, _useGenericMugshot;

		public string allyName { get { return _allyName; } set { _allyName = value; PC(); } }
		public string allyID { get { return _allyID; } set { _allyID = value; PC(); } }
		public Guid setTrigger { get { return _setTrigger; } set { _setTrigger = value; PC(); } }
		public Guid setEvent { get { return _setEvent; } set { _setEvent = value; PC(); } }
		public DeploymentSpot deploymentPoint { get { return _deploymentPoint; } set { _deploymentPoint = value; PC(); } }
		public Guid specificDeploymentPoint { get { return _specificDeploymentPoint; } set { _specificDeploymentPoint = value; PC(); } }
		public int threatCost { get { return _threatCost; } set { _threatCost = value; PC(); } }
		public bool useThreat { get { return _useThreat; } set { _useThreat = value; PC(); } }
		public bool useGenericMugshot { get { return _useGenericMugshot; } set { _useGenericMugshot = value; PC(); } }

		public AllyDeployment()
		{

		}

		public AllyDeployment( string dname
			, EventActionType et ) : base( et, dname )
		{
			_setTrigger = Guid.Empty;
			_setEvent = Guid.Empty;
			_specificDeploymentPoint = Guid.Empty;
			_allyID = "A001";
			_deploymentPoint = DeploymentSpot.Active;
			_threatCost = 0;
			_useThreat = false;
			_useGenericMugshot = false;
		}
	}
}
