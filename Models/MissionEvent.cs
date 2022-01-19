using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class MissionEvent : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		string _eventName, _eventText, _allyDefeated, _heroWounded, _heroWithdraws, _activationOf;
		int _startOfRound, _endOfRound;
		bool _useStartOfRound, _useEndOfRound, _useStartOfEachRound, _useEndOfEachRound, _useAllGroupsDefeated, _useAllHeroesWounded, _useAllyDefeated, _useHeroWounded, _useHeroWithdraws, _useActivation, _useEndOfCurrentRound, _behaviorAll;
		bool _isGlobal;

		public Guid GUID { get; set; }
		public bool isGlobal
		{
			get { return _isGlobal; }
			set { _isGlobal = value; PC(); }
		}
		public string name
		{
			get { return _eventName; }
			set { _eventName = value; PC(); }
		}
		public string eventText
		{
			get { return _eventText; }
			set { _eventText = value; PC(); }
		}
		public string allyDefeated
		{
			get { return _allyDefeated; }
			set { _allyDefeated = value; PC(); }
		}
		public string heroWounded
		{
			get { return _heroWounded; }
			set { _heroWounded = value; PC(); }
		}
		public string heroWithdraws
		{
			get { return _heroWithdraws; }
			set { _heroWithdraws = value; PC(); }
		}
		public string activationOf
		{
			get { return _activationOf; }
			set { _activationOf = value; PC(); }
		}
		public int startOfRound
		{
			get { return _startOfRound; }
			set { _startOfRound = value; PC(); }
		}
		public int endOfRound
		{
			get { return _endOfRound; }
			set { _endOfRound = value; PC(); }
		}
		public bool useStartOfRound
		{
			get { return _useStartOfRound; }
			set { _useStartOfRound = value; PC(); }
		}
		public bool useEndOfRound
		{
			get { return _useEndOfRound; }
			set { _useEndOfRound = value; PC(); }
		}
		public bool useEndOfCurrentRound
		{
			get { return _useEndOfCurrentRound; }
			set { _useEndOfCurrentRound = value; PC(); }
		}
		public bool useStartOfEachRound
		{
			get { return _useStartOfEachRound; }
			set { _useStartOfEachRound = value; PC(); }
		}
		public bool useEndOfEachRound
		{
			get { return _useEndOfEachRound; }
			set { _useEndOfEachRound = value; PC(); }
		}
		public bool useAllGroupsDefeated
		{
			get { return _useAllGroupsDefeated; }
			set { _useAllGroupsDefeated = value; PC(); }
		}
		public bool useAllHeroesWounded
		{
			get { return _useAllHeroesWounded; }
			set { _useAllHeroesWounded = value; PC(); }
		}
		public bool useAllyDefeated
		{
			get { return _useAllyDefeated; }
			set { _useAllyDefeated = value; PC(); }
		}
		public bool useHeroWounded
		{
			get { return _useHeroWounded; }
			set { _useHeroWounded = value; PC(); }
		}
		public bool useHeroWithdraws
		{
			get { return _useHeroWithdraws; }
			set { _useHeroWithdraws = value; PC(); }
		}
		public bool useActivation
		{
			get { return _useActivation; }
			set { _useActivation = value; PC(); }
		}
		public bool behaviorAll { get { return _behaviorAll; } set { _behaviorAll = value; PC(); } }
		public ObservableCollection<Trigger> additionalTriggers { get; set; }

		[JsonConverter( typeof( EventActionConverter ) )]
		public ObservableCollection<IEventAction> eventActions { get; set; }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public MissionEvent()
		{
			GUID = Guid.NewGuid();
			name = "Event Name";
			isGlobal = false;
			eventText = "";
			startOfRound = endOfRound = 1;
			useStartOfRound = useEndOfRound = useStartOfEachRound = useEndOfEachRound = useAllGroupsDefeated = useAllHeroesWounded = useAllyDefeated = useHeroWounded = useHeroWithdraws = useActivation = useEndOfCurrentRound = false;
			behaviorAll = true;
			additionalTriggers = new();
			eventActions = new();

			heroWounded = "Diala Passil";
			heroWithdraws = "Diala Passil";
			allyDefeated = "Luke Skywalker (Hero)";
			activationOf = "DG001";
		}

		public bool Validate()
		{
			if ( !Utils.ValidateEvent( GUID ) )
			{
				name = "None (Global)";
				GUID = Guid.Empty;
				return false;
			}
			return true;
		}
	}
}
