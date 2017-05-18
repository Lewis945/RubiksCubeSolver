using Newtonsoft.Json;
using ScrarchEngine.Libraries.RubiksCube.Models;
using ScrarchEngine.Libraries.RubiksCube.Solver;
using System;
using System.Collections.Generic;

namespace ScrarchEngine.Libraries.RubiksCube.Json
{
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
}
