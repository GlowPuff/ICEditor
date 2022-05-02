using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class ChangeInstructions : EventAction
	{
		string _theText;
		CustomInstructionType _instructionType;

		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public CustomInstructionType instructionType { get { return _instructionType; } set { _instructionType = value; PC(); } }
		public ObservableCollection<DCPointer> groupsToAdd { get; set; } = new();

		public ChangeInstructions()
		{

		}

		public ChangeInstructions( string dname
			, EventActionType et ) : base( et, dname )
		{
			instructionType = CustomInstructionType.Replace;
		}
	}
}
