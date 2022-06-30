using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class DeploymentPointProps : INotifyPropertyChanged
	{
		bool _incImperial, _incMercenary;
		bool _incSmall, _incMedium, _incLarge, _incHuge;
		bool _incBrawler, _incCreature, _incDroid, _incForceUser;
		bool _incGuardian, _incHeavyWeapon, _incHunter, _incLeader;
		bool _incSmuggler, _incSpy, _incTrooper, _incWookiee, _incVehicle;

		public bool incImperial { get { return _incImperial; } set { _incImperial = value; PC(); } }
		public bool incMercenary { get { return _incMercenary; } set { _incMercenary = value; PC(); } }
		public bool incSmall { get { return _incSmall; } set { _incSmall = value; PC(); } }
		public bool incMedium { get { return _incMedium; } set { _incMedium = value; PC(); } }
		public bool incLarge { get { return _incLarge; } set { _incLarge = value; PC(); } }
		public bool incHuge { get { return _incHuge; } set { _incHuge = value; PC(); } }
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
		public bool incVehicle { get { return _incVehicle; } set { _incVehicle = value; PC(); } }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public DeploymentPointProps()
		{
			incImperial = incMercenary = true;
			incSmall = incMedium = incLarge = incHuge = true;
			incBrawler = incCreature = incDroid = incForceUser = true;
			incGuardian = incHeavyWeapon = incHunter = incLeader = true;
			incSmuggler = incSpy = incTrooper = incWookiee = incVehicle = true;
		}

		public void CopyFrom( DeploymentPointProps dp )
		{
			incImperial = dp.incImperial;
			incMercenary = dp.incMercenary;
			incSmall = dp.incSmall;
			incMedium = dp.incMedium;
			incLarge = dp.incLarge;
			incHuge = dp.incHuge;
			incBrawler = dp.incBrawler;
			incCreature = dp.incCreature;
			incDroid = dp.incDroid;
			incForceUser = dp.incForceUser;
			incGuardian = dp.incGuardian;
			incHeavyWeapon = dp.incHeavyWeapon;
			incHunter = dp.incHunter;
			incLeader = dp.incLeader;
			incSmuggler = dp.incSmuggler;
			incSpy = dp.incSpy;
			incTrooper = dp.incTrooper;
			incWookiee = dp.incWookiee;
			incVehicle = dp.incVehicle;
		}
	}
}
