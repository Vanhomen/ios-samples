// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using CoreGraphics;
using System.Linq;
using System.Collections.Generic;

namespace ScanningAndDetecting3DObjects {
	// A custom pinch gesture recognizer that fires only when a threshold is reached
	internal partial class ThresholdPinchGestureRecognizer : UIPinchGestureRecognizer {
		// The threshold in screen pixels after which this gesture is detected.
		const double threshold = 50;

		// Indicates whether the currently active gesture has exceeded the threshold.
		private bool thresholdExceeded = false;
		internal bool ThresholdExceeded { get => thresholdExceeded; private set => thresholdExceeded = value; }

		private double initialTouchDistance = 0;

		internal ThresholdPinchGestureRecognizer (IntPtr handle) : base (handle)
		{
		}

		public override UIGestureRecognizerState State {
			get => base.State;
			set {
				base.State = value;
				switch (value) {
				case UIGestureRecognizerState.Began:
				case UIGestureRecognizerState.Changed:
					break;
				default:
					// Reset threshold check
					ThresholdExceeded = false;
					break;
				}
			}
		}

		double TouchDistance (CGPoint t1, CGPoint t2) => Math.Sqrt (Math.Pow (t1.X - t2.X, 2) + Math.Pow (t1.Y - t2.Y, 2));

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			if (touches.Count != 2) {
				return;
			}
			base.TouchesMoved (touches, evt);

			IEnumerable<CGPoint> tPoints = touches.ToArray<UITouch> ().Select (t => t.LocationInView (View));
			var t0 = tPoints.First ();
			var t1 = tPoints.Skip (1).First ();

			switch (State) {
			case UIGestureRecognizerState.Began:
				initialTouchDistance = TouchDistance (t0, t1);
				break;
			case UIGestureRecognizerState.Changed:
				var touchDistance = TouchDistance (t0, t1);
				if (Math.Abs (touchDistance - initialTouchDistance) > threshold) {
					thresholdExceeded = true;
				}
				break;
			default:
				break;
			}

			if (!thresholdExceeded) {
				Scale = 1.0f;
			}
		}
	}
}
