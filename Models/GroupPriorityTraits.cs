using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class GroupPriorityTraits : INotifyPropertyChanged
	{
		bool _incBrawler, _incCreature, _incDroid, _incForceUser;
		bool _incGuardian, _incHeavyWeapon, _incHunter, _incLeader;
		bool _incSmuggler, _incSpy, _incTrooper, _incWookiee;
		bool _useDefaultPriority;

		public bool incBrawler { get { return _incBrawler; } set { _incBrawler = value; PC(); } }
		public bool incCreature { get { return _incCreature; } set { _incCreature = value; PC(); } }
		public bool incDroid { get { return _incDroid; } set { _incDroid = value; PC(); } }
		public bool incForceUser { get { return _incForceUser; } set { _incForceUser = value; PC(); } }
		public bool incGuardian { get { return _incGuardian; } set { _incGuardian = value; PC(); } }
		public bool incHeavyWeapon { get { return _incHeavyWeapon; } set { _incHeavyWeapon = value; PC(); } }
		public bool incHunter { get { return _incHunter; } set { _incHunter = value; PC(); } }
		public bool incLeader { get { return _incLeader; } set { _incLeader = value; PC(); } }
		public bool incSmuggler { get { return _incSmuggler; } set { _incSmuggler = value; PC(); } }
		public bool incSpy { get { return _incSpy; } set { _incSpy = value; PC(); } }
		public bool incTrooper { get { return _incTrooper; } set { _incTrooper = value; PC(); } }
		public bool incWookiee { get { return _incWookiee; } set { _incWookiee = value; PC(); } }
		public bool useDefaultPriority { get { return _useDefaultPriority; } set { _useDefaultPriority = value; PC(); } }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public GroupPriorityTraits()
		{
			incBrawler = incCreature = incDroid = incForceUser = true;
			incGuardian = incHeavyWeapon = incHunter = incLeader = true;
			incSmuggler = incSpy = incTrooper = incWookiee = true;
			useDefaultPriority = true;
		}

		public void CheckAll()
		{
			incBrawler = incCreature = incDroid = incForceUser = true;
			incGuardian = incHeavyWeapon = incHunter = incLeader = true;
			incSmuggler = incSpy = incTrooper = incWookiee = true;
		}

		public void ClearAll()
		{
			incBrawler = incCreature = incDroid = incForceUser = false;
			incGuardian = incHeavyWeapon = incHunter = incLeader = false;
			incSmuggler = incSpy = incTrooper = incWookiee = false;
		}
	}
}
