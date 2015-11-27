using System;
using System.Collections.Generic;
using System.Linq;
using ZooBurst.Core;
using ZooBurst.View.Activities;

namespace ZooBurst.View.Animations
{
    public class AnimationMatches : Animation
    {
        public override void Play(GameView view, Action<object[]> onComplete, params object[] args)
        {
            var chains = args[0] as List<Chain>;

            if (chains == null)
                return;

            foreach (var animal in chains.SelectMany(chain => chain.Animals))
            {
                animal.Sprite?.AddActivity(new ScaleActivity(animal.Sprite.Scale, 0.01F, TimeSpan.FromSeconds(0.33F), EaseMode.Out, e =>
                {
                    animal.Sprite?.Detach();
                    animal.SetSprite(null);
                }));
            }

            view.AddActivity(new WaitActivity(TimeSpan.FromSeconds(0.33F), onComplete));
        }
    }
}