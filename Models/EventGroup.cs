using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

		public BrokenRefInfo NotifyEventRemoved( Guid guid, NotifyMode mode )
		{
			var e = missionEvents.Where( x => x == guid ).ToList();

			if ( e.Count() > 0 )
			{
				if ( mode == NotifyMode.Fix )
				{
					for ( int i = missionEvents.Count - 1; i >= 0; i-- )
					{
						if ( missionEvents[i] == guid )
							missionEvents.RemoveAt( i );
					}
				}

				return new()
				{
					itemName = $"Event Group [{name}]",
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = $"Event REMOVED from [Event Group List]"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo SelfCheckEvents()
		{
			List<string> strings = new();
			foreach ( var item in missionEvents )
			{
				if ( !Utils.ValidateEvent( item ) )
					strings.Add( "Missing Event" );
			}
			if ( strings.Count > 0 )
			{
				return new()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.Event,
					itemName = $"Event Group [{name}]",
					ownerGuid = GUID,
					brokenGuid = Guid.Empty,
					details = string.Join( "\n", strings )
				};
			}
			return new() { isBroken = false };
		}
	}
}
