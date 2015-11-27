using Microsoft.Xna.Framework;

namespace ZooBurst.View.Fonts
{
    public enum SymbolType
    {
        Icon,
        Character
    }

    public interface ISymbol
    {
        Rectangle Bounds { get; set; }
        int Channel { get; set; }
        char Character { get; set; }
        Point Offset { get; set; }
        int TexturePage { get; set; }
        int XAdvance { get; set; }
        bool IsNewline { get; }
        int CustomSpacing { get; }
        SymbolType SymbolType { get; }

        int GetKerning(BitmapFont font, ISymbol previousData);
    }
}
