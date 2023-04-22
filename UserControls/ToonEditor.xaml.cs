using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ToonEditor.xaml
	/// </summary>
	public partial class ToonEditor : UserControl, INotifyPropertyChanged
	{
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		ToonEditorPanel toonEditorPanel;
		CustomToon _selectedToon;
		//if we're in the standalone character editor and NOT in the mission editor
		bool isStandalone = false;

		public CustomToon selectedToon { get { return _selectedToon; } set { _selectedToon = value; PC(); } }

		public ObservableCollection<CustomToon> toonList { get; set; }

		public ToonEditor()
		{
			InitializeComponent();

			toonList = new();
			DataContext = this;

			selectedToon = null;
		}

		public void UpdateUI()
		{
			toonList.Clear();
			foreach ( var item in Utils.mainWindow.mission.customCharacters )
			{
				toonList.Add( item );
			}
		}

		public void SetStandalone()
		{
			isStandalone = true;
			toonListCB.Visibility = Visibility.Collapsed;
			remToonButton.Visibility = Visibility.Collapsed;
			editToonButton.Visibility = Visibility.Collapsed;
		}

		private void remToonButton_Click( object sender, RoutedEventArgs e )
		{
			if ( !isStandalone )
			{
				Utils.RemoveCustomToon( selectedToon.deploymentCard );
				Utils.mainWindow.mission.customCharacters.Remove( selectedToon );
				toonList.Remove( selectedToon );
			}

			selectedToon = null;
			stackPanel.Children.Remove( toonEditorPanel );
			toonEditorPanel = null;
		}

		private void editToonButton_Click( object sender, RoutedEventArgs e )
		{
			stackPanel.Children.Remove( toonEditorPanel );

			toonEditorPanel = new ToonEditorPanel( selectedToon );
			toonEditorPanel.SetThumbnailImage();
			if ( isStandalone )
				toonEditorPanel.SetStandalone();
			stackPanel.Children.Add( toonEditorPanel );
		}

		private void newToonButton_Click( object sender, RoutedEventArgs e )
		{
			stackPanel.Children.Remove( toonEditorPanel );

			CustomToon newToon = new();
			newToon.Create();
			if ( !isStandalone )
			{
				toonList.Add( newToon );
				Utils.mainWindow.mission.customCharacters.Add( newToon );
				Utils.AddCustomToon( newToon.deploymentCard );
			}
			toonEditorPanel = new ToonEditorPanel( newToon );
			if ( isStandalone )
				toonEditorPanel.SetStandalone();
			stackPanel.Children.Add( toonEditorPanel );
			selectedToon = newToon;
		}

		private void exportToonButton_Click( object sender, RoutedEventArgs e )
		{
			if ( FileManager.ExportCharacter( selectedToon ) )
			{
				if ( !isStandalone )
					Utils.mainWindow.SetStatus( "Exported Character" );
			}
			else
			{
				if ( !isStandalone )
					Utils.mainWindow.SetStatus( "Character Not Exported" );
			}
		}

		private void importToonButton_Click( object sender, RoutedEventArgs e )
		{
			stackPanel.Children.Remove( toonEditorPanel );

			var toon = FileManager.ImportCharacter();
			if ( toon != null )
			{
				CustomToon newToon = CustomToon.ImportFrom( toon );
				if ( !isStandalone )
				{
					toonList.Add( newToon );
					Utils.mainWindow.mission.customCharacters.Add( newToon );
					Utils.AddCustomToon( newToon.deploymentCard );
				}
				toonEditorPanel = new ToonEditorPanel( newToon );
				if ( isStandalone )
					toonEditorPanel.SetStandalone();
				toonEditorPanel.SetThumbnailImage();
				stackPanel.Children.Add( toonEditorPanel );
				selectedToon = newToon;

				if ( !isStandalone )
					Utils.mainWindow.SetStatus( "Imported Character" );
			}
			else
			{
				if ( !isStandalone )
					Utils.mainWindow.SetStatus( "Character Not Imported" );
				else
					MessageBox.Show( "Could not import the character. Did you choose the correct file?", "Error Importing Character", MessageBoxButton.OK, MessageBoxImage.Warning );
			}
		}
	}
}
