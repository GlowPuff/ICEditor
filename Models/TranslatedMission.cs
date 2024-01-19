using System;
using System.Collections.Generic;

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
	public class TranslatedMission
	{
		public string languageID;
		public TranslatedMissionProperties missionProperties;
		public List<TranslatedEvents> events;
		public List<TranslatedMapEntity> mapEntities;
		public List<TranslatedInitialGroup> initialGroups;
	}

	public class TranslatedMissionProperties
	{
		public string missionName, missionDescription, missionInfo, campaignName, startingObjective, repositionOverride, additionalMissionInfo;
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

	public class TranslatedEvents
	{
		public Guid GUID;
		public string eventText;
		public List<ITranslatedEventAction> eventActions = new();
	}

	// translated event actions

	public class TranslatedModifyMapEntity : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }// { get => EventActionType.M2; }

		public string theText;
		public List<TranslatedGUIDText> buttonList = new();
	}

	public class TranslatedEnemyDeployment : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }//{ get => EventActionType.D1; }

		//customText in the JSON is "custom instruction" in the model
		public string enemyName, customText, modification, repositionInstructions;
	}

	public class TranslatedInputPrompt : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }//{ get => EventActionType.G9; }

		public string mainText, failText;
		public List<TranslatedGUIDText> inputList = new();
	}

	public class TranslatedTextBox : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string tbText;
	}

	public class TranslatedChangeMissionInfo : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string theText;
	}

	public class TranslatedChangeObjective : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string shortText, longText;
	}

	public class TranslatedQuestionPrompt : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string mainText;
		public List<TranslatedGUIDText> buttonList = new();
	}

	public class TranslatedAllyDeployment : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string customName;
	}

	public class TranslatedChangeGroupInstructions : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string newInstructions;
	}

	public class TranslatedChangeRepositionInstructions : ITranslatedEventAction
	{
		public Guid GUID { get; set; }
		public EventActionType eventActionType { get; set; }

		public string repositionText;
	}
}
