using UnityEngine;

namespace SaiMayank.EasingPlayground
{
    [System.Serializable]
    public class EasingFunction
    {
        public enum Source { Library, Bezier, AnimationCurve }

        public Source source = Source.Library;

        // Library Source
        public EasingType library = EasingType.QuadInOut;

        // Bezier Source
        public Vector2 p1 = new(0.25f, 0.1f);
        public Vector2 p2 = new(0.25f, 1.0f);

        // AnimationCurve Source
        public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public float Evaluate(float t)
        {
            return source switch
            {
                Source.Bezier =>            BezierEasing.Evaluate(p1, p2, t),
                Source.AnimationCurve =>    curve != null ? curve.Evaluate(t) : t,
                _ =>                        Easing.Evaluate(library, t),
            };
        }
    }
}