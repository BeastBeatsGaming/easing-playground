using UnityEditor;
using UnityEngine;

namespace SaiMayank.EasingPlayground.Editor 
{
    [CustomPropertyDrawer(typeof(EasingFunction))]
    public class EasingFunctionDrawer : PropertyDrawer
    {
        private const float PreviewHeight = 56f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float pad = EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty source = property.FindPropertyRelative("source");

            float h = EditorGUI.GetPropertyHeight(source) + pad;

            switch (source.enumValueIndex)
            {
                case 1: // Bezier
                    h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("p1")) + pad;
                    h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("p2")) + pad;
                    break;
                case 2: // AnimationCurve
                    h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("curve")) + pad;
                    break;
                default: // Library
                    h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("library")) + pad;
                    break;
            }

            h += PreviewHeight + pad;
            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            float pad = EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty source  = property.FindPropertyRelative("source");
            SerializedProperty library = property.FindPropertyRelative("library");
            SerializedProperty p1      = property.FindPropertyRelative("p1");
            SerializedProperty p2      = property.FindPropertyRelative("p2");
            SerializedProperty curve   = property.FindPropertyRelative("curve");


            Rect row = new(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(source));
            EditorGUI.PropertyField(row, source, label);
            row.y += row.height + pad;

            // Show relevant controls
            switch (source.enumValueIndex)
            {
                case 1: // Bezier
                    row.height = EditorGUI.GetPropertyHeight(p1);
                    EditorGUI.PropertyField(row, p1, new GUIContent("Control P1"));
                    row.y += row.height + pad;
                    row.height = EditorGUI.GetPropertyHeight(p2);
                    EditorGUI.PropertyField(row, p2, new GUIContent("Control P2"));
                    row.y += row.height + pad;
                    break;
                case 2: // AnimationCurve
                    row.height = EditorGUI.GetPropertyHeight(curve);
                    EditorGUI.PropertyField(row, curve, new GUIContent("Curve"));
                    row.y += row.height + pad;
                    break;
                default: // Library
                    row.height = EditorGUI.GetPropertyHeight(library);
                    EditorGUI.PropertyField(row, library, new GUIContent("Library"));
                    row.y += row.height + pad;
                    break;
            }

            // Mini preview
            Rect previewRect = new(position.x, row.y, position.width, PreviewHeight);
            if (Event.current.type == EventType.Repaint)
                DrawMiniPreview(previewRect, source, library, p1, p2, curve);

            EditorGUI.EndProperty();
        }

        private void DrawMiniPreview(Rect area, SerializedProperty source,
            SerializedProperty library, SerializedProperty p1, SerializedProperty p2,
            SerializedProperty curve)
        {
            EditorGUI.DrawRect(area, new Color(0.12f, 0.12f, 0.12f));

            const float inset = 6f, vmin = -0.4f, vmax = 1.4f;
            float X(float t) => Mathf.Lerp(area.xMin + inset, area.xMax - inset, t);
            float Y(float v)
            {
                float n = Mathf.InverseLerp(vmin, vmax, v);
                return Mathf.Lerp(area.yMax - inset, area.yMin + inset, n);
            }

            // Faint guide lines at value 0 and value 1.
            Handles.color = new Color(1f, 1f, 1f, 0.18f);
            Handles.DrawLine(new Vector3(X(0f), Y(0f)), new Vector3(X(1f), Y(0f)));
            Handles.DrawLine(new Vector3(X(0f), Y(1f)), new Vector3(X(1f), Y(1f)));

            // Evaluate active source to get points for curve preview.
            AnimationCurve ac = curve.animationCurveValue;
            float Eval(float t)
            {
                return source.enumValueIndex switch
                {
                    1 => BezierEasing.Evaluate(p1.vector2Value, p2.vector2Value, t),
                    2 => ac != null ? ac.Evaluate(t) : t,
                    _ => Easing.Evaluate((EasingType)library.enumValueIndex, t),
                };
            }

            const int seg = 48;
            Vector3[] pts = new Vector3[seg + 1];
            for (int i = 0; i <= seg; i++)
            {
                float t = (float)i / seg;
                pts[i] = new Vector3(X(t), Y(Eval(t)), 0f);
            }
            Handles.color = new Color(1f, 0.65f, 0.2f);
            Handles.DrawAAPolyLine(2f, pts);
        }
    }
}