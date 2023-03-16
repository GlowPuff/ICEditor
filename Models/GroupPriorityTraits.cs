using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class GroupPriorityTraits : INotifyPropertyChanged
	{
		bool _incBrawler, _incCreature, _incDroid, _incForceUser;
		bool _incGuardian, _incHeavyWeapon, _incHunter, _incLeader;
		bool _incSmuggler, _incSpy, _incTrooper, _incWookiee, _incVehicle;
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
		public bool incVehicle { get { return _incVehicle; } set { _incVehicle = value; PC(); } }
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
			incSmuggler = incSpy = incTrooper = incWookiee = incVehicle = true;
			useDefaultPriority = true;
		}

		public void CheckAll()
		{
			incBrawler = incCreature = incDroid = incForceUser = true;
			incGuardian = incHeavyWeapon = incHunter = incLeader = true;
			incSmuggler = incSpy = incTrooper = incWookiee = incVehicle = true;
		}

		public void ClearAll()
		{
			incBrawler = incCreature = incDroid = incForceUser = false;
			incGuardian = incHeavyWeapon = incHunter = incLeader = false;
			incSmuggler = incSpy = incTrooper = incWookiee = incVehicle = false;
		}

		/// <summary>
		/// Changes the traits based on given string[] of traits
		/// </summary>
		public void FromArray( GroupTraits[] traits )
		{
			//GroupPriorityTraits result = new GroupPriorityTraits()
			//{
			incBrawler = incCreature = incDroid = incForceUser =
			incGuardian = incHeavyWeapon = incHunter = incLeader =
			incSmuggler = incSpy = incTrooper = incWookiee = incVehicle = false;
			//};

			foreach ( GroupTraits t in traits )
			{
				if ( t == GroupTraits.Brawler )
					incBrawler = true;
				if ( t == GroupTraits.Creature )
					incCreature = true;
				if ( t == GroupTraits.Droid )
					incDroid = true;
				if ( t == GroupTraits.ForceUser )
					incForceUser = true;
				if ( t == GroupTraits.Guardian )
					incGuardian = true;
				if ( t == GroupTraits.HeavyWeapon )
					incHeavyWeapon = true;
				if ( t == GroupTraits.Hunter )
					incHunter = true;
				if ( t == GroupTraits.Leader )
					incLeader = true;
				if ( t == GroupTraits.Smuggler )
					incSmuggler = true;
				if ( t == GroupTraits.Spy )
					incSpy = true;
				if ( t == GroupTraits.Trooper )
					incTrooper = true;
				if ( t == GroupTraits.Wookiee )
					incWookiee = true;
				if ( t == GroupTraits.Vehicle )
					incVehicle = true;
			}
			//return result;
		}

		public string[] GetTraitArray()
		{
			List<GroupTraits> list = new List<GroupTraits>();
			if ( incBrawler )
				list.Add( GroupTraits.Brawler );
			if ( incCreature )
				list.Add( GroupTraits.Creature );
			if ( incDroid )
				list.Add( GroupTraits.Droid );
			if ( incForceUser )
				list.Add( GroupTraits.ForceUser );
			if ( incGuardian )
				list.Add( GroupTraits.Guardian );
			if ( incHeavyWeapon )
				list.Add( GroupTraits.HeavyWeapon );
			if ( incHunter )
				list.Add( GroupTraits.Hunter );
			if ( incLeader )
				list.Add( GroupTraits.Leader );
			if ( incSmuggler )
				list.Add( GroupTraits.Smuggler );
			if ( incSpy )
				list.Add( GroupTraits.Spy );
			if ( incTrooper )
				list.Add( GroupTraits.Trooper );
			if ( incWookiee )
				list.Add( GroupTraits.Wookiee );
			if ( incVehicle )
				list.Add( GroupTraits.Vehicle );

			return list.Select( x => x.ToString() ).ToArray();
		}
	}
}
