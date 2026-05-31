using UnityEngine;

namespace SaiMayank.EasingPlayground
{
    public static class BezierEasing
    {
        // Evaluate the cubic Bezier curve defined by control points P0=(0,0), P1=p1, P2=p2, P3=(1,1) at the given x in [0,1].
        public static float Evaluate(Vector2 p1, Vector2 p2, float x) => Evaluate(p1.x, p1.y, p2.x, p2.y, x);

        public static float Evaluate(float x1, float y1, float x2, float y2, float x)
        {
            // Endpoints are exact (and skip the solve).
            if (x <= 0f) return 0f;
            if (x >= 1f) return 1f;

            // Precompute the polynomial coefficients from the control points.
            float cx = 3f * x1;
            float bx = 3f * (x2 - x1) - cx;
            float ax = 1f - cx - bx;

            float cy = 3f * y1;
            float by = 3f * (y2 - y1) - cy;
            float ay = 1f - cy - by;

            float s = SolveForX(x, ax, bx, cx);
            return ((ay * s + by) * s + cy) * s;
        }

        // Given an x value, find a parametric value s on the curve such that curveX(s) ~= x. 
        // Then we can use that s to find the corresponding y.
        private static float SolveForX(float targetX, float ax, float bx, float cx)
        {
            const float epsilon = 1e-6f;

            // First try a few iterations of Newton-Raphson method (normally very fast).
            float s = targetX;
            for (int i = 0; i < 8; i++)
            {
                float error = (((ax * s + bx) * s + cx) * s) - targetX;
                if (Mathf.Abs(error) < epsilon) return s;

                float slope = (3f * ax * s + 2f * bx) * s + cx;
                if (Mathf.Abs(slope) < epsilon) break; // too flat to trust Newton -> hand off to bisection

                s -= error / slope;
            }

            // Fall back to the bisection method for reliability.
            float lo = 0f, hi = 1f;
            s = targetX;
            for (int i = 0; i < 30; i++)
            {
                float curveX = ((ax * s + bx) * s + cx) * s;
                if (Mathf.Abs(curveX - targetX) < epsilon) return s;

                if (curveX < targetX) lo = s; else hi = s;
                s = 0.5f * (lo + hi);
            }

            return s;
        }
    }
}