namespace Imperial_Commander_Editor
{
	public class MissionManagement : EventAction
	{
		bool _incRoundCounter, _pauseDeployment, _unpauseDeployment, _pauseThreat, _unpauseThreat, _endMission;

		public bool incRoundCounter
		{
			get { return _incRoundCounter; }
			set { _incRoundCounter = value; PC(); }
		}
		public bool pauseDeployment
		{
			get { return _pauseDeployment; }
			set { _pauseDeployment = value; PC(); }
		}
		public bool unpauseDeployment
		{
			get { return _unpauseDeployment; }
			set { _unpauseDeployment = value; PC(); }
		}
		public bool pauseThreat
		{
			get { return _pauseThreat; }
			set { _pauseThreat = value; PC(); }
		}
		public bool unpauseThreat
		{
			get { return _unpauseThreat; }
			set { _unpauseThreat = value; PC(); }
		}
		public bool endMission
		{
			get { return _endMission; }
			set { _endMission = value; PC(); }
		}

		public MissionManagement()
		{

		}
		public MissionManagement( string dname
			, EventActionType et ) : base( et, dname )
		{
		}
	}
}
