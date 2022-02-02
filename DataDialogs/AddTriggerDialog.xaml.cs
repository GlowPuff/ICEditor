using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for AddTriggerDialog.xaml
	/// </summary>
	public partial class AddTriggerDialog : Window, INotifyPropertyChanged
	{
		Trigger _trigger;
		public Trigger editedTrigger;

		public static MainWindow mainWindow { get { return Utils.mainWindow; } }
		public Trigger trigger
		{
			get { return _trigger; }
			set { _trigger = value; PC(); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public AddTriggerDialog( Trigger t = null )
		{
			InitializeComponent();

			DataContext = this;

			if ( t == null )
				trigger = new() { name = "New Trigger" };
			else
			{
				trigger = t.Clone() as Trigger;
				editedTrigger = t;
				gbox.Header = $"Edit Trigger '{trigger.name}'";
			}

			////nameTB.Focus();
			//nameTB.Select( 0, nameTB.Text.Length );
		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}

		private void cancelButton_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = false;
			Close();
		}

		private void okButton_Click( object sender, RoutedEventArgs e )
		{
			if ( string.IsNullOrEmpty( nameTB.Text ) )
			{
				nameTB.Focus();
				nameTB.Text = "ENTER A NAME";
				nameTB.SelectAll();
			}
			else
			{
				DialogResult = true;
				Close();
			}
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			nameTB.Focus();
			nameTB.SelectAll();
		}

		private void nameTB_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}
	}
}
