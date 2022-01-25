using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for HighlightProps.xaml
	/// </summary>
	public partial class HighlightProps : UserControl, IPropertyModel, INotifyPropertyChanged
	{
		string _ownerName;

		public string ownerName { get { return _ownerName; } set { _ownerName = value; PC(); } }

		public ObservableCollection<DeploymentColor> deploymentColors
		{
			get { return Utils.deploymentColors; }
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public HighlightProps( SpaceHighlight dp )
		{
			InitializeComponent();
			DataContext = dp;

			bool found = Utils.mainWindow.mission.mapSections.Any( x => x.GUID == dp.mapSectionOwner );
			if ( found )
				ownerName = Utils.mainWindow.mission.mapSections.First( x => x.GUID == dp.mapSectionOwner ).name;
			else
				ownerName = "SECTION NOT FOUND";
		}

		private void editPropsBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			EditEntityProperties dlg = new( (DataContext as SpaceHighlight).entityProperties );
			dlg.ShowDialog();
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				Utils.mainWindow.mapEditor.UpdateUI();
				(DataContext as SpaceHighlight).entityProperties.name = (DataContext as SpaceHighlight).name;
			}
		}

		private void ownerChangeBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			((sender as FrameworkElement).DataContext as SpaceHighlight).mapSectionOwner = Utils.mainWindow.activeSection.GUID;
			Utils.mainWindow.SetStatus( $"Owner Set To '{Utils.mainWindow.activeSection.name}'" );
			ownerName = Utils.mainWindow.activeSection.name;
			Utils.mainWindow.mapEditor.UpdateUI();
		}

		private void sizeKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				(DataContext as SpaceHighlight).Rebuild();
			}
		}
	}
}
