using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class DeploymentCard : INotifyPropertyChanged
	{
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		string _name, _id, _faction, _deploymentOutlineColor, _mugShotPath;
		int _cost, _rcost, _size, _health, _speed, _priority, _tier, _fame, _reimb;
		bool _isElite;
		AttackType _attackType;
		FigureSize _miniSize;
		GroupTraits[] _groupTraits, _preferredTargets;
		string[] _traits, _surges, _keywords;
		GroupAbility[] _abilities;
		CharacterType _characterType;

		//== data from JSON
		public string name { get => _name; set { _name = value; PC(); } }
		public string id { get => _id; set { _id = value; PC(); } }
		public int tier { get => _tier; set { _tier = value; PC(); } }
		public string faction { get => _faction; set { _faction = value; PC(); } }
		public int priority { get => _priority; set { _priority = value; PC(); } }
		public int cost { get => _cost; set { _cost = value; PC(); } }
		public int rcost { get => _rcost; set { _rcost = value; PC(); } }
		public int size { get => _size; set { _size = value; PC(); } }
		public int fame { get => _fame; set { _fame = value; PC(); } }
		public int reimb { get => _reimb; set { _reimb = value; PC(); } }
		public string expansion;
		public string ignored;
		public bool isElite { get => _isElite; set { _isElite = value; PC(); } }
		//==
		public string subname;
		public int health { get => _health; set { _health = value; PC(); } }
		public int speed { get => _speed; set { _speed = value; PC(); } }
		public string[] traits { get => _traits; set { _traits = value; PC(); } }
		public string[] surges { get => _surges; set { _surges = value; PC(); } }
		public string[] keywords { get => _keywords; set { _keywords = value; PC(); } }
		public GroupAbility[] abilities { get => _abilities; set { _abilities = value; PC(); } }
		public DiceColor[] defense;
		public DiceColor[] attacks;
		public AttackType attackType { get => _attackType; set { _attackType = value; PC(); } }
		public FigureSize miniSize { get => _miniSize; set { _miniSize = value; PC(); } }
		public GroupTraits[] groupTraits { get => _groupTraits; set { _groupTraits = value; PC(); } }
		public GroupTraits[] preferredTargets { get => _preferredTargets; set { _preferredTargets = value; PC(); } }
		public string deploymentOutlineColor { get => _deploymentOutlineColor; set { _deploymentOutlineColor = value; PC(); } }
		public string mugShotPath { get => _mugShotPath; set { _mugShotPath = value; PC(); } }
		public CharacterType characterType { get => _characterType; set { _characterType = value; PC(); } }

		public DeploymentCard()
		{

		}

		public DeploymentCard Copy()
		{
			var json = JsonConvert.SerializeObject( this, Formatting.Indented );
			return JsonConvert.DeserializeObject<DeploymentCard>( json );
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
