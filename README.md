# Easing Playground

A visual easing toolkit for Unity — preview, design, and apply easing curves without leaving the editor.

See all 31 standard easing curves at a glance, sculpt your own with draggable cubic-bézier handles (the same model as CSS `cubic-bezier()`) or Unity `AnimationCurve`s, and drive any GameObject with them through a reusable `EasingFunction` type. Ships with an editor window, a custom inspector with a live preview, and a plain-C# runtime library you can call from your own code.

## Features

- **Easing Playground window** (`Tools > Easing Playground`) — live preview with play/loop and a value readout.
- **31 built-in easings** — Sine, Quad, Cubic, Quart, Quint, Expo, Circ, Back, Elastic, Bounce (In / Out / In-Out), matching easings.net.
- **Bézier curve creator** — drag two control points to author a curve; copy it out as a `cubic-bezier(...)` string.
- **Custom AnimationCurve mode** — full keyframe and tangent control for any shape, including multi-bounce.
- **`EasingMover` component** — move any object along a chosen curve, with once / loop / ping-pong.
- **`EasingFunction`** — a serializable curve field with a custom inspector and inline preview you can drop into your own scripts.
- **Runtime library** — `Easing.Evaluate(...)` and `BezierEasing.Evaluate(...)`, no editor dependency, ships in builds.

## Installation

### Via Git URL
In Unity, open **Window > Package Manager**, click **＋ > Add package from git URL…**, and paste your repository URL, for example:

```
https://github.com/BeastBeatsGaming/easing-playground.git
```

### Local
Copy this folder into your project's `Packages/` directory.

## Quick start

### 1. Preview curves
Open **Tools > Easing Playground**. Switch between **Library**, **Bézier**, and **Custom Curve**, then press **Play** to watch the dot ride the curve.

### 2. Move an object
Add an **Easing Mover** component (**Add Component > Easing > Easing Mover**), choose a curve and a `Move By` offset, and press Play.

### 3. Use a curve in your own script
```csharp
using UnityEngine;

public class FadePanel : MonoBehaviour
{
    [SerializeField] private EasingFunction _ease = new EasingFunction();
    [SerializeField] private CanvasGroup _group;

    private void Update()
    {
        float t = Mathf.PingPong(Time.time, 1f);
        _group.alpha = _ease.Evaluate(t);
    }
}
```

Or call the library directly:
```csharp
float v = Easing.Evaluate(EasingType.BackOut, t);
float b = BezierEasing.Evaluate(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f), t);
```

## API

| Member | Description |
| --- | --- |
| `Easing.Evaluate(EasingType type, float t)` | Evaluate one of the 31 built-in curves. |
| `BezierEasing.Evaluate(Vector2 p1, Vector2 p2, float t)` | Evaluate a cubic-bézier easing (P0 = (0,0), P3 = (1,1)). |
| `EasingFunction` | Serializable curve (library / bézier / AnimationCurve) with `Evaluate(t)`. |
| `EasingMover` | Component that moves a Transform along a chosen curve. |

`t` is normalized 0–1; outputs may exceed that range for overshoot curves such as Back and Elastic — use `Vector3.LerpUnclamped` (not `Lerp`) to preserve the overshoot.

## Requirements

Unity 2021.3 or newer.

## License

MIT — see [LICENSE.md](LICENSE.md).