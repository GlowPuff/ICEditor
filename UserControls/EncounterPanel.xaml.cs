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
		public string selectedGroup { get; set; }
		public string selectedResGroup { get; set; }
		public ObservableCollection<EnemyGroupData> initialGroups
		{
			get { return Utils.mainWindow.mission.initialDeploymentGroups; }
			set { Utils.mainWindow.mission.initialDeploymentGroups = value; }
		}
		public ObservableCollection<DeploymentCard> reservedGroups
		{
			get { return Utils.mainWindow.mission.reservedDeploymentGroups; }
			set { Utils.mainWindow.mission.reservedDeploymentGroups = value; }
		}
		public List<DeploymentPoint> allPoints { get; set; }
		public bool buttonEnabled { get { return _buttonEnabled; } set { _buttonEnabled = value; PC(); } }

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
		}


		public void Populate()
		{
			gCB_initial.ItemsSource = Utils.enemyData;
			dpCB_reserved.ItemsSource = Utils.enemyData;
			dpCB.ItemsSource = Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>();
			allPoints = Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>().ToList();

			//verify dp's exist
			for ( int i = 0; i < initialGroups.Count; i++ )
			{
				for ( int y = 0; y < initialGroups[i].pointList.Count; y++ )
				{
					if ( !Utils.mainWindow.mission.EntityExists( initialGroups[i].pointList[y].GUID ) )
						initialGroups[i].pointList[y].GUID = Guid.Empty;
				}
			}
		}

		private void addReservedGroupButton_Click( object sender, RoutedEventArgs e )
		{
			Utils.mainWindow.mission.reservedDeploymentGroups.Add( Utils.enemyData.Where( x => x.id == selectedResGroup ).First() );
		}

		private void addInitialGroupButton_Click( object sender, RoutedEventArgs e )
		{
			var ig = new EnemyGroupData( Utils.enemyData.Where( x => x.id == selectedGroup ).First(), deploymentPoint );

			Utils.mainWindow.mission.initialDeploymentGroups.Add( ig );
			deploymentPoint = null;
		}

		private void remInitialGroupButton_Click( object sender, RoutedEventArgs e )
		{
			Utils.mainWindow.mission.initialDeploymentGroups.Remove( (sender as FrameworkElement).DataContext as EnemyGroupData );
		}

		private void remReservedGroupButton_Click( object sender, RoutedEventArgs e )
		{
			Utils.mainWindow.mission.reservedDeploymentGroups.Remove( (sender as FrameworkElement).DataContext as DeploymentCard );
		}

		private void editGroup_Click( object sender, RoutedEventArgs e )
		{
			EditInitialGroupDialog dialog = new EditInitialGroupDialog( (sender as FrameworkElement).DataContext as EnemyGroupData );
			dialog.ShowDialog();
		}
	}
}
