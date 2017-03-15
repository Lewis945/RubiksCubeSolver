using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScrarchEngine.Libraries.RubiksCube.Models;
using System;
using System.Collections.Generic;

namespace ScrarchEngine.Libraries.RubiksCube.Json
{
    public class CustomStateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var state = new Dictionary<FaceType, FaceType?[,]>();

            var token = JToken.ReadFrom(reader);

            foreach (var child in token)
            {
                FaceType face;
                Enum.TryParse(child.Path, out face);

                var vals = child.Children();
                var items = vals.Children();

                var faceState = new FaceType?[3, 3];

                int i = 0;
                foreach (var item in items)
                {
                    int j = 0;
                    foreach (var val in item.Children())
                    {
                        var value = val.Value<string>();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            FaceType type;
                            Enum.TryParse(value, out type);
                            faceState[i, j] = type;
                        }
                        else
                            faceState[i, j] = null;

                        j++;
                    }

                    i++;
                }

                state.Add(face, faceState);
            }

            return state;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
