using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace ZooBurst.View.Fonts
{
    internal static class BitmapFontLoader
    {
        public static BitmapFont LoadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName", "File name not specified");

            if (!File.Exists(fileName))
                throw new FileNotFoundException($"Cannot find file '{fileName}'", fileName);

            using (var file = File.OpenRead(fileName))
            {
                using (var reader = new StreamReader(file))
                {
                    var line = reader.ReadLine();

                    if (line?.StartsWith("info ") ?? false)
                        return LoadTextFile(fileName);

                    if (line?.StartsWith("<?xml") ?? false)
                        return LoadXmlFile(fileName);

                    throw new InvalidDataException("Unknown file format.");
                }
            }
        }

        public static BitmapFont LoadTextFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (!File.Exists(fileName))
                throw new FileNotFoundException($"Cannot find file '{fileName}'", fileName);

            var font = new BitmapFont();

            using (var stream = File.OpenRead(fileName))
            {
                font.LoadText(stream);
            }

            QualifyResourcePaths(font, Path.GetDirectoryName(fileName));
            return font;
        }

        public static BitmapFont LoadXmlFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (!File.Exists(fileName))
                throw new FileNotFoundException($"Cannot find file '{fileName}'", fileName);

            var font = new BitmapFont();

            using (Stream stream = File.OpenRead(fileName))
            {
                font.LoadXml(stream);
            }

            QualifyResourcePaths(font, Path.GetDirectoryName(fileName));
            return font;
        }

        internal static bool GetNamedBool(string[] parts, string name)
        {
            return GetNamedInt(parts, name) != 0;
        }

        internal static int GetNamedInt(string[] parts, string name)
        {
            return Convert.ToInt32(GetNamedString(parts, name));
        }

        internal static string GetNamedString(string[] parts, string name)
        {
            var result = string.Empty;

            foreach (string part in parts)
            {
                var nameEndIndex = part.IndexOf('=');

                if (nameEndIndex == -1)
                    continue;

                var namePart = part.Substring(0, nameEndIndex);
                var valuePart = part.Substring(nameEndIndex + 1);

                if (!string.Equals(name, namePart, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var length = valuePart.Length;
                if (length > 1 && valuePart[0] == '"' && valuePart[length - 1] == '"')
                    valuePart = valuePart.Substring(1, length - 2);

                result = valuePart;
                break;
            }

            return result;
        }

        internal static Padding ParsePadding(string s)
        {
            var parts = s.Split(',');

            return new Padding()
            {
                Left = Convert.ToInt32(parts[3].Trim()),
                Top = Convert.ToInt32(parts[0].Trim()),
                Right = Convert.ToInt32(parts[1].Trim()),
                Bottom = Convert.ToInt32(parts[2].Trim())
            };
        }

        internal static Point ParsePoint(string s)
        {
            var parts = s.Split(',');

            return new Point()
            {
                X = Convert.ToInt32(parts[0].Trim()),
                Y = Convert.ToInt32(parts[1].Trim())
            };
        }

        internal static string[] Split(string s, char delimiter)
        {
            if (s.IndexOf('"') == -1)
                return s.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

            var partStart = -1;
            var parts = new List<string>();

            do
            {
                var quoteStart = s.IndexOf('"', partStart + 1);
                var quoteEnd = s.IndexOf('"', quoteStart + 1);
                var partEnd = s.IndexOf(delimiter, partStart + 1);

                if (partEnd == -1)
                    partEnd = s.Length;

                var hasQuotes = quoteStart != -1 && partEnd > quoteStart && partEnd < quoteEnd;
                if (hasQuotes)
                    partEnd = s.IndexOf(delimiter, quoteEnd + 1);

                parts.Add(s.Substring(partStart + 1, partEnd - partStart - 1));

                if (hasQuotes)
                    partStart = partEnd - 1;

                partStart = s.IndexOf(delimiter, partStart + 1);
            }
            while (partStart != -1);

            return parts.ToArray();
        }

        internal static T[] ToArray<T>(ICollection<T> values)
        {
            var result = new T[values.Count];
            values.CopyTo(result, 0);
            return result;
        }

        internal static void QualifyResourcePaths(BitmapFont font, string resourcePath)
        {
            var pages = font.Pages;

            for (var i = 0; i < pages.Length; i++)
            {
                var page = pages[i];
                page.FileName = Path.Combine(resourcePath, page.FileName);
                pages[i] = page;
            }

            font.Pages = pages;
        }
    }
}
