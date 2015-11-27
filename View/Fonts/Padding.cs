namespace ZooBurst.View.Fonts
{
    public struct Padding
    {
        public int Bottom;
        public int Left;
        public int Right;
        public int Top;

        public Padding(int left, int top, int right, int bottom)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }

        public override string ToString()
        {
            return $"{Left}, {Top}, {Right}, {Bottom}";
        }
    }
}
