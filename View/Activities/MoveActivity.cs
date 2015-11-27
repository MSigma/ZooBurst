using System;
using Microsoft.Xna.Framework;
using ZooBurst.Utils;
using ZooBurst.View.Graphics;

namespace ZooBurst.View.Activities
{
    public class MoveActivity : Activity
    {
        private Vector2 _fromPosition;
        private readonly Vector2 _toPosition;
        private readonly EaseMode _easeMode;

        public MoveActivity(Vector2 fromPosition, Vector2 toPosition, TimeSpan duration, EaseMode easeMode, Action<object[]> onCompletion = null)
            : base(duration, onCompletion)
        {
            _fromPosition = fromPosition;
            _toPosition = toPosition;
            _easeMode = easeMode;
        }

        public MoveActivity(Vector2 toPosition, TimeSpan duration, EaseMode easeMode, Action<object[]> onCompletion = null)
            : this(-Vector2.One, toPosition, duration, easeMode, onCompletion)
        {
        }

        protected override void Refresh(Renderable renderItem, float delta)
        {
            if (_fromPosition == -Vector2.One)
                _fromPosition = renderItem.LocalPosition;

            var newPosition = Vector2.Lerp(_fromPosition, _toPosition, Easing.Perform(_easeMode, Elapsed, (float)Duration.TotalSeconds));
            renderItem.LocalPosition = newPosition;
        }

        protected override void End(Renderable renderItem)
        {
            renderItem.LocalPosition = _toPosition;
        }

        public override Activity Clone()
        {
            return new MoveActivity(_fromPosition, _toPosition, Duration, _easeMode, OnCompletion);
        }
    }
}