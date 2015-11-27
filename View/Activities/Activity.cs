using ZooBurst.View.Graphics;
using System;

namespace ZooBurst.View.Activities
{
    public abstract class Activity
    {
        protected TimeSpan Duration { get; }
        private DateTime _start;

        protected Action<object[]> OnCompletion;
        protected float Elapsed => (float)(DateTime.Now - _start).TotalSeconds;

        protected Activity(TimeSpan duration, Action<object[]> onCompletion = null)
        {
            Duration = duration;
            OnCompletion = onCompletion;
        }

        protected Activity(Action<object[]> onCompletion = null)
        {
            OnCompletion = onCompletion;
        }

        public virtual bool Run(Renderable renderItem, float delta)
        {
            if (_start == default(DateTime))
                Start();

            return Update(renderItem, delta);
        }

        protected virtual void Start()
        {
            _start = DateTime.Now;
        }

        protected virtual bool Update(Renderable renderItem, float delta)
        {
            if (DateTime.Now < _start + Duration)
            {
                Refresh(renderItem, delta);
                return false;
            }

            End(renderItem);
            OnCompletion?.Invoke(null);
            return true;
        }

        protected virtual void Refresh(Renderable renderItem, float delta)
        {
        }

        protected virtual void End(Renderable renderItem)
        {
        }

        public virtual Activity Clone()
        {
            throw new Exception("Clone must be overridden in the child classes.");
        }
    }
}