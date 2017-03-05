using Newtonsoft.Json;
using ScrarchEngine.Libraries.RubiksCube.Models;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;

namespace ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners
{
    public class MoveAlgorithm
    {
        public string Name { get; set; }

        public Phase Phase { get; set; }
        public bool IsFinal { get; set; }

        public bool IsFlip { get; set; }
        public FlipAxis Axis { get; set; }
        public RotationType RotationType { get; set; }

        [JsonConverter(typeof(CustomStateConverter))]
        public Dictionary<FaceType, FaceType?[,]> StateFrom { get; set; }

        [JsonConverter(typeof(CustomMoveConverter))]
        public List<Move> Moves { get; set; }

        public MoveAlgorithm()
        {
            StateFrom = new Dictionary<FaceType, FaceType?[,]>();
            Moves = new List<Move>();
        }

        public MoveAlgorithm(FlipAxis axis, RotationType type)
        {
            IsFlip = true;
            Axis = axis;
            RotationType = type;
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
                    var moveLetter = value[i].ToString();
                    var nextLetter = i < value.Length - 1 ? value[i + 1].ToString() : null;

                    if (moveLetter == "F")
                    {
                        if (nextLetter == "\'")
                        {
                            moves.Add(new Move(LayerType.Front, RotationType.CounterClockwise));
                            i++;
                        }
                        else if (nextLetter == "2")
                        {
                            moves.Add(new Move(LayerType.Front, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Front, RotationType.Clockwise));
                            i++;
                        }
                        else
                        {
                            moves.Add(new Move(LayerType.Front, RotationType.Clockwise));
                        }
                    }
                    else if (moveLetter == "B")
                    {
                        if (nextLetter == "\'")
                        {
                            moves.Add(new Move(LayerType.Back, RotationType.CounterClockwise));
                            i++;
                        }
                        else if (nextLetter == "2")
                        {
                            moves.Add(new Move(LayerType.Back, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Back, RotationType.Clockwise));
                            i++;
                        }
                        else
                        {
                            moves.Add(new Move(LayerType.Back, RotationType.Clockwise));
                        }
                    }
                    else if (moveLetter == "U")
                    {
                        if (nextLetter == "\'")
                        {
                            moves.Add(new Move(LayerType.Top, RotationType.CounterClockwise));
                            i++;
                        }
                        else if (nextLetter == "2")
                        {
                            moves.Add(new Move(LayerType.Top, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Top, RotationType.Clockwise));
                            i++;
                        }
                        else
                        {
                            moves.Add(new Move(LayerType.Top, RotationType.Clockwise));
                        }
                    }
                    else if (moveLetter == "D")
                    {
                        if (nextLetter == "\'")
                        {
                            moves.Add(new Move(LayerType.Bottom, RotationType.CounterClockwise));
                            i++;
                        }
                        else if (nextLetter == "2")
                        {
                            moves.Add(new Move(LayerType.Bottom, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Bottom, RotationType.Clockwise));
                            i++;
                        }
                        else
                        {
                            moves.Add(new Move(LayerType.Bottom, RotationType.Clockwise));
                        }
                    }
                    else if (moveLetter == "L")
                    {
                        if (nextLetter == "\'")
                        {
                            moves.Add(new Move(LayerType.Left, RotationType.CounterClockwise));
                            i++;
                        }
                        else if (nextLetter == "2")
                        {
                            moves.Add(new Move(LayerType.Left, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Left, RotationType.Clockwise));
                            i++;
                        }
                        else { moves.Add(new Move(LayerType.Left, RotationType.Clockwise)); }
                    }
                    else if (moveLetter == "R")
                    {
                        if (nextLetter == "\'")
                        {
                            moves.Add(new Move(LayerType.Right, RotationType.CounterClockwise));
                            i++;
                        }
                        else if (nextLetter == "2")
                        {
                            moves.Add(new Move(LayerType.Right, RotationType.Clockwise));
                            moves.Add(new Move(LayerType.Right, RotationType.Clockwise));
                            i++;
                        }
                        else { moves.Add(new Move(LayerType.Right, RotationType.Clockwise)); }
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
