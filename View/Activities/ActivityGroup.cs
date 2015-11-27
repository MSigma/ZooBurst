using ZooBurst.View.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZooBurst.View.Activities
{
    public class ActivityGroup : Activity
    {
        private readonly List<Activity> _actions;

        public ActivityGroup(IEnumerable<Activity> array, Action<object[]> onCompletion = null)
            : base(onCompletion)
        {
            _actions = array.ToList();
        }

        protected override bool Update(Renderable renderItem, float delta)
        {
            for (var i = 0; i < _actions.Count; i++)
            {
                if (!_actions[i].Run(renderItem, delta))
                    continue;

                _actions.RemoveAt(i--);
            }

            if (_actions.Count > 0)
                return false;

            OnCompletion?.Invoke(null);
            return true;
        }
    }
}
