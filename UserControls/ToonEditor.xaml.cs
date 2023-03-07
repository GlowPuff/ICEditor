using System.ComponentModel;
using System.Runtime.CompilerServices;
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

		public ToonEditor()
		{
			InitializeComponent();

			DataContext = this;

		}
	}
}
