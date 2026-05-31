# Changelog

All notable changes to this package are documented in this file.
This project adheres to [Semantic Versioning](https://semver.org/).

## [1.0.0] - 2026-06-01

### Added
- Easing Playground editor window (`Tools > Easing Playground`) with live preview, play/loop, and an eased-value readout.
- 31 built-in easing curves via `Easing` / `EasingType` (Sine, Quad, Cubic, Quart, Quint, Expo, Circ, Back, Elastic, Bounce).
- Bézier custom curve creator with draggable control-point handles and `cubic-bezier()` export.
- Custom AnimationCurve curve mode.
- `BezierEasing` runtime evaluator (Newton–Raphson with a bisection fallback).
- `EasingFunction` serializable curve type with a custom property drawer and an inline live preview.
- `EasingMover` component with once / loop / ping-pong playback.