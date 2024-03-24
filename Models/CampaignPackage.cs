using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class CampaignPackage : ObservableObject
	{
		string _campaignName, _campaignInstructions, _campaignIconName;
		BitmapImage _bmpImage;

		public string packageVersion;
		public Guid GUID;
		public string campaignName { get => _campaignName; set { SetProperty( ref _campaignName, value ); } }
		public string campaignInstructions { get => _campaignInstructions; set { SetProperty( ref _campaignInstructions, value ); } }
		public string campaignIconName { get => _campaignIconName; set { SetProperty( ref _campaignIconName, value ); } }
		[JsonIgnore]
		public BitmapImage bmpImage { get => _bmpImage; set { SetProperty( ref _bmpImage, value ); } }
		public ObservableCollection<CampaignMissionItem> campaignMissionItems { get; set; } = new();
		public ObservableCollection<CampaignStructure> campaignStructure { get; set; } = new();
		public ObservableCollection<CampaignTranslationItem> campaignTranslationItems { get; set; } = new();

		[JsonIgnore]
		public byte[] iconBytesBuffer;

		public CampaignPackage()
		{
			packageVersion = "2";

			GUID = Guid.NewGuid();
			campaignIconName = "none.png";
			SetDefaultIcon();
		}

		public void SetDefaultIcon()
		{
			bmpImage = new( new Uri( $"pack://application:,,,/{Assembly.GetEntryAssembly().GetName().Name};component/Assets/Thumbnails/Other/none.png" ) );

			StreamResourceInfo sri = Application.GetResourceStream( new Uri( $"pack://application:,,,/{Assembly.GetEntryAssembly().GetName().Name};component/Assets/Thumbnails/Other/none.png" ) );
			using ( var s = sri.Stream )
			{
				iconBytesBuffer = new byte[s.Length];
				s.Read( iconBytesBuffer, 0, iconBytesBuffer.Length );
			}
		}

		public void SetIcon( string filename )
		{
			iconBytesBuffer = File.ReadAllBytes( filename );
			bmpImage = new( new Uri( filename ) );
		}

		/// <summary>
		/// filename does NOT include the full path
		/// </summary>
		public CampaignMissionItem AddMission( string filename, Mission mission )
		{
			//make sure the Mission's 'filename' property is the same as that of the actual loaded file, because it could have been renamed in the file system by the user
			mission.fileName = filename;

			var item = new CampaignMissionItem() { missionName = mission.missionProperties.missionName, missionGUID = mission.missionGUID, mission = mission, GUID = Guid.NewGuid(), customMissionIdentifier = mission.missionProperties.customMissionIdentifier };
			campaignMissionItems.Add( item );
			return item;
		}

		public CampaignMissionItem ReplaceMission( CampaignMissionItem missionItem, string filename, Mission mission )
		{
			//update any structure using this Mission
			var structure = campaignStructure.Where( x => x.missionID == missionItem.missionGUID.ToString() ).FirstOr( null );
			if ( structure != null )
			{
				structure.missionID = mission.missionGUID.ToString();
				structure.projectItem.Title = mission.missionProperties.missionName;
				structure.projectItem.missionGUID = mission.missionGUID.ToString();
				structure.mission = mission;
				Utils.Log( $"ReplaceMission()::Mission Structure updated with {mission.missionProperties.missionName}" );
			}
			else
				Utils.Log( $"ReplaceMission()::No Mission Structure found with GUID={mission.missionGUID}" );

			//update the mission pool item itself

			//make sure the Mission's 'filename' property is the same as that of the actual loaded file, because it could have been renamed in the file system by the user
			mission.fileName = filename;

			missionItem.missionName = mission.missionProperties.missionName;
			missionItem.missionGUID = mission.missionGUID;
			missionItem.mission = mission;
			missionItem.customMissionIdentifier = mission.missionProperties.customMissionIdentifier;

			return missionItem;
		}

		public void RemoveMission( CampaignMissionItem item )
		{
			campaignMissionItems.Remove( item );
		}

		public void AddStructure()
		{
			var cs = new CampaignStructure();
			cs.missionSource = MissionSource.None;
			cs.itemTier = new string[] { "1" };
			cs.isAgendaMission = false;
			cs.missionType = MissionType.Story;
			cs.missionID = Guid.Empty.ToString();
			cs.projectItem.Title = "Player's Choice";
			cs.projectItem.missionGUID = Guid.Empty.ToString();
			cs.expansionCode = "Imported";
			cs.hasCustomSetNextEventActions = false;
			cs.mission = null;

			campaignStructure.Add( cs );
		}

		public void RemoveStructure( CampaignStructure cs )
		{
			campaignStructure.Remove( cs );
		}

		/// <summary>
		/// filename = JUST the filename, EXCLUDING the full path
		/// </summary>
		public CampaignTranslationItem AddMissionTranslation( TranslatedMission tm, string filename, CampaignMissionItem missionItem )
		{
			var item = new CampaignTranslationItem()
			{
				translatedMission = tm,
				fileName = filename,
				isInstruction = false,
				assignedMissionGUID = missionItem.missionGUID,
				assignedMissionName = missionItem.missionName
			};
			campaignTranslationItems.Add( item );
			return item;
		}

		public CampaignTranslationItem AddCampaignInfoTranslation( string instruction, string filename )
		{
			var item = new CampaignTranslationItem()
			{
				campaignInstructionTranslation = instruction,
				fileName = filename,
				isInstruction = true,
				assignedMissionName = "Campaign Instructions"
			};
			campaignTranslationItems.Add( item );
			return item;
		}

		public void RemoveTranslation( CampaignTranslationItem tm )
		{
			campaignTranslationItems.Remove( tm );
		}

		public void ValidateMissions()
		{
			if ( campaignMissionItems.Count == 0 )
				campaignStructure.ToList().ForEach( x => x.Reset() );
			else
			{
				var broken = campaignStructure.Where( x => campaignMissionItems.Any( y => y.missionGUID.ToString() != x.missionID ) ).ToList();
				broken.ForEach( x => x.Reset() );
			}
		}
	}

	public class CampaignTranslationItem : ObservableObject
	{
		string _fileName;//filename of the translation
		bool _isInstruction;
		Guid _assignedMissionGUID;
		string _assignedMissionName;

		public string fileName { get => _fileName; set => SetProperty( ref _fileName, value ); }
		public string assignedMissionName { get => _assignedMissionName; set => SetProperty( ref _assignedMissionName, value ); }
		public bool isInstruction { get => _isInstruction; set => SetProperty( ref _isInstruction, value ); }
		public Guid assignedMissionGUID { get => _assignedMissionGUID; set => SetProperty( ref _assignedMissionGUID, value ); }//assigned Mission's GUID

		//store the actual translations for packing as individual files later, but don't serialize it here
		[JsonIgnore]
		public TranslatedMission translatedMission { get; set; }
		[JsonIgnore]
		public string campaignInstructionTranslation { get; set; }
	}

	public class CampaignMissionItem : ObservableObject
	{
		string _missionName;
		bool _hasTranslation;

		public Guid GUID { get; set; }//GUID of this object, not the mission
		public Guid missionGUID { get; set; }
		public string customMissionIdentifier { get; set; } = Guid.Empty.ToString();
		public string missionName { get => _missionName; set { SetProperty( ref _missionName, value ); } }

		//store the actual mission for packing as an individual file later, but don't serialize it here
		[JsonIgnore]
		public Mission mission { get; set; }
		[JsonIgnore]
		public bool hasTranslation { get => _hasTranslation; set => SetProperty( ref _hasTranslation, value ); }
	}

	public class CampaignStructure : ObservableObject
	{
		MissionType _missionType;
		int _threatLevel;
		bool _isAgendaMission, _hasCustomSetNextEventActions;
		string _customMissionIdentifier;

		public string missionID;//in ICE, this is the missionGUID of the CampaignMissionItem.mission as a string
		public string[] itemTier;
		public string expansionCode;

		//set in campaign UI - not loaded from campaign structure JSONs
		public bool isItemChecked = false;
		public bool isForced = false;
		public AgendaType agendaType = AgendaType.NotSet;
		public MissionSource missionSource = MissionSource.None;
		public Guid structureGUID;
		public bool canModify = true;
		public ProjectItem projectItem { get; set; }

		public string customMissionIdentifier { get => _customMissionIdentifier; set { SetProperty( ref _customMissionIdentifier, value ); } }
		public bool hasCustomSetNextEventActions { get => _hasCustomSetNextEventActions; set { SetProperty( ref _hasCustomSetNextEventActions, value ); } }
		public MissionType missionType { get => _missionType; set { SetProperty( ref _missionType, value ); } }
		public int threatLevel { get => _threatLevel; set { SetProperty( ref _threatLevel, value ); } }
		public bool isAgendaMission { get => _isAgendaMission; set { SetProperty( ref _isAgendaMission, value ); } }

		[JsonIgnore]
		public string tier
		{
			get => itemTier.Aggregate( ( acc, cur ) => $"{acc},{cur}" );
			set
			{
				//itemTier = value.Split( (',') );
				SetProperty( ref itemTier, value.Split( (',') ) );
			}
		}
		[JsonIgnore]
		public Mission mission;

		public CampaignStructure()
		{
			structureGUID = Guid.NewGuid();
			projectItem = new ProjectItem();
		}

		public void Reset()
		{
			projectItem.Title = "Player's Choice";
			missionID = Guid.Empty.ToString();
			customMissionIdentifier = Guid.Empty.ToString();
			missionSource = MissionSource.None;
			projectItem.missionGUID = Guid.Empty.ToString();
			hasCustomSetNextEventActions = false;
			mission = null;
		}
	}
}
