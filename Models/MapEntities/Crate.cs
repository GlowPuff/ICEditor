using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
  public class Crate : INotifyPropertyChanged, IMapEntity
  {
    ///PROPS: description, interaction button text, event

    string _name;

    //common props
    public Guid GUID { get; set; }
    public string name { get { return _name; } set { _name = value; PC(); } }
    public EntityType entityType { get; set; }
    public Vector entityPosition { get; set; }
    public double entityRotation { get; set; }
    [JsonIgnore]
    public EntityRenderer mapRenderer { get; set; }
    public EntityProperties entityProperties { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public void PC( [CallerMemberName] string n = "" )
    {
      if ( !string.IsNullOrEmpty( n ) )
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
    }

    public Crate()
    {
      GUID = Guid.NewGuid();
      name = "New Crate";
      entityType = EntityType.Crate;
      entityProperties = new();
    }

    public void BuildRenderer( Canvas c, Vector where, bool showPanel, double scale )
    {
      mapRenderer = new( this, where, showPanel, scale, new( 1, 1 ) )
      {
        unselectedBGColor = new SolidColorBrush( Colors.SaddleBrown ),
        selectedBGColor = new SolidColorBrush( Colors.SaddleBrown ),
        selectedZ = 300
      };
      mapRenderer.BuildShape( TokenShape.Square );
      c.Children.Add( mapRenderer.entityShape );
    }
  }
}
