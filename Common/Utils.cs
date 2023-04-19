using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public static class Utils
	{
		public const string formatVersion = "21";
		public const string appVersion = "1.0.30";

		public static List<DeploymentCard> allyData;
		public static List<DeploymentCard> enemyData;
		public static List<DeploymentCard> villainData;
		public static List<DeploymentCard> heroData;
		public static List<TileDescriptor> tileData;
		public static ThumbnailData thumbnailData;
		public static List<CardInstruction> enemyInstructions;
		public static List<BonusEffect> enemyBonusEffects;

		public static ObservableCollection<DeploymentColor> deploymentColors;

		public static MainWindow mainWindow
		{
			get { return Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(); }
		}

		public static Guid GUIDOne { get { return Guid.Parse( "11111111-1111-1111-1111-111111111111" ); } }

		public static void InitColors()
		{
			deploymentColors = new ObservableCollection<DeploymentColor>
			{
				new( "Gray", ColorFromFloats( .3301887f, .3301887f, .3301887f ) ),
				new( "Purple", ColorFromFloats( .6784314f, 0f, 1f ) ),
				new( "Black", ColorFromFloats( 0, 0, 0 ) ),
				new( "Blue", ColorFromFloats( 0, 0.3294118f, 1 ) ),
				new( "Green", ColorFromFloats( 0, 0.735849f, 0.1056484f ) ),
				new( "Red", ColorFromFloats( 1, 0, 0 ) ),
				new( "Yellow", ColorFromFloats( 1, 202f / 255f, 40f / 255f ) )
			};
		}

		//public static void Init( MainWindow mw )
		//{
		//	mainWindow = mw;
		//}

		public static void LoadAllCardData()
		{
			LoadCardData();
			tileData = TileDescriptor.LoadData();
			enemyInstructions = FileManager.LoadAsset<List<CardInstruction>>( "instructions.json" );
			enemyBonusEffects = FileManager.LoadAsset<List<BonusEffect>>( "bonuseffects.json" );
			thumbnailData = FileManager.LoadAsset<ThumbnailData>( "thumbnails.json" );
			//append stock icon data
			//remove duplicate names and elite version, only need 1 thumb for each ID
			//caution - enemyData contains enemies AND villains, remove villain data
			var edata = enemyData.Where( x => villainData.All( v => v.id != x.id && !x.name.Contains( "Elite" ) ) ).Select( x => new Thumbnail() { ID = $"StockImperial{x.id.GetDigits()}", Name = x.name } ).ToList();
			for ( int i = 0; i < edata.Count; i++ )
			{
				if ( thumbnailData.StockImperial.All( x => x.Name != edata[i].Name ) )
					thumbnailData.StockImperial.Add( edata[i] );
			}
			thumbnailData.StockAlly = allyData.Select( x => new Thumbnail() { ID = $"StockAlly{x.id.GetDigits()}", Name = x.name } ).ToList();
			thumbnailData.StockHero = heroData.Select( x => new Thumbnail() { ID = $"StockHero{x.id.GetDigits()}", Name = x.name } ).ToList();
			thumbnailData.StockVillain = villainData.Select( x => new Thumbnail() { ID = $"StockVillain{x.id.GetDigits()}", Name = x.name } ).ToList();
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
			Debug.WriteLine( s );
		}

		public static void LoadCardData()
		{
			allyData = FileManager.LoadAsset<List<DeploymentCard>>( "allies.json" );
			enemyData = FileManager.LoadAsset<List<DeploymentCard>>( "enemies.json" );
			villainData = FileManager.LoadAsset<List<DeploymentCard>>( "villains.json" );
			heroData = FileManager.LoadAsset<List<DeploymentCard>>( "heroes.json" );

			enemyData = enemyData.Concat( villainData ).ToList();
		}

		public static void AddCustomToon( DeploymentCard card )
		{
			enemyData.Add( card );
		}

		public static void RemoveCustomToon( DeploymentCard card )
		{
			enemyData.Remove( card );
		}

		public static string GetAvailableCustomToonID()
		{
			var usedIDs = enemyData.Where( x => x.id.Contains( "TC" ) ).Select( x => int.Parse( x.id.Substring( 2 ) ) ).ToList();
			int highest = 1;
			if ( usedIDs.Count() >= 1 )
			{
				for ( int i = 0; i < usedIDs.Count(); i++ )
				{
					if ( !usedIDs.Contains( highest + i + 1 ) )//start at 1
					{
						highest += i + 1;
						break;
					}
				}
			}
			return $"TC{highest}";
		}

		/// <summary>
		/// Check if a Trigger exists in the mission
		/// </summary>
		public static bool ValidateTrigger( Guid guid )
		{
			return mainWindow.mission.TriggerExists( guid );
		}

		/// <summary>
		/// Check if an Event exists in the mission
		/// </summary>
		public static bool ValidateEvent( Guid guid )
		{
			return mainWindow.mission.EventExists( guid );
		}

		/// <summary>
		/// Check if a map entity exists in the mission
		/// </summary>
		public static bool ValidateMapEntity( Guid guid )
		{
			return mainWindow.mission.EntityExists( guid ) || guid == Utils.GUIDOne;
		}

		/// <summary>
		/// Remove all tiles/entities/events/triggers associated with this Section
		/// </summary>
		public static void RemoveMapSectionObjects( MapSection ms )
		{
			ms.triggers.Clear();
			ms.missionEvents.Clear();
			//map entities
			for ( int i = mainWindow.mission.mapEntities.Count - 1; i >= 0; i-- )
			{
				if ( mainWindow.mission.mapEntities[i].mapSectionOwner == ms.GUID )
				{
					mainWindow.mapEditor.RemoveEntityFromMap( mainWindow.mission.mapEntities[i] );
					mainWindow.mission.mapEntities.RemoveAt( i );
				}
			}
			//tiles
			for ( int i = ms.mapTiles.Count - 1; i >= 0; i-- )
			{
				mainWindow.mapEditor.RemoveEntityFromMap( ms.mapTiles[i] );
			}
			ms.mapTiles.Clear();

			mainWindow.mapEditor.UpdateUI();
		}

		/// <summary>
		/// Given map section "ms", set it's child objects' owner to default section
		/// </summary>
		public static void SetOwnerToDefaultSection( MapSection ms )
		{
			foreach ( var item in ms.triggers )
				if ( item.GUID != Guid.Empty )
					mainWindow.mission.globalTriggers.Add( item );
			foreach ( var item in ms.missionEvents )
				if ( item.GUID != Guid.Empty )
					mainWindow.mission.globalEvents.Add( item );
			//entities
			var entities = mainWindow.mission.mapEntities.Where( x => x.mapSectionOwner == ms.GUID ).ToList();
			foreach ( var entity in entities )
				entity.mapSectionOwner = Guid.Parse( "11111111-1111-1111-1111-111111111111" );
			//tiles
			foreach ( var item in ms.mapTiles )
				item.mapSectionOwner = Guid.Parse( "11111111-1111-1111-1111-111111111111" );
			mainWindow.mapEditor.UpdateUI();
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

		public static DiceColor[] ParseCustomDice( string[] dice )
		{
			List<DiceColor> diceColors = new List<DiceColor>();
			var regex = new Regex( @"\d\w+", RegexOptions.IgnoreCase );

			try
			{
				foreach ( var diceItem in dice )
				{
					var m = regex.Matches( diceItem );
					foreach ( var match in regex.Matches( diceItem ) )
					{
						int count = int.Parse( match.ToString()[0].ToString() );

						for ( int i = 0; i < count; i++ )
							diceColors.Add( (DiceColor)Enum.Parse( typeof( DiceColor ), match.ToString().Substring( 1 ) ) );
					}
				}
				return diceColors.ToArray();
			}
			catch ( Exception e )
			{
				MessageBox.Show( $"Check your formatting.\n\nException:\n{e.Message}", "Error Parsing Dice Values" );
				return new DiceColor[0];
			}
		}
	}
}
