using Newtonsoft.Json;
using ScrarchEngine.Libraries.RubiksCube.Models;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Linq;

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

        private int Process(Move move, Move nextMove, int i, LayerType layer, string layerLetter, StringBuilder builder)
        {
            if (move.Layer == layer)
            {
                if (move.Rotation == RotationType.CounterClockwise)
                    builder.Append($"{layerLetter}'");
                else if (nextMove != null && nextMove.Layer == layer)
                {
                    builder.Append($"{layerLetter}2");
                    i++;
                }
                else
                    builder.Append(layerLetter);
            }

            return i;
        }

        public override string ToString()
        {
            var algorithm = new StringBuilder();
            if (Moves != null)
                for (int i = 0; i < Moves.Count; i++)
                {
                    var move = Moves[i];
                    var nextMove = i < Moves.Count - 1 ? Moves[i + 1] : null;

                    i = Process(move, nextMove, i, LayerType.Front, "F", algorithm);
                    i = Process(move, nextMove, i, LayerType.Back, "B", algorithm);
                    i = Process(move, nextMove, i, LayerType.Top, "U", algorithm);
                    i = Process(move, nextMove, i, LayerType.Bottom, "D", algorithm);
                    i = Process(move, nextMove, i, LayerType.Left, "L", algorithm);
                    i = Process(move, nextMove, i, LayerType.Right, "R", algorithm);
                }
            else if(IsFlip)
                algorithm.Append($"Flip {Axis.ToString()} in {RotationType.ToString()}");

            return algorithm.ToString();
        }
    }

    public class CustomMoveConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        private int Process(string moveLetter, string nextMoveLetter, string layerLetter, int i, LayerType layer, List<Move> moves)
        {
            if (moveLetter == layerLetter)
            {
                if (nextMoveLetter == "\'")
                {
                    moves.Add(new Move(layer, RotationType.CounterClockwise));
                    i++;
                }
                else if (nextMoveLetter == "2")
                {
                    moves.Add(new Move(layer, RotationType.Clockwise));
                    moves.Add(new Move(layer, RotationType.Clockwise));
                    i++;
                }
                else
                    moves.Add(new Move(layer, RotationType.Clockwise));
            }

            return i;
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

                    i = Process(moveLetter, nextLetter, "F", i, LayerType.Front, moves);
                    i = Process(moveLetter, nextLetter, "B", i, LayerType.Back, moves);
                    i = Process(moveLetter, nextLetter, "U", i, LayerType.Top, moves);
                    i = Process(moveLetter, nextLetter, "D", i, LayerType.Bottom, moves);
                    i = Process(moveLetter, nextLetter, "L", i, LayerType.Left, moves);
                    i = Process(moveLetter, nextLetter, "R", i, LayerType.Right, moves);
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
