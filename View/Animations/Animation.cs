using System;

namespace ZooBurst.View.Animations
{
    public abstract class Animation
    {
        public abstract void Play(GameView view, Action<object[]> onComplete, params object[] args);
    }
}
