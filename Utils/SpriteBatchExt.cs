using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZooBurst.View.Fonts;
using ZooBurst.View.Graphics;

namespace ZooBurst.Utils
{
    public static class SpriteBatchExt
    {
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            DrawRectangleInternal(spriteBatch, rectangle, color, color);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color outlineColor, Color fillColor)
        {
            DrawRectangleInternal(spriteBatch, rectangle, outlineColor, fillColor);
        }

        private static void DrawRectangleInternal(SpriteBatch spriteBatch, Rectangle rectangle, Color? outlineColor, Color? fillColor)
        {
            var fillRectangle = rectangle;
            var pixel = Assets.GetTexture("pixel");

            if (outlineColor != null && fillColor != outlineColor)
            {
                spriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 1), outlineColor.Value);
                spriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, 1, rectangle.Height), outlineColor.Value);
                spriteBatch.Draw(pixel, new Rectangle((rectangle.Right - 1), rectangle.Y, 1, rectangle.Height), outlineColor.Value);
                spriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Bottom - 1, rectangle.Width, 1), outlineColor.Value);
                fillRectangle = new Rectangle(rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2);
            }

            if (fillColor != null)
                spriteBatch.Draw(pixel, fillRectangle, fillColor.Value);
        }

        private static void DrawSymbol(SpriteBatch spriteBatch, SpriteBitmapFont font, ISymbol symbol, int x, int y, Color color, Color outlineColor)
        {
            if (symbol.SymbolType == SymbolType.Icon)
            {
                var iconSymbol = (IconSymbol)symbol;

                spriteBatch.Draw(iconSymbol.Texture, new Vector2(x, y), null, Color.White);
                return;
            }

            spriteBatch.Draw(font.Textures[symbol.TexturePage], new Vector2(x, y), symbol.Bounds, color);
        }

        public static void DrawString(this SpriteBatch spriteBatch, SpriteBitmapFont font, string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, text, position, color, color);
        }

        public static void DrawString(this SpriteBatch spriteBatch, SpriteBitmapFont font, string text, Vector2 position, Color color, Color outlineColor)
        {
            var previousCharacter = ' ';
            var normalizedText = text.Replace("\r\n", "\n").Replace("\r", "\n");

            var x = 0;
            var y = 0;

            foreach (var character in normalizedText)
            {
                if (character == '\n')
                {
                    x = 0;
                    y += font.LineHeight;
                    continue;
                }

                var data = font[character];
                var kerning = font.GetKerning(previousCharacter, character);

                DrawSymbol(spriteBatch, font, data, (int)position.X + (x + data.Offset.X + kerning), (int)position.Y + (y + data.Offset.Y), color, outlineColor);
                x += data.XAdvance + kerning;

                previousCharacter = character;
            }
        }
    }
}
