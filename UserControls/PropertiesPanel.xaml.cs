using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for PropertiesPanel.xaml
	/// </summary>
	public partial class PropertiesPanel : UserControl, INotifyPropertyChanged
	{
		//private MapSection _mapSection;
		//public MapSection mapSection { get { return _mapSection; } set { _mapSection = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public PropertiesPanel()
		{
			InitializeComponent();
		}

		public void Populate( MapSection ms )
		{
			DataContext = ms;
		}

		private void TextBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				Utils.mainWindow.tabControl.SelectedIndex = 3;
				Utils.mainWindow.tabControl.SelectedIndex = 4;
			}
		}
	}
}
