using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
  public class MapSection : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    string _name;
    Guid _GUID;
    bool _isActive;//for UI only
    bool _invisibleUntilActivated;

    public string name
    {
      get { return _name; }
      set { _name = value; PC(); }
    }
    public Guid GUID
    {
      get { return _GUID; }
      set { _GUID = value; PC(); }
    }


    [JsonIgnore]
    public bool canRemove { get; set; } = true;

    [JsonIgnore]
    public bool isActive { get { return _isActive; } set { _isActive = value; PC(); } }
    public bool invisibleUntilActivated { get { return _invisibleUntilActivated; } set { _invisibleUntilActivated = value; PC(); } }

    public ObservableCollection<Trigger> triggers { get; set; } = new ObservableCollection<Trigger>();
    public ObservableCollection<MissionEvent> missionEvents { get; set; } = new ObservableCollection<MissionEvent>();
    public ObservableCollection<MapTile> mapTiles { get; set; } = new ObservableCollection<MapTile>();

    public void PC( [CallerMemberName] string n = "" )
    {
      if ( !string.IsNullOrEmpty( n ) )
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
    }

    public MapSection()
    {
      isActive = false;
      GUID = Guid.NewGuid();
      name = "New Section";
      _invisibleUntilActivated = false;
    }

    public void Init()
    {
      triggers = new ObservableCollection<Trigger>()
        {
          new()
          {
            name = "None (Section)",
            GUID = Guid.Empty,
            isGlobal = false
          }
        };
      missionEvents = new ObservableCollection<MissionEvent>()
        {
          new()
          {
            name = "None (Section)",
            GUID = Guid.Empty,
            isGlobal = false
          }
        };
    }
  }
}
