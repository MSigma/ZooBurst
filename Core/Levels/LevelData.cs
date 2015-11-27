namespace ZooBurst.Core.Levels
{
    public struct LevelData
    {
        public static readonly LevelData Empty = new LevelData { Width = -1, Height = -1, TargetScore = -1, Moves = -1, Map = null };

        public int Width { get; set; }
        public int Height { get; set; }
        public int TargetScore { get; set; }
        public int Moves { get; set; }
        public int[,] Map { get; set; }
    }
}