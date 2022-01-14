using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ChangeInstructions : EventAction
	{
		string _allText, _specificText;
		CustomInstructionType _allPlacementType, _specificPlacementType;

		public string allText { get { return _allText; } set { _allText = value; PC(); } }
		public string specificText { get { return _specificText; } set { _specificText = value; PC(); } }
		public CustomInstructionType allPlacementType { get { return _allPlacementType; } set { _allPlacementType = value; PC(); } }
		public CustomInstructionType specificPlacementType { get { return _specificPlacementType; } set { _specificPlacementType = value; PC(); } }
		public ObservableCollection<DeploymentCard> groupsToAdd { get; set; } = new();

		public ChangeInstructions()
		{

		}

		public ChangeInstructions( string dname
			, EventActionType et ) : base( et, dname )
		{
			allPlacementType = CustomInstructionType.Replace;
			specificPlacementType = CustomInstructionType.Replace;
		}
	}
}
