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
		int _triggerValue, _initialValue, _setValue, _modifyValue;
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
		public int triggerValue
		{
			get { return _triggerValue; }
			set { _triggerValue = value; PC(); }
		}
		public int initialValue
		{
			get { return _initialValue; }
			set { _initialValue = value; PC(); }
		}
		public int setValue
		{
			get { return _setValue; }
			set { _setValue = value; PC(); }
		}
		public int modifyValue
		{
			get { return _modifyValue; }
			set { _modifyValue = value; PC(); }
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
				triggerValue = this.triggerValue,
				initialValue = this.initialValue,
				setValue = this.setValue,
				modifyValue = this.modifyValue,
				eventGUID = this.eventGUID
			};
		}

		public Trigger()
		{
			GUID = Guid.NewGuid();
			eventGUID = Guid.Empty;
			isGlobal = true;
			useReset = false;
			triggerValue = 0;
			initialValue = 0;
			setValue = 0;
			modifyValue = 0;
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
}
