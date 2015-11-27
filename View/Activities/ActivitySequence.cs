using ZooBurst.View.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZooBurst.View.Activities
{
    public class ActivitySequence : Activity
    {
        private readonly List<Activity> _actions;

        public ActivitySequence(IEnumerable<Activity> array, Action<object[]> onCompletion = null)
            : base(onCompletion)
        {
            _actions = array.ToList();
        }

        protected override bool Update(Renderable renderItem, float delta)
        {
            if (_actions[0].Run(renderItem, delta))
                _actions.RemoveAt(0);

            if (_actions.Count > 0)
                return false;

            OnCompletion?.Invoke(null);
            return true;
        }
    }
}
