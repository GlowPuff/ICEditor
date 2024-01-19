using System;
using System.Collections.Generic;
using System.Linq;

namespace Imperial_Commander_Editor
{
	public interface ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		//public string theText { get; set; }
		public EventActionType eventActionType { get; set; }
	}

	/// <summary>
	/// used for buttons, input prompt items
	/// </summary>
	public class TranslatedGUIDText
	{
		public Guid GUID { get; set; }
		public string theText { get; set; }
	}

	/// <summary>
	/// The main translation container for missions props, events, entities, initial groups
	/// </summary>
	public sealed class TranslatedMission
	{
		public string languageID;
		public TranslatedMissionProperties missionProperties;
		public List<TranslatedEvent> events = new();
		public List<TranslatedMapEntity> mapEntities = new();
		public List<TranslatedInitialGroup> initialGroups = new();

		/// <summary>
		/// make this class only useable from within CreateTranslation()
		/// </summary>
		private TranslatedMission() { }

		public static TranslatedMission CreateTranslation( Mission mission )
		{
			TranslatedMission translatedMission = new();
			translatedMission.languageID = mission.languageID;

			//mission properties
			translatedMission.missionProperties = new( mission );

			//events
			foreach ( var e in mission.GetAllEvents().Where( x => x.GUID != Guid.Empty ) )
			{
				translatedMission.events.Add( new TranslatedEvent( e ) );
			}

			//entities
			foreach ( var e in mission.mapEntities )
			{
				var me = new TranslatedMapEntity()
				{
					GUID = e.GUID,
					mainText = e.entityProperties.theText
				};
				foreach ( var item in e.entityProperties.buttonActions )
				{
					me.buttonList.Add( new() { GUID = item.GUID, theText = item.buttonText } );
				}
				translatedMission.mapEntities.Add( me );
			}

			//initial groups
			foreach ( var item in mission.initialDeploymentGroups )
			{
				var dg = new TranslatedInitialGroup() { customInstructions = item.customText };
				translatedMission.initialGroups.Add( dg );
			}

			return translatedMission;
		}
	}

	public class TranslatedMissionProperties
	{
		public string missionName, missionDescription, missionInfo, campaignName, startingObjective, repositionOverride, additionalMissionInfo;

		public TranslatedMissionProperties()
		{

		}

		public TranslatedMissionProperties( Mission mission )
		{
			missionName = mission.missionProperties.missionName;
			missionDescription = mission.missionProperties.missionDescription;
			missionInfo = mission.missionProperties.missionInfo;
			campaignName = mission.missionProperties.campaignName;
			startingObjective = mission.missionProperties.startingObjective;
			repositionOverride = mission.missionProperties.changeRepositionOverride?.theText;
			additionalMissionInfo = mission.missionProperties.additionalMissionInfo;
		}
	}

	public class TranslatedInitialGroup
	{
		public string customInstructions;
	}

	public class TranslatedMapEntity
	{
		public Guid GUID;
		public string mainText;
		public List<TranslatedGUIDText> buttonList = new();
	}

	public class TranslatedEvent
	{
		public Guid GUID;
		public string eventText;
		public List<ITranslatedEventAction> eventActions;

		public TranslatedEvent() { }

		public TranslatedEvent( MissionEvent ev )
		{
			GUID = ev.GUID;
			eventText = ev.eventText;
			eventActions = new();
			foreach ( var item in ev.eventActions )
			{
				switch ( item.eventActionType )
				{
					case EventActionType.M2:
						eventActions.Add( new TranslatedModifyMapEntity( item ) );
						break;
					case EventActionType.D1:
						eventActions.Add( new TranslatedEnemyDeployment( item ) );
						break;
					case EventActionType.G9:
						eventActions.Add( new TranslatedInputPrompt( item ) );
						break;
					case EventActionType.G7:
						eventActions.Add( new TranslatedTextBox( item ) );
						break;
					case EventActionType.G2:
						eventActions.Add( new TranslatedChangeMissionInfo( item ) );
						break;
					case EventActionType.G3:
						eventActions.Add( new TranslatedChangeObjective( item ) );
						break;
					case EventActionType.G6:
						eventActions.Add( new TranslatedQuestionPrompt( item ) );
						break;
					case EventActionType.D2:
						eventActions.Add( new TranslatedAllyDeployment( item ) );
						break;
					case EventActionType.GM1:
						eventActions.Add( new TranslatedChangeGroupInstructions( item ) );
						break;
					case EventActionType.GM4:
						eventActions.Add( new TranslatedChangeRepositionInstructions( item ) );
						break;
				}
			}
		}
	}

	#region event action models
	public class TranslatedModifyMapEntity : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }// { get => EventActionType.M2; }
		public List<TranslatedEntityProperties> translatedEntityProperties = new();

		public class TranslatedEntityProperties
		{
			public Guid GUID;
			public string theText = "";
			public List<TranslatedGUIDText> buttonList = new();
		}

		public TranslatedModifyMapEntity( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			foreach ( var item in ((ModifyMapEntity)ea).entitiesToModify )
			{
				var tep = new TranslatedEntityProperties();
				tep.GUID = item.GUID;
				tep.theText = item.entityProperties.theText;
				foreach ( var btn in item.entityProperties.buttonActions )
				{
					tep.buttonList.Add( new() { GUID = btn.GUID, theText = btn.buttonText } );
				}
				translatedEntityProperties.Add( tep );
			}
		}
	}

	public class TranslatedEnemyDeployment : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }//{ get => EventActionType.D1; }

		//customText in the JSON is "custom instruction" in the model
		public string enemyName, customText, modification, repositionInstructions;

		public TranslatedEnemyDeployment( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			enemyName = ((EnemyDeployment)ea).enemyName;
			customText = ((EnemyDeployment)ea).enemyGroupData.customText;
			modification = ((EnemyDeployment)ea).modification;
			repositionInstructions = ((EnemyDeployment)ea).repositionInstructions;
		}
	}

	public class TranslatedInputPrompt : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }//{ get => EventActionType.G9; }

		public string mainText, failText;
		public List<TranslatedGUIDText> inputList = new();

		public TranslatedInputPrompt( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			mainText = ((InputPrompt)ea).theText;
			failText = ((InputPrompt)ea).failText;
			foreach ( var item in ((InputPrompt)ea).inputList )
			{
				inputList.Add( new() { GUID = item.GUID, theText = item.theText } );
			}
		}
	}

	public class TranslatedTextBox : ITranslatedEventAction//G7
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string tbText;

		public TranslatedTextBox( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			tbText = ((ShowTextBox)ea).theText;
		}
	}

	public class TranslatedChangeMissionInfo : ITranslatedEventAction//G2
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string theText;

		public TranslatedChangeMissionInfo( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			theText = ((ChangeMissionInfo)ea).theText;
		}
	}

	public class TranslatedChangeObjective : ITranslatedEventAction//G3
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string shortText, longText;

		public TranslatedChangeObjective( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			shortText = ((ChangeObjective)ea).theText;
			longText = ((ChangeObjective)ea).longText;
		}
	}

	public class TranslatedQuestionPrompt : ITranslatedEventAction//G6
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string mainText;
		public List<TranslatedGUIDText> buttonList = new();

		public TranslatedQuestionPrompt( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			mainText = ((QuestionPrompt)ea).theText;
			foreach ( var item in ((QuestionPrompt)ea).buttonList )
			{
				buttonList.Add( new() { GUID = item.GUID, theText = item.buttonText } );
			}
		}
	}

	public class TranslatedAllyDeployment : ITranslatedEventAction//D2
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string customName;

		public TranslatedAllyDeployment( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			customName = ((AllyDeployment)ea).allyName;

		}
	}

	public class TranslatedChangeGroupInstructions : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string newInstructions;

		public TranslatedChangeGroupInstructions( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			newInstructions = ((ChangeInstructions)ea).theText;
		}
	}

	public class TranslatedChangeRepositionInstructions : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string repositionText;

		public TranslatedChangeRepositionInstructions( IEventAction ea )
		{
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			repositionText = ((ChangeReposition)ea).theText;
		}
	}
	#endregion
}
