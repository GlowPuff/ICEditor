﻿using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Imperial_Commander_Editor
{
	public class CustomToon : ObservableObject
	{
		//these properties don't change, even when copying from another Deployment Group
		public Guid customCharacterGUID { get; set; }
		string _cardName, _cardSubName, _cardID, _outlineColor;

		CharacterType _characterType; //deploy as
		Thumbnail _thumbnail;
		DeploymentCard _deploymentCard;
		GroupPriorityTraits _groupPriorityTraits;
		string _groupAttack, _groupDefense;
		CardInstruction _cardInstruction;
		EnemyBonusEffect _bonusEffect;
		Factions _faction;
		bool _canRedeploy, _canReinforce, _canBeDefeated, _useThreatMultiplier;

		//update the embedded DG's name/subname and id when it changes
		public string cardName { get => _cardName; set { SetProperty( ref _cardName, value ); deploymentCard.name = value; } }
		public string cardSubName { get => _cardSubName; set { SetProperty( ref _cardSubName, value ); deploymentCard.subname = value; } }
		public string cardID { get => _cardID; set { SetProperty( ref _cardID, value ); deploymentCard.id = value; } }

		public string groupAttack
		{
			get => _groupAttack;
			set
			{
				SetProperty( ref _groupAttack, value );
				deploymentCard.attacks = Utils.ParseCustomDice( value.Split( ' ' ) );
			}
		}
		public string groupDefense
		{
			get => _groupDefense;
			set
			{
				SetProperty( ref _groupDefense, value );
				deploymentCard.defense = Utils.ParseCustomDice( value.Split( ' ' ) );
			}
		}
		public string outlineColor { get => _outlineColor; set { SetProperty( ref _outlineColor, value ); } }
		public GroupPriorityTraits groupPriorityTraits { get => _groupPriorityTraits; set => SetProperty( ref _groupPriorityTraits, value ); }

		public DeploymentCard deploymentCard { get => _deploymentCard; set => SetProperty( ref _deploymentCard, value ); }

		public CharacterType characterType
		{
			get => _characterType;
			set => SetProperty( ref _characterType, value );
		}
		public Thumbnail thumbnail
		{
			get => _thumbnail;
			set => SetProperty( ref _thumbnail, value );
		}
		public EnemyBonusEffect bonusEffect
		{
			get => _bonusEffect;
			set => SetProperty( ref _bonusEffect, value );
		}
		public CardInstruction cardInstruction
		{
			get => _cardInstruction;
			set => SetProperty( ref _cardInstruction, value );
		}
		public bool canRedeploy
		{
			get => _canRedeploy;
			set => SetProperty( ref _canRedeploy, value );
		}
		public bool canReinforce
		{
			get => _canReinforce;
			set => SetProperty( ref _canReinforce, value );
		}
		public bool canBeDefeated
		{
			get => _canBeDefeated;
			set => SetProperty( ref _canBeDefeated, value );
		}
		public bool useThreatMultiplier
		{
			get => _useThreatMultiplier;
			set => SetProperty( ref _useThreatMultiplier, value );
		}
		public Factions faction
		{
			get => _faction;
			set
			{
				SetProperty( ref _faction, value );
				deploymentCard.faction = value.ToString();
			}
		}

		public CustomToon()
		{
			deploymentCard = new();
		}

		public static CustomToon ImportFrom( CustomToon toon )
		{
			CustomToon newToon = new();
			newToon = toon;

			string newID = Utils.GetAvailableCustomToonID();
			newToon.deploymentCard.id = newID;
			newToon.cardID = newID;
			return newToon;
		}

		public void Create()
		{
			customCharacterGUID = Guid.NewGuid();

			deploymentCard = new()
			{
				name = "New Character",
				subname = "",
				//assign a free custom ID
				id = Utils.GetAvailableCustomToonID(),
				expansion = "Core",
				isElite = false,
				cost = 3,
				rcost = 1,
				size = 1,
				health = 3,
				speed = 2,
				tier = 2,
				priority = 2,
				fame = 6,
				reimb = 3,
				faction = "Imperial",
				attackType = AttackType.Melee,
				traits = new string[0],
				surges = new string[] { "{B}: Bleed", "{B}: Focus", "{B}: Pierce 2" },
				keywords = new string[] { "+2 {H}", "Habitat: Snow" },
				abilities = new GroupAbility[] { new() { name = "Composite Plating", text = "While defending, if the attacker is 4 or more spaces away, apply +1 {G} to the defense roll.\nEfficient Travel:Snowtroopers (Elite) ignores additional movement point costs for difficult terrain and hostile figures." } },
				miniSize = FigureSize.Small1x1//Small1x1, Medium1x2, Large2x2, Huge2x3
			};

			//default properties
			cardName = deploymentCard.name;
			cardSubName = deploymentCard.subname;
			cardID = deploymentCard.id;
			outlineColor = "Gray";
			groupPriorityTraits = new();
			characterType = CharacterType.Imperial;
			faction = Factions.Imperial;
			useThreatMultiplier = false;
			canRedeploy = canReinforce = canBeDefeated = true;
			groupAttack = "1Blue 2Yellow";
			groupDefense = "1White 1Black";
			bonusEffect = new()
			{
				bonusID = cardID,
				effects = new()
				{
					"CHARGE: The first time this figure attacks or uses Trample, add 1 blue die to its dice pool.",
					"CRUSH: Each Rebel that suffers {H} during this activation also becomes Weakened."
				}
			};
			cardInstruction = new()
			{
				instID = cardID,
				instName = cardName,
				content = new()
				{
					new(){instruction = new(){"{-} MISSILE SALVO: This figure’s attacks do not require line of sight or Accuracy.", "{A} Move 2 to reposition 4."}},
					new(){instruction = new(){"This is a second randomized Instruction Group", "Separate randomized Instruction Groups with ***"}},
					new(){instruction = new(){"This is a third randomized Instruction Group", "When this character Activates, one of these 3 Instruction Groups will be randomly chosen to Activate with"}}
				}
			};
			//default thumbnail
			thumbnail = Utils.thumbnailData.NoneThumb;
		}

		/// <summary>
		/// Copies embedded DeploymnentCard data to this objects properties (cost, size, etc)
		/// </summary>
		public void CopyFrom( DeploymentCard card )
		{
			//set these first, because these 2 properties also force set the deploymentCard values
			groupAttack = groupDefense = "";
			//now make the card copy
			deploymentCard = card.Copy();
			//keep the original ID, name and subname of this toon, do NOT use the copied card's data
			deploymentCard.id = cardID;
			deploymentCard.name = cardName;
			deploymentCard.subname = cardSubName;
			//set groupAttack from the copied card
			HashSet<DiceColor> colors = new();
			for ( int i = 0; i < card.attacks.Length; i++ )
				colors.Add( card.attacks[i] );
			//set instructions from the copied card
			cardInstruction = Utils.enemyInstructions.Where( x => x.instID == card.id ).FirstOr( null );
			//set bonuses from the copied card
			bonusEffect = Utils.enemyBonusEffects.Where( x => x.bonusID == card.id ).FirstOr( null );
			//get number of each dice color
			foreach ( var c in colors )
			{
				int n = card.attacks.Count( x => x == c );
				groupAttack += $"{n}{c} ";
			}
			//set groupDefense from the copied card
			colors = new();
			for ( int i = 0; i < card.defense.Length; i++ )
				colors.Add( card.defense[i] );
			//get number of each dice color
			foreach ( var c in colors )
			{
				int n = card.defense.Count( x => x == c );
				groupDefense += $"{n}{c} ";
			}
			//trim values
			groupAttack = groupAttack.Trim();
			groupDefense = groupDefense.Trim();
		}
	}
}
