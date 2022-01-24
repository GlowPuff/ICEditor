using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class EntityGroup : INotifyPropertyChanged
	{
		string _name;
		Guid _GUID, _triggerGUID;

		public string name { get { return _name; } set { _name = value; PC(); } }
		public Guid GUID { get { return _GUID; } set { _GUID = value; PC(); } }
		public Guid triggerGUID { get { return _triggerGUID; } set { _triggerGUID = value; PC(); } }
		public ObservableCollection<Guid> missionEntities { get; set; }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public EntityGroup()
		{
			name = "New Entity Group";
			GUID = Guid.NewGuid();
			triggerGUID = Guid.Empty;
			missionEntities = new();
		}
	}
}
