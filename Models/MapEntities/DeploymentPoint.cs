using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
  public class DeploymentPoint : INotifyPropertyChanged, IMapEntity
  {
    ///PROPS: color, active
    string _name;
    DeploymentColor _deploymentColor;

    public Guid GUID { get; set; }
    public string name { get { return _name; } set { _name = value; PC(); } }
    public EntityType entityType { get; set; }
    public Vector entityPosition { get; set; }
    [JsonIgnore]
    public EntityRenderer mapRenderer { get; set; }
    public EntityProperties entityProperties { get; set; }

    //deployment props
    [JsonIgnore]
    public List<DeploymentColor> deploymentColors { get; set; }
    public DeploymentColor deploymentColor
    {
      get { return _deploymentColor; }
      set
      {
        _deploymentColor = value;
        PC();
        if ( mapRenderer != null )
        {
          mapRenderer.entityShape.Fill = new SolidColorBrush( _deploymentColor.color );
          mapRenderer.unselectedBGColor = new SolidColorBrush( _deploymentColor.color );
          mapRenderer.selectedBGColor = new SolidColorBrush( _deploymentColor.color );
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void PC( [CallerMemberName] string n = "" )
    {
      if ( !string.IsNullOrEmpty( n ) )
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
    }

    public DeploymentPoint()
    {

    }

    public DeploymentPoint( DeploymentColor[] dc )
    {
      GUID = Guid.NewGuid();
      name = "New Deployment Point";
      entityType = EntityType.DeploymentPoint;
      entityProperties = new();

      deploymentColors = new List<DeploymentColor>( dc );
      _deploymentColor = dc[0];//gray
    }

    public void BuildRenderer( Canvas c, Vector where, bool showPanel, double scale )
    {
      mapRenderer = new( this, where, showPanel, scale, new( 1, 1 ) )
      {
        selectedZ = 300,
        selectedBGColor = new( _deploymentColor.color ),
        unselectedBGColor = new( _deploymentColor.color ),
        unselectedStrokeColor = new( Colors.Red )
      };
      mapRenderer.BuildShape( TokenShape.Circle );
      c.Children.Add( mapRenderer.entityShape );
    }
  }
}
