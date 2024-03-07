using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for MissionPropsPanel.xaml
	/// </summary>
	public partial class MissionPropsPanel : UserControl, INotifyPropertyChanged
	{
		DeploymentCard _selectedBanGroupRemove;
		DeploymentCard _selectedBanGroupAdd;

		public List<DeploymentCard> deploymentGroups { get { return Utils.enemyData; } }
		public ObservableCollection<DeploymentCard> bannedGroups { get; set; } = new();
		public DeploymentCard selectedBanGroupRemove { get { return _selectedBanGroupRemove; } set { _selectedBanGroupRemove = value; PC(); } }
		public DeploymentCard selectedBanGroupAdd { get { return _selectedBanGroupAdd; } set { _selectedBanGroupAdd = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public MissionPropsPanel()
		{
			InitializeComponent();
			DataContext = null;

			missionIDCB.Items.Add( "Custom" );
			for ( int i = 1; i <= 32; i++ )
				missionIDCB.Items.Add( "Core " + i );
			for ( int i = 1; i <= 6; i++ )
				missionIDCB.Items.Add( "Bespin " + i );
			for ( int i = 1; i <= 16; i++ )
				missionIDCB.Items.Add( "Empire " + i );
			for ( int i = 1; i <= 16; i++ )
				missionIDCB.Items.Add( "Hoth " + i );
			for ( int i = 1; i <= 16; i++ )
				missionIDCB.Items.Add( "Jabba " + i );
			for ( int i = 1; i <= 6; i++ )
				missionIDCB.Items.Add( "Lothal " + i );
			for ( int i = 1; i <= 6; i++ )
				missionIDCB.Items.Add( "Twin " + i );
			for ( int i = 1; i <= 40; i++ )
				missionIDCB.Items.Add( "Other " + i );
		}

		private void ciButton_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			GenericTextDialog dlg = new GenericTextDialog( "Custom Instructions", Utils.mainWindow.mission.missionProperties.missionInfo );
			dlg.textHint = "Leave this completely empty to UNSET Custom Instructions.";
			dlg.ShowDialog();

			Utils.mainWindow.mission.missionProperties.missionInfo = dlg.theText;

			ciInfo.Text = string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.missionInfo ) ? "Text Not Set" : "Text Set";
		}

		public void Refresh()
		{
			DataContext = Utils.mainWindow.mission.missionProperties;

			allyCB.ItemsSource = Utils.allyData;
			fixedAllyCB.ItemsSource = Utils.allyData;
			bannedAllyCB.ItemsSource = Utils.allyNoCustomData;
			heroCB.ItemsSource = Utils.heroData;
			var items = from e in Utils.mainWindow.mission.globalEvents where e.isGlobal select e;
			eventCB.ItemsSource = Utils.mainWindow.localEvents;
			eventCBRoundLimit.ItemsSource = Utils.mainWindow.localEvents;

			ciInfo.Text = string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.missionInfo ) ? "Text Not Set" : "Text Set";
			SolidColorBrush brush = new( string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.missionInfo ) ? Colors.Red : Colors.LawnGreen );
			ciInfo.Foreground = brush;

			objInfo.Text = string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.startingObjective ) ? "Text Not Set" : "Text Set";
			brush = new( string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.startingObjective ) ? Colors.Red : Colors.LawnGreen );
			objInfo.Foreground = brush;

			brush = new( string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.missionDescription ) ? Colors.Red : Colors.LawnGreen );
			descriptionBtn.Foreground = brush;
			brush = new( string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.additionalMissionInfo ) ? Colors.Red : Colors.LawnGreen );
			addInfoBtn.Foreground = brush;

			remRepoBtn.IsEnabled = Utils.mainWindow.mission.missionProperties.changeRepositionOverride == null ? false : true;

			selectedBanGroupAdd = Utils.enemyData.First( x => x.id == "DG001" );
			foreach ( var item in Utils.mainWindow.mission.missionProperties.bannedGroups )
			{
				bannedGroups.Add( Utils.enemyData.First( x => x.id == item ) );
				selectedBanGroupRemove = Utils.enemyData.First( x => x.id == item );
			}
		}

		private void Validate_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
			}
		}

		private void descriptionBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "Mission Description", Utils.mainWindow.mission.missionProperties.missionDescription );
			dlg.textHint = "This text is shown on the setup screen to describe the Mission.\nWhen left empty, it is automatically filled in when using any Mission ID\nother than 'Custom'.";
			dlg.ShowDialog();
			Utils.mainWindow.mission.missionProperties.missionDescription = dlg.theText;
			SolidColorBrush brush = new( string.IsNullOrEmpty( dlg.theText ) ? Colors.Red : Colors.LawnGreen );
			descriptionBtn.Foreground = brush;
		}

		private void addInfoBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "Additional Information", Utils.mainWindow.mission.missionProperties.additionalMissionInfo );
			dlg.textHint = "A single line, shown on the setup screen, to provide additional\ninformation for the Mission.";
			dlg.ShowDialog();
			Utils.mainWindow.mission.missionProperties.additionalMissionInfo = dlg.theText;
			SolidColorBrush brush = new( string.IsNullOrEmpty( dlg.theText ) ? Colors.Red : Colors.LawnGreen );
			addInfoBtn.Foreground = brush;
		}

		private void infoBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "Mission Info", Utils.mainWindow.mission.missionProperties.missionInfo );
			dlg.textHint = "This text is shown during the mission and can be changed with an Event Action.";
			dlg.ShowDialog();

			Utils.mainWindow.mission.missionProperties.missionInfo = dlg.theText;

			ciInfo.Text = string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.missionInfo ) ? "Text Not Set" : "Text Set";
			SolidColorBrush brush = new( string.IsNullOrEmpty( dlg.theText ) ? Colors.Red : Colors.LawnGreen );
			ciInfo.Foreground = brush;
		}

		private void addmBanBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			Utils.mainWindow.mission.missionProperties.bannedGroups.Add( selectedBanGroupAdd.id );
			bannedGroups.Add( selectedBanGroupAdd );
			selectedBanGroupRemove = selectedBanGroupAdd;
		}

		private void remBanBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			DeploymentCard card = Utils.enemyData.First( x => x.id == selectedBanGroupRemove.id );
			Utils.mainWindow.mission.missionProperties.bannedGroups.Remove( selectedBanGroupRemove.id );
			bannedGroups.Remove( card );
			if ( Utils.mainWindow.mission.missionProperties.bannedGroups.Count > 0 )
				selectedBanGroupRemove = Utils.enemyData.First( x => x.id == Utils.mainWindow.mission.missionProperties.bannedGroups[0] );
		}

		private void TextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			selectedBanGroupAdd = deploymentGroups.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );

			//try id
			if ( selectedBanGroupAdd == null )
				selectedBanGroupAdd = deploymentGroups.Where( x => x.id.Contains( filterBox.Text ) ).FirstOr( null );
		}

		private void filterBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				if ( selectedBanGroupAdd != null )
				{
					addmBanBtn_Click( null, null );
					filterBox.Text = "";
				}
			}
		}

		private void objButton_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "Starting Objective", Utils.mainWindow.mission.missionProperties.startingObjective );
			dlg.textHint = "Leave this empty to NOT create a starting Objective.";
			dlg.ShowDialog();

			Utils.mainWindow.mission.missionProperties.startingObjective = dlg.theText;

			objInfo.Text = string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.startingObjective ) ? "Text Not Set" : "Text Set";
			SolidColorBrush brush = new( string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.startingObjective ) ? Colors.Red : Colors.LawnGreen );
			objInfo.Foreground = brush;
		}

		private void chTargetBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new ChangeRepositionDialog( "", EventActionType.GM4, Utils.mainWindow.mission.missionProperties.changeRepositionOverride );
			dlg.ShowDialog();
			Utils.mainWindow.mission.missionProperties.changeRepositionOverride = dlg.eventAction as ChangeReposition;
			remRepoBtn.IsEnabled = true;
		}

		private void remRepoBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			Utils.mainWindow.mission.missionProperties.changeRepositionOverride = null;
			remRepoBtn.IsEnabled = false;
		}

		private void editMultiBanAllyBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new MultipleBannedAlliesDialog();
			dlg.ShowDialog();
		}

		private void addNewEventButton_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null )
			{
				eventCB.ItemsSource = Utils.mainWindow.localEvents;
				Utils.mainWindow.mission.missionProperties.startingEvent = me.GUID;
			}
		}

		private void refreshIdentifier_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			Utils.mainWindow.mission.missionProperties.customMissionIdentifier = Guid.NewGuid().ToString();
		}

		private void propsTabControl_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			e.Handled = true;
		}

		private void addNewEventButtonRoundLimit_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			MissionEvent me = Utils.mainWindow.leftPanel.AddNewEvent();
			if ( me != null )
			{
				eventCBRoundLimit.ItemsSource = Utils.mainWindow.localEvents;
				Utils.mainWindow.mission.missionProperties.roundLimitEvent = me.GUID;
			}
		}

		private void eventCB_DropDownOpened( object sender, EventArgs e )
		{
			eventCB.ItemsSource = Utils.mainWindow.localEvents;
		}

		private void eventCBRoundLimit_DropDownOpened( object sender, EventArgs e )
		{
			eventCBRoundLimit.ItemsSource = Utils.mainWindow.localEvents;
		}
	}
}
