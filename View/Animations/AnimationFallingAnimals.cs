using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ZooBurst.Core;
using ZooBurst.View.Activities;

namespace ZooBurst.View.Animations
{
    public class AnimationFallingAnimals : Animation
    {
        public override void Play(GameView view, Action<object[]> onComplete, params object[] args)
        {
            var longestDuration = TimeSpan.Zero;
            var columns = args[0] as List<List<Animal>>;

            if (columns == null)
                return;

            foreach (var column in columns)
            {
                for (var index = 0; index < column.Count; index++)
                {
                    var animal = column[index];

                    var duration = TimeSpan.FromSeconds(((new Vector2(animal.X * view.TileSize.Y + (view.TileSize.Y / 2.0F),
                        animal.Y * view.TileSize.Y + (view.TileSize.Y / 2.0F)).Y - animal.Sprite.LocalPosition.Y) / view.TileSize.Y) * 0.1F);

                    if (duration + TimeSpan.FromSeconds(0.05F + (0.15F * index)) > longestDuration)
                        longestDuration = duration + TimeSpan.FromSeconds(0.05F + (0.15F * index));

                    animal.Sprite.AddActivity(new ActivitySequence(new Activity[]
                    {
                        new WaitActivity(TimeSpan.FromSeconds(0.05F + (0.15F * index))),
                        new MoveActivity(new Vector2(animal.X * view.TileSize.Y + (view.TileSize.Y / 2.0F), animal.Y * view.TileSize.Y + (view.TileSize.Y / 2.0F)), duration, EaseMode.Out)
                    }));
                }
            }

            view.AddActivity(new WaitActivity(longestDuration, onComplete));
        }
    }
}
