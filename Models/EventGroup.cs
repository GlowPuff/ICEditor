﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class EventGroup : INotifyPropertyChanged
	{
		string _name;
		Guid _GUID, _triggerGUID;
		bool _repeateable, _isUnique;

		public string name { get { return _name; } set { _name = value; PC(); } }
		public Guid GUID { get { return _GUID; } set { _GUID = value; PC(); } }
		public bool repeateable { get { return _repeateable; } set { _repeateable = value; PC(); } }
		public bool isUnique { get { return _isUnique; } set { _isUnique = value; PC(); } }
		public Guid triggerGUID { get { return _triggerGUID; } set { _triggerGUID = value; PC(); } }
		public ObservableCollection<Guid> missionEvents { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public EventGroup()
		{
			name = "New Event Group";
			GUID = Guid.NewGuid();
			triggerGUID = Guid.Empty;
			missionEvents = new();
			repeateable = false;
			isUnique = true;
		}
	}
}
