using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZooBurst.Core;
using ZooBurst.View.Animations;
using ZooBurst.Core.Levels;
using ZooBurst.Utils;
using ZooBurst.View.Graphics;
using ZooBurst.View.Activities;

namespace ZooBurst.View
{
    public class GameView : Renderable
    {
        public Point ViewSize { get; }
        public Point TileSize { get; }
        public Swap HighlightedMove { get; private set; }
        public Level Level { get; private set; }
        public Sprite SelectedSprite { get; set; }

        public Layer AnimalLayer { get; }

        public Animation ValidSwapAnimation { get; private set; }
        public Animation InvalidSwapAnimation { get; private set; }
        public Animation MatchAnimation { get; private set; }
        public Animation FallingAnimalsAnimation { get; private set; }
        public Animation NewAnimalsAnimation { get; private set; }

        public GameView(Point viewSize, Point tileSize, Level level)
        {
            ViewSize = viewSize;
            TileSize = tileSize;

            SelectedSprite = new Sprite { BaseAlpha = 0.35F };

            var baseLayer = new Layer
            {
                BackgroundTexture = Assets.GetTexture("background"),
                Size = ViewSize
            };

            AnimalLayer = new Layer();
            baseLayer.AddChild(AnimalLayer);
            AddChild(baseLayer);

            ValidSwapAnimation = new AnimationValidSwap();
            InvalidSwapAnimation = new AnimationInvalidSwap();
            MatchAnimation = new AnimationMatches();
            FallingAnimalsAnimation = new AnimationFallingAnimals();
            NewAnimalsAnimation = new AnimationNewAnimals();

            Level = level;
            Level.ComboMultiplier = 1;
            RefreshLayers();
        }

        public void ShowSuggestion(Swap swap) => HighlightedMove = swap;
        public void HideSuggestion() => HighlightedMove = null;
        public void Wipe() => AnimalLayer.HideChildren();

        public void RefreshSprites(List<Animal> animals)
        {
            AnimalLayer.Clear();

            foreach (var animal in animals)
            {
                if (animal == null)
                    continue;

                var sprite = CreateAnimalSprite(animal, animal.X, animal.Y);
                AnimalLayer.AddChild(sprite);
                animal.SetSprite(sprite);
                animal.Sprite.SetAlpha(0.0F);

                var fullScale = animal.Sprite.Scale;
                animal.Sprite.Scale *= 0.75F;

                animal.Sprite.AddActivity(new ActivitySequence(new Activity[]
                {
                    new WaitActivity(TimeSpan.FromSeconds(0.1F)),
                    new ActivityGroup(new Activity[]
                    {
                        new FadeActivity(FadeActivity.Fade.In, TimeSpan.FromSeconds(0.2F), EaseMode.In),
                        new ScaleActivity(animal.Sprite.Scale, fullScale, TimeSpan.FromSeconds(0.2F), EaseMode.In)
                    })
                }));
            }
        }

        public void DropAnimal(Animal animal, int y)
        {
            var sprite = CreateAnimalSprite(animal, animal.X, y);
            AnimalLayer.AddChild(sprite);
            animal.SetSprite(sprite);
            animal.Sprite.SetAlpha(0.0F);
        }

        public void ShowSelection(Animal animal)
        {
            if (SelectedSprite.Parent != null)
                SelectedSprite.Detach();

            SelectedSprite.SetTexture(Assets.GetTextureCollection("animals_white")[(int)animal.Species]);
            SelectedSprite.Scale = animal.Sprite.Scale;
            SelectedSprite.SetAlpha(1.0F);

            animal.Sprite.AddChild(SelectedSprite);
        }

        public void HideSelection()
        {
            if (SelectedSprite.Parent == null)
                return;

            SelectedSprite.AddActivity(new FadeActivity(FadeActivity.Fade.Out, TimeSpan.FromSeconds(0.33F), EaseMode.InOut, args => SelectedSprite.Detach()));
        }

        private void RefreshLayers()
        {
            if (Level == null)
                return;

            AnimalLayer.Size = new Point(TileSize.X * Level.Width, TileSize.Y * Level.Height);
            AnimalLayer.SetPositionWithCenter(ViewSize.X / 2.0F, ViewSize.Y / 2.0F);
        }

        private Rectangle GetAnimalRectangle(Animal animal)
        {
            return new Rectangle((int)AnimalLayer.Position.X + (animal.X * TileSize.X),
                                 (int)AnimalLayer.Position.Y + (animal.Y * TileSize.Y),
                                 TileSize.X, TileSize.Y);
        }

        private Sprite CreateAnimalSprite(Animal animal, int x, int y)
        {
            var texture = Assets.GetTextureCollection("animals")[(int)animal.Species];
            return new Sprite(texture, 0.21875F) { LocalPosition = new Vector2((x * TileSize.X) + (TileSize.X / 2.0F), (y * TileSize.Y) + (TileSize.Y / 2.0F)) };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (HighlightedMove == null)
                return;

            var sine = (1.0F + (float)(Math.Sin((float)gameTime.TotalGameTime.TotalSeconds * 6.0F) * 1.0F)) / 2.0F;
            spriteBatch.DrawRectangle(GetAnimalRectangle(HighlightedMove.From), Color.Black * Math.Max(0.0F, sine * 0.25F));
            spriteBatch.DrawRectangle(GetAnimalRectangle(HighlightedMove.To), Color.Black * Math.Max(0.0F, sine * 0.25F));
        }
    }
}
