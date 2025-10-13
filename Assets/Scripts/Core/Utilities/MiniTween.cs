using System;

namespace TestBench2025.Core.Utilities
{
    internal static class MiniTween
    {
        public static float Linear(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }

        public static float BackEaseOut(float t, float b, float c, float d)
        {
            var s = 1.70158f;
            t = t / d - 1;
            return c * (t * t * ((s + 1) * t + s) + 1) + b;
        }

        public static float BackEaseIn(float t, float b, float c, float d)
        {
            var s = 1.70158f;
            t /= d;
            return c * t * t * ((s + 1) * t - s) + b;
        }

        public static float BackEaseInOut(float t, float b, float c, float d)
        {
            var s = 1.70158f;
            t /= d / 2;
            if (t < 1)
            {
                s *= 1.525f;
                return c / 2 * (t * t * (((s + 1) * t) - s)) + b;
            }
            t -= 2;
            s *= 1.525f;
            return c / 2 * (t * t * (((s + 1) * t) + s) + 2) + b;
        }

        public static float Evaluate(Func<float, float, float, float, float> ease, float normalizedTime)
        {
            return ease(normalizedTime, 0f, 1f, 1f);
        }
    }
}