using System;

namespace Imperial_Commander_Editor
{
	public class AllyDeployment : EventAction, IHasEventReference, IHasTriggerReference, IHasEntityReference
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

		public BrokenRefInfo NotifyEventRemoved( Guid guid, NotifyMode mode )
		{
			if ( setEvent == guid )
			{
				if ( mode == NotifyMode.Fix )
					setEvent = Guid.Empty;
				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "Fixed 'On Defeated' Event"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo NotifyTriggerRemoved( Guid guid, NotifyMode mode )
		{
			if ( setTrigger == guid )
			{
				if ( mode == NotifyMode.Fix )
					setTrigger = Guid.Empty;
				return new()
				{
					itemName = displayName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "Fixed 'On Defeated' Trigger"
				};
			}
			return new() { isBroken = false };
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

		public BrokenRefInfo SelfCheckEvents()
		{
			if ( !Utils.ValidateEvent( setEvent ) )
			{
				return new()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Event,
					itemName = displayName,
					ownerGuid = GUID,
					brokenGuid = setEvent,
					details = "Missing 'On Defeated' Event"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckTriggers()
		{
			if ( !Utils.ValidateTrigger( setTrigger ) )
			{
				return new()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Trigger,
					itemName = displayName,
					ownerGuid = GUID,
					brokenGuid = setTrigger,
					details = "Missing 'On Defeated' Trigger"
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
