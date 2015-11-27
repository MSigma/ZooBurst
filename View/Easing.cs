using System;

namespace ZooBurst.View
{
    public enum EaseMode
    {
        None,
        Linear,
        In,
        Out,
        InOut
    }

    public class Easing
    {
        public static float Perform(EaseMode mode, float elapsed, float duration)
        {
            switch (mode)
            {
                case EaseMode.Out:
                    return Out(elapsed, duration);
                case EaseMode.In:
                    return In(elapsed, duration);
                case EaseMode.InOut:
                    return InOut(elapsed, duration);
                default:
                    return elapsed;
            }
        }

        private static float In(float elapsed, float duration)
        {
            return 1.0F - (float)Math.Cos((elapsed / duration) * Math.PI * 0.5F);
        }

        public static float Out(float elapsed, float duration)
        {
            return (float)Math.Sin((elapsed / duration) * Math.PI * 0.5F);
        }

        private static float InOut(float elapsed, float duration)
        {
            var step = (elapsed / duration);
            return (step * step) * (3.0F - (2.0F * step));
        }
    }
}
