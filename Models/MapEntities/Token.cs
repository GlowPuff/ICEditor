using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
  public class Token : INotifyPropertyChanged, IMapEntity
  {
    ///PROPS: event?

    string _name;

    public Guid GUID { get; set; }
    public string name { get { return _name; } set { _name = value; PC(); } }
    public EntityType entityType { get; set; }
    public Vector entityPosition { get; set; }
    [JsonIgnore]
    public EntityRenderer mapRenderer { get; set; }
    public EntityProperties entityProperties { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public void PC( [CallerMemberName] string n = "" )
    {
      if ( !string.IsNullOrEmpty( n ) )
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
    }

    public Token()
    {
      GUID = Guid.NewGuid();
      name = "New Marker";
      entityType = EntityType.Token;
      entityProperties = new();
    }

    public void BuildRenderer( Canvas c, Vector where, bool showPanel, double scale )
    {
      mapRenderer = new( this, where, showPanel, scale, new( 1, 1 ) )
      {
        selectedZ = 300,
        unselectedBGColor = new( Colors.Orange ),
        selectedBGColor = new( Colors.Orange )
      };
      mapRenderer.BuildShape( TokenShape.Circle );
      c.Children.Add( mapRenderer.entityShape );
    }
  }
}
