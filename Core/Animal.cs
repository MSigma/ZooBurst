using ZooBurst.View.Graphics;

namespace ZooBurst.Core
{
    public class Animal
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public AnimalType Species { get; private set; }
        public Sprite Sprite { get; private set; }

        public Animal(int x, int y, AnimalType species)
        {
            X = x;
            Y = y;
            Species = species;
        }

        public void MoveTo(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetSprite(Sprite sprite)
        {
            Sprite = sprite;
        }
    }
}
