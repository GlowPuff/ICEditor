using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for ConfirmSectionRemoveDialog.xaml
	/// </summary>
	public partial class ConfirmSectionRemoveDialog : Window, INotifyPropertyChanged
	{
		string _sectionName;
		bool _removeChildren;

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public string sectionName { get { return _sectionName; } set { _sectionName = value; PC(); } }
		public bool removeChildren { get { return _removeChildren; } set { _removeChildren = value; PC(); } }

		public ConfirmSectionRemoveDialog( string sname )
		{
			InitializeComponent();
			DataContext = this;

			sectionName = sname;
			removeChildren = true;
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = true;
			Close();
		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = false;
			Close();
		}
	}
}
