using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZooBurst.View.Graphics
{
    public class Sprite : Renderable
    {
        public float Scale { get; set; }
        public Vector2 Origin { get; set; }
        public float BaseAlpha { get; set; }
        public Texture2D Texture { get; private set; }
        public int Width => (int)(Texture.Width * Scale);
        public int Height => (int)(Texture.Height * Scale);

        public Sprite(Texture2D texture, float scale)
        {
            SetTexture(texture);
            Scale = scale;
            BaseAlpha = 1.0F;
        }

        public Sprite(Texture2D texture)
            : this(texture, 1.0F)
        {
        }

        public Sprite()
            : this(null)
        {
        }

        public void SetTexture(Texture2D texture)
        {
            if (texture == null)
                return;

            Texture = texture;
            Origin = new Vector2(texture.Width / 2.0F, texture.Height / 2.0F);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var position = new Vector2((int)Position.X, (int)Position.Y);
            spriteBatch.Draw(Texture, position, null, (Color.White * BaseAlpha) * Alpha,
                0.0F, Origin, Scale, SpriteEffects.None, 0.0F);

            base.Draw(gameTime, spriteBatch);
        }
    }
}