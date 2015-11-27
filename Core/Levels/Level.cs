using System.Collections.Generic;
using System.Linq;
using ZooBurst.Core.Tiles;
using ZooBurst.Utils;
using Microsoft.Xna.Framework;

namespace ZooBurst.Core.Levels
{
    public class Level
    {
        public LevelData Data { get; }
        public int Width => Data.Width;
        public int Height => Data.Height;
        public int TargetScore => Data.TargetScore;
        public int Moves => Data.Moves;
        public bool HasPossibleSwaps => _possibleSwaps.Count > 0;
        public int ComboMultiplier { get; set; }

        private readonly BaseTile[,] _tiles;
        private readonly Animal[,] _animals;
        private readonly List<Swap> _possibleSwaps;
        private IRandomGenerator _random;

        private const int ScorePerBonusAnimal = 60;

        public Level(LevelData data, IRandomGenerator random)
        {
            Data = data;

            _random = random;

            _animals = new Animal[Width, Height];
            _tiles = new BaseTile[Width, Height];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    switch (Data.Map[x, y])
                    {
                        case 1:
                            _tiles[x, y] = new SimpleTile();
                            break;
                    }
                }
            }

            _possibleSwaps = new List<Swap>();
        }

        public Swap RandomPossibleSwap() => _random.ChooseFromList(_possibleSwaps);
        public bool IsPossibleSwap(Swap swap) => _possibleSwaps.Contains(swap);
        public Animal GetAnimalAt(int x, int y) => Contains(x, y) ? _animals[x, y] : null;
        public AnimalType? GetSpeciesIdAt(int x, int y) => Contains(x, y) && _animals[x, y] != null ? (AnimalType?)_animals[x, y].Species : null;
        public BaseTile GetTileAt(int x, int y) => Contains(x, y) ? _tiles[x, y] : null;
        public bool Contains(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;
        public bool Contains(Point point) => point.X >= 0 && point.Y >= 0 && point.X < Width && point.Y < Height;

        private bool SpeciesMatch(int x, int y, AnimalType typeToMatch) => (GetSpeciesIdAt(x, y) == typeToMatch);
        private Animal SetAnimalAt(int x, int y, Animal animal) => _animals[x, y] = animal;
        private void CalculateScore(List<Chain> chains) => chains.ForEach(e => e.Score = (ScorePerBonusAnimal * (e.Animals.Count - 2)) * ComboMultiplier++);

        public List<Animal> Shuffle()
        {
            List<Animal> list;

            do
            {
                list = Populate();
                RefreshPossibilities();
            }
            while (_possibleSwaps.Count < 1);

            return list;
        }

        public void Swap(Swap swap)
        {
            var from = new Point(swap.From.X, swap.From.Y);
            var to = new Point(swap.To.X, swap.To.Y);
            _animals[from.X, from.Y] = swap.To; swap.To.MoveTo(from.X, from.Y);
            _animals[to.X, to.Y] = swap.From; swap.From.MoveTo(to.X, to.Y);
        }

        public void RefreshPossibilities()
        {
            _possibleSwaps.Clear();

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var animal = GetAnimalAt(x, y);

                    if (animal == null)
                        continue;

                    var swap = AttemptSwap(x, y, true, animal);
                    if (swap != null)
                        _possibleSwaps.Add(swap);

                    swap = AttemptSwap(x, y, false, animal);
                    if (swap != null)
                        _possibleSwaps.Add(swap);
                }
            }
        }

        public void Clear()
        {
            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                    _animals[x, y] = null;

            _possibleSwaps.Clear();
            ComboMultiplier = 1;
        }

        public List<List<Animal>> RestockColumns()
        {
            var restockedColumns = new List<List<Animal>>();

            for (var x = 0; x < Width; x++)
            {
                List<Animal> column = null;
                for (var y = 0; y < Height && _animals[x, y] == null; y++)
                {
                    if (_tiles[x, y] == null)
                        continue;

                    if (column == null)
                    {
                        column = new List<Animal>();
                        restockedColumns.Add(column);
                    }

                    column.Add(SetAnimalAt(x, y, new Animal(x, y, _random.ChooseFromEnum<AnimalType>())));
                }
            }

            return restockedColumns;
        }

        public List<List<Animal>> LetAnimalsFall()
        {
            var columns = new List<List<Animal>>();

            for (var x = 0; x < Width; x++)
            {
                List<Animal> column = null;
                for (var y = Height - 1; y >= 0; y--)
                {
                    if (_tiles[x, y] == null || _animals[x, y] != null)
                        continue;

                    for (var i = y - 1; i >= 0; i--)
                    {
                        var animal = GetAnimalAt(x, i);
                        if (animal == null)
                            continue;

                        SetAnimalAt(x, i, null);
                        SetAnimalAt(x, y, animal);
                        animal.MoveTo(animal.X, y);

                        if (column == null)
                        {
                            column = new List<Animal>();
                            columns.Add(column);
                        }

                        column.Add(animal);
                        break;
                    }
                }
            }

            return columns;
        }

        public List<Chain> RemoveMatches()
        {
            var chains = GetMatches();

            foreach (var animal in chains.SelectMany(chain => chain.Animals))
                _animals[animal.X, animal.Y] = null;

            CalculateScore(chains);
            return chains;
        }

        private List<Animal> Populate()
        {
            var list = new List<Animal>();

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    AnimalType type;
                    do
                        type = _random.ChooseFromEnum<AnimalType>();
                    while ((x >= 2 && SpeciesMatch(x - 1, y, type) && SpeciesMatch(x - 2, y, type)) ||
                           (y >= 2 && SpeciesMatch(x, y - 1, type) && SpeciesMatch(x, y - 2, type)));

                    if (_tiles[x, y] != null)
                        list.Add(SetAnimalAt(x, y, new Animal(x, y, type)));
                }
            }

            return list;
        }

        private Swap AttemptSwap(int x, int y, bool horizontal, Animal animal)
        {
            var xDelta = horizontal ? 1 : 0;
            var yDelta = horizontal ? 0 : 1;

            if ((!horizontal || x >= Width - 1) && (horizontal || y >= Height - 1))
                return null;

            var other = GetAnimalAt(x + xDelta, y + yDelta);
            if (other == null)
                return null;

            SetAnimalAt(x, y, other);
            SetAnimalAt(x + xDelta, y + yDelta, animal);

            Swap swap = null;
            if (ChainExistsAt(x + xDelta, y + yDelta) || ChainExistsAt(x, y))
                swap = new Swap(animal, other);

            SetAnimalAt(x, y, animal);
            SetAnimalAt(x + xDelta, y + yDelta, other);
            return swap;
        }

        private bool ChainExistsAt(int x, int y)
        {
            var animal = GetAnimalAt(x, y);
            if (animal == null)
                return false;

            var width = 1;
            for (var i = x - 1; i >= 0 && SpeciesMatch(i, y, animal.Species); i--)
                width++;

            for (var i = x + 1; i < Width && SpeciesMatch(i, y, animal.Species); i++)
                width++;

            if (width >= 3)
                return true;

            var height = 1;
            for (var i = y - 1; i >= 0 && SpeciesMatch(x, i, animal.Species); i--)
                height++;

            for (var i = y + 1; i < Height && SpeciesMatch(x, i, animal.Species); i++)
                height++;

            return height >= 3;
        }

        private List<Chain> GetMatches()
        {
            var matches = new List<Chain>();

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width - 2;)
                {
                    var animal = GetAnimalAt(x, y);

                    if (animal == null || !SpeciesMatch(x + 1, y, animal.Species) || !SpeciesMatch(x + 2, y, animal.Species))
                    {
                        x++;
                        continue;
                    }

                    var chain = new Chain(ChainType.Horizontal);
                    do chain.Add(GetAnimalAt(x++, y));
                    while (x < Width && SpeciesMatch(x, y, animal.Species));
                    matches.Add(chain);
                }
            }

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height - 2;)
                {
                    var animal = GetAnimalAt(x, y);

                    if (animal == null || !SpeciesMatch(x, y + 1, animal.Species) || !SpeciesMatch(x, y + 2, animal.Species))
                    {
                        y++;
                        continue;
                    }

                    var chain = new Chain(ChainType.Vertical);
                    do chain.Add(GetAnimalAt(x, y++));
                    while (y < Height && SpeciesMatch(x, y, animal.Species));
                    matches.Add(chain);
                }
            }

            return matches;
        }
    }
}
