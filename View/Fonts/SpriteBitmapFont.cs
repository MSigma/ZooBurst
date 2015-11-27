using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ZooBurst.View.Fonts
{
    public class SpriteBitmapFont : BitmapFont
    {
        public Texture2D[] Textures;

        public static SpriteBitmapFont LoadFile(Stream stream, GraphicsDevice graphicsDevice)
        {
            var font = new SpriteBitmapFont();
            font.Load(stream);

            font.Textures = new Texture2D[font.Pages.Length];
            for (int i = 0; i < font.Pages.Length; i++)
            {
                using (var fs = new FileStream(font.Pages[i].FileName, FileMode.Open, FileAccess.Read))
                {
                    font.Textures[i] = Texture2D.FromStream(graphicsDevice, fs);
                }
            }

            return font;
        }

        public static SpriteBitmapFont LoadFile(Stream stream, ContentManager contentManager)
        {
            var font = new SpriteBitmapFont();
            font.Load(stream);

            font.Textures = new Texture2D[font.Pages.Length];
            for (int i = 0; i < font.Pages.Length; i++)
            {
                font.Textures[i] = contentManager.Load<Texture2D>(font.Pages[i].FileName);
            }

            return font;
        }
    }
}
