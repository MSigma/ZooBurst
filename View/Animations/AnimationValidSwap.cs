using System;
using ZooBurst.Core;
using ZooBurst.Utils;
using ZooBurst.View.Activities;

namespace ZooBurst.View.Animations
{
    public class AnimationValidSwap : Animation
    {
        public override void Play(GameView view, Action<object[]> onComplete, params object[] args)
        {
            var swap = args[0] as Swap;

            if (swap == null)
                return;

            swap.From.Sprite.AddActivity(new MoveActivity(swap.From.Sprite.LocalPosition, swap.To.Sprite.LocalPosition, TimeSpan.FromSeconds(0.33D), EaseMode.Out));
            swap.To.Sprite.AddActivity(new MoveActivity(swap.To.Sprite.LocalPosition, swap.From.Sprite.LocalPosition, TimeSpan.FromSeconds(0.33D), EaseMode.Out, onComplete));
        }
    }
}
