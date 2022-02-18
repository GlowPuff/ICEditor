using System;
using System.Linq;

namespace Imperial_Commander_Editor
{
	public class EnemyDeployment : EventAction
	{
		string _enemyName, _deploymentGroup, _modification;
		SourceType _sourceType;
		int _threatCost;
		bool _canReinforce, _canRedeploy, _canBeDefeated, _useThreat, _showMod, _useCustomInstructions, _useDefaultPriority;
		Guid _setTrigger, _specificDeploymentPoint;
		DeploymentSpot _deploymentPoint;

		public string enemyName { get { return _enemyName; } set { _enemyName = value; PC(); } }
		public string deploymentGroup { get { return _deploymentGroup; } set { _deploymentGroup = value; PC(); } }
		public int threatCost { get { return _threatCost; } set { _threatCost = value; PC(); } }
		public string modification { get { return _modification; } set { _modification = value; PC(); } }
		public SourceType sourceType { get { return _sourceType; } set { _sourceType = value; PC(); } }
		public bool canReinforce { get { return _canReinforce; } set { _canReinforce = value; PC(); } }
		public bool canRedeploy { get { return _canRedeploy; } set { _canRedeploy = value; PC(); } }
		public bool canBeDefeated { get { return _canBeDefeated; } set { _canBeDefeated = value; PC(); } }
		public bool useThreat { get { return _useThreat; } set { _useThreat = value; PC(); } }
		public bool useDefaultPriority { get { return _useDefaultPriority; } set { _useDefaultPriority = value; PC(); } }
		public bool useCustomInstructions { get { return _useCustomInstructions; } set { _useCustomInstructions = value; PC(); } }
		public bool showMod { get { return _showMod; } set { _showMod = value; PC(); } }
		public Guid setTrigger { get { return _setTrigger; } set { _setTrigger = value; PC(); } }
		public Guid specificDeploymentPoint { get { return _specificDeploymentPoint; } set { _specificDeploymentPoint = value; PC(); } }
		public DeploymentSpot deploymentPoint { get { return _deploymentPoint; } set { _deploymentPoint = value; PC(); } }
		public EnemyGroupData enemyGroupData { get; set; }

		public EnemyDeployment()
		{

		}

		public EnemyDeployment( string dname
			, EventActionType et ) : base( et, dname )
		{
			_deploymentGroup = "DG001";
			_threatCost = 0;
			_sourceType = SourceType.InitialReserved;
			_canRedeploy = _canReinforce = true;
			_setTrigger = Guid.Empty;
			_deploymentPoint = DeploymentSpot.Active;
			_specificDeploymentPoint = Guid.Empty;
			_useThreat = _showMod = false;
			_useCustomInstructions = false;
			_canBeDefeated = true;
			_useDefaultPriority = true;

			DeploymentCard card = Utils.enemyData.First( x => x.id == _deploymentGroup );
			enemyGroupData = new( card, new() { name = "None", GUID = Guid.Empty } );
		}
	}
}
