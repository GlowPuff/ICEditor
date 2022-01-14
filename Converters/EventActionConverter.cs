using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;

namespace Imperial_Commander_Editor
{
  public class EventActionConverter : JsonConverter
  {
    public override bool CanWrite => false;
    public override bool CanRead => true;

    public override bool CanConvert( Type objectType )
    {
      return objectType == typeof( EventAction );
    }

    public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
    {
      throw new NotImplementedException();
    }

    public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
    {
      var jsonObject = JArray.Load( reader );
      var eventActionAction = default( IEventAction );
      ObservableCollection<IEventAction> eObserver = new();

      foreach ( var item in jsonObject )
      {
        switch ( item["eventActionType"].Value<int>() )
        {
          case 0:
            eventActionAction = item.ToObject<MissionManagement>();
            break;
          case 1:
            eventActionAction = item.ToObject<ChangeMissionInfo>();
            break;
          case 2:
            eventActionAction = item.ToObject<ChangeObjective>();
            break;
          case 3:
            eventActionAction = item.ToObject<ModifyVariable>();
            break;
          case 4:
            eventActionAction = item.ToObject<ModifyThreat>();
            break;
          case 5:
            eventActionAction = item.ToObject<QuestionPrompt>();
            break;
          case 6:
            eventActionAction = item.ToObject<EnemyDeployment>();
            break;
          case 7:
            eventActionAction = item.ToObject<AllyDeployment>();
            break;
          case 8:
            eventActionAction = item.ToObject<OptionalDeployment>();
            break;
          case 9:
            eventActionAction = item.ToObject<RandomDeployment>();
            break;
          case 10:
            eventActionAction = item.ToObject<AddGroupDeployment>();
            break;
          case 11:
            eventActionAction = item.ToObject<ChangeInstructions>();
            break;
          case 12:
            eventActionAction = item.ToObject<ChangeTarget>();
            break;
          case 13:
            eventActionAction = item.ToObject<ChangeGroupStatus>();
            break;
          case 14:
            eventActionAction = item.ToObject<MapManagement>();
            break;
          case 15:
            eventActionAction = item.ToObject<ModifyDeployment>();
            break;
          case 16:
            eventActionAction = item.ToObject<ModifyEntity>();
            break;
          case 17:
            eventActionAction = item.ToObject<ModifyHighlight>();
            break;
        }
        eObserver.Add( eventActionAction );
      }

      return eObserver;
    }
  }
}
