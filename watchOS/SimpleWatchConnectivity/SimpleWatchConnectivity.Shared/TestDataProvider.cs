
namespace SimpleWatchConnectivity {
	using System;
	using Foundation;
	using UIKit;
	using System.Linq;

	// Constants to access the payload dictionary.
	// isCurrentComplicationInfo is to tell if the userInfo is from transferCurrentComplicationUserInfo
	// in session:didReceiveUserInfo: (see SessionDelegater).
	public static class PayloadKey {
		public static NSString TimeStamp = new NSString ("timeStamp");
		public static NSString ColorData = new NSString ("colorData");
		public static NSString IsCurrentComplicationInfo = new NSString ("isCurrentComplicationInfo");
	}

	// Constants to identify the app group container used for Settings-Watch.bundle and access
	// the information in Settings-Watch.bundle.
	public static class WatchSettings {
		//public static string SharedContainerId = "" // Specify your group container ID here and Root.plist to make watch settings work.
		public static NSString SharedContainerId = new NSString ("group.com.xamarin.simple-watch-connectivity");
		public static NSString UseLogFileForFileTransfer = new NSString ("useLogFileForFileTransfer");
		public static NSString ClearLogsAfterTransferred = new NSString ("clearLogsAfterTransferred");
	}

	/// <summary>
	/// Generate default payload for commands, which contains a random color and a time stamp.
	/// </summary>
	public static class TestDataProvider {
		/// <summary>
		/// Generate a dictionary containing a time stamp and a random color data.
		/// </summary>
		private static NSDictionary<NSString, NSObject> GetTimedColor ()
		{
			var random = new Random ();
			var red = (float) random.NextDouble ();
			var green = (float) random.NextDouble ();
			var blue = (float) random.NextDouble ();

			var randomColor = UIColor.FromRGBA (red, green, blue, 1f);

			var data = NSKeyedArchiver.ArchivedDataWithRootObject (randomColor, false, out NSError error);
			if (data == null || error != null) {
				throw new Exception ("Failed to archive a UIColor!");
			}

			var dateFormatter = new NSDateFormatter { TimeStyle = NSDateFormatterStyle.Medium };
			var timeString = dateFormatter.StringFor (new NSDate ());

			return NSDictionary<NSString, NSObject>.FromObjectsAndKeys (new NSObject [] { new NSString (timeString), data },
																	   new NSString [] { PayloadKey.TimeStamp, PayloadKey.ColorData });
		}

		/// <summary>
		/// Generate an app context, used as the payload for updateApplicationContext.
		/// </summary>
		public static NSDictionary<NSString, NSObject> AppContext => GetTimedColor ();

		/// <summary>
		/// Generate a message, used as the payload for sendMessage.
		/// </summary>
		public static NSDictionary<NSString, NSObject> Message => GetTimedColor ();

		/// <summary>
		/// Generate a message, used as the payload for sendMessageData.
		/// </summary>
		public static NSData MessageData {
			get {
				var data = NSKeyedArchiver.ArchivedDataWithRootObject (GetTimedColor (), false, out NSError error);
				if (data == null || error != null) {
					throw new Exception ("Failed to archive a timedColor dictionary!");
				}

				return data;
			}
		}

		/// <summary>
		/// Generate a userInfo dictionary, used as the payload for transferUserInfo.
		/// </summary>
		/// <value>The user info.</value>
		public static NSDictionary<NSString, NSObject> UserInfo => GetTimedColor ();

		// Generate a file URL, used as the payload for transferFile.
		//
		// Use WatchSettings to choose the log file, which is generated by Logger
		// for debugging purpose, for file transfer from the watch side.
		// This is only for watchOS as the iOS app doesn't have WKBackgroundTask.
		public static NSUrl File {
			get {
#if __WATCHOS__
                if (!string.IsNullOrEmpty(WatchSettings.SharedContainerId))
                {
                    var defaults = new NSUserDefaults(WatchSettings.SharedContainerId, NSUserDefaultsType.SuiteName);
                    if (defaults.BoolForKey(WatchSettings.UseLogFileForFileTransfer))
                    {
                        return Logger.Shared.GetFileURL();
                    }
                }
#endif

				// Use Info.plist for file transfer.
				// Change this to a bigger file to make the file transfer progress more obvious.
				var url = NSBundle.MainBundle.GetUrlForResource ("Info", "plist"); ;
				if (url == null) {
					throw new Exception ("Failed to find Info.plist in current bundle!");
				}

				return url;
			}
		}

		/// <summary>
		/// Generate a file metadata dictionary, used as the payload for transferFile.
		/// </summary>
		public static NSDictionary<NSString, NSObject> FileMetaData => GetTimedColor ();

		/// <summary>
		/// Generate a complication info dictionary, used as the payload for transferCurrentComplicationUserInfo.
		/// </summary>
		public static NSDictionary<NSString, NSObject> CurrentComplicationInfo {
			get {
				var complicationInfo = GetTimedColor ();

				var objects = complicationInfo.Values.Append (new NSNumber (true)).ToArray ();
				var keys = complicationInfo.Keys.Append (PayloadKey.IsCurrentComplicationInfo).ToArray ();
				return NSDictionary<NSString, NSObject>.FromObjectsAndKeys (objects, keys);
			}
		}
	}
}
