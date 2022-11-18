using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class EntityGroup : INotifyPropertyChanged
	{
		string _name;
		Guid _GUID, _triggerGUID;
		bool _repeateable;

		public string name { get { return _name; } set { _name = value; PC(); } }
		public Guid GUID { get { return _GUID; } set { _GUID = value; PC(); } }
		public bool repeateable { get { return _repeateable; } set { _repeateable = value; PC(); } }
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
			repeateable = false;
		}

		public BrokenRefInfo NotifyEntityRemoved( Guid guid, NotifyMode mode )
		{
			var e = missionEntities.Where( x => x == guid ).ToList();

			if ( e.Count() > 0 )
			{
				if ( mode == NotifyMode.Fix )
				{
					for ( int i = missionEntities.Count - 1; i >= 0; i-- )
					{
						if ( missionEntities[i] == guid )
							missionEntities.RemoveAt( i );
					}
				}

				return new()
				{
					itemName = $"Entity Group [{name}]",
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = $"Entity REMOVED from [Entity Group List]"
				};
			}
			return new() { isBroken = false };
		}
	}
}
