using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public class ProjectItem : IComparable<ProjectItem>
	{
		public string Title { get; set; }
		public string Date { get; set; }
		public string Description { get; set; }
		public string fileName { get; set; }
		public string relativePath { get; set; }
		public string fileVersion { get; set; }
		public long timeTicks { get; set; }

		public int CompareTo( ProjectItem other ) => timeTicks > other.timeTicks ? -1 : timeTicks < other.timeTicks ? 1 : 0;
	}

	public class DeploymentColor
	{
		public string name { get; set; }
		public Color color { get; set; }

		public DeploymentColor( string n, Color c )
		{
			name = n;
			color = c;
		}
	}

	//public class Question
	//{
	//	public string buttonText { get; set; }
	//	public Guid triggerGUID { get; set; }
	//	public Guid eventGUID { get; set; }
	//}

	public class EntityModifier
	{
		public Guid sourceGUID { get; set; }
		public bool hasColor { get; set; }
		public bool hasProperties { get; set; }
		public EntityProperties entityProperties { get; set; }
	}

	public class ButtonAction
	{
		public string buttonText { get; set; }
		public Guid triggerGUID { get; set; }
		public Guid eventGUID { get; set; }
		//public string triggerName { get; set; }
	}

	public class DPData
	{
		public Guid GUID { get; set; }
	}

	public class EnemyGroupData : INotifyPropertyChanged, IHasEventReference, IHasTriggerReference, IHasEntityReference
	{
		CustomInstructionType _customInstructionType;
		string _customText, _cardName, _cardID;
		Guid _defeatedTrigger, _defeatedEvent;
		bool _useGenericMugshot, _useInitialGroupCustomName;

		public Guid GUID { get; set; }
		public string cardName { get { return _cardName; } set { _cardName = value; PC(); } }
		public string cardID { get { return _cardID; } set { _cardID = value; PC(); } }
		public CustomInstructionType customInstructionType { get { return _customInstructionType; } set { _customInstructionType = value; PC(); } }
		public string customText { get { return _customText; } set { _customText = value; PC(); } }
		public ObservableCollection<DPData> pointList { get; set; } = new();
		public GroupPriorityTraits groupPriorityTraits { get; set; }
		public Guid defeatedTrigger { get { return _defeatedTrigger; } set { _defeatedTrigger = value; PC(); } }
		public Guid defeatedEvent { get { return _defeatedEvent; } set { _defeatedEvent = value; PC(); } }
		public bool useGenericMugshot { get { return _useGenericMugshot; } set { _useGenericMugshot = value; PC(); } }
		public bool useInitialGroupCustomName { get { return _useInitialGroupCustomName; } set { _useInitialGroupCustomName = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public EnemyGroupData()
		{

		}

		public EnemyGroupData( DeploymentCard dc, DeploymentPoint dp )
		{
			GUID = Guid.NewGuid();
			cardName = dc.name;
			cardID = dc.id;
			customText = "";
			customInstructionType = CustomInstructionType.Replace;
			groupPriorityTraits = new();
			defeatedTrigger = Guid.Empty;
			defeatedEvent = Guid.Empty;
			useGenericMugshot = false;
			useInitialGroupCustomName = false;
			for ( int i = 0; i < dc.size; i++ )
			{
				pointList.Add( new() { GUID = dp.GUID } );
			}
		}

		public void SetDP( Guid guid )
		{
			int c = pointList.Count;
			pointList.Clear();
			for ( int i = 0; i < c; i++ )
			{
				pointList.Add( new() { GUID = guid } );
			}
		}

		public void UpdateCard( DeploymentCard newcard )
		{
			cardName = newcard.name;
			cardID = newcard.id;

			var oldPoints = pointList.ToArray();
			pointList.Clear();
			for ( int i = 0; i < newcard.size; i++ )
			{
				if ( i < oldPoints.Length )
					pointList.Add( oldPoints[i] );
				else
					pointList.Add( new() { GUID = Guid.Empty } );
			}
		}

		public BrokenRefInfo NotifyEventRemoved( Guid guid, NotifyMode mode )
		{
			if ( defeatedEvent == guid )
			{
				if ( mode == NotifyMode.Update )
					defeatedEvent = Guid.Empty;
				return new()
				{
					itemName = cardName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "Event from [On Defeated]"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo NotifyTriggerRemoved( Guid guid, NotifyMode mode )
		{
			if ( defeatedTrigger == guid )
			{
				if ( mode == NotifyMode.Update )
					defeatedTrigger = Guid.Empty;
				return new()
				{
					itemName = cardName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "Trigger from [On Defeated]"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo NotifyEntityRemoved( Guid guid, NotifyMode mode )
		{
			if ( pointList.Any( x => x.GUID == guid ) )
			{
				if ( mode == NotifyMode.Update )
				{
					foreach ( var dp in pointList )
					{
						if ( dp.GUID == guid )
							dp.GUID = Guid.Empty;
					}
				}
				return new()
				{
					itemName = cardName,
					isBroken = true,
					ownerGuid = GUID,
					brokenGuid = guid,
					details = "Broken Deployment Point(s) reset to Active Deployment Point"
				};
			}
			return new() { isBroken = false };
		}
	}

	public class GitHubResponse
	{
		public string tag_name;
		public string body;
	}

	public class InputRange : INotifyPropertyChanged
	{
		string _theText;
		int _fromValue, _toValue;
		Guid _triggerGUID, _eventGUID;

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public int fromValue { get { return _fromValue; } set { _fromValue = value; PC(); } }
		public int toValue { get { return _toValue; } set { _toValue = value; PC(); } }
		public Guid triggerGUID { get { return _triggerGUID; } set { _triggerGUID = value; PC(); } }
		public Guid eventGUID { get { return _eventGUID; } set { _eventGUID = value; PC(); } }

		public InputRange()
		{

		}
	}

	public class BrokenRefInfo
	{
		/// <summary>
		/// name of the top-most Event/Trigger,Entity that contains this item
		/// </summary>
		public string topOwnerName;
		public NotifyType notifyType;
		public string itemName;
		public bool isBroken;
		public string details;
		/// <summary>
		/// Guid of the object that contains the broken reference
		/// </summary>
		public Guid ownerGuid;
		/// <summary>
		/// Guid of the broken reference
		/// </summary>
		public Guid brokenGuid;
	}
}
