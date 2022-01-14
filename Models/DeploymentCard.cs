using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Imperial_Commander_Editor
{
  public class DeploymentCard
  {
    //== data from JSON
    public string name { get; set; }
    public string id { get; set; }
    public int tier;
    public string faction;
    public int priority;
    public int cost;
    public int rcost;
    public int size;
    public int fame;
    public int reimb;
    public string expansion;
    public string ignored;
    [DefaultValue( false )]
    [JsonProperty( DefaultValueHandling = DefaultValueHandling.Populate )]
    public bool isElite;
    //==

    //==upkeep properties
    //public int currentSize;
    //public bool isHealthy;
    //public int colorIndex;
    //public HeroHealth heroHealth;
    //start v.1.0.17 additions
    //[DefaultValue( false )]
    //[JsonProperty( DefaultValueHandling = DefaultValueHandling.Populate )]
    //public bool hasActivated;
    //public string bonusName, bonusText, rebelName;
    //public InstructionOption instructionOption;
    //==end v.1.0.17 additions

    public DeploymentCard()
    {

    }

    public static List<DeploymentCard> LoadData( string assetName )
    {
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

        return JsonConvert.DeserializeObject<List<DeploymentCard>>( json );
      }
      catch ( JsonReaderException e )
      {
        Utils.Log( $"DeploymentCard::LoadData() ERROR:\r\nError parsing {assetName}" );
        Utils.Log( e.Message );
        throw new Exception( $"DeploymentCard::LoadData() ERROR:\r\nError parsing {assetName}" );
      }
    }
  }
}
