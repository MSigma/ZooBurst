using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZooBurst.View.Fonts
{
    public struct IconSymbol : ISymbol
    {
        public Rectangle Bounds { get; set; }
        public int Channel { get; set; }
        public char Character { get; set; }
        public Point Offset { get; set; }
        public int TexturePage { get; set; }
        public int XAdvance { get; set; }

        public bool IsNewline => false;
        public int CustomSpacing => Texture.Width;
        public SymbolType SymbolType => SymbolType.Icon;

        public const int IconSpacing = 2;
        public Texture2D Texture;

        public IconSymbol(Texture2D texture)
            : this()
        {
            Texture = texture;
        }

        public override string ToString()
        {
            return Character.ToString();
        }

        public int GetKerning(BitmapFont font, ISymbol previousData)
        {
            return IconSpacing;
        }
    }
}
