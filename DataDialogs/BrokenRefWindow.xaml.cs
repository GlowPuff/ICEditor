using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for BrokenRefWindow.xaml
	/// </summary>
	public partial class BrokenRefWindow : Window, INotifyPropertyChanged
	{
		string _infoText, _detailsText;
		Guid brokenGUID;
		NotifyType notifyType;

		public string infoText { get => _infoText; set { _infoText = value; PC(); } }
		public string detailsText { get => _detailsText; set { _detailsText = value; PC(); } }

		public static MainWindow mainWindow { get { return Utils.mainWindow; } }

		public BrokenRefWindow( NotifyType ntype, string message, List<BrokenRefInfo> info, Guid guid, string sourceName )
		{
			InitializeComponent();
			DataContext = this;

			notifyType = ntype;
			brokenGUID = guid;
			infoText = message;
			detailsText = string.Join( "\n", info.Select( x => $"{x.notifyType} Name: {x.topOwnerName}\n\x2192 {x.itemName}: {x.details}\n" ) );

			gbox.Header = Title = $"Broken References Report For {ntype} [{sourceName}]";
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			if ( notifyType == NotifyType.Event )
			{
				foreach ( var group in mainWindow.mission.initialDeploymentGroups )
				{
					group.NotifyEventRemoved( brokenGUID, NotifyMode.Update );
				}
				if ( mainWindow.mission.missionProperties.startingEvent == brokenGUID )
				{
					mainWindow.mission.missionProperties.startingEvent = Guid.Empty;
				}
				foreach ( var item in mainWindow.mission.eventGroups )
				{
					item.NotifyEventRemoved( brokenGUID, NotifyMode.Update );
				}
				foreach ( var item in mainWindow.mission.GetAllEvents().SelectMany( x => x.eventActions ).OfType<IHasEventReference>() )
				{
					item.NotifyEventRemoved( brokenGUID, NotifyMode.Update );
				}
				foreach ( var item in mainWindow.mission.GetAllTriggers().OfType<IHasEventReference>() )
				{
					item.NotifyEventRemoved( brokenGUID, NotifyMode.Update );
				}
				foreach ( var item in mainWindow.mission.mapEntities.OfType<IHasEventReference>() )
				{
					item.NotifyEventRemoved( brokenGUID, NotifyMode.Update );
				}
			}
			else if ( notifyType == NotifyType.Trigger )
			{
				foreach ( var group in mainWindow.mission.initialDeploymentGroups )
				{
					group.NotifyTriggerRemoved( brokenGUID, NotifyMode.Update );
				}
				foreach ( var item in mainWindow.mission.GetAllEvents() )
				{
					item.NotifyTriggerRemoved( brokenGUID, NotifyMode.Update );
				}
				foreach ( var item in mainWindow.mission.GetAllEvents().SelectMany( x => x.eventActions ).OfType<IHasTriggerReference>() )
				{
					item.NotifyTriggerRemoved( brokenGUID, NotifyMode.Update );
				}
				foreach ( var item in mainWindow.mission.mapEntities.OfType<IHasTriggerReference>() )
				{
					item.NotifyTriggerRemoved( brokenGUID, NotifyMode.Update );
				}
			}
			else if ( notifyType == NotifyType.Entity )
			{
				foreach ( var item in mainWindow.mission.entityGroups )
				{
					item.NotifyEntityRemoved( brokenGUID, NotifyMode.Update );
				}
				foreach ( var item in mainWindow.mission.GetAllEvents().SelectMany( x => x.eventActions ).OfType<IHasEntityReference>() )
				{
					item.NotifyEntityRemoved( brokenGUID, NotifyMode.Update );
				}
				foreach ( var item in mainWindow.mission.mapEntities.OfType<IHasEntityReference>() )
				{
					item.NotifyEntityRemoved( brokenGUID, NotifyMode.Update );
				}
			}

			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}
	}
}
