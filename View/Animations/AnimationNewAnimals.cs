using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ZooBurst.Core;
using ZooBurst.View.Activities;

namespace ZooBurst.View.Animations
{
    public class AnimationNewAnimals : Animation
    {
        public override void Play(GameView view, Action<object[]> onComplete, params object[] args)
        {
            var columns = args[0] as List<List<Animal>>;
            if (columns == null)
                return;

            var longestDuration = TimeSpan.Zero;
            foreach (var column in columns)
            {
                for (var index = 0; index < column.Count; index++)
                {
                    var animal = column[index];

                    view.DropAnimal(animal, column[0].Y - 1);
                    var duration = TimeSpan.FromSeconds((animal.Y - (column[0].Y - 1)) * 0.1F);

                    if (duration + TimeSpan.FromSeconds(0.1F + (0.2F * (column.Count - index - 1))) > longestDuration)
                        longestDuration = duration + TimeSpan.FromSeconds(0.1F + (0.2F * (column.Count - index - 1)));

                    animal.Sprite.AddActivity(new ActivitySequence(new Activity[]
                    {
                        new WaitActivity(TimeSpan.FromSeconds(0.1F + (0.2F * (column.Count - index - 1)))),
                        new ActivityGroup(new Activity[]
                        {
                            new FadeActivity(FadeActivity.Fade.In, TimeSpan.FromSeconds(0.05F), EaseMode.InOut),
                            new MoveActivity(new Vector2((animal.X * view.TileSize.X) + (view.TileSize.X / 2), 
                                (animal.Y * view.TileSize.Y) + (view.TileSize.Y / 2)), duration, EaseMode.Out)
                        })
                    }));
                }
            }

            view.AddActivity(new WaitActivity(longestDuration, onComplete));
        }
    }
}
