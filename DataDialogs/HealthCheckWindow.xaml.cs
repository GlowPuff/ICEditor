using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for HealthCheckWindow.xaml
	/// </summary>
	public partial class HealthCheckWindow : Window, INotifyPropertyChanged
	{
		private static MainWindow mainWindow { get { return Utils.mainWindow; } }
		string _infoText, _detailsText;

		public string infoText { get => _infoText; set { _infoText = value; PC(); } }
		public ObservableCollection<BrokenRefInfo> brokenList { get; set; }

		public HealthCheckWindow()
		{
			InitializeComponent();

			brokenList = new();

			FindBroken();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		void FindBroken()
		{
			//await Task.Run( () =>
			//{
			List<BrokenRefInfo> theList = new();
			List<HealthReport> evReports = new();

			//events
			foreach ( var ev in mainWindow.mission.GetAllEvents() )
			{
				foreach ( var eva in ev.eventActions.OfType<IHasEventReference>() )
				{
					var info = eva.SelfCheckEvents();
					info.topOwnerName = ev.name;
					if ( info.isBroken )
						theList.Add( info );
				}
			}

			Utils.Log( "EVENTS" );
			Utils.Log( string.Join( "\n", theList.Select( x =>
			{
				return $"{x.topOwnerName} :: {x.itemName} :: {x.details}";
			} ) ) );

			//triggers


			//entities

			//} );
		}

		private void closeBtn_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void fixAllBtn_Click( object sender, RoutedEventArgs e )
		{

		}

		private void fixBtn_Click( object sender, RoutedEventArgs e )
		{

		}

		private void Window_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			if ( e.LeftButton == System.Windows.Input.MouseButtonState.Pressed )
				DragMove();
		}
	}
}
