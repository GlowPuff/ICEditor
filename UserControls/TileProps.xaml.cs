﻿using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Imperial_Commander_Editor
{
	/// <summary>
	/// Interaction logic for TileProps.xaml
	/// </summary>
	public partial class TileProps : UserControl, IPropertyModel, INotifyPropertyChanged
	{
		string _ownerName;
		public string ownerName { get { return _ownerName; } set { _ownerName = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public TileProps( MapTile dp )
		{
			InitializeComponent();
			DataContext = dp;

			bool found = Utils.mainWindow.mission.mapSections.Any( x => x.GUID == dp.mapSectionOwner );
			if ( found )
				ownerName = Utils.mainWindow.mission.mapSections.First( x => x.GUID == dp.mapSectionOwner ).name;
			else
				ownerName = "SECTION NOT FOUND";
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
			{
				Utils.LoseFocus( sender as Control );
				Utils.mainWindow.mapEditor.UpdateUI();
				(DataContext as MapTile).entityProperties.name = (DataContext as MapTile).name;
			}
		}

		private void ownerChangeBtn_Click( object sender, System.Windows.RoutedEventArgs e )
		{
			MapTile tile = (sender as FrameworkElement).DataContext as MapTile;

			var prevSection = Utils.mainWindow.mission.mapSections.Where( x => x.GUID == tile.mapSectionOwner ).FirstOr( null );

			foreach ( var section in Utils.mainWindow.mission.mapSections )
			{
				foreach ( var t in section.mapTiles )
				{
					if ( tile == t )
					{
						section.mapTiles.Remove( t );
						break;
					}
				}
			}

			tile.mapSectionOwner = Utils.mainWindow.activeSection.GUID;
			Utils.mainWindow.activeSection.mapTiles.Add( tile );
			Utils.mainWindow.SetStatus( $"Owner Set To '{Utils.mainWindow.activeSection.name}'" );
			ownerName = Utils.mainWindow.activeSection.name;
			Utils.mainWindow.mapEditor.UpdateUI();
		}
	}
}
