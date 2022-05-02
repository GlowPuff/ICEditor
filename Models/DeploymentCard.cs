using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class DeploymentCard
	{
		//== data from JSON
		public string name { get; set; }
		public string id { get; set; }
		public int tier;
		public string faction;
		public int priority;
		public int cost;
		public int rcost;
		public int size;
		public int fame;
		public int reimb;
		public string expansion;
		public string ignored;
		public bool isElite;
		//==
		public string subname;
		public int health;
		public int speed;
		public string[] traits;
		public string[] surges;
		public string[] keywords;
		public GroupAbility[] abilities;
		public DiceColor[] defense;
		public DiceColor[] attacks;
		public AttackType attackType;
		public FigureSize miniSize;
		public GroupTraits[] groupTraits;

		public DeploymentCard()
		{

		}

		public static List<DeploymentCard> LoadData( string assetName )
		{
			try
			{
				string json = "";
				var assembly = Assembly.GetExecutingAssembly();
				var resourceName = assembly.GetManifestResourceNames().Single( str => str.EndsWith( assetName ) );
				using ( Stream stream = assembly.GetManifestResourceStream( resourceName ) )
				using ( StreamReader reader = new StreamReader( stream ) )
				{
					json = reader.ReadToEnd();
				}

				return JsonConvert.DeserializeObject<List<DeploymentCard>>( json );
			}
			catch ( JsonReaderException e )
			{
				Utils.Log( $"DeploymentCard::LoadData() ERROR:\r\nError parsing {assetName}" );
				Utils.Log( e.Message );
				throw new Exception( $"DeploymentCard::LoadData() ERROR:\r\nError parsing {assetName}" );
			}
		}
	}

	public class GroupAbility
	{
		public string name;
		public string text;
	}

	public class DCPointer
	{
		public string name { get; set; }
		public string id { get; set; }

		public DCPointer()
		{

		}

		public DCPointer( DeploymentCard dc )
		{
			name = dc.name;
			id = dc.id;
		}
	}
}
