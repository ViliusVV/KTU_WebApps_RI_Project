//using MongoDB.Bson;
//using MongoDB.Bson.IO;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.Threading.Tasks;

//namespace RobotsIntelect_WebApi.Converters
//{
//    public class ObjectIdConverter : JsonConverter
//    {
//        public override bool CanConvert(Type objectType)
//        {
//            return objectType == typeof(ObjectId);
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            JToken token = JToken.Load(reader);
//            return new ObjectId(token.ToObject<string>());
//        }

//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            if (value is ObjectId)
//            {
//                var objectId = (ObjectId)value;
//                writer.WriteValue(objectId != ObjectId.Empty ? objectId.ToString() : string.Empty);
//            }
//            else
//            {
//                throw new Exception("Expected ObjectId value.");
//            }
//        }
//    }

//}
