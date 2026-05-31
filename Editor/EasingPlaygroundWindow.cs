using UnityEditor;
using UnityEngine;

namespace SaiMayank.EasingPlayground
{
    public class EasingPlaygroundWindow : EditorWindow
    {
        // Serialized state (automatically saved by Unity)
        private enum CurveMode { Library, Bezier, AnimationCurve }
        [SerializeField] private CurveMode _mode = CurveMode.Library;
        [SerializeField] private EasingType _selectedEasing = EasingType.Linear;
        [SerializeField] private Vector2 _p1 = new(0.25f, 0f);
        [SerializeField] private Vector2 _p2 = new(0.25f, 1.0f);
        [SerializeField] private AnimationCurve _animCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _loop = false;

        // Non-serialized runtime state (reset on domain reload, not saved)
        private float _t = 0f;
        private bool _isPlaying = false;
        private double _lastUpdateTime;

        // Which Bezier handle is being dragged: 0 = none, 1 = P1, 2 = P2.
        private int _dragging = 0;

        [MenuItem("Tools/Easing Playground")]
        public static void ShowWindow()
        {
            EasingPlaygroundWindow window = GetWindow<EasingPlaygroundWindow>();
            window.titleContent = new GUIContent("Easing Playground");
            window.minSize = new Vector2(380f, 470f);
        }

        private void OnEnable()
        {
            _animCurve ??= AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable() { EditorApplication.update -= OnEditorUpdate; }

        private void OnEditorUpdate()
        {
            if (!_isPlaying) return;

            double now = EditorApplication.timeSinceStartup;
            float delta = (float)(now - _lastUpdateTime);
            _lastUpdateTime = now;

            _t += delta / Mathf.Max(_duration, 0.0001f);
            if (_t >= 1f)
            {
                if (_loop) _t = Mathf.Repeat(_t, 1f);
                else { _t = 1f; _isPlaying = false; }
            }
            Repaint();
        }

        private void TogglePlay()
        {
            if (!_isPlaying && _t >= 1f) _t = 0f;
            _isPlaying = !_isPlaying;
            _lastUpdateTime = EditorApplication.timeSinceStartup;
        }

        // The whole UI asks THIS one method for a value. 
        // Adding a new curve source later means adding a single case here — nothing else has to change.
        private float EvaluateActive(float t)
        {
            return _mode switch
            {
                CurveMode.Bezier =>                 BezierEasing.Evaluate(_p1, _p2, t),
                CurveMode.AnimationCurve =>         _animCurve.Evaluate(t),
                _ =>                                Easing.Evaluate(_selectedEasing, t),
            };
        }

        // --- UI -------------------------------------------------------------
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Easing Playground", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Mode selector: library, custom Bézier, or freeform AnimationCurve.
            _mode = (CurveMode)GUILayout.Toolbar((int)_mode, new[] { "Library", "Bézier", "Custom Curve" }); 
            EditorGUILayout.Space();

            switch (_mode)
            {
                case CurveMode.Library:
                    _selectedEasing = (EasingType)EditorGUILayout.EnumPopup("Easing", _selectedEasing);
                    break;

                case CurveMode.Bezier:
                    DrawBezierControls();
                    break;

                case CurveMode.AnimationCurve:
                    _animCurve = EditorGUILayout.CurveField("Curve", _animCurve);
                    EditorGUILayout.HelpBox(
                        "Click the curve to open Unity's editor. Add keys and drag tangents " +
                        "for any shape — including ones a single Bézier can't make, like multi-bounce.",
                        MessageType.None);
                    break;
            }

            EditorGUILayout.Space();

            // Shared playback controls (all three modes use these).
            _t = EditorGUILayout.Slider("Time (Normalized)", _t, 0f, 1f);
            _duration = EditorGUILayout.Slider("Duration (sec)", _duration, 0.1f, 5f);
            _loop = EditorGUILayout.Toggle("Loop", _loop);
            EditorGUILayout.LabelField("Eased value", EvaluateActive(_t).ToString("F3"));

            EditorGUILayout.Space();
            if (GUILayout.Button(_isPlaying ? "Pause" : "Play", GUILayout.Height(28f)))
                TogglePlay();
            EditorGUILayout.Space();

            // The preview area is shared by all three modes, but only Bézier mode has interactive handles
            // so we pass the event handling function in as an argument.
            Rect previewRect = GUILayoutUtility.GetRect(100f, 220f);
            if (_mode == CurveMode.Bezier) HandleBezierInput(previewRect);
            DrawPreview(previewRect);
        }

        private void DrawBezierControls()
        {
            _p1 = EditorGUILayout.Vector2Field("Control P1", _p1);
            _p2 = EditorGUILayout.Vector2Field("Control P2", _p2);

            // CSS Presets
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Presets");
                if (GUILayout.Button("Linear")) SetBezier(0f, 0f, 1f, 1f);
                if (GUILayout.Button("Ease"))   SetBezier(0.25f, 0.1f, 0.25f, 1f);
                if (GUILayout.Button("In"))     SetBezier(0.42f, 0f, 1f, 1f);
                if (GUILayout.Button("Out"))    SetBezier(0f, 0f, 0.58f, 1f);
                if (GUILayout.Button("In-Out")) SetBezier(0.42f, 0f, 0.58f, 1f);
            }

            // Show the equivalent CSS code for the current control points, and a button to copy it to the clipboard.
            string css = $"cubic-bezier({_p1.x:0.###}, {_p1.y:0.###}, {_p2.x:0.###}, {_p2.y:0.###})";
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.SelectableLabel(css, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Copy", GUILayout.Width(50f))) EditorGUIUtility.systemCopyBuffer = css;
            }

            EditorGUILayout.HelpBox("Drag the two blue handles in the preview to shape the curve.", MessageType.None);
        }

        private void SetBezier(float x1, float y1, float x2, float y2)
        {
            _p1 = new Vector2(x1, y1);
            _p2 = new Vector2(x2, y2);
            Repaint();
        }

        // Bezier handle dragging
        private void HandleBezierInput(Rect area)
        {
            Event e = Event.current;
            const float grab = 12f; // click within this many px of a handle to grab it

            Vector2 p1 = new(TtoX(area, _p1.x), EtoY(area, _p1.y));
            Vector2 p2 = new(TtoX(area, _p2.x), EtoY(area, _p2.y));

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0 && area.Contains(e.mousePosition))
                    {
                        if (Vector2.Distance(e.mousePosition, p1) <= grab) { _dragging = 1; e.Use(); }
                        else if (Vector2.Distance(e.mousePosition, p2) <= grab) { _dragging = 2; e.Use(); }
                    }
                    break;

                case EventType.MouseDrag:
                    if (_dragging != 0)
                    {
                        // Convert the mouse position back into normalized coordinates, clamping to a reasonable range.
                        float nx = Mathf.Clamp01(XtoT(area, e.mousePosition.x));
                        float ny = Mathf.Clamp(YtoValue(area, e.mousePosition.y), ValueMin, ValueMax);
                        if (_dragging == 1) _p1 = new Vector2(nx, ny);
                        else                _p2 = new Vector2(nx, ny);
                        e.Use();
                        Repaint();
                    }
                    break;

                case EventType.MouseUp:
                    if (_dragging != 0) { _dragging = 0; e.Use(); }
                    break;
            }
        }

        // Preview
        private void DrawPreview(Rect area)
        {
            if (Event.current.type != EventType.Repaint) return;

            EditorGUI.DrawRect(area, new Color(0.12f, 0.12f, 0.12f));

            DrawHGuide(area, 0f, 0.22f);   // value == 0 (start)
            DrawHGuide(area, 1f, 0.22f);   // value == 1 (target)

            Handles.color = new Color(1f, 1f, 1f, 0.10f);
            Handles.DrawAAPolyLine(1f, new Vector3(TtoX(area, 0f), EtoY(area, 0f), 0f), new Vector3(TtoX(area, 1f), EtoY(area, 1f), 0f));

            DrawCurve(area, new Color(1f, 0.65f, 0.2f), 2.5f);

            if (_mode == CurveMode.Bezier) DrawBezierHandles(area);

            // The animated dot, riding whichever curve is active.
            float eased = EvaluateActive(_t);
            float dx = TtoX(area, _t);
            float dy = EtoY(area, eased);
            float dot = 10f;
            EditorGUI.DrawRect(new Rect(dx - dot * 0.5f, dy - dot * 0.5f, dot, dot), new Color(1f, 0.8f, 0.25f));
        }

        // Draw the curve itself by sampling it at regular intervals and connecting the dots with lines.
        private void DrawCurve(Rect area, Color color, float width)
        {
            const int segments = 96;
            Vector3[] points = new Vector3[segments + 1];

            for (int i = 0; i <= segments; i++)
            {
                float ct = (float)i / segments;
                float cv = EvaluateActive(ct);
                points[i] = new Vector3(TtoX(area, ct), EtoY(area, cv), 0f);
            }

            Handles.color = color;
            Handles.DrawAAPolyLine(width, points);
        }

        // Draw the Bézier control points and the lines connecting them to the fixed endpoints.
        private void DrawBezierHandles(Rect area)
        {
            float p0x = TtoX(area, 0f),     p0y = EtoY(area, 0f);
            float p3x = TtoX(area, 1f),     p3y = EtoY(area, 1f);
            float p1x = TtoX(area, _p1.x),  p1y = EtoY(area, _p1.y);
            float p2x = TtoX(area, _p2.x),  p2y = EtoY(area, _p2.y);

            // Handle lines from each fixed endpoint to its control point.
            Handles.color = new Color(0.45f, 0.7f, 1f, 0.7f);
            Handles.DrawLine(new Vector3(p0x, p0y, 0f), new Vector3(p1x, p1y, 0f));
            Handles.DrawLine(new Vector3(p3x, p3y, 0f), new Vector3(p2x, p2y, 0f));

            DrawHandleDot(p1x, p1y, _dragging == 1);
            DrawHandleDot(p2x, p2y, _dragging == 2);
        }

        // Draw a square handle for a control point. If it's active (being dragged), make it bigger and brighter.
        private void DrawHandleDot(float cx, float cy, bool active)
        {
            float s = active ? 12f : 10f;
            Color c = active ? new Color(0.7f, 0.88f, 1f) : new Color(0.45f, 0.7f, 1f);
            EditorGUI.DrawRect(new Rect(cx - s * 0.5f, cy - s * 0.5f, s, s), c);
        }

        // Draw a horizontal guide line across the preview at a given value (e.g. 0 or 1) with a given alpha for the color.
        private void DrawHGuide(Rect area, float value, float alpha)
        {
            float y = EtoY(area, value);
            Handles.color = new Color(1f, 1f, 1f, alpha);
            Handles.DrawLine(new Vector3(area.xMin + Padding, y, 0f),
                            new Vector3(area.xMax - Padding, y, 0f));
        }

        // Coordinate conversions: normalized t (0 to 1) and curve value -> screen x and y, and back.
        private const float Padding = 16f;
        private const float ValueMin = -0.4f;  // headroom for undershoot
        private const float ValueMax = 1.4f;   // headroom for overshoot

        private float TtoX(Rect area, float t) =>
            Mathf.Lerp(area.xMin + Padding, area.xMax - Padding, t);

        private float EtoY(Rect area, float value)
        {
            float n = Mathf.InverseLerp(ValueMin, ValueMax, value);
            return Mathf.Lerp(area.yMax - Padding, area.yMin + Padding, n);
        }

        // Inverse of TtoX: screen x -> normalized t.
        private float XtoT(Rect area, float screenX) =>
            Mathf.InverseLerp(area.xMin + Padding, area.xMax - Padding, screenX);

        // Inverse of EtoY: screen y -> curve value.
        private float YtoValue(Rect area, float screenY)
        {
            float n = Mathf.InverseLerp(area.yMax - Padding, area.yMin + Padding, screenY);
            return Mathf.Lerp(ValueMin, ValueMax, n);
        }
    }
}