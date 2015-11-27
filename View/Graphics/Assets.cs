using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZooBurst.View.Fonts;

namespace ZooBurst.View.Graphics
{
    public static class Assets
    {
        private static readonly Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, SpriteBitmapFont> Fonts = new Dictionary<string, SpriteBitmapFont>();
        private static readonly Dictionary<string, List<Texture2D>> TextureCollections = new Dictionary<string, List<Texture2D>>();

        private static GraphicsDevice _gd;
        private static Texture2D _pixel;

        private static string _assetFolder = "Assets";

        public static void Initialize(GraphicsDevice graphicsDevice, string assetFolder)
        {
            _assetFolder = assetFolder;

            _gd = graphicsDevice;

            _pixel = new Texture2D(_gd, 1, 1);
            _pixel.SetData(new[] { Color.White });

            Textures.Add("pixel", _pixel);
        }

        private static Texture2D MakeWhiteCopy(Texture2D texture)
        {
            var originalColorData = new Color[texture.Width * texture.Height];
            texture.GetData(originalColorData);

            var whiteColorData = new Color[texture.Width * texture.Height];
            for (var i = 0; i < originalColorData.Length; i++)
            {
                if (originalColorData[i].A > 0)
                    whiteColorData[i] = Color.White;
            }

            var whiteCopy = new Texture2D(_gd, texture.Width, texture.Height);
            whiteCopy.SetData(whiteColorData);
            return whiteCopy;
        }

        public static Texture2D LoadTexture(string fileName)
        {
            if (!fileName.StartsWith(_assetFolder))
                fileName = Path.Combine(_assetFolder, fileName);

            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var texture = Texture2D.FromStream(_gd, fs);
                texture.Name = fileName;
                return texture;
            }
        }

        public static Texture2D RegisterTexture(string fileName, string alias = null, bool makeWhiteCopy = false)
        {
            try
            {
                var texture = LoadTexture(fileName);
                var key = alias ?? fileName;

                if (!Textures.ContainsKey(key))
                    Textures.Add(key, texture);

                if (!makeWhiteCopy)
                    return texture;

                var whiteCopy = MakeWhiteCopy(texture);
                if (whiteCopy == null || Textures.ContainsKey(key + "_white"))
                    throw new Exception($"Unable to produce white copy for '{fileName}'.");

                Textures.Add(key + "_white", whiteCopy);
                return texture;
            }
            catch (Exception)
            {
                throw new Exception($"Missing or invalid texture '{fileName}'.");
            }
        }

        public static List<Texture2D> RegisterTextureCollection(string collectionName, string[] fileNames, bool makeWhiteCopies = false)
        {
            var textureList = new List<Texture2D>();
            var whiteCopyList = new List<Texture2D>();

            foreach (var fn in fileNames)
            {
                var texture = LoadTexture(fn);
                textureList.Add(texture);

                if (!makeWhiteCopies)
                    continue;

                var whiteCopy = MakeWhiteCopy(texture);
                if (whiteCopy == null)
                    throw new Exception($"Unable to produce white copy for '{fn}'.");

                whiteCopyList.Add(whiteCopy);
            }

            TextureCollections.Add(collectionName, textureList);

            if (makeWhiteCopies)
                TextureCollections.Add(collectionName + "_white", whiteCopyList);

            return textureList;
        }

        public static Texture2D GetTexture(string key)
        {
            if (Textures.ContainsKey(key))
                return Textures[key];

            Console.WriteLine($"Tried to access missing texture key '{key}' in Assets!");
            return null;
        }

        public static List<Texture2D> GetTextureCollection(string key)
        {
            if (TextureCollections.ContainsKey(key))
                return TextureCollections[key];

            Console.WriteLine($"Tried to access missing texture collection key '{key}' in Assets!");
            return null;
        }

        public static SpriteBitmapFont RegisterFont(string fileName, string alias = null)
        {
            if (!fileName.StartsWith(_assetFolder))
                fileName = Path.Combine(_assetFolder, fileName);

            try
            {
                var key = alias ?? fileName;

                if (Fonts.ContainsKey(key))
                    return null;

                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    var font = SpriteBitmapFont.LoadFile(fs, _gd);
                    Fonts.Add(key, font);
                    return font;
                }
            }
            catch (Exception)
            {
                throw new Exception($"Missing or invalid font '{fileName}'.");
            }
        }

        public static SpriteBitmapFont GetFont(string key)
        {
            if (Fonts.ContainsKey(key))
                return Fonts[key];

            Console.WriteLine($"Tried to access missing font key '{key}' in Assets!");
            return null;
        }
    }
}