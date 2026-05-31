using UnityEngine;

namespace SaiMayank.EasingPlayground
{
    [AddComponentMenu("Easing/Easing Mover")]
    public class EasingMover : MonoBehaviour
    {
        public enum LoopMode { Once, Loop, PingPong }
        [SerializeField] private EasingFunction _easing = new();

        [Header("Motion")]
        [Tooltip("How far to move from the start position, in world units.")]
        [SerializeField] private Vector3 _moveBy = new(5f, 0f, 0f);

        [Tooltip("Seconds for one trip from start to end.")]
        [SerializeField] private float _duration = 1.5f;

        [SerializeField] private LoopMode _loopMode = LoopMode.Loop;

        private Vector3 _start;
        private float _elapsed;
        private bool _finished;

        private void Start()
        {
            _start = transform.position;
            _elapsed = 0f;
            _finished = false;
        }

        private void Update()
        {
            if (_finished) return;

            _elapsed += Time.deltaTime;
            float trips = _elapsed / Mathf.Max(_duration, 0.0001f);

            float t;
            switch (_loopMode)
            {
                case LoopMode.Once:
                    t = Mathf.Clamp01(trips);
                    if (trips >= 1f) _finished = true;
                    break;
                case LoopMode.Loop:
                    t = Mathf.Repeat(trips, 1f);
                    break;
                case LoopMode.PingPong:
                default:
                    t = Mathf.PingPong(trips, 1f);
                    break;
            }

            // Apply easing to t, then interpolate position.
            float eased = _easing.Evaluate(t);
            transform.position = Vector3.LerpUnclamped(_start, _start + _moveBy, eased);
        }

        private void OnDrawGizmos()
        {
            Vector3 from = Application.isPlaying ? _start : transform.position;
            Vector3 to = from + _moveBy;

            Gizmos.color = new Color(1f, 0.65f, 0.2f);
            Gizmos.DrawLine(from, to);
            Gizmos.DrawWireSphere(from, 0.15f);
            Gizmos.DrawWireSphere(to, 0.15f);
        }
    }
}