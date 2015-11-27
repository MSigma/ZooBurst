using ZooBurst.Utils;
using ZooBurst.View.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace ZooBurst.View.Activities
{
    public class FadeActivity : Activity
    {
        public enum Fade
        {
            In,
            Out
        }

        private readonly float _from;
        private readonly float _to;
        private readonly EaseMode _easeMode;

        public FadeActivity(Fade fade, TimeSpan duration, EaseMode easeMode, Action<object[]> onCompletion = null)
            : base(duration, onCompletion)
        {
            switch (fade)
            {
                case Fade.In:
                    _from = 0.0F;
                    _to = 1.0F;
                    break;
                case Fade.Out:
                    _from = 1.0F;
                    _to = 0.0F;
                    break;
            }

            _easeMode = easeMode;
        }

        protected override void Refresh(Renderable renderItem, float delta)
        {
            renderItem.SetAlpha(MathHelper.Lerp(_from, _to, Easing.Perform(_easeMode, Elapsed, (float)Duration.TotalSeconds)));
        }

        protected override void End(Renderable renderItem)
        {
            renderItem.SetAlpha(_to);
        }
    }
}
