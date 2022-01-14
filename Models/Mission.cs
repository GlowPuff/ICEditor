using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
  public class Mission : INotifyPropertyChanged
  {
    /// Bindable data

    public event PropertyChangedEventHandler PropertyChanged;

    public MissionProperties missionProperties { get; set; }
    public Guid missionGUID;
    /// <summary>
    /// JUST the filename+extension
    /// </summary>
    public string fileName;
    /// <summary>
    /// folder path+filename RELATIVE to SpecialFolder.MyDocuments
    /// </summary>
    public string relativePath;

    /// <summary>
    ///	increment this each time file format gets updated
    /// </summary>
    public string fileVersion = "1";

    public string saveDate;

    public ObservableCollection<MapSection> mapSections { get; set; }
    public ObservableCollection<Trigger> globalTriggers { get; set; }
    public ObservableCollection<MissionEvent> globalEvents { get; set; }

    [JsonConverter( typeof( MapEntityConverter ) )]
    public ObservableCollection<IMapEntity> mapEntities { get; set; }
    public ObservableCollection<InitialGroupData> initialDeploymentGroups { get; set; }
    public ObservableCollection<DeploymentCard> reservedDeploymentGroups { get; set; }

    public void PC( [CallerMemberName] string n = "" )
    {
      if ( !string.IsNullOrEmpty( n ) )
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
    }

    //public static Mission CreateInstance( FileManager fm )
    //{
    //	return null;
    //}

    public Mission()
    {
      //defaults
      missionGUID = Guid.NewGuid();
      fileName = "";
      relativePath = "";
      saveDate = DateTime.Now.ToString( "M/d/yyyy" );
      missionProperties = new MissionProperties();

      mapSections = new();
      globalTriggers = new();
      globalEvents = new();
      mapEntities = new();
      initialDeploymentGroups = new();
      reservedDeploymentGroups = new();
    }

    public void InitDefaultData()
    {
      mapSections.Add( new()
      {
        name = "Start Section",
        GUID = Guid.Parse( "11111111-1111-1111-1111-111111111111" ),//Guid.Empty,
        canRemove = false,
        isActive = true
      } );

      //default global NONE trigger
      globalTriggers.Add( new()
      {
        name = "None (Global)",
        isGlobal = true,
        GUID = Guid.Empty
      } );
      //default global NONE event
      globalEvents.Add( new() { name = "None (Global)", GUID = Guid.Empty } );

      //debug
      mapSections.Add( new() { name = "blah" } );
    }

    public bool TriggerExists( Trigger t )
    {
      bool g = globalTriggers.Any( x => x.GUID == t.GUID );
      bool m = mapSections.Any( x => x.triggers.Any( xt => xt.GUID == t.GUID ) );
      return g || m;
    }

    public bool TriggerExists( Guid guid )
    {
      bool g = globalTriggers.Any( x => x.GUID == guid );
      bool m = mapSections.Any( x => x.triggers.Any( xt => xt.GUID == guid ) );
      return g || m;
    }

    public Trigger GetTriggerFromGUID( Guid guid )
    {
      if ( globalTriggers.Any( x => x.GUID == guid ) )
        return globalTriggers.First( x => x.GUID == guid );
      else if ( mapSections.Any( x => x.triggers.Any( xt => xt.GUID == guid ) ) )
        return mapSections.First( x => x.triggers.Any( xt => xt.GUID == guid ) ).triggers.First( x => x.GUID == guid );
      else
        return null;
    }

    public bool EntityExists( Guid guid )
    {
      return mapEntities.Any( x => x.GUID == guid );
    }

    public IMapEntity GetEntityFromGUID( Guid guid )
    {
      return mapEntities.Where( x => x.GUID == guid ).FirstOr( null );
    }
  }
}
