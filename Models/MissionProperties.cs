using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class MissionProperties : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		string _missionName, _missionID, _fixedAlly, _bannedAlly, _customInstructionText, _specificAlly, _specificHero, _priorityOther, _missionDescription;
		bool _optionalDeployment, _factionImperial, _factionMercenary;
		YesNoAll _useFixedAlly, _useBannedAlly, _banAllAllies;
		CustomInstructionType _customInstructionType;
		ThreatModifierType _initialThreatType;
		PriorityTargetType _priorityTargetType;
		Guid _startingEvent;
		int _fixedValue, _threatLevel;

		public string missionName
		{
			get { return _missionName; }
			set { _missionName = value; PC(); }
		}
		public string missionDescription
		{
			get { return _missionDescription; }
			set { _missionDescription = value; PC(); }
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

		//set to EXPANSIONXX where XX is the mission number and EXPANSION is the expansion name (ie: CORE, EMPIRE)
		public string missionID
		{
			get { return _missionID; }
			set { _missionID = value; PC(); }
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
		public YesNoAll banAllAllies
		{
			get { return _banAllAllies; }
			set { _banAllAllies = value; PC(); }
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

		public string customInstructionText
		{
			get { return _customInstructionText; }
			set { _customInstructionText = value; PC(); }
		}

		public ThreatModifierType initialThreatType
		{
			get { return _initialThreatType; }
			set { _initialThreatType = value; PC(); }
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

		public int fixedValue
		{
			get { return _fixedValue; }
			set { _fixedValue = value; PC(); }
		}

		public int threatLevel
		{
			get { return _threatLevel; }
			set { _threatLevel = value; PC(); }
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public MissionProperties()
		{
			missionID = "Unique Mission ID";
			missionName = "Mission Name";
			missionDescription = "";
			customInstructionText = "";
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
			banAllAllies = YesNoAll.No;
			initialThreatType = ThreatModifierType.None;
			priorityTargetType = PriorityTargetType.Rebel;
			fixedValue = 0;
			threatLevel = 1;
			startingEvent = Guid.Empty;
		}
	}
}
