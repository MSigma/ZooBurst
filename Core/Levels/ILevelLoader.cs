namespace ZooBurst.Core.Levels
{
    public interface ILevelLoader
    {
        LevelData Load(string filePath);
    }
}