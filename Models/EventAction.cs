using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public abstract class EventAction : INotifyPropertyChanged, IEventAction
	{
		public event PropertyChangedEventHandler PropertyChanged;

		string _displayName;

		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string displayName { get { return _displayName; } set { _displayName = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public EventAction()
		{

		}

		public EventAction( EventActionType et, string dname )
		{
			GUID = Guid.NewGuid();
			displayName = dname;
			eventActionType = et;
		}
	}
}
