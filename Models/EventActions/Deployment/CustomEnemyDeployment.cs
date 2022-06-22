using System;
using System.Linq;

namespace Imperial_Commander_Editor
{
	public class CustomEnemyDeployment : EventAction
	{
		string _thumbnailGroupImperial, _thumbnailGroupRebel, _modification, _repositionInstructions, _groupAttack, _groupDefense, _surges, _bonuses, _keywords, _abilities;
		int _groupCost, _groupRedeployCost, _groupSize, _groupHealth, _groupSpeed;
		bool _canReinforce, _canRedeploy, _canBeDefeated, _useDeductCost, _useCustomInstructions, _useThreatMultiplier, _useResetOnRedeployment;
		Guid _specificDeploymentPoint;
		DeploymentSpot _deploymentPoint;
		MarkerType _customType, _iconType;

		public MarkerType customType { get { return _customType; } set { _customType = value; PC(); } }
		public MarkerType iconType { get { return _iconType; } set { _iconType = value; PC(); } }
		public string thumbnailGroupImperial { get { return _thumbnailGroupImperial; } set { _thumbnailGroupImperial = value; PC(); } }
		public string thumbnailGroupRebel { get { return _thumbnailGroupRebel; } set { _thumbnailGroupRebel = value; PC(); } }
		public string repositionInstructions { get { return _repositionInstructions; } set { _repositionInstructions = value; PC(); } }
		public string groupAttack { get { return _groupAttack; } set { _groupAttack = value; PC(); } }
		public string groupDefense { get { return _groupDefense; } set { _groupDefense = value; PC(); } }
		public string surges { get { return _surges; } set { _surges = value; PC(); } }
		public string bonuses { get { return _bonuses; } set { _bonuses = value; PC(); } }
		public string keywords { get { return _keywords; } set { _keywords = value; PC(); } }
		public string abilities { get { return _abilities; } set { _abilities = value; PC(); } }
		public int groupCost { get { return _groupCost; } set { _groupCost = value; PC(); } }
		public int groupRedeployCost { get { return _groupRedeployCost; } set { _groupRedeployCost = value; PC(); } }
		public int groupSize { get { return _groupSize; } set { _groupSize = value; PC(); } }
		public int groupHealth { get { return _groupHealth; } set { _groupHealth = value; PC(); } }
		public int groupSpeed { get { return _groupSpeed; } set { _groupSpeed = value; PC(); } }
		public string modification { get { return _modification; } set { _modification = value; PC(); } }
		public bool canReinforce { get { return _canReinforce; } set { _canReinforce = value; PC(); } }
		public bool canRedeploy { get { return _canRedeploy; } set { _canRedeploy = value; PC(); } }
		public bool canBeDefeated { get { return _canBeDefeated; } set { _canBeDefeated = value; PC(); } }
		public bool useDeductCost { get { return _useDeductCost; } set { _useDeductCost = value; PC(); } }
		public bool useCustomInstructions { get { return _useCustomInstructions; } set { _useCustomInstructions = value; PC(); } }
		public bool useThreatMultiplier { get { return _useThreatMultiplier; } set { _useThreatMultiplier = value; PC(); } }
		public bool useResetOnRedeployment { get { return _useResetOnRedeployment; } set { _useResetOnRedeployment = value; PC(); } }
		public Guid specificDeploymentPoint { get { return _specificDeploymentPoint; } set { _specificDeploymentPoint = value; PC(); } }
		public DeploymentSpot deploymentPoint { get { return _deploymentPoint; } set { _deploymentPoint = value; PC(); } }
		public EnemyGroupData enemyGroupData { get; set; }

		public CustomEnemyDeployment()
		{

		}

		public CustomEnemyDeployment( string dname
			, EventActionType et ) : base( et, dname )
		{
			_customType = MarkerType.Rebel;
			_iconType = MarkerType.Rebel;
			_thumbnailGroupImperial = "DG001";
			_thumbnailGroupRebel = "A001";
			_groupCost = 0;
			_groupRedeployCost = 0;
			_canRedeploy = _canReinforce = false;
			_deploymentPoint = DeploymentSpot.Active;
			_specificDeploymentPoint = Guid.Empty;
			_useDeductCost = false;
			_useCustomInstructions = false;
			_canBeDefeated = true;
			_repositionInstructions = _groupAttack = _groupDefense = "";
			_groupSize = 1;
			_groupHealth = 0;
			_groupSpeed = 0;
			_useThreatMultiplier = false;
			_useResetOnRedeployment = false;

			_surges = "{B}: Bleed\n{B}: Focus\n{B}: Pierce 2";
			_keywords = "+2 {H}\nHabitat: Snow";
			_bonuses = "CHARGE: The first time this figure attacks or uses Trample, add 1 blue die to its dice pool.\nCRUSH: Each Rebel that suffers {H} during this activation also becomes Weakened.";
			_abilities = "Composite Plating:While defending, if the attacker is 4 or more spaces away, apply +1 {G} to the defense roll.\nEfficient Travel:Snowtroopers (Elite) ignores additional movement point costs for difficult terrain and hostile figures.";

			DeploymentCard card = new()
			{
				name = "",
				id = Guid.NewGuid().ToString(),//sets enemyGroupData.cardID below
				size = 1
			};

			enemyGroupData = new( card, new()
			{
				name = "Active Deployment Point",
				GUID = Guid.Empty,
			} )
			{
				customText = "{A} Move 3 to engage as many Rebels as possible (minimum 1).\n{-} INVASIVE PROCEDURE: An adjacent Rebel suffers 1 {H}"
			};

			UpdateDP();
		}

		public void UpdateDP()
		{
			var oldPoints = enemyGroupData.pointList.ToArray();
			enemyGroupData.pointList.Clear();
			for ( int i = 0; i < groupSize; i++ )
			{
				if ( i < oldPoints.Length )
					enemyGroupData.pointList.Add( oldPoints[i] );
				else
					enemyGroupData.pointList.Add( new() { GUID = Guid.Empty } );
			}

		}
	}
}
