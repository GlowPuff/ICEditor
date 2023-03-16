using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class Mission : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Guid missionGUID;
		/// <summary>
		/// JUST the filename+extension
		/// </summary>
		public string fileName;
		/// <summary>
		/// folder path+filename RELATIVE to SpecialFolder.MyDocuments
		/// </summary>
		//[JsonIgnore]
		//public string relativePath;

		//only used internally to track the filename when saving
		[JsonIgnore]
		public string fullPathToFile;

		/// <summary>
		///	increment this each time file format gets updated
		/// </summary>
		public string fileVersion = "1";

		public string saveDate;
		/// <summary>
		/// File save time so recent list can sort by recent first
		/// </summary>
		public long timeTicks;

		[DefaultValue( "English (EN)" )]
		[JsonProperty( DefaultValueHandling = DefaultValueHandling.Populate )]
		public string languageID { get; set; }
		public MissionProperties missionProperties { get; set; }

		public ObservableCollection<MapSection> mapSections { get; set; }
		public ObservableCollection<Trigger> globalTriggers { get; set; }
		public ObservableCollection<MissionEvent> globalEvents { get; set; }

		[JsonConverter( typeof( MapEntityConverter ) )]
		public ObservableCollection<IMapEntity> mapEntities { get; set; }
		public ObservableCollection<EnemyGroupData> initialDeploymentGroups { get; set; }
		public ObservableCollection<EnemyGroupData> reservedDeploymentGroups { get; set; }
		public ObservableCollection<EventGroup> eventGroups { get; set; }
		public ObservableCollection<EntityGroup> entityGroups { get; set; }
		public ObservableCollection<CustomToon> customCharacters { get; set; }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public Mission()
		{
			//defaults
			missionGUID = Guid.NewGuid();
			fileName = "";
			//relativePath = "";
			fullPathToFile = "";
			languageID = "English (EN)";
			saveDate = DateTime.Now.ToString( "M/d/yyyy" );
			timeTicks = DateTime.Now.Ticks;
			missionProperties = new MissionProperties();

			mapSections = new();
			globalTriggers = new();
			globalEvents = new();
			mapEntities = new();
			initialDeploymentGroups = new();
			reservedDeploymentGroups = new();
			eventGroups = new();
			entityGroups = new();
			customCharacters = new();
		}

		public void InitDefaultData()
		{
			MapSection mapSection = new()
			{
				name = "Start Section",
				GUID = Guid.Parse( "11111111-1111-1111-1111-111111111111" ),//Guid.Empty,
				canRemove = false,
				isActive = true
			};
			mapSection.Init();

			mapSections.Add( mapSection );

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
			//mapSections.Add( new() { name = "blah" } );
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

		public bool EventExists( Guid guid )
		{
			bool g = globalEvents.Any( x => x.GUID == guid );
			bool m = mapSections.Any( x => x.missionEvents.Any( xt => xt.GUID == guid ) );
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

		public MissionEvent GetEventFromGUID( Guid guid )
		{
			if ( globalEvents.Any( x => x.GUID == guid ) )
				return globalEvents.First( x => x.GUID == guid );
			else if ( mapSections.Any( x => x.missionEvents.Any( xt => xt.GUID == guid ) ) )
				return mapSections.First( x => x.missionEvents.Any( xt => xt.GUID == guid ) ).missionEvents.First( x => x.GUID == guid );
			else
				return null;
		}

		public bool EntityExists( Guid guid )
		{
			if ( guid == Guid.Empty )
				return true;
			return mapEntities.Any( x => x.GUID == guid );
		}

		public IMapEntity GetEntityFromGUID( Guid guid )
		{
			return mapEntities.Where( x => x.GUID == guid ).FirstOr( null );
		}

		/// <summary>
		/// returns concatenation of global events + all map section events
		/// </summary>
		public List<MissionEvent> GetAllEvents()
		{
			return globalEvents.Concat( mapSections.SelectMany( x => x.missionEvents ) ).ToList();
		}

		public List<Trigger> GetAllTriggers()
		{
			return globalTriggers.Concat( mapSections.SelectMany( x => x.triggers ) ).ToList();
		}
	}
}
