using System;
using ZooBurst.Core.Input;
using ZooBurst.Core.Levels;
using ZooBurst.View;

namespace ZooBurst.Core
{
    public class Controller
    {
        public Level Level { get; private set; }
        public GameView View { get; }
        public int MovesLeft { get; private set; }
        public int Score { get; private set; }
        public PlayState State { get; private set; }

        private DateTime _helpTimerStart;

        private const double SuggestionDelay = 5.0D;

        public Controller(Level level, GameView view)
        {
            View = view;
            Level = level;

            _helpTimerStart = DateTime.Now;

            Restart();
        }

        public void SwapAnimals(Swap swap)
        {
            InputManager.MouseEnabled = false;

            if (!Level.IsPossibleSwap(swap))
            {
                View.InvalidSwapAnimation.Play(View, args => InputManager.MouseEnabled = true, swap);
                return;
            }

            View.HideSuggestion();
            Level.Swap(swap);
            View.ValidSwapAnimation.Play(View, HandleMatches, swap);
        }

        public void Restart()
        {
            State = PlayState.Playing;

            Level.ComboMultiplier = 1;
            MovesLeft = Level.Moves;
            Score = 0;

            Shuffle();
        }

        public void Update(float delta)
        {
            View.Update(delta);
            UpdateSuggestion();
        }

        private void Shuffle()
        {
            if (Level == null)
                throw new Exception("Can't shuffle a null level!");

            View.RefreshSprites(Level.Shuffle());
        }

        private void UpdateSuggestion()
        {
            if (State != PlayState.Playing)
            {
                View.HideSuggestion();
                return;
            }

            if (View.HighlightedMove == null && InputManager.MouseEnabled && (DateTime.Now - _helpTimerStart).TotalSeconds > SuggestionDelay)
                View.ShowSuggestion(Level.RandomPossibleSwap());
        }

        private void HandleMatches(object[] args)
        {
            var chains = Level.RemoveMatches();

            if (chains.Count < 1)
            {
                NextTurn();
                return;
            }

            View.MatchAnimation.Play(View, f =>
            {
                chains.ForEach(e => Score += e.Score);

                View.FallingAnimalsAnimation.Play(View, g =>
                {
                    View.NewAnimalsAnimation.Play(View, HandleMatches, Level.RestockColumns());
                }, Level.LetAnimalsFall());
            }, chains);
        }

        private void NextTurn()
        {
            Level.ComboMultiplier = 1;
            Level.RefreshPossibilities();
            UseMove();

            if (State == PlayState.Playing && !Level.HasPossibleSwaps)
            {
                Console.WriteLine("Impossible to complete! Reshuffling.");

                do
                {
                    Shuffle();
                    Level.RefreshPossibilities();
                } while (!Level.HasPossibleSwaps);
            }

            InputManager.MouseEnabled = true;
            _helpTimerStart = DateTime.Now;
        }

        private void UseMove()
        {
            MovesLeft--;

            if (MovesLeft < 0)
                MovesLeft = 0;

            if (Score >= Level.TargetScore)
                Win();
            else if (MovesLeft == 0)
                Lose();
        }

        private void Win()
        {
            State = PlayState.Success;
            ClearLevel();
        }

        private void Lose()
        {
            State = PlayState.Failure;
            ClearLevel();
        }

        private void ClearLevel()
        {
            Level.Clear();
            View.Wipe();
        }
    }
}
