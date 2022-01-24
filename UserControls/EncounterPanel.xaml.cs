using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EncounterPanel.xaml
	/// </summary>
	public partial class EncounterPanel : UserControl, INotifyPropertyChanged
	{
		bool _buttonEnabled;
		DeploymentPoint _deploymentPoint;
		string _selectedGroup, _customName, _customInstructions;

		public DeploymentPoint deploymentPoint
		{
			get { return _deploymentPoint; }
			set
			{
				_deploymentPoint = value;
				PC();
				buttonEnabled = _deploymentPoint != null;
			}
		}
		public DeploymentPoint modifyDeploymentPoint { get; set; }
		public string selectedGroup { get { return _selectedGroup; } set { _selectedGroup = value; PC(); } }
		public string selectedResGroup { get; set; }
		public ObservableCollection<EnemyGroupData> initialGroups
		{
			get { return Utils.mainWindow.mission.initialDeploymentGroups; }
			set { Utils.mainWindow.mission.initialDeploymentGroups = value; }
		}
		public ObservableCollection<EnemyGroupData> reservedGroups
		{
			get { return Utils.mainWindow.mission.reservedDeploymentGroups; }
			set { Utils.mainWindow.mission.reservedDeploymentGroups = value; }
		}
		public List<DeploymentPoint> allPoints { get; set; }
		public bool buttonEnabled { get { return _buttonEnabled; } set { _buttonEnabled = value; PC(); } }
		public CustomInstructionType customInstructionType { get; set; } = CustomInstructionType.Replace;
		public string customInstructions { get { return _customInstructions; } set { _customInstructions = value; PC(); } }
		public string customName { get { return _customName; } set { _customName = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public EncounterPanel()
		{
			InitializeComponent();

			DataContext = this;
			selectedGroup = "DG001";
			customInstructions = customName = "";
		}

		public void UpdateUI()
		{
			gCB_initial.ItemsSource = Utils.enemyData;
			//dpCB_reserved.ItemsSource = Utils.enemyData;
			dpCB.ItemsSource = Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>();
			allPoints = Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>().ToList();

			//verify dp's exist
			for ( int i = 0; i < initialGroups.Count; i++ )
			{
				for ( int y = 0; y < initialGroups[i].pointList.Count; y++ )
				{
					if ( !Utils.ValidateMapEntity( initialGroups[i].pointList[y].GUID ) )
						initialGroups[i].pointList[y].GUID = Guid.Empty;
				}
			}
		}

		private void addReservedGroupButton_Click( object sender, RoutedEventArgs e )
		{
			var ig = new EnemyGroupData( Utils.enemyData.Where( x => x.id == selectedGroup ).First(), new( Guid.Empty ) );

			ig.customInstructionType = customInstructionType;
			if ( !string.IsNullOrEmpty( customName.Trim() ) )
				ig.cardName = customName.Trim();
			ig.customText = customInstructions;

			Utils.mainWindow.mission.reservedDeploymentGroups.Add( ig );
			customName = customInstructions = "";
		}

		private void addInitialGroupButton_Click( object sender, RoutedEventArgs e )
		{
			var ig = new EnemyGroupData( Utils.enemyData.Where( x => x.id == selectedGroup ).First(), deploymentPoint );

			ig.customInstructionType = customInstructionType;
			if ( !string.IsNullOrEmpty( customName.Trim() ) )
				ig.cardName = customName.Trim();
			ig.customText = customInstructions;

			Utils.mainWindow.mission.initialDeploymentGroups.Add( ig );
			deploymentPoint = null;
			customName = customInstructions = "";
		}

		private void remInitialGroupButton_Click( object sender, RoutedEventArgs e )
		{
			Utils.mainWindow.mission.initialDeploymentGroups.Remove( (sender as FrameworkElement).DataContext as EnemyGroupData );
		}

		private void remReservedGroupButton_Click( object sender, RoutedEventArgs e )
		{
			Utils.mainWindow.mission.reservedDeploymentGroups.Remove( (sender as FrameworkElement).DataContext as EnemyGroupData );
		}

		private void editGroup_Click( object sender, RoutedEventArgs e )
		{
			EditInitialGroupDialog dialog = new EditInitialGroupDialog( (sender as FrameworkElement).DataContext as EnemyGroupData );
			dialog.ShowDialog();
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void editCustomBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT CUSTOM INSTRUCTIONS", customInstructions );
			dlg.ShowDialog();
			customInstructions = dlg.theText.Trim();
		}

		private void editText_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT CUSTOM INSTRUCTIONS - " + ((sender as FrameworkElement).DataContext as EnemyGroupData).customInstructionType.ToString().ToUpper(), ((sender as FrameworkElement).DataContext as EnemyGroupData).customText );
			dlg.ShowDialog();
			((sender as FrameworkElement).DataContext as EnemyGroupData).customText = dlg.theText.Trim();
		}

		private void editResText_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT CUSTOM INSTRUCTIONS - " + ((sender as FrameworkElement).DataContext as EnemyGroupData).customInstructionType.ToString().ToUpper(), ((sender as FrameworkElement).DataContext as EnemyGroupData).customText );
			dlg.ShowDialog();
			((sender as FrameworkElement).DataContext as EnemyGroupData).customText = dlg.theText.Trim();
		}
	}
}
