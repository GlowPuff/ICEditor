using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for TokenGroupDialog.xaml
	/// </summary>
	public partial class EntityGroupDialog : Window, INotifyPropertyChanged
	{
		bool _buttonEnabled;
		IMapEntity _selectedEntity;

		public bool buttonEnabled { get { return _buttonEnabled; } set { _buttonEnabled = value; PC(); } }
		public EntityGroup entityGroup { get; set; }
		public IMapEntity selectedEntity
		{
			get { return _selectedEntity; }
			set { _selectedEntity = value; PC(); buttonEnabled = _selectedEntity != null; }
		}
		public bool showCancelBtn { get; set; }
		public ObservableCollection<IMapEntity> addedEntities { get; set; } = new();

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public EntityGroupDialog( EntityGroup eg = null )
		{
			InitializeComponent();
			DataContext = this;

			tokensCB.ItemsSource = Utils.mainWindow.mission.mapEntities;
			tCB.ItemsSource = Utils.mainWindow.localTriggers;

			this.entityGroup = eg ?? new();
			showCancelBtn = eg == null;
			buttonEnabled = false;

			//make sure entity still exists
			int count = 0;
			for ( int i = entityGroup.missionEntities.Count - 1; i >= 0; i-- )
			{
				var e = Utils.mainWindow.mission.GetEntityFromGUID( entityGroup.missionEntities[i] );
				if ( e != null )
					addedEntities.Add( e );
				else
				{
					count++;
					entityGroup.missionEntities.RemoveAt( i );
				}
			}
			if ( count > 0 )
				MessageBox.Show( $"This Entity Group is referencing one or more Entities that no longer exist in the Mission.\n\nFound and removed {count} missing Entities.", "Missing Reference(s) Found" );

			//make sure trigger still exists
			//if ( Utils.mainWindow.mission.GetTriggerFromGUID( entityGroup.triggerGUID ) == null )
			//	entityGroup.triggerGUID = Guid.Empty;
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			if ( !string.IsNullOrEmpty( entityGroup.name ) )
			{
				DialogResult = true;
				Close();
			}
		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = false;
			Close();
		}

		private void addNewTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			Trigger t = Utils.mainWindow.leftPanel.addNewTrigger();
			if ( t != null )
			{
				tCB.ItemsSource = Utils.mainWindow.localTriggers;
				entityGroup.triggerGUID = t.GUID;
			}
		}

		private void tCB_GotFocus( object sender, RoutedEventArgs e )
		{
			tCB.GotFocus -= tCB_GotFocus;
			tCB.ItemsSource = Utils.mainWindow.localTriggers;
			tCB.GotFocus += tCB_GotFocus;
		}

		private void addGroupBtn_Click( object sender, RoutedEventArgs e )
		{
			if ( selectedEntity == null )
				return;
			entityGroup.missionEntities.Add( selectedEntity.GUID );
			addedEntities.Add( selectedEntity );
			selectedEntity = null;
		}

		private void remButton_Click( object sender, RoutedEventArgs e )
		{
			entityGroup.missionEntities.Remove( ((sender as FrameworkElement).DataContext as IMapEntity).GUID );
			addedEntities.Remove( (sender as FrameworkElement).DataContext as IMapEntity );
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
			}
		}
	}
}
