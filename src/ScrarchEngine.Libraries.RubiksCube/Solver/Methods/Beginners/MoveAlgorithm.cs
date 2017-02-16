using Newtonsoft.Json;
using ScrarchEngine.Libraries.RubiksCube.Models;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;

namespace ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners
{
    public class MoveAlgorithm
    {
        public Phase Phase { get; set; }
        public bool IsFinal { get; set; }

        [JsonConverter(typeof(CustomStateConverter))]
        public Dictionary<FaceType, FaceType?[,]> StateFrom { get; set; }

        [JsonConverter(typeof(CustomMoveConverter))]
        public List<Move> Moves { get; set; }

        public MoveAlgorithm()
        {
            StateFrom = new Dictionary<FaceType, FaceType?[,]>();
            Moves = new List<Move>();
        }
    }

    public class CustomMoveConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(string))
            {
                var moves = new List<Move>();

                var value = (string)reader.Value;

                for (int i = 0; i < value.Length; i++)
                {
                    var moveLetter = value[i];
                    var nextLetter = i < value.Length - 1 ? value[i + 1] : (char?)null;

                    if (moveLetter == 'F')
                    {
                        if (!nextLetter.HasValue)
                            moves.Add(new Move(LayerType.Front, RotationType.Clockwise));
                        else if (nextLetter.Value == '\'')
                            moves.Add(new Move(LayerType.Front, RotationType.CounterClockwise));
                        else if (nextLetter.Value == '2')
                        {
                            moves.Add(new Move(LayerType.Front, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Front, RotationType.Clockwise));
                        }
                    }
                    else if (moveLetter == 'B')
                    {
                        if (!nextLetter.HasValue)
                            moves.Add(new Move(LayerType.Back, RotationType.Clockwise));
                        else if (nextLetter.Value == '\'')
                            moves.Add(new Move(LayerType.Back, RotationType.CounterClockwise));
                        else if (nextLetter.Value == '2')
                        {
                            moves.Add(new Move(LayerType.Back, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Back, RotationType.Clockwise));
                        }
                    }
                    else if (moveLetter == 'U')
                    {
                        if (!nextLetter.HasValue)
                            moves.Add(new Move(LayerType.Top, RotationType.Clockwise));
                        else if (nextLetter.Value == '\'')
                            moves.Add(new Move(LayerType.Top, RotationType.CounterClockwise));
                        else if (nextLetter.Value == '2')
                        {
                            moves.Add(new Move(LayerType.Top, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Top, RotationType.Clockwise));
                        }
                    }
                    else if (moveLetter == 'D')
                    {
                        if (!nextLetter.HasValue)
                            moves.Add(new Move(LayerType.Bottom, RotationType.Clockwise));
                        else if (nextLetter.Value == '\'')
                            moves.Add(new Move(LayerType.Bottom, RotationType.CounterClockwise));
                        else if (nextLetter.Value == '2')
                        {
                            moves.Add(new Move(LayerType.Bottom, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Bottom, RotationType.Clockwise));
                        }
                    }
                    else if (moveLetter == 'L')
                    {
                        if (!nextLetter.HasValue)
                            moves.Add(new Move(LayerType.Left, RotationType.Clockwise));
                        else if (nextLetter.Value == '\'')
                            moves.Add(new Move(LayerType.Left, RotationType.CounterClockwise));
                        else if (nextLetter.Value == '2')
                        {
                            moves.Add(new Move(LayerType.Left, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Left, RotationType.Clockwise));
                        }
                    }
                    else if (moveLetter == 'R')
                    {
                        if (!nextLetter.HasValue)
                            moves.Add(new Move(LayerType.Right, RotationType.Clockwise));
                        else if (nextLetter.Value == '\'')
                            moves.Add(new Move(LayerType.Right, RotationType.CounterClockwise));
                        else if (nextLetter.Value == '2')
                        {
                            moves.Add(new Move(LayerType.Right, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Right, RotationType.Clockwise));
                        }
                    }
                }

                return moves;
            }
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomStateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var state = new Dictionary<FaceType, FaceType?[,]>();

            //FaceType face;
            //Enum.TryParse(key, out face);

            var token = JToken.ReadFrom(reader);

            foreach (var child in token)
            {
                var t = child;
                
                FaceType face;
                Enum.TryParse(child.Path, out face);

                var vals = child.Children();
                var items = vals.Children();

                var faceState = new FaceType?[3, 3];
                for (int i = 0; i < 3; i++)
                {
                    var row = items.Values(i);

                    int j = 0;
                    foreach (var tkn in row)
                    {
                        var value = tkn.Value<string>();
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
