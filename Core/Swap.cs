namespace ZooBurst.Core
{
    public class Swap
    {
        public Animal From { get; }
        public Animal To { get; }

        public Swap(Animal from, Animal to)
        {
            From = from;
            To = to;
        }

        public override bool Equals(object obj)
        {
            var p = obj as Swap;

            if (p == null)
                return false;

            return (this == p);
        }

        public bool Equals(Swap obj)
        {
            if (obj == null)
                return false;

            return (this == obj);
        }

        public static bool operator ==(Swap a, Swap b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            return ((a.From == b.From && a.To == b.To) || (a.From == b.To && a.To == b.From));
        }

        public static bool operator !=(Swap a, Swap b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() ^ To.GetHashCode();
        }
    }
}
