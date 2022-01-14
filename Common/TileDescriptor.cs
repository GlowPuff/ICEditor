using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using System.Linq;

namespace Imperial_Commander_Editor
{
  public class TileDescriptor
  {
    public string expansion { get; set; }
    public int id { get; set; }
    public int width { get; set; }
    public int height { get; set; }

    public static List<TileDescriptor> LoadData()
    {
      string assetName = "dimensions.json";
      try
      {
        string json = "";
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single( str => str.EndsWith( assetName ) );
        using ( Stream stream = assembly.GetManifestResourceStream( resourceName ) )
        using ( StreamReader reader = new StreamReader( stream ) )
        {
          json = reader.ReadToEnd();
        }

        return JsonConvert.DeserializeObject<List<TileDescriptor>>( json );
      }
      catch ( JsonReaderException e )
      {
        Utils.Log( $"TileDescriptor::LoadData() ERROR:\r\nError parsing {assetName}" );
        Utils.Log( e.Message );
        throw new Exception( $"TileDescriptor::LoadData() ERROR:\r\nError parsing {assetName}" );
      }
    }
  }
}
