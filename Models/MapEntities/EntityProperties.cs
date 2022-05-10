using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Imperial_Commander_Editor
{
	public class EntityProperties : INotifyPropertyChanged
	{
		string _entityColor;

		public string name { get; set; }
		public bool isActive { get; set; }
		public string theText { get; set; }
		public string entityColor { get { return _entityColor; } set { _entityColor = value; PC(); } }
		public ObservableCollection<ButtonAction> buttonActions { get; set; } = new();

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public EntityProperties()
		{
			isActive = true;
		}

		public void ValidateTriggers()
		{
			for ( int i = 0; i < buttonActions.Count; i++ )
			{
				var t = Utils.mainWindow.mission.GetTriggerFromGUID( buttonActions[i].triggerGUID );
				if ( t == null )
				{
					buttonActions[i].triggerGUID = Guid.Empty;
				}
			}
		}

		public void ValidateEvents()
		{
			for ( int i = 0; i < buttonActions.Count; i++ )
			{
				var t = Utils.mainWindow.mission.GetEventFromGUID( buttonActions[i].eventGUID );
				if ( t == null )
				{
					buttonActions[i].eventGUID = Guid.Empty;
				}
			}
		}

		public void CopyFrom( IMapEntity me )
		{
			name = me.name;
			isActive = me.entityProperties.isActive;
			theText = me.entityProperties.theText;
			entityColor = me.entityProperties.entityColor;
			foreach ( ButtonAction ba in me.entityProperties.buttonActions )
				buttonActions.Add( new()
				{
					buttonText = ba.buttonText,
					triggerGUID = ba.triggerGUID,
					eventGUID = ba.eventGUID,
				} );
		}
	}
}
