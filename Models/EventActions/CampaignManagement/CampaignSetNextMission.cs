namespace Imperial_Commander_Editor
{
	public class CampaignSetNextMission : EventAction
	{
		string _customMissionID, _missionID;
		public string customMissionID { get => _customMissionID; set { _customMissionID = value; PC(); } }
		public string missionID { get => _missionID; set { _missionID = value; PC(); } }

		public CampaignSetNextMission()
		{

		}

		public CampaignSetNextMission( string dname, EventActionType et ) : base( et, dname )
		{
			customMissionID = "";
			missionID = "Core 1";
		}
	}
}
