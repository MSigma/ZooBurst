using System;
using System.Collections.Generic;

namespace ZooBurst.Utils
{
    public class MersenneTwister : IRandomGenerator
    {
        private readonly RandomMt _random;

        public MersenneTwister()
        {
            _random = new RandomMt();
        }

        public int GetRandomInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        public float GetRandomFloat(int min, int max)
        {
            return min + (_random.NextFloatPositive() * (max - min));
        }

        public double GetRandomDouble(int min, int max)
        {
            return min + (_random.NextDoublePositive() * (max - min));
        }

        public T ChooseFromEnum<T>()
        {
            var array = Enum.GetValues(typeof(T));
            return (T)array.GetValue(_random.Next(array.Length));
        }

        public T ChooseFromList<T>(List<T> list)
        {
            return list[_random.Next(list.Count)];
        }
    }
}
