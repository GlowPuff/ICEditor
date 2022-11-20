using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for HealthCheckWindow.xaml
	/// </summary>
	public partial class HealthCheckWindow : Window, INotifyPropertyChanged
	{
		private static MainWindow mainWindow { get { return Utils.mainWindow; } }
		string _infoText;

		public string infoText { get => _infoText; set { _infoText = value; PC(); } }
		public ObservableCollection<BrokenRefInfo> brokenList { get; set; }

		public HealthCheckWindow()
		{
			InitializeComponent();
			DataContext = this;

			infoText = "If this Mission contains missing references, they will be listed below.  Entries are color coded by the type of the affected item.";
			brokenList = new();

			FindBroken();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		void FindBroken()
		{
			//await Task.Run( () =>
			//{
			//List<BrokenRefInfo> theList = new();
			brokenList.Clear();

			//mission starting event
			if ( !Utils.ValidateEvent( mainWindow.mission.missionProperties.startingEvent ) )
			{
				var info = new BrokenRefInfo()
				{
					isBroken = true,
					topLevelNotifyType = NotifyType.StartingEvent,
					topOwnerName = "Mission Properties Tab",
					itemName = "Mission Properties",
					ownerGuid = Guid.Empty,
					details = "Event from Mission 'Starting Event'"
				};
				brokenList.Add( info );
			}
			//initial groups (defeated Event and deployment point)
			foreach ( var group in mainWindow.mission.initialDeploymentGroups )
			{
				var info = group.SelfCheckEvents();
				if ( info.isBroken )
				{
					info.topOwnerName = "Initial Group: " + group.cardName;
					info.topLevelNotifyType = NotifyType.InitialGroup;
					info.ownerGuid = group.GUID;
					brokenList.Add( info );
				}

				info = group.SelfCheckEntities();
				if ( info.isBroken )
				{
					info.topOwnerName = "Initial Group: " + group.cardName;
					info.topLevelNotifyType = NotifyType.InitialGroup;
					info.ownerGuid = group.GUID;
					brokenList.Add( info );
				}
			}
			//event groups (event list)
			foreach ( var group in mainWindow.mission.eventGroups )
			{
				var info = group.SelfCheckEvents();
				if ( info.isBroken )
				{
					info.topOwnerName = "Event Group: " + group.name;
					info.topLevelNotifyType = NotifyType.EventGroup;
					info.ownerGuid = group.GUID;
					brokenList.Add( info );
				}
			}
			//entity groups - (entity list)
			foreach ( var group in mainWindow.mission.entityGroups )
			{
				var info = group.SelfCheckEntities();
				if ( info.isBroken )
				{
					info.topOwnerName = "Entity Group: " + group.name;
					info.topLevelNotifyType = NotifyType.EntityGroup;
					info.ownerGuid = group.GUID;
					brokenList.Add( info );
				}
			}

			//events
			foreach ( var ev in mainWindow.mission.GetAllEvents() )
			{
				//additional triggers
				var einfo = ev.SelfCheckTriggers();
				einfo.topOwnerName = "Event: " + ev.name;
				einfo.ownerGuid = ev.GUID;
				einfo.topLevelNotifyType = NotifyType.Event;
				if ( einfo.isBroken )
					brokenList.Add( einfo );
				//event actions - events
				foreach ( var eva in ev.eventActions.OfType<IHasEventReference>() )
				{
					var info = eva.SelfCheckEvents();
					info.topOwnerName = "Event: " + ev.name;
					info.ownerGuid = ev.GUID;
					info.topLevelNotifyType = NotifyType.Event;
					if ( info.isBroken )
						brokenList.Add( info );
				}
				//event actions - triggers
				foreach ( var eva in ev.eventActions.OfType<IHasTriggerReference>() )
				{
					var info = eva.SelfCheckTriggers();
					info.topOwnerName = "Event: " + ev.name;
					info.ownerGuid = ev.GUID;
					info.topLevelNotifyType = NotifyType.Event;
					if ( info.isBroken )
						brokenList.Add( info );
				}
				//event actions - entities
				foreach ( var eva in ev.eventActions.OfType<IHasEntityReference>() )
				{
					var info = eva.SelfCheckEntities();
					info.topOwnerName = "Event: " + ev.name;
					info.ownerGuid = ev.GUID;
					info.topLevelNotifyType = NotifyType.Event;
					if ( info.isBroken )
						brokenList.Add( info );
				}
			}

			//triggers
			foreach ( var t in mainWindow.mission.GetAllTriggers() )
			{
				var info = t.SelfCheckEvents();
				info.topOwnerName = "Trigger: " + t.name;
				info.topLevelNotifyType = NotifyType.Trigger;
				info.ownerGuid = t.GUID;
				if ( info.isBroken )
					brokenList.Add( info );
			}

			//entities <- this should never trigger unless the json is modified by hand
			foreach ( var en in mainWindow.mission.mapEntities )
			{
				if ( !Utils.ValidateMapEntity( en.GUID ) )
				{
					var info = new BrokenRefInfo()
					{
						isBroken = true,
						topOwnerName = "Mission Map",
						itemName = "Map Entities",
						topLevelNotifyType = NotifyType.Entity,
						ownerGuid = en.GUID,
						details = $"Missing Map Entity: {en.name}"
					};
					brokenList.Add( info );
				}
			}

			//Utils.Log( string.Join( "\n", brokenList.Select( x =>
			//{
			//	return $"{x.topOwnerName} :: {x.itemName} :: {x.details}";
			//} ) ) );

			//} );
		}

		private void closeBtn_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void fixAllBtn_Click( object sender, RoutedEventArgs e )
		{

		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void Window_ContentRendered( object sender, EventArgs e )
		{
			MaxWidth = double.PositiveInfinity;
			MaxHeight = double.PositiveInfinity;
			// Clear the SizeToContent so that it doesn't automatically resize to large datasets
			SizeToContent = SizeToContent.Manual;
		}

		private void editBtn_Click( object sender, RoutedEventArgs e )
		{
			BrokenRefInfo info = ((FrameworkElement)sender).DataContext as BrokenRefInfo;
			switch ( info.topLevelNotifyType )
			{
				case NotifyType.Entity:
					{
						var ent = mainWindow.mission.GetEntityFromGUID( info.ownerGuid );
						if ( ent.hasProperties )
						{
							EditEntityProperties dlg = new( ent.entityProperties );
							dlg.ShowDialog();
						}
						break;
					}
				case NotifyType.Event:
					{
						var ev = mainWindow.mission.GetEventFromGUID( info.ownerGuid );
						mainWindow.leftPanel.SelectEvent( ev );
						break;
					}
				case NotifyType.Trigger:
					{
						var t = mainWindow.mission.GetTriggerFromGUID( info.ownerGuid );
						mainWindow.leftPanel.SelectTrigger( t );
						break;
					}
				case NotifyType.StartingEvent:
					{
						mainWindow.tabControl.SelectedIndex = 0;
						Close();
						break;
					}
				case NotifyType.EventGroup:
					{
						var eg = mainWindow.mission.eventGroups.First( x => x.GUID == info.ownerGuid );
						var dlg = new EventGroupDialog( eg );
						dlg.ShowDialog();
						break;
					}
				case NotifyType.EntityGroup:
					{
						var eg = mainWindow.mission.entityGroups.First( x => x.GUID == info.ownerGuid );
						var dlg = new EntityGroupDialog( eg );
						dlg.ShowDialog();
						break;
					}
				case NotifyType.InitialGroup:
					{
						var group = mainWindow.mission.initialDeploymentGroups.First( x => x.GUID == info.ownerGuid );
						var dlg = new EditInitialGroupDialog( group );
						dlg.ShowDialog();
						break;
					}
			}
		}

		private void refreshBtn_Click( object sender, RoutedEventArgs e )
		{
			FindBroken();
		}
	}
}
