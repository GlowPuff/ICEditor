using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class CampaignPackage : ObservableObject
	{
		string _campaignName, _campaignInstructions;

		public Guid GUID;
		public string campaignName { get => _campaignName; set { SetProperty( ref _campaignName, value ); } }
		public string campaignInstructions { get => _campaignInstructions; set { SetProperty( ref _campaignInstructions, value ); } }
		public ObservableCollection<CampaignMissionItem> campaignMissionItems { get; set; } = new();
		public ObservableCollection<CampaignStructure> campaignStructure { get; set; } = new();

		public CampaignPackage()
		{
			GUID = Guid.NewGuid();
		}

		public CampaignMissionItem AddMission( Mission mission )
		{
			var item = new CampaignMissionItem() { missionName = mission.missionProperties.missionName, missionGUID = mission.missionGUID, mission = mission, GUID = Guid.NewGuid() };
			campaignMissionItems.Add( item );
			return item;
		}

		public void RemoveMission( CampaignMissionItem item )
		{
			campaignMissionItems.Remove( item );
		}

		public void AddStructure()
		{
			var cs = new CampaignStructure();
			cs.missionSource = MissionSource.None;
			cs.itemTier = new string[] { "1" };
			cs.isAgendaMission = false;
			cs.missionType = MissionType.Story;
			cs.missionID = Guid.Empty.ToString();
			cs.projectItem.Title = "Player's Choice";
			cs.projectItem.missionGUID = Guid.Empty.ToString();
			cs.expansionCode = "Imported";

			campaignStructure.Add( cs );
		}

		public void RemoveStructure( CampaignStructure cs )
		{
			campaignStructure.Remove( cs );
		}
	}

	public class CampaignMissionItem : ObservableObject
	{
		string _missionName;

		public Guid GUID { get; set; }//GUID of this object, not the mission
		public Guid missionGUID { get; set; }
		public string missionName { get => _missionName; set { SetProperty( ref _missionName, value ); } }

		//store the actual mission for packing as an individual file later, but don't serialize it here
		[JsonIgnore]
		public Mission mission { get; set; }
	}

	public class CampaignStructure : ObservableObject
	{
		MissionType _missionType;
		int _threatLevel;
		bool _isAgendaMission;

		public string missionID;//in ICE, this is the missionGUID of the CampaignMissionItem.mission as a string
		public string[] itemTier;
		public string expansionCode;

		//set in campaign UI - not loaded from campaign structure JSONs
		public bool isItemChecked = false;
		public bool isForced = false;
		public AgendaType agendaType = AgendaType.NotSet;
		public MissionSource missionSource = MissionSource.None;
		public Guid structureGUID;
		public bool canModify = true;
		public ProjectItem projectItem { get; set; }

		public MissionType missionType { get => _missionType; set { SetProperty( ref _missionType, value ); } }
		public int threatLevel { get => _threatLevel; set { SetProperty( ref _threatLevel, value ); } }
		public bool isAgendaMission { get => _isAgendaMission; set { SetProperty( ref _isAgendaMission, value ); } }

		[JsonIgnore]
		public string tier
		{
			get => itemTier.Aggregate( ( acc, cur ) => $"{acc},{cur}" );
			set
			{
				//itemTier = value.Split( (',') );
				SetProperty( ref itemTier, value.Split( (',') ) );
			}
		}

		public CampaignStructure()
		{
			structureGUID = Guid.NewGuid();
			projectItem = new ProjectItem();
		}

		public void Reset()
		{
			projectItem.Title = "Player's Choice";
			missionID = Guid.Empty.ToString();
			missionSource = MissionSource.None;
			projectItem.missionGUID = Guid.Empty.ToString();
		}
	}
}
