using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EncounterPanel.xaml
	/// </summary>
	public partial class EncounterPanel : UserControl, INotifyPropertyChanged
	{
		bool _buttonEnabled, _useDefaultPriority;
		DeploymentPoint _deploymentPoint;
		string _selectedGroup, _customName, _customInstructions;
		GroupPriorityTraits _priorityTraits;

		private DeploymentPoint emptyDP = new() { name = "None", GUID = Guid.Empty };

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
		public ObservableCollection<DeploymentPoint> deploymentPoints { get; set; } = new();
		public List<DeploymentPoint> allPoints { get; set; }
		public bool buttonEnabled { get { return _buttonEnabled; } set { _buttonEnabled = value; PC(); } }
		public CustomInstructionType customInstructionType { get; set; } = CustomInstructionType.Replace;
		public string customInstructions { get { return _customInstructions; } set { _customInstructions = value; PC(); } }
		public string customName { get { return _customName; } set { _customName = value; PC(); } }
		public bool useDefaultPriority { get { return _useDefaultPriority; } set { _useDefaultPriority = value; PC(); } }

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
			deploymentPoints.Add( emptyDP );
			deploymentPoint = emptyDP;
			useDefaultPriority = true;
		}

		public void UpdateUI()
		{
			gCB_initial.ItemsSource = Utils.enemyData;

			foreach ( var ee in Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>() )
			{
				if ( !deploymentPoints.Contains( ee ) )
					deploymentPoints.Add( ee );
			}

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

			if ( !useDefaultPriority )
			{
				_priorityTraits = _priorityTraits ?? new();
				ig.groupPriorityTraits = _priorityTraits;
				_priorityTraits = null;
			}
			else
			{
				//TODO - get priorityTraits from card data JSON and assign it to "ig"
			}

			ig.customInstructionType = customInstructionType;
			if ( !string.IsNullOrEmpty( customName.Trim() ) )
				ig.cardName = customName.Trim();
			ig.customText = customInstructions;

			Utils.mainWindow.mission.reservedDeploymentGroups.Add( ig );
			customName = customInstructions = "";
			useDefaultPriority = true;
		}

		private void addInitialGroupButton_Click( object sender, RoutedEventArgs e )
		{
			var ig = new EnemyGroupData( Utils.enemyData.Where( x => x.id == selectedGroup ).First(), deploymentPoint );

			if ( !useDefaultPriority )
			{
				_priorityTraits = _priorityTraits ?? new();
				ig.groupPriorityTraits = _priorityTraits;
				_priorityTraits = null;
			}
			else
			{
				//TODO - get priorityTraits from card data JSON and assign it to "ig"
			}

			ig.customInstructionType = customInstructionType;
			if ( !string.IsNullOrEmpty( customName.Trim() ) )
				ig.cardName = customName.Trim();
			ig.customText = customInstructions;

			Utils.mainWindow.mission.initialDeploymentGroups.Add( ig );
			deploymentPoint = deploymentPoints[0];
			customName = customInstructions = "";
			useDefaultPriority = true;
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

		private void TextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			var s = Utils.enemyData.Where( x => x.name.ToLower().Contains( filterBox.Text.ToLower() ) ).FirstOr( null );
			if ( s != null )
				selectedGroup = s.id;
			else
				selectedGroup = null;

			//try id
			if ( s == null )
			{
				var ss = Utils.enemyData.Where( x => x.id.Contains( filterBox.Text ) ).FirstOr( null );
				if ( ss != null )
					selectedGroup = ss.id;
				else
					selectedGroup = null;
			}
		}

		private void filterBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				if ( !string.IsNullOrEmpty( selectedGroup ) )
				{
					//addInitialGroupButton_Click( null, null );
					filterBox.Text = "";
				}
			}
		}

		private void editTraits_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new PriorityTraitsDialog( (((Control)sender).DataContext as EnemyGroupData).groupPriorityTraits );
			dlg.ShowDialog();
		}

		private void editResTraits_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new PriorityTraitsDialog( (((Control)sender).DataContext as EnemyGroupData).groupPriorityTraits );
			dlg.ShowDialog();
		}

		private void targetBtn_Click( object sender, RoutedEventArgs e )
		{
			_priorityTraits = _priorityTraits ?? new();

			var dlg = new PriorityTraitsDialog( _priorityTraits );
			dlg.ShowDialog();
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
