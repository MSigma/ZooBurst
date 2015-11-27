using System;

namespace ZooBurst.View.Activities
{
    public class WaitActivity : Activity
    {
        public WaitActivity(TimeSpan duration, Action<object[]> onCompletion = null) 
            : base(duration, onCompletion)
        {
        }
    }
}