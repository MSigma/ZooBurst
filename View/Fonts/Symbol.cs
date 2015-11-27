using Microsoft.Xna.Framework;

namespace ZooBurst.View.Fonts
{
    public struct Symbol : ISymbol
    {
        public Rectangle Bounds { get; set; }
        public int Channel { get; set; }
        public char Character { get; set; }
        public Point Offset { get; set; }
        public int TexturePage { get; set; }
        public int XAdvance { get; set; }
        public bool IsNewline => Character == '\n';
        public int CustomSpacing => 0;
        public SymbolType SymbolType => SymbolType.Character;
        public override string ToString() => Character.ToString();
        public int GetKerning(BitmapFont font, ISymbol previousData) => previousData.SymbolType == SymbolType.Icon ? 0 : font.GetKerning(previousData.Character, Character);
    }
}
