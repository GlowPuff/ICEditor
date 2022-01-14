using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
  public static class Utils
  {
    public const string formatVersion = "1";
    public const string appVersion = ".1";

    public static List<DeploymentCard> allyData;
    public static List<DeploymentCard> enemyData;
    public static List<DeploymentCard> villainData;
    public static List<DeploymentCard> heroData;
    public static List<TileDescriptor> tileData;

    public static DeploymentColor[] deploymentColors;

    public static MainWindow mainWindow;

    public static void Init( MainWindow mw )
    {
      mainWindow = mw;

      deploymentColors = new DeploymentColor[6];
      deploymentColors[0] = new( "Gray", ColorFromFloats( .3301887f, .3301887f, .3301887f ) );
      deploymentColors[1] = new( "Purple", ColorFromFloats( .6784314f, 0f, 1f ) );
      deploymentColors[2] = new( "Black", ColorFromFloats( 0, 0, 0 ) );
      deploymentColors[3] = new( "Blue", ColorFromFloats( 0, 0.3294118f, 1 ) );
      deploymentColors[4] = new( "Green", ColorFromFloats( 0, 0.735849f, 0.1056484f ) );
      deploymentColors[5] = new( "Red", ColorFromFloats( 1, 0, 0 ) );

      LoadCardData();
      tileData = TileDescriptor.LoadData();
    }

    public static Color ColorFromFloats( float r, float g, float b )
    {
      return Color.FromRgb(
        (byte)(r * 255f),
        (byte)(g * 255f),
        (byte)(b * 255f) );
    }

    public static void Log( string s )
    {
      Debug.WriteLine( s );
    }

    public static void LoadCardData()
    {
      allyData = DeploymentCard.LoadData( "allies.json" );
      enemyData = DeploymentCard.LoadData( "enemies.json" );
      villainData = DeploymentCard.LoadData( "villains.json" );
      heroData = DeploymentCard.LoadData( "heroes.json" );
    }

    ///EXTENSIONS
    public static double RoundOff( this double i, double value )
    {
      return ((double)Math.Round( i / value )) * value;
    }
  }
}
