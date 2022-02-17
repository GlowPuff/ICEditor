using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

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
		}

		private void ciButton_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			GenericTextDialog dlg = new GenericTextDialog( "Custom Instructions", Utils.mainWindow.mission.missionProperties.customInstructionText );
			dlg.textHint = "Leave this completely empty to UNSET Custom Instructions.";
			dlg.ShowDialog();

			Utils.mainWindow.mission.missionProperties.customInstructionText = dlg.theText;

			ciInfo.Text = string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.customInstructionText ) ? "Text Not Set" : "Text Set";
		}

		public void Refresh()
		{
			DataContext = Utils.mainWindow.mission.missionProperties;

			allyCB.ItemsSource = Utils.allyData;
			fixedAllyCB.ItemsSource = Utils.allyData;
			bannedAllyCB.ItemsSource = Utils.allyData;
			heroCB.ItemsSource = Utils.heroData;
			var items = from e in Utils.mainWindow.mission.globalEvents where e.isGlobal select e;
			eventCB.ItemsSource = Utils.mainWindow.localEvents;
			mTypeCB.ItemsSource = Enum.GetNames<MissionType>();

			ciInfo.Text = string.IsNullOrEmpty( Utils.mainWindow.mission.missionProperties.customInstructionText ) ? "Text Not Set" : "Text Set";

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

		private void eventCB_GotFocus( object sender, System.Windows.RoutedEventArgs e )
		{
			eventCB.GotFocus -= eventCB_GotFocus;
			eventCB.ItemsSource = Utils.mainWindow.localEvents;
			eventCB.GotFocus += eventCB_GotFocus;
		}

		private void descriptionBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "Mission Description", Utils.mainWindow.mission.missionProperties.missionDescription );
			dlg.ShowDialog();
			Utils.mainWindow.mission.missionProperties.missionDescription = dlg.theText;
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
	}
}
