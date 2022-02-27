using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public static class Utils
	{
		public const string formatVersion = "7";
		public const string appVersion = ".10";

		public static List<DeploymentCard> allyData;
		public static List<DeploymentCard> enemyData;
		public static List<DeploymentCard> villainData;
		public static List<DeploymentCard> heroData;
		public static List<TileDescriptor> tileData;

		public static ObservableCollection<DeploymentColor> deploymentColors;

		public static MainWindow mainWindow;

		public static void InitColors()
		{
			deploymentColors = new ObservableCollection<DeploymentColor>();
			deploymentColors.Add( new( "Gray", ColorFromFloats( .3301887f, .3301887f, .3301887f ) ) );
			deploymentColors.Add( new( "Purple", ColorFromFloats( .6784314f, 0f, 1f ) ) );
			deploymentColors.Add( new( "Black", ColorFromFloats( 0, 0, 0 ) ) );
			deploymentColors.Add( new( "Blue", ColorFromFloats( 0, 0.3294118f, 1 ) ) );
			deploymentColors.Add( new( "Green", ColorFromFloats( 0, 0.735849f, 0.1056484f ) ) );
			deploymentColors.Add( new( "Red", ColorFromFloats( 1, 0, 0 ) ) );
			deploymentColors.Add( new( "Yellow", ColorFromFloats( 1, 202f / 255f, 40f / 255f ) ) );
		}

		public static void Init( MainWindow mw )
		{
			mainWindow = mw;
		}

		public static void LoadAllCardData()
		{
			LoadCardData();
			tileData = TileDescriptor.LoadData();
		}

		public static DeploymentColor ColorFromName( string n )
		{
			if ( string.IsNullOrEmpty( n ) )
				return deploymentColors[0];
			return deploymentColors.Where( x => x.name == n ).First();
		}

		public static Color ColorFromFloats( float r, float g, float b )
		{
			return Color.FromRgb(
				(byte)(r * 255f),
				(byte)(g * 255f),
				(byte)(b * 255f) );
		}

		public static void LoseFocus( Control element )
		{
			FrameworkElement parent = (FrameworkElement)element.Parent;
			while ( parent != null && parent is IInputElement && !((IInputElement)parent).Focusable )
			{
				parent = (FrameworkElement)parent.Parent;
			}

			DependencyObject scope = FocusManager.GetFocusScope( element );
			FocusManager.SetFocusedElement( scope, parent );
		}

		public static void Log( string s )
		{
#if DEBUG
			Debug.WriteLine( s );
#endif
		}

		public static void LoadCardData()
		{
			allyData = DeploymentCard.LoadData( "allies.json" );
			enemyData = DeploymentCard.LoadData( "enemies.json" );
			villainData = DeploymentCard.LoadData( "villains.json" );
			heroData = DeploymentCard.LoadData( "heroes.json" );

			enemyData = enemyData.Concat( villainData ).ToList();
		}

		public static bool ValidateTrigger( Guid guid )
		{
			return mainWindow.mission.TriggerExists( guid );
		}

		public static bool ValidateEvent( Guid guid )
		{
			return mainWindow.mission.EventExists( guid );
		}

		public static bool ValidateMapEntity( Guid guid )
		{
			return mainWindow.mission.EntityExists( guid );
		}

		///EXTENSIONS
		public static double RoundOff( this double i, double value )
		{
			return ((double)Math.Round( i / value )) * value;
		}

		public static ObservableCollection<IMapEntity> Sort<T>( this ObservableCollection<IMapEntity> collection )
		{
			ObservableCollection<IMapEntity> temp;
			temp = new ObservableCollection<IMapEntity>( collection.OrderBy( p => p.name ) );
			collection.Clear();
			foreach ( IMapEntity j in temp )
				collection.Add( j );
			return collection;
		}
	}
}
