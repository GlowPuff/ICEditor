using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
	public class QuestionPrompt : EventAction
	{
		string _theText;
		bool _includeCancel;

		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public bool includeCancel { get { return _includeCancel; } set { _includeCancel = value; PC(); } }

		public ObservableCollection<ButtonAction> buttonList { get; set; } = new();

		public QuestionPrompt()
		{

		}

		public QuestionPrompt( string dname
			, EventActionType et ) : base( et, dname )
		{
			_includeCancel = false;
		}
	}
}
