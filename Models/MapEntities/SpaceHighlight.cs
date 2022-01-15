using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
  public class SpaceHighlight : INotifyPropertyChanged, IMapEntity
  {
    string _name;
    DeploymentColor _deploymentColor;
    int _width, _height, _duration;

    //common props
    public Guid GUID { get; set; }
    public string name { get { return _name; } set { _name = value; PC(); } }
    public EntityType entityType { get; set; }
    public Vector entityPosition { get; set; }
    public double entityRotation { get; set; }
    [JsonIgnore]
    public EntityRenderer mapRenderer { get; set; }
    public EntityProperties entityProperties { get; set; }

    //highlight props
    //[JsonIgnore]
    //public List<DeploymentColor> deploymentColors { get; set; }
    public DeploymentColor deploymentColor
    {
      get { return _deploymentColor; }
      set
      {
        _deploymentColor = value;
        PC();
        if ( mapRenderer != null )
        {
          mapRenderer.entityShape.Fill = new SolidColorBrush( Color.FromArgb( 100, _deploymentColor.color.R, _deploymentColor.color.G, _deploymentColor.color.B ) );//new SolidColorBrush( _deploymentColor.color );
          mapRenderer.unselectedBGColor = new SolidColorBrush( Color.FromArgb( 100, _deploymentColor.color.R, _deploymentColor.color.G, _deploymentColor.color.B ) );//new SolidColorBrush( _deploymentColor.color );
          mapRenderer.selectedBGColor = new SolidColorBrush( Color.FromArgb( 100, _deploymentColor.color.R, _deploymentColor.color.G, _deploymentColor.color.B ) );//new SolidColorBrush( _deploymentColor.color );
        }
      }
    }
    public int Width { get { return _width; } set { _width = value; PC(); } }
    public int Height { get { return _height; } set { _height = value; PC(); } }
    public int Duration { get { return _duration; } set { _duration = value; PC(); } }

    public void PC( [CallerMemberName] string n = "" )
    {
      if ( !string.IsNullOrEmpty( n ) )
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
    }


    public event PropertyChangedEventHandler PropertyChanged;

    public SpaceHighlight()
    {
      GUID = Guid.NewGuid();
      name = "New Highlight";
      entityType = EntityType.Highlight;
      //defaults NOT ACTIVE, unlike other entities
      entityProperties = new() { isActive = false };
      Width = 1;
      Height = 1;
      Duration = 0;

      //deploymentColors = new List<DeploymentColor>( dc );
      _deploymentColor = new( "Green", Utils.ColorFromFloats( 0, 0.735849f, 0.1056484f ) );//dc[4];
    }

    public void Rebuild()
    {
      Canvas c = mapRenderer.entityShape.Parent as Canvas;
      Vector w = mapRenderer.GetPosition();

      mapRenderer.RemoveVisual();

      mapRenderer = new( this, mapRenderer.where, Utils.mainWindow.mapEditor.showPanel, Utils.mainWindow.mapEditor.mScale, new( Width, Height ) )
      {
        selectedBGColor = new( Color.FromArgb( 100, _deploymentColor.color.R, _deploymentColor.color.G, _deploymentColor.color.B ) ),//new( _deploymentColor.color ),
        unselectedBGColor = new( Color.FromArgb( 100, _deploymentColor.color.R, _deploymentColor.color.G, _deploymentColor.color.B ) ),//new( _deploymentColor.color ),
        unselectedStrokeColor = new( Colors.Red ),
        selectedZ = 200
      };
      mapRenderer.BuildShape( TokenShape.Square );

      c.Children.Add( mapRenderer.entityShape );
      mapRenderer.Select();
      mapRenderer.SetPosition( w );
    }

    public void BuildRenderer( Canvas c, Vector where, bool showPanel, double scale )
    {
      mapRenderer = new( this, where, showPanel, scale, new( Width, Height ) )
      {
        selectedBGColor = new( Color.FromArgb( 100, _deploymentColor.color.R, _deploymentColor.color.G, _deploymentColor.color.B ) ), //new( _deploymentColor.color ),
        unselectedBGColor = new( Color.FromArgb( 100, _deploymentColor.color.R, _deploymentColor.color.G, _deploymentColor.color.B ) ),//new( _deploymentColor.color ),
        unselectedStrokeColor = new( Colors.Red ),
        unselectedStrokeWidth = 1,
        selectedZ = 200
      };
      mapRenderer.BuildShape( TokenShape.Square );
      c.Children.Add( mapRenderer.entityShape );
    }
  }
}
