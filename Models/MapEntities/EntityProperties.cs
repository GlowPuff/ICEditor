using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class EntityProperties : INotifyPropertyChanged
	{
		string _entityColor;

		public string name { get; set; }
		public bool isActive { get; set; }
		public string theText { get; set; }
		public string entityColor { get { return _entityColor; } set { _entityColor = value; PC(); } }
		[JsonIgnore]
		public Guid ownerGUID { get; set; }
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
			ownerGUID = me.entityProperties.ownerGUID;
			foreach ( ButtonAction ba in me.entityProperties.buttonActions )
				buttonActions.Add( new()
				{
					buttonText = ba.buttonText,
					triggerGUID = ba.triggerGUID,
					eventGUID = ba.eventGUID,
				} );
		}

		public BrokenRefInfo NotifyEventRemoved( string entityName, Guid eventGuid, Guid owner, NotifyMode mode )
		{
			if ( buttonActions.Any( x => x.eventGUID == eventGuid ) )
			{
				var ranges = new List<string>();

				if ( mode == NotifyMode.Update )
				{
					foreach ( var item in buttonActions )
					{
						if ( item.eventGUID == eventGuid )
							item.eventGUID = Guid.Empty;
					}
				}

				ranges = buttonActions.Where( x => x.eventGUID == eventGuid ).Select( x =>
				{
					return $"'{x.buttonText}'";
				} ).ToList();

				return new()
				{
					topOwnerName = entityName,
					itemName = name,
					isBroken = true,
					ownerGuid = owner,
					brokenGuid = eventGuid,
					details = $"Button(s) affected: '{string.Join( ", ", ranges )}'"
				};
			}
			return new() { isBroken = false };
		}

		public BrokenRefInfo NotifyTriggerRemoved( string entityName, Guid triggerGuid, Guid owner, NotifyMode mode )
		{
			if ( buttonActions.Any( x => x.triggerGUID == triggerGuid ) )
			{
				var ranges = new List<string>();

				if ( mode == NotifyMode.Update )
				{
					foreach ( var item in buttonActions )
					{
						if ( item.triggerGUID == triggerGuid )
							item.triggerGUID = Guid.Empty;
					}
				}

				ranges = buttonActions.Where( x => x.triggerGUID == triggerGuid ).Select( x =>
				{
					return $"'{x.buttonText}'";
				} ).ToList();

				return new()
				{
					topOwnerName = entityName,
					itemName = name,
					isBroken = true,
					ownerGuid = owner,
					brokenGuid = triggerGuid,
					details = $"Button(s) affected: '{string.Join( ", ", ranges )}'"
				};
			}
			return new() { isBroken = false };
		}
	}
}
