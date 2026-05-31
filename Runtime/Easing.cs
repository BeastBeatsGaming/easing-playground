using UnityEngine;

// Source: https://easings.net/ (MIT License)
namespace SaiMayank.EasingPlayground
{
    public enum EasingType
    {
        Linear,
        SineIn, SineOut, SineInOut,
        QuadIn, QuadOut, QuadInOut,
        CubicIn, CubicOut, CubicInOut,
        QuartIn, QuartOut, QuartInOut,
        QuintIn, QuintOut, QuintInOut,
        ExpoIn, ExpoOut, ExpoInOut,
        CircIn, CircOut, CircInOut,
        BackIn, BackOut, BackInOut,
        ElasticIn, ElasticOut, ElasticInOut,
        BounceIn, BounceOut, BounceInOut
    }

    // Collection of easing functions
    public static class Easing
    {
        public static float Evaluate(EasingType type, float t)
        {
            return type switch
            {
                EasingType.Linear =>        t,
                EasingType.SineIn =>        InSine(t),
                EasingType.SineOut =>       OutSine(t),
                EasingType.SineInOut =>     InOutSine(t),
                EasingType.QuadIn =>        InQuad(t),
                EasingType.QuadOut =>       OutQuad(t),
                EasingType.QuadInOut =>     InOutQuad(t),
                EasingType.CubicIn =>       InCubic(t),
                EasingType.CubicOut =>      OutCubic(t),
                EasingType.CubicInOut =>    InOutCubic(t),
                EasingType.QuartIn =>       InQuart(t),
                EasingType.QuartOut =>      OutQuart(t),
                EasingType.QuartInOut =>    InOutQuart(t),
                EasingType.QuintIn =>       InQuint(t),
                EasingType.QuintOut =>      OutQuint(t),
                EasingType.QuintInOut =>    InOutQuint(t),
                EasingType.ExpoIn =>        InExpo(t),
                EasingType.ExpoOut =>       OutExpo(t),
                EasingType.ExpoInOut =>     InOutExpo(t),
                EasingType.CircIn =>        InCirc(t),
                EasingType.CircOut =>       OutCirc(t),
                EasingType.CircInOut =>     InOutCirc(t),
                EasingType.BackIn =>        InBack(t),
                EasingType.BackOut =>       OutBack(t),
                EasingType.BackInOut =>     InOutBack(t),
                EasingType.ElasticIn =>     InElastic(t),
                EasingType.ElasticOut =>    OutElastic(t),
                EasingType.ElasticInOut =>  InOutElastic(t),
                EasingType.BounceIn =>      InBounce(t),
                EasingType.BounceOut =>     OutBounce(t),
                EasingType.BounceInOut =>   InOutBounce(t),
                _ =>                        t,
            };
        }

        // ---- Sine ----
        static float InSine(float x)    => 1f - Mathf.Cos((x * Mathf.PI) / 2f);
        static float OutSine(float x)   => Mathf.Sin((x * Mathf.PI) / 2f);
        static float InOutSine(float x) => -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;

        // ---- Quad ----
        static float InQuad(float x)    => x * x;
        static float OutQuad(float x)   => 1f - (1f - x) * (1f - x);
        static float InOutQuad(float x) => x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;

        // ---- Cubic ----
        static float InCubic(float x)    => x * x * x;
        static float OutCubic(float x)   => 1f - Mathf.Pow(1f - x, 3f);
        static float InOutCubic(float x) => x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;

        // ---- Quart ----
        static float InQuart(float x)    => x * x * x * x;
        static float OutQuart(float x)   => 1f - Mathf.Pow(1f - x, 4f);
        static float InOutQuart(float x) => x < 0.5f ? 8f * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 4f) / 2f;

        // ---- Quint ----
        static float InQuint(float x)    => x * x * x * x * x;
        static float OutQuint(float x)   => 1f - Mathf.Pow(1f - x, 5f);
        static float InOutQuint(float x) => x < 0.5f ? 16f * x * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f;

        // ---- Expo ----
        static float InExpo(float x)  => x <= 0f ? 0f : Mathf.Pow(2f, 10f * x - 10f);
        static float OutExpo(float x) => x >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
        static float InOutExpo(float x) => x <= 0f ? 0f : x >= 1f ? 1f : x < 0.5f ? Mathf.Pow(2f, 20f * x - 10f) / 2f : (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;

        // ---- Circ ----
        static float InCirc(float x)    => 1f - Mathf.Sqrt(1f - x * x);
        static float OutCirc(float x)   => Mathf.Sqrt(1f - (x - 1f) * (x - 1f));
        static float InOutCirc(float x) => x < 0.5f ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;

        // ---- Back ----
        const float C1 = 1.70158f;
        const float C2 = C1 * 1.525f;
        const float C3 = C1 + 1f;
        static float InBack(float x)  => C3 * x * x * x - C1 * x * x;
        static float OutBack(float x) => 1f + C3 * Mathf.Pow(x - 1f, 3f) + C1 * Mathf.Pow(x - 1f, 2f);
        static float InOutBack(float x) => x < 0.5f ? Mathf.Pow(2f * x, 2f) * ((C2 + 1f) * 2f * x - C2) / 2f : (Mathf.Pow(2f * x - 2f, 2f) * ((C2 + 1f) * (2f * x - 2f) + C2) + 2f) / 2f;

        // ---- Elastic ----
        const float C4 = 2f * Mathf.PI / 3f;
        const float C5 = 2f * Mathf.PI / 4.5f;
        static float InElastic(float x) => x <= 0f ? 0f : x >= 1f ? 1f : -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * C4);
        static float OutElastic(float x) => x <= 0f ? 0f : x >= 1f ? 1f : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * C4) + 1f;
        static float InOutElastic(float x) => x <= 0f ? 0f : x >= 1f ? 1f : x < 0.5f ? -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * C5)) / 2f :  Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * C5) / 2f + 1f;

        // ---- Bounce ----
        static float OutBounce(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            if (x < 1f / d1)            return n1 * x * x;
            else if (x < 2f / d1)       { x -= 1.5f / d1;   return n1 * x * x + 0.75f; }
            else if (x < 2.5f / d1)     { x -= 2.25f / d1;  return n1 * x * x + 0.9375f; }
            else                        { x -= 2.625f / d1; return n1 * x * x + 0.984375f; }
        }
        static float InBounce(float x)    => 1f - OutBounce(1f - x);
        static float InOutBounce(float x) => x < 0.5f ? (1f - OutBounce(1f - 2f * x)) / 2f : (1f + OutBounce(2f * x - 1f)) / 2f;
    }
}