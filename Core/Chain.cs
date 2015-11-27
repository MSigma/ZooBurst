using System.Collections.Generic;

namespace ZooBurst.Core
{
    public class Chain
    {
        public int Score { get; set; }
        public ChainType ChainType { get; }
        public IReadOnlyList<Animal> Animals => _animals.AsReadOnly();
        private readonly List<Animal> _animals;

        public Chain(ChainType type)
        {
            _animals = new List<Animal>();
            ChainType = type;
        }

        public void Add(Animal animal) => _animals.Add(animal);
    }
}
