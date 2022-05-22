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
		DeploymentPoint _deploymentPoint;
		string _selectedGroup;

		private DeploymentPoint emptyDP = new() { name = "None", GUID = Guid.Empty };

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
			deploymentPoints.Add( emptyDP );
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

			//ciInfo.Text = string.IsNullOrEmpty( customInstructions ) ? "Text Not Set" : "Text Set";
			//SolidColorBrush brush = new( string.IsNullOrEmpty( customInstructions ) ? Colors.Red : Colors.LawnGreen );
			//ciInfo.Foreground = brush;
		}

		private void addReservedGroupButton_Click( object sender, RoutedEventArgs e )
		{
			var ig = new EnemyGroupData( Utils.enemyData.Where( x => x.id == selectedGroup ).First(), new() { name = "None", GUID = Guid.Empty } );

			Utils.mainWindow.mission.reservedDeploymentGroups.Add( ig );
			filterBox.Text = "";
			selectedGroup = "DG001";
		}

		private void addInitialGroupButton_Click( object sender, RoutedEventArgs e )
		{
			var ig = new EnemyGroupData( Utils.enemyData.Where( x => x.id == selectedGroup ).First(), new() { name = "None", GUID = Guid.Empty } );

			Utils.mainWindow.mission.initialDeploymentGroups.Add( ig );
			filterBox.Text = "";
			selectedGroup = "DG001";
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
			EditDPDialog dialog = new EditDPDialog( (sender as FrameworkElement).DataContext as EnemyGroupData );
			dialog.ShowDialog();
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
					//filterBox.Text = "";
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

		private void editInitialGroup_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new EditInitialGroupDialog( (sender as FrameworkElement).DataContext as EnemyGroupData );
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
