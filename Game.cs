using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZooBurst.Core.Levels;
using ZooBurst.View.Graphics;
using ZooBurst.Core;
using ZooBurst.Core.Input;
using ZooBurst.Utils;
using ZooBurst.View;
using ZooBurst.View.Fonts;

namespace ZooBurst
{
    internal class Game : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager Graphics { get; set; }
        public SpriteBatch SpriteBatch { get; set; }

        private Controller _controller;
        private SpriteBitmapFont _font;

        public Game()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Graphics.PreferredBackBufferWidth = 800;
            Graphics.PreferredBackBufferHeight = 800;
            Graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var mersenneTwister = new MersenneTwister();

            var loader = new LevelLoader();
            var firstLevel = new Level(loader.Load("Content/first"), mersenneTwister);
            var view = new GameView(new Point(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), new Point(64, 64), firstLevel);

            _controller = new Controller(firstLevel, view);
            var input = new MouseInputHandler(_controller, view);
            input.Swap += _controller.SwapAnimals;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Assets.Initialize(GraphicsDevice, Content.RootDirectory);

            Assets.RegisterTextureCollection("animals", new[]
            {
                "monkey.png",
                "parrot.png",
                "penguin.png",
                "pig.png",
                "snake.png",
                "rabbit.png",
                "giraffe.png"
            },
            true);

            Assets.RegisterTexture("starburst_1024_blue.png", "background");
            Assets.RegisterFont("Content/Fonts/dsfont.fnt", "dsfont");

            _font = Assets.GetFont("dsfont");
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            InputManager.Update(gameTime);

            _controller.Update(delta);
            base.Update(gameTime);
        }

        private void DrawText(string text, Vector2 position)
        {
            SpriteBatch.DrawString(_font, text, new Vector2((int)position.X, (int)position.Y + 1), Color.Black);
            SpriteBatch.DrawString(_font, text, new Vector2((int)position.X, (int)position.Y), Color.White);
        }

        private void DrawFullscreenMessage(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.DrawRectangle(new Rectangle(0, 0, _controller.View.ViewSize.X, _controller.View.ViewSize.Y), Color.Black * 0.75F);

            var textSize = _font.Measure(text);
            DrawText(text, new Vector2((int)((_controller.View.ViewSize.X / 2.0F) - (textSize.X / 2.0F)),
                                       (int)((_controller.View.ViewSize.Y / 2.0F) - textSize.Y)));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();

            _controller.View.Draw(gameTime, SpriteBatch);

            DrawText($"{_controller.MovesLeft} moves left", new Vector2(6, 6));
            DrawText($"{_controller.Score}/{_controller.Level.TargetScore} points", new Vector2(6, 18));

            switch (_controller.State)
            {
                case PlayState.Success:
                    DrawFullscreenMessage(SpriteBatch, "You won! Press Enter to play again.");
                    break;
                case PlayState.Failure:
                    DrawFullscreenMessage(SpriteBatch, "You lost... Press Enter to try again.");
                    break;
            }

            SpriteBatch.End();
            base.Draw(gameTime);
        }

        internal static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
