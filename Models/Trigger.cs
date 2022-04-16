using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class Trigger : INotifyPropertyChanged, ICloneable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		string _name;
		Guid _GUID, _eventGUID;
		int _initialValue, _maxValue;
		bool _isGlobal, _useReset;

		public string name
		{
			get { return _name; }
			set { _name = value; PC(); }
		}
		public Guid GUID
		{
			get { return _GUID; }
			set { _GUID = value; PC(); }
		}
		public bool isGlobal
		{
			get { return _isGlobal; }
			set { _isGlobal = value; PC(); }
		}
		public bool useReset
		{
			get { return _useReset; }
			set { _useReset = value; PC(); }
		}
		public int maxValue
		{
			get { return _maxValue; }
			set { _maxValue = value; PC(); }
		}
		public int initialValue
		{
			get { return _initialValue; }
			set { _initialValue = value; PC(); }
		}
		public Guid eventGUID
		{
			get { return _eventGUID; }
			set { _eventGUID = value; PC(); }
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public object Clone()
		{
			return new Trigger()
			{
				name = this.name,
				GUID = this.GUID,
				isGlobal = this.isGlobal,
				useReset = this.useReset,
				initialValue = this.initialValue,
				eventGUID = this.eventGUID,
				maxValue = this.maxValue,
			};
		}

		public Trigger()
		{
			GUID = Guid.NewGuid();
			eventGUID = Guid.Empty;
			isGlobal = true;
			useReset = false;
			initialValue = 0;
			maxValue = -1;
		}

		public bool Validate()
		{
			if ( !Utils.ValidateTrigger( GUID ) )
			{
				name = "None (Global)";
				GUID = Guid.Empty;
				return false;
			}
			return true;
		}
	}

	public class TriggeredBy : INotifyPropertyChanged
	{
		string _triggerName;
		Guid _triggerGUID;
		int _triggerValue;

		public string triggerName { get { return _triggerName; } set { _triggerName = value; PC(); } }
		//guid of the trigger to listen to
		public Guid triggerGUID { get { return _triggerGUID; } set { _triggerGUID = value; PC(); } }
		//fire the event when trigger is this value
		public int triggerValue { get { return _triggerValue; } set { _triggerValue = value; PC(); } }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public TriggeredBy()
		{

		}

		public TriggeredBy( Trigger t )
		{
			triggerName = t.name;
			triggerGUID = t.GUID;
			triggerValue = 0;
		}
	}

	public class TriggerModifier : INotifyPropertyChanged
	{
		string _triggerName;
		Guid _triggerGUID;
		int _setValue, _modifyValue;

		public string triggerName { get { return _triggerName; } set { _triggerName = value; PC(); } }
		//guid of the trigger to modify
		public Guid triggerGUID { get { return _triggerGUID; } set { _triggerGUID = value; PC(); } }
		//set the trigger to this value, null to disregard
		public int setValue { get { return _setValue; } set { _setValue = value; PC(); } }
		//modify the trigger value by this, null to disregard
		public int modifyValue { get { return _modifyValue; } set { _modifyValue = value; PC(); } }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public TriggerModifier()
		{

		}

		public TriggerModifier( Trigger t )
		{
			triggerName = t.name;
			triggerGUID = t.GUID;
			setValue = -1;
			modifyValue = 0;
		}
	}
}