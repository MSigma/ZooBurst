using System;
using Microsoft.Xna.Framework;

namespace ZooBurst.Core.Input
{
    public abstract class InputListener
    {
        protected void RaiseEvent<T>(EventHandler<T> eventHandler, T args)
            where T : EventArgs
        {
            eventHandler?.Invoke(this, args);
        }

        internal abstract void Update(GameTime gameTime);
    }
}