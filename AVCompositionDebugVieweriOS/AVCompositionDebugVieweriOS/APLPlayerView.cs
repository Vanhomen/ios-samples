// This file has been autogenerated from a class added in the UI designer.

using System;
using AVFoundation;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace AVCompositionDebugVieweriOS {
	public partial class APLPlayerView : UIView {
		[Export ("layerClass")]
		public static Class LayerClass ()
		{
			return new Class (typeof (AVPlayerLayer));
		}

		public AVPlayer Player {
			get {
				return (Layer as AVPlayerLayer).Player;
			}
			set {
				(Layer as AVPlayerLayer).Player = value;
			}
		}

		public APLPlayerView (IntPtr handle) : base (handle)
		{
		}
	}
}
