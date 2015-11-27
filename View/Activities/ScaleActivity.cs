using ZooBurst.Utils;
using ZooBurst.View.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace ZooBurst.View.Activities
{
    public class ScaleActivity : Activity
    {
        private readonly float _from;
        private readonly float _to;
        private readonly EaseMode _easeMode;

        public ScaleActivity(float scaleFrom, float scaleTo, TimeSpan duration, EaseMode easeMode, Action<object[]> onCompletion = null)
            : base(duration, onCompletion)
        {
            _from = scaleFrom;
            _to = scaleTo;
            _easeMode = easeMode;
        }

        protected override void Refresh(Renderable renderItem, float delta)
        {
            var sprite = renderItem as Sprite;

            if (sprite == null)
                return;

            sprite.Scale = MathHelper.Lerp(_from, _to, Easing.Perform(_easeMode, Elapsed, (float)Duration.TotalSeconds));
        }

        protected override void End(Renderable renderItem)
        {
            var sprite = renderItem as Sprite;

            if (sprite == null)
                return;

            sprite.Scale = _to;
        }
    }
}
