using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class MissionProperties : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		string _missionName, _missionID, _fixedAlly, _bannedAlly, _missionInfo, _specificAlly, _specificHero, _priorityOther, _missionDescription, _campaignName, _startingObjective, _additionalMissionInfo, _customMissionIdentifier;
		bool _optionalDeployment, _factionImperial, _factionMercenary, _useAlternateEventSystem;
		int _roundLimit;
		YesNoAll _useFixedAlly, _useBannedAlly;
		CustomInstructionType _customInstructionType;
		PriorityTargetType _priorityTargetType;
		Guid _startingEvent, _roundLimitEvent;
		MissionType _missionType;
		ChangeReposition _changeRepositionOverride;
		ObservableCollection<MissionSubType> _missionSubType;
		ObservableCollection<string> _multipleBannedAllies;

		public string missionName
		{
			get { return _missionName; }
			set { _missionName = value; PC(); }
		}
		public string campaignName
		{
			get { return _campaignName; }
			set { _campaignName = value; PC(); }
		}
		public string missionDescription
		{
			get { return _missionDescription; }
			set { _missionDescription = value; PC(); }
		}
		public string additionalMissionInfo
		{
			get { return _additionalMissionInfo; }
			set { _additionalMissionInfo = value; PC(); }
		}
		public string fixedAlly
		{
			get { return _fixedAlly; }
			set { _fixedAlly = value; PC(); }
		}
		public string bannedAlly
		{
			get { return _bannedAlly; }
			set { _bannedAlly = value; PC(); }
		}
		public string startingObjective
		{
			get { return _startingObjective; }
			set { _startingObjective = value; PC(); }
		}

		//set to EXPANSIONXX where XX is the mission number and EXPANSION is the expansion name (ie: CORE, EMPIRE)
		public string missionID
		{
			get { return _missionID; }
			set { _missionID = value; PC(); }
		}
		public string customMissionIdentifier
		{
			get { return _customMissionIdentifier; }
			set { _customMissionIdentifier = value; PC(); }
		}
		public string missionInfo
		{
			get { return _missionInfo; }
			set { _missionInfo = value; PC(); }
		}

		public bool optionalDeployment
		{
			get { return _optionalDeployment; }
			set { _optionalDeployment = value; PC(); }
		}

		public bool factionImperial
		{
			get { return _factionImperial; }
			set { _factionImperial = value; PC(); }
		}

		public bool factionMercenary
		{
			get { return _factionMercenary; }
			set { _factionMercenary = value; PC(); }
		}
		public int roundLimit
		{
			get { return _roundLimit; }
			set { _roundLimit = value; PC(); }
		}
		public YesNoAll useFixedAlly
		{
			get { return _useFixedAlly; }
			set { _useFixedAlly = value; PC(); }
		}
		public YesNoAll useBannedAlly
		{
			get { return _useBannedAlly; }
			set { _useBannedAlly = value; PC(); }
		}

		public CustomInstructionType customInstructionType
		{
			get { return _customInstructionType; }
			set { _customInstructionType = value; PC(); }
		}
		public PriorityTargetType priorityTargetType
		{
			get { return _priorityTargetType; }
			set { _priorityTargetType = value; PC(); }
		}
		public string specificAlly
		{
			get { return _specificAlly; }
			set { _specificAlly = value; PC(); }
		}
		public string specificHero
		{
			get { return _specificHero; }
			set { _specificHero = value; PC(); }
		}
		public string priorityOther
		{
			get { return _priorityOther; }
			set { _priorityOther = value; PC(); }
		}
		public Guid startingEvent
		{
			get { return _startingEvent; }
			set { _startingEvent = value; PC(); }
		}
		public Guid roundLimitEvent
		{
			get { return _roundLimitEvent; }
			set { _roundLimitEvent = value; PC(); }
		}
		public bool useAlternateEventSystem
		{
			get { return _useAlternateEventSystem; }
			set { _useAlternateEventSystem = value; PC(); }
		}

		public MissionType missionType { get { return _missionType; } set { _missionType = value; PC(); } }
		public ChangeReposition changeRepositionOverride { get { return _changeRepositionOverride; } set { _changeRepositionOverride = value; PC(); } }
		public ObservableCollection<MissionSubType> missionSubType { get { return _missionSubType; } set { _missionSubType = value; PC(); } }

		public ObservableCollection<string> bannedGroups { get; set; } = new();
		public ObservableCollection<string> multipleBannedAllies
		{
			get
			{
				//added for Mission Format v.20
				if ( _multipleBannedAllies == null )
					_multipleBannedAllies = new();
				return _multipleBannedAllies;
			}
			set
			{
				_multipleBannedAllies = value;
				PC();
			}
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public MissionProperties()
		{
			missionID = "Custom";//"Example: CORE01";//"Unique Mission ID";
			missionName = "Mission Name";
			customMissionIdentifier = Guid.NewGuid().ToString();
			missionDescription = "";
			additionalMissionInfo = "";
			missionInfo = "";
			startingObjective = "";
			fixedAlly = "A001";
			bannedAlly = "A001";
			priorityOther = "";
			specificAlly = "A001";
			specificHero = "H1";
			optionalDeployment = false;
			factionImperial = true;
			factionMercenary = true;
			customInstructionType = CustomInstructionType.Replace;
			useFixedAlly = YesNoAll.No;
			useBannedAlly = YesNoAll.No;
			priorityTargetType = PriorityTargetType.Rebel;
			startingEvent = Guid.Empty;
			roundLimitEvent = Guid.Empty;
			roundLimit = -1;
			missionType = MissionType.Story;
			changeRepositionOverride = null;
			missionSubType = new();
			_multipleBannedAllies = new();
			useAlternateEventSystem = false;
		}
	}
}
