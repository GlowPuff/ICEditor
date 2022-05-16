using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for EditInitialGroupDialog.xaml
	/// </summary>
	public partial class EditInitialGroupDialog : Window, INotifyPropertyChanged
	{
		DeploymentPoint _deploymentPoint;
		string _customName;

		public string customName { get { return _customName; } set { _customName = value; PC(); } }
		public EnemyGroupData enemyGroupData { get; set; }
		public ObservableCollection<DeploymentPoint> deploymentPoints { get; set; } = new();
		public DeploymentPoint deploymentPoint
		{
			get { return _deploymentPoint; }
			set
			{
				_deploymentPoint = value;
				PC();
			}
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		private DeploymentPoint emptyDP = new() { name = "None", GUID = Guid.Empty };

		public EditInitialGroupDialog( EnemyGroupData egd )
		{
			InitializeComponent();

			enemyGroupData = egd;
			DataContext = enemyGroupData;

			ciInfo.Text = string.IsNullOrEmpty( enemyGroupData.customText ) ? "Text Not Set" : "Text Set";
			SolidColorBrush brush = new( string.IsNullOrEmpty( enemyGroupData.customText ) ? Colors.Red : Colors.LawnGreen );
			ciInfo.Foreground = brush;

			deploymentPoints.Add( emptyDP );
			foreach ( var ee in Utils.mainWindow.mission.mapEntities.OfType<DeploymentPoint>() )
			{
				if ( !deploymentPoints.Contains( ee ) )
					deploymentPoints.Add( ee );
			}
			deploymentPoint = emptyDP;

			string expectedName = Utils.enemyData.Where( x => x.id == enemyGroupData.cardID ).First().name;
			if ( !string.IsNullOrEmpty( enemyGroupData.cardName ) && enemyGroupData.cardName != expectedName )
				customName = enemyGroupData.cardName;

			//verify dp's exist
			for ( int y = 0; y < enemyGroupData.pointList.Count; y++ )
			{
				if ( !Utils.ValidateMapEntity( enemyGroupData.pointList[y].GUID ) )
					enemyGroupData.pointList[y].GUID = Guid.Empty;
			}
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			if ( !string.IsNullOrEmpty( customName?.Trim() ) )
				enemyGroupData.cardName = customName.Trim();
			else
				enemyGroupData.cardName = Utils.enemyData.Where( x => x.id == enemyGroupData.cardID ).First().name;

			Close();
		}

		private void targetBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new PriorityTraitsDialog( enemyGroupData.groupPriorityTraits );
			dlg.ShowDialog();
		}

		private void editCustomBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new GenericTextDialog( "EDIT CUSTOM INSTRUCTIONS", enemyGroupData.customText );
			dlg.ShowDialog();
			enemyGroupData.customText = dlg.theText.Trim();

			ciInfo.Text = string.IsNullOrEmpty( enemyGroupData.customText ) ? "Text Not Set" : "Text Set";
			SolidColorBrush brush = new( string.IsNullOrEmpty( enemyGroupData.customText ) ? Colors.Red : Colors.LawnGreen );
			ciInfo.Foreground = brush;
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}

		private void editDPbtn_Click( object sender, RoutedEventArgs e )
		{
			EditDPDialog dialog = new EditDPDialog( enemyGroupData );
			dialog.ShowDialog();
		}

		private void Window_ContentRendered( object sender, EventArgs e )
		{
			nameText.Focus();
			nameText.SelectAll();
		}
	}
}
