using System;
using ZooBurst.Core;
using ZooBurst.Utils;
using ZooBurst.View.Activities;

namespace ZooBurst.View.Animations
{
    public class AnimationInvalidSwap : Animation
    {
        public override void Play(GameView view, Action<object[]> onComplete, params object[] args)
        {
            var swap = args[0] as Swap;

            if (swap == null)
                return;

            var moveTo = new MoveActivity(swap.From.Sprite.LocalPosition, swap.To.Sprite.LocalPosition, TimeSpan.FromSeconds(0.2D), EaseMode.Out);
            var moveFrom = new MoveActivity(swap.To.Sprite.LocalPosition, swap.From.Sprite.LocalPosition, TimeSpan.FromSeconds(0.2D), EaseMode.Out);

            swap.From.Sprite.AddActivity(new ActivitySequence(new Activity[] { moveTo, moveFrom }));
            swap.To.Sprite.AddActivity(new ActivitySequence(new[] { moveFrom.Clone(), moveTo.Clone() }, onComplete));
        }
    }
}