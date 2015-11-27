namespace ZooBurst.View.Fonts
{
    public struct Kerning
    {
        public int Amount;
        public char First;
        public char Second;

        public Kerning(char first, char second, int amount)
        {
            First = first;
            Second = second;
            Amount = amount;
        }

        public override string ToString()
        {
            return string.Format("{0} <> {1} = {2}", First, Second, Amount);
        }
    }
}