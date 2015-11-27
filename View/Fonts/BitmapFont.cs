using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace ZooBurst.View.Fonts
{
    public class BitmapFont : IEnumerable<Symbol>
    {
        public int AlphaChannel { get; set; }
        public int BaseHeight { get; set; }
        public int BlueChannel { get; set; }
        public bool Bold { get; set; }

        public IDictionary<char, Symbol> Characters { get; set; }

        public string Charset { get; set; }
        public string FamilyName { get; set; }
        public int FontSize { get; set; }
        public int GreenChannel { get; set; }
        public bool Italic { get; set; }

        public Symbol this[char character] => Characters[character];
        public IDictionary<Kerning, int> Kernings { get; set; }

        public int LineHeight { get; set; }
        public int OutlineSize { get; set; }
        public bool Packed { get; set; }
        public Padding Padding { get; set; }
        public Page[] Pages { get; set; }
        public int RedChannel { get; set; }
        public bool Smoothed { get; set; }
        public Point Spacing { get; set; }
        public int StretchedHeight { get; set; }
        public int SuperSampling { get; set; }
        public Point TextureSize { get; set; }
        public bool Unicode { get; set; }
        public int TabLength { get; set; }

        public BitmapFont()
        {
            TabLength = 4;
        }

        public int GetKerning(char previous, char current)
        {
            var key = new Kerning(previous, current, 0);

            int result;
            
            if (!Kernings.TryGetValue(key, out result))
                return 0;

            return result;
        }

        public virtual void Load(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanSeek)
                throw new ArgumentException("Stream must be seekable in order to determine file format.", "stream");

            var buffer = new byte[5];
            stream.Read(buffer, 0, 5);
            stream.Seek(0, SeekOrigin.Begin);
            var header = Encoding.ASCII.GetString(buffer);

            switch (header)
            {
                case "info ":
                    LoadText(stream);
                    break;
                case "<?xml":
                    LoadXml(stream);
                    break;
                default:
                    throw new InvalidDataException("Unknown file format.");
            }
        }

        public static BitmapFont LoadFile(Stream stream)
        {
            var font = new BitmapFont();
            font.Load(stream);
            return font;
        }

        public void Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (!File.Exists(fileName))
                throw new FileNotFoundException(string.Format("Cannot find file '{0}'.", fileName), fileName);

            using (Stream stream = File.OpenRead(fileName))
            {
                Load(stream);
            }

            BitmapFontLoader.QualifyResourcePaths(this, Path.GetDirectoryName(fileName));
        }

        public void LoadText(string text)
        {
            using (var reader = new StringReader(text))
            {
                LoadText(reader);
            }
        }

        public void LoadText(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var reader = new StreamReader(stream))
            {
                LoadText(reader);
            }
        }

        public void LoadXml(string xml)
        {
            using (var reader = new StringReader(xml))
            {
                LoadXml(reader);
            }
        }

        public void LoadXml(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (TextReader reader = new StreamReader(stream))
            {
                LoadXml(reader);
            }
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            foreach (KeyValuePair<char, Symbol> pair in Characters)
            {
                yield return pair.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Point Measure(string text)
        {
            return Measure(text, null);
        }

        public Point Measure(string text, double? maxWidth)
        {
            string wrappedText;
            return Measure(text, maxWidth, false, out wrappedText);
        }

        public Point Measure(string text, double? maxWidth, bool wordMode, out string wrappedText)
        {
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");
            wrappedText = text;

            if (string.IsNullOrEmpty(text))
                return Point.Zero;

            var length = text.Length;
            var previousCharacter = ' ';
            var currentLineWidth = 0;
            var currentLineHeight = LineHeight;
            var blockWidth = 0;
            var lineHeights = new List<int>();
            var latestSpaceWidth = 0;

            for (int i = 0; i < length; i++)
            {
                var character = text[i];

                if (character == '\n')
                {
                    if (character == '\n' || i + 1 == length || text[i + 1] != '\n')
                    {
                        lineHeights.Add(currentLineHeight);
                        blockWidth = Math.Max(blockWidth, currentLineWidth);
                        currentLineWidth = 0;
                        currentLineHeight = LineHeight;
                    }
                }
                else
                {
                    Symbol data;
                    int width;

                    if (character == '\t')
                    {
                        data = this[' '];
                        width = data.XAdvance * TabLength + GetKerning(previousCharacter, character);
                    }
                    else
                    {
                        data = this[character];
                        width = data.XAdvance + GetKerning(previousCharacter, character);

                        if (character == ' ')
                            latestSpaceWidth = currentLineWidth;
                    }

                    if (maxWidth.HasValue && currentLineWidth + width >= maxWidth)
                    {
                        if (wordMode)
                        {
                            // backtrack to latest space
                            int j;
                            for (j = i - 1; j > 0; j--)
                            {
                                if (text[j] == ' ')
                                    break;
                            }

                            if (j < 1) // no space found, panic
                                text = text.Insert(i, "\n");
                            else
                            {
                                text = text.Insert(j, "\n");
                                text = text.Remove(j + 1, 1); // remove the space
                                i = j - 1;
                                currentLineWidth = latestSpaceWidth;
                                continue;
                            }
                        }
                        else
                        {
                            text = text.Insert(i, "\n");
                        }

                        lineHeights.Add(currentLineHeight);
                        blockWidth = Math.Max(blockWidth, currentLineWidth);
                        currentLineWidth = 0;
                        currentLineHeight = LineHeight;
                    }

                    currentLineWidth += width;
                    currentLineHeight = Math.Max(currentLineHeight, data.Bounds.Height + data.Offset.Y);
                    previousCharacter = character;
                }
            }

            // finish off the current line if required
            if (currentLineHeight != 0)
                lineHeights.Add(currentLineHeight);

            // reduce any lines other than the last back to the base
            for (var i = 0; i < lineHeights.Count - 1; i++)
                lineHeights[i] = LineHeight;

            // calculate the final block height
            var blockHeight = lineHeights.Sum();

            wrappedText = text;
            return new Point(Math.Max(currentLineWidth, blockWidth), blockHeight);
        }

        public virtual void LoadText(TextReader reader)
        {
            string line;

            if (reader == null)
                throw new ArgumentNullException("reader");

            var pageData = new SortedDictionary<int, Page>();
            var kerningDictionary = new Dictionary<Kerning, int>();
            var charDictionary = new Dictionary<char, Symbol>();

            do
            {
                line = reader.ReadLine();

                if (line != null)
                {
                    var parts = BitmapFontLoader.Split(line, ' ');

                    if (parts.Length != 0)
                    {
                        switch (parts[0])
                        {
                            case "info":
                                FamilyName = BitmapFontLoader.GetNamedString(parts, "face");
                                FontSize = BitmapFontLoader.GetNamedInt(parts, "size");
                                Bold = BitmapFontLoader.GetNamedBool(parts, "bold");
                                Italic = BitmapFontLoader.GetNamedBool(parts, "italic");
                                Charset = BitmapFontLoader.GetNamedString(parts, "charset");
                                Unicode = BitmapFontLoader.GetNamedBool(parts, "unicode");
                                StretchedHeight = BitmapFontLoader.GetNamedInt(parts, "stretchH");
                                Smoothed = BitmapFontLoader.GetNamedBool(parts, "smooth");
                                SuperSampling = BitmapFontLoader.GetNamedInt(parts, "aa");
                                Padding = BitmapFontLoader.ParsePadding(BitmapFontLoader.GetNamedString(parts, "padding"));
                                Spacing = BitmapFontLoader.ParsePoint(BitmapFontLoader.GetNamedString(parts, "spacing"));
                                OutlineSize = BitmapFontLoader.GetNamedInt(parts, "outline");
                                break;
                            case "common":
                                LineHeight = BitmapFontLoader.GetNamedInt(parts, "lineHeight");
                                BaseHeight = BitmapFontLoader.GetNamedInt(parts, "base");
                                TextureSize = new Point(BitmapFontLoader.GetNamedInt(parts, "scaleW"), BitmapFontLoader.GetNamedInt(parts, "scaleH"));
                                Packed = BitmapFontLoader.GetNamedBool(parts, "packed");
                                AlphaChannel = BitmapFontLoader.GetNamedInt(parts, "alphaChnl");
                                RedChannel = BitmapFontLoader.GetNamedInt(parts, "redChnl");
                                GreenChannel = BitmapFontLoader.GetNamedInt(parts, "greenChnl");
                                BlueChannel = BitmapFontLoader.GetNamedInt(parts, "blueChnl");
                                break;
                            case "page":
                                var id = BitmapFontLoader.GetNamedInt(parts, "id");
                                var name = BitmapFontLoader.GetNamedString(parts, "file");
                                pageData.Add(id, new Page(id, name));
                                break;
                            case "char":
                                var charData = new Symbol
                                {
                                    Character = (char)BitmapFontLoader.GetNamedInt(parts, "id"),
                                    Bounds = new Rectangle(BitmapFontLoader.GetNamedInt(parts, "x"), BitmapFontLoader.GetNamedInt(parts, "y"), BitmapFontLoader.GetNamedInt(parts, "width"), BitmapFontLoader.GetNamedInt(parts, "height")),
                                    Offset = new Point(BitmapFontLoader.GetNamedInt(parts, "xoffset"), BitmapFontLoader.GetNamedInt(parts, "yoffset")),
                                    XAdvance = BitmapFontLoader.GetNamedInt(parts, "xadvance"),
                                    TexturePage = BitmapFontLoader.GetNamedInt(parts, "page"),
                                    Channel = BitmapFontLoader.GetNamedInt(parts, "chnl")
                                };

                                charDictionary.Add(charData.Character, charData);
                                break;
                            case "kerning":
                                var key = new Kerning((char)BitmapFontLoader.GetNamedInt(parts, "first"), (char)BitmapFontLoader.GetNamedInt(parts, "second"), BitmapFontLoader.GetNamedInt(parts, "amount"));

                                if (!kerningDictionary.ContainsKey(key))
                                    kerningDictionary.Add(key, key.Amount);

                                break;
                        }
                    }
                }
            }
            while (line != null);

            Pages = BitmapFontLoader.ToArray(pageData.Values);
            Characters = charDictionary;
            Kernings = kerningDictionary;
        }

        public virtual void LoadXml(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            var document = new XmlDocument();
            var pageData = new SortedDictionary<int, Page>();
            var kerningDictionary = new Dictionary<Kerning, int>();
            var charDictionary = new Dictionary<char, Symbol>();

            document.Load(reader);
            XmlNode root = document.DocumentElement;

            // load the basic attributes
            var properties = root?.SelectSingleNode("info");
            FamilyName = properties?.Attributes["face"].Value;
            FontSize = Convert.ToInt32(properties.Attributes["size"].Value);
            Bold = Convert.ToInt32(properties.Attributes["bold"].Value) != 0;
            Italic = Convert.ToInt32(properties.Attributes["italic"].Value) != 0;
            Unicode = Convert.ToInt32(properties.Attributes["unicode"].Value) != 0;
            StretchedHeight = Convert.ToInt32(properties.Attributes["stretchH"].Value);
            Charset = properties.Attributes["charset"].Value;
            Smoothed = Convert.ToInt32(properties.Attributes["smooth"].Value) != 0;
            SuperSampling = Convert.ToInt32(properties.Attributes["aa"].Value);
            Padding = BitmapFontLoader.ParsePadding(properties.Attributes["padding"].Value);
            Spacing = BitmapFontLoader.ParsePoint(properties.Attributes["spacing"].Value);
            OutlineSize = Convert.ToInt32(properties.Attributes["outline"].Value);

            // common attributes
            properties = root.SelectSingleNode("common");
            BaseHeight = Convert.ToInt32(properties?.Attributes["lineHeight"].Value);
            LineHeight = Convert.ToInt32(properties?.Attributes["base"].Value);
            TextureSize = new Point(Convert.ToInt32(properties?.Attributes["scaleW"].Value), Convert.ToInt32(properties.Attributes["scaleH"].Value));
            Packed = Convert.ToInt32(properties.Attributes["packed"].Value) != 0;
            AlphaChannel = Convert.ToInt32(properties.Attributes["alphaChnl"].Value);
            RedChannel = Convert.ToInt32(properties.Attributes["redChnl"].Value);
            GreenChannel = Convert.ToInt32(properties.Attributes["greenChnl"].Value);
            BlueChannel = Convert.ToInt32(properties.Attributes["blueChnl"].Value);

            // load texture information
            foreach (XmlNode node in root.SelectNodes("pages/page"))
            {
                var page = new Page()
                {
                    Id = Convert.ToInt32(node?.Attributes["id"].Value),
                    FileName = node?.Attributes["file"].Value
                };

                pageData.Add(page.Id, page);
            }

            Pages = BitmapFontLoader.ToArray(pageData.Values);

            // load character information
            foreach (XmlNode node in root.SelectNodes("chars/char"))
            {
                if (node?.Attributes == null)
                    continue;

                var character = new Symbol
                {
                    Character = (char) Convert.ToInt32(node?.Attributes["id"].Value),
                    Bounds = new Rectangle(Convert.ToInt32(node.Attributes["x"].Value),
                        Convert.ToInt32(node.Attributes["y"].Value), Convert.ToInt32(node.Attributes["width"].Value),
                        Convert.ToInt32(node.Attributes["height"].Value)),
                    Offset = new Point(Convert.ToInt32(node.Attributes["xoffset"].Value), Convert.ToInt32(node.Attributes["yoffset"].Value)),
                    XAdvance = Convert.ToInt32(node.Attributes["xadvance"].Value),
                    TexturePage = Convert.ToInt32(node.Attributes["page"].Value),
                    Channel = Convert.ToInt32(node.Attributes["chnl"].Value)
                };

                charDictionary.Add(character.Character, character);
            }

            Characters = charDictionary;

            // loading kerning information
            foreach (XmlNode node in root.SelectNodes("kernings/kerning"))
            {
                var key = new Kerning((char)Convert.ToInt32(node.Attributes["first"].Value), (char)Convert.ToInt32(node.Attributes["second"].Value), Convert.ToInt32(node.Attributes["amount"].Value));

                if (!kerningDictionary.ContainsKey(key))
                    kerningDictionary.Add(key, key.Amount);
            }

            Kernings = kerningDictionary;
        }

    }
}
