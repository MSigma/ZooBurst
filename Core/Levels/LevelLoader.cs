using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZooBurst.Core.Levels
{
    public class LevelLoader : ILevelLoader
    {
        public LevelData Load(string filePath)
        {
            if (!filePath.EndsWith(".zbl"))
                filePath += ".zbl";

            if (!File.Exists(filePath))
                return LevelData.Empty;

            var index = 0;
            var source = File.ReadAllText(filePath, Encoding.UTF8);
            var data = new LevelData
            {
                Width = -1,
                Height = -1,
                Moves = -1,
                TargetScore = -1
            };

            if (string.IsNullOrWhiteSpace(source))
                throw new Exception($"The map file '{filePath}' was empty.");

            IList<int> mapValueList = null;
            while (index < source.Length)
            {
                index = SkipWhitespace(source, index);

                string word;
                index = ReadWord(source, index, out word);
                word = word.ToLower();

                if (!word.StartsWith("@"))
                    throw new Exception("Expected '@'");

                switch (word.Remove(0, 1))
                {
                    case "width":
                        data.Width = ReadInteger(filePath, source, ref index, word);
                        break;
                    case "height":
                        data.Height = ReadInteger(filePath, source, ref index, word);
                        break;
                    case "score":
                        data.TargetScore = ReadInteger(filePath, source, ref index, word);
                        break;
                    case "moves":
                        data.Moves = ReadInteger(filePath, source, ref index, word);
                        break;
                    case "map":
                        mapValueList = ReadMap(source, ref index);
                        break;
                }
            }

            if (data.Width == -1) data.Width = 9;
            if (data.Height == -1) data.Height = 9;
            if (data.TargetScore == -1) data.TargetScore = 1000;
            if (data.Moves == -1) data.Moves = 15;
            if (mapValueList == null)
            {
                mapValueList = new List<int>();

                for (var i = 0; i < data.Width*data.Height; i++)
                    mapValueList.Add(1);
            }

            data.Map = new int[data.Width, data.Height];

            for (var y = 0; y < data.Height; y++)
                for (var x = 0; x < data.Width; x++)
                    data.Map[x, y] = mapValueList[(data.Width*y) + x];

            return data;
        }

        private int ReadInteger(string fileName, string source, ref int index, string param)
        {
            if (source[index++] != ':')
                throw new Exception("Expected ':'");

            index = SkipWhitespace(source, index);

            string stringValue;
            index = ReadWord(source, index, out stringValue);

            int readWord;
            if (!int.TryParse(stringValue, out readWord))
                throw new Exception($"The map file '{fileName}' contained an invalid {param} '{stringValue}'.");
            return readWord;
        }

        private IList<int> ReadMap(string source, ref int index)
        {
            if (source[index++] != ':')
                throw new Exception("Expected ':'");

            index = SkipWhitespace(source, index);

            var startIndex = index;
            while (index < source.Length && source[index] != '@')
                index++;

            var mapString = source.Substring(startIndex, index - startIndex);
            return mapString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        }

        public int SkipWhitespace(string source, int index)
        {
            while (index < source.Length && char.IsWhiteSpace(source[index]))
                index++;

            return index;
        }

        public int ReadWord(string source, int index, out string word)
        {
            var startIndex = index;

            while (index < source.Length && (char.IsLetterOrDigit(source[index]) || source[index] == '@'))
                index++;

            word = source.Substring(startIndex, index - startIndex);
            return index;
        }
    }
}