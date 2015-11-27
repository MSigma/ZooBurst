using System.Collections.Generic;

namespace ZooBurst.Utils
{
    public interface IRandomGenerator
    {
        int GetRandomInt(int min, int max);
        float GetRandomFloat(int min, int max);
        double GetRandomDouble(int min, int max);
        T ChooseFromEnum<T>();
        T ChooseFromList<T>(List<T> list);
    }
}