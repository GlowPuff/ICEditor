using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ChangeTarget : EventAction
	{
		PriorityTargetType _targetType;
		GroupType _groupType;
		string _otherTarget, _specificHero, _specificAlly;
		int _percentChance;

		public PriorityTargetType targetType { get { return _targetType; } set { _targetType = value; PC(); } }
		public GroupType groupType { get { return _groupType; } set { _groupType = value; PC(); } }
		public string otherTarget { get { return _otherTarget; } set { _otherTarget = value; PC(); } }
		public string specificAlly { get { return _specificAlly; } set { _specificAlly = value; PC(); } }
		public string specificHero { get { return _specificHero; } set { _specificHero = value; PC(); } }
		public int percentChance { get { return _percentChance; } set { _percentChance = value; PC(); } }
		public ObservableCollection<DCPointer> groupsToAdd { get; set; } = new();
		public GroupPriorityTraits groupPriorityTraits { get; set; }

		public ChangeTarget()
		{

		}

		public ChangeTarget( string dname
			, EventActionType et ) : base( et, dname )
		{
			_targetType = PriorityTargetType.Rebel;
			_groupType = GroupType.All;
			specificAlly = "A001";
			specificHero = "H1";
			groupPriorityTraits = new();
			groupPriorityTraits.useDefaultPriority = false;
			percentChance = 60;
		}
	}
}
