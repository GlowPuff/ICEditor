using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public interface ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		string eaName { get; set; }
	}

	public class TranslatedEntityProperties
	{
		public string entityName { get; set; }
		public Guid GUID { get; set; }
		public string theText { get; set; } = "";
		public List<TranslatedGUIDText> buttonList { get; set; } = new();
		public TranslatedEntityProperties() { }
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
					entityName = e.name,
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
				var dg = new TranslatedInitialGroup() { customInstructions = item.customText, cardName = item.cardName };
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
		public string cardName;
		public string customInstructions;
	}

	public class TranslatedMapEntity
	{
		public string entityName;
		public Guid GUID;
		public string mainText;
		public List<TranslatedGUIDText> buttonList = new();
	}

	public class TranslatedEvent
	{
		public string eventName;
		public Guid GUID;
		public string eventText;
		[JsonConverter( typeof( TranslatedEventActionConverter ) )]
		public List<ITranslatedEventAction> eventActions;

		public TranslatedEvent() { }

		public TranslatedEvent( MissionEvent ev )
		{
			eventName = ev.name;
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
					case EventActionType.D6:
						eventActions.Add( new TranslatedCustomEnemyDeployment( item ) );
						break;
					case EventActionType.GM2:
						eventActions.Add(new TranslatedChangeTarget(item));
						break;
				}
			}
		}
	}

	#region event action models
	public class TranslatedModifyMapEntity : ITranslatedEventAction//M2
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string eaName { get; set; }

		public List<TranslatedEntityProperties> translatedEntityProperties = new();

		public TranslatedModifyMapEntity()
		{

		}

		public TranslatedModifyMapEntity( IEventAction ea )
		{
			eaName = ea.displayName;
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			foreach ( var item in ((ModifyMapEntity)ea).entitiesToModify )
			{
				var tep = new TranslatedEntityProperties();
				tep.entityName = item.entityProperties.name;
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

	public class TranslatedEnemyDeployment : ITranslatedEventAction//D1
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string eaName { get; set; }

		//customText in the JSON is "custom instruction" in the model
		public string enemyName, customText, modification, repositionInstructions;

		public TranslatedEnemyDeployment()
		{

		}

		public TranslatedEnemyDeployment( IEventAction ea )
		{
			eaName = ea.displayName;
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			enemyName = ((EnemyDeployment)ea).enemyName;
			customText = ((EnemyDeployment)ea).enemyGroupData.customText;
			modification = ((EnemyDeployment)ea).modification;
			repositionInstructions = ((EnemyDeployment)ea).repositionInstructions;
		}
	}

	public class TranslatedInputPrompt : ITranslatedEventAction//G9
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string eaName { get; set; }

		public string mainText, failText;
		public List<TranslatedGUIDText> inputList = new();

		public TranslatedInputPrompt()
		{

		}

		public TranslatedInputPrompt( IEventAction ea )
		{
			eaName = ea.displayName;
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
		public string eaName { get; set; }

		public string tbText;

		public TranslatedTextBox()
		{

		}

		public TranslatedTextBox( IEventAction ea )
		{
			eaName = ea.displayName;
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			tbText = ((ShowTextBox)ea).theText;
		}
	}

	public class TranslatedChangeMissionInfo : ITranslatedEventAction//G2
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string eaName { get; set; }

		public string theText;

		public TranslatedChangeMissionInfo()
		{

		}

		public TranslatedChangeMissionInfo( IEventAction ea )
		{
			eaName = ea.displayName;
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			theText = ((ChangeMissionInfo)ea).theText;
		}
	}

	public class TranslatedChangeObjective : ITranslatedEventAction//G3
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string eaName { get; set; }

		public string shortText, longText;

		public TranslatedChangeObjective()
		{

		}

		public TranslatedChangeObjective( IEventAction ea )
		{
			eaName = ea.displayName;
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
		public string eaName { get; set; }

		public string mainText;
		public List<TranslatedGUIDText> buttonList = new();

		public TranslatedQuestionPrompt()
		{

		}

		public TranslatedQuestionPrompt( IEventAction ea )
		{
			eaName = ea.displayName;
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
		public string eaName { get; set; }

		public string customName;

		public TranslatedAllyDeployment()
		{

		}

		public TranslatedAllyDeployment( IEventAction ea )
		{
			eaName = ea.displayName;
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			customName = ((AllyDeployment)ea).allyName;

		}
	}

	public class TranslatedChangeGroupInstructions : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string eaName { get; set; }

		public string newInstructions;

		public TranslatedChangeGroupInstructions()
		{

		}

		public TranslatedChangeGroupInstructions( IEventAction ea )
		{
			eaName = ea.displayName;
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			newInstructions = ((ChangeInstructions)ea).theText;
		}
	}

	public class TranslatedChangeRepositionInstructions : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string eaName { get; set; }

		public string repositionText;

		public TranslatedChangeRepositionInstructions()
		{

		}

		public TranslatedChangeRepositionInstructions( IEventAction ea )
		{
			eaName = ea.displayName;
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			repositionText = ((ChangeReposition)ea).theText;
		}
	}

	public class TranslatedChangeTarget : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string eaName { get; set; }

		public string otherTarget;

		public TranslatedChangeTarget()
		{

		}

		public TranslatedChangeTarget(IEventAction ea)
		{
			eaName = ea.displayName;
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;
			otherTarget = ((ChangeTarget)ea).otherTarget;
		}
	}

	public class TranslatedCustomEnemyDeployment : ITranslatedEventAction//D6
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }
		public string eaName { get; set; }

		public string repositionInstructions, surges, bonuses, keywords, abilities, customText, cardName;

		public TranslatedCustomEnemyDeployment()
		{

		}

		public TranslatedCustomEnemyDeployment( IEventAction ea )
		{
			eaName = ea.displayName;
			GUID = ea.GUID;
			eventActionType = ea.eventActionType;

			repositionInstructions = ((CustomEnemyDeployment)ea).repositionInstructions;
			cardName = ((CustomEnemyDeployment)ea).enemyGroupData.cardName;
			surges = ((CustomEnemyDeployment)ea).surges;
			bonuses = ((CustomEnemyDeployment)ea).bonuses;
			keywords = ((CustomEnemyDeployment)ea).keywords;
			abilities = ((CustomEnemyDeployment)ea).abilities;
			customText = ((CustomEnemyDeployment)ea).enemyGroupData.customText;
		}
	}
	#endregion
}
