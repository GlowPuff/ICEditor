using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EditInitialGroupDialog.xaml
	/// </summary>
	public partial class EditInitialGroupDialog : Window
	{
		public EnemyGroupData initialGroupData { get; set; }
		public DeploymentPoint selectedPoint { get; set; }
		public Guid selectedGUID { get; set; }

		public List<DeploymentPoint> deploymentPoints
		{
			get { return Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>().ToList(); }
		}

		public EditInitialGroupDialog( EnemyGroupData data )
		{
			InitializeComponent();
			DataContext = this;

			//validate each dp
			for ( int i = 0; i < data.pointList.Count; i++ )
			{
				if ( !Utils.ValidateMapEntity( data.pointList[i].GUID ) )
					data.pointList[i].GUID = Guid.Empty;
			}

			initialGroupData = data;
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			DragMove();
		}
	}
}
