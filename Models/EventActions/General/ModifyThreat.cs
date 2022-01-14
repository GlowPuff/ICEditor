namespace Imperial_Commander_Editor
{
	public class ModifyThreat : EventAction
	{
		ThreatAction _threatAction;
		int _fixedValue, _threatValue;
		ThreatModifierType _threatModifierType;

		public ThreatAction threatAction { get { return _threatAction; } set { _threatAction = value; PC(); } }
		public int fixedValue { get { return _fixedValue; } set { _fixedValue = value; PC(); } }
		public int threatValue { get { return _threatValue; } set { _threatValue = value; PC(); } }
		public ThreatModifierType threatModifierType { get { return _threatModifierType; } set { _threatModifierType = value; PC(); } }

		public ModifyThreat()
		{

		}

		public ModifyThreat( string dname
			, EventActionType et ) : base( et, dname )
		{
			threatAction = ThreatAction.Add;
			fixedValue = 0;
			threatValue = 0;
			threatModifierType = ThreatModifierType.Fixed;
		}
	}
}
