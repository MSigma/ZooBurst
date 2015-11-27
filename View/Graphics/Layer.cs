using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ZooBurst.Utils;
using ZooBurst.View.Activities;

namespace ZooBurst.View.Graphics
{
    public class Layer : Renderable
    {
        public Color? BackgroundColor { get; set; }
        public Texture2D BackgroundTexture { get; set; }
        public Point Size { get; set; }

        public Vector2 SetPositionWithCenter(float x, float y) => (LocalPosition = new Vector2(x - (Size.X / 2.0F), y - (Size.Y / 2.0F)));

        public void HideChildren()
        {
            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].AddActivity(new ActivitySequence(new Activity[]
                {
                    new WaitActivity(TimeSpan.FromSeconds(0.1D)),
                    new FadeActivity(FadeActivity.Fade.Out, TimeSpan.FromSeconds(0.15D), EaseMode.Out)
                },
                args =>
                {
                    if (args == null || args.Length < 1)
                        return;

                    int index;
                    if (int.TryParse(args[0].ToString(), out index))
                        Children.RemoveAt(index);
                }));
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (BackgroundColor.HasValue)
                spriteBatch.DrawRectangle(new Rectangle((int)Position.X, (int)Position.Y, Size.X, Size.Y), BackgroundColor.Value * Alpha);
            
            if (BackgroundTexture != null)
                spriteBatch.Draw(BackgroundTexture, new Rectangle((int)Position.X, (int)Position.Y, Size.X, Size.Y), Color.White * Alpha);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
