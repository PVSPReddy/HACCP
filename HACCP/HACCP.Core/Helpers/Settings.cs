// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace HACCP.Core
{
	/// <summary>
	///     This is the Settings static class that can be used in your Core solution or in any
	///     of your client applications. All settings are laid out the same exact way with getters
	///     and setters.
	/// </summary>
	public static class Settings
	{
		#region Setting Constants

		private const string DeviceIdKey = "HACCP_device_ID_Key";
		private static readonly string DeviceIdKeyDefault = string.Empty;
		private const string LastLoginUseridKey = "LAST_LOGIN_USERID";
		private static readonly long DefaultLastLoginUserid = 0;
		private const string LanguageIdKey = "HACCP_LANGUAGE_ID_Key";
		private static readonly long LanguageIdKeyDefault = 1;
		private const string LanguageStringKey = "HACCP_LANGUAGE_String_Key";
		private static readonly string LanguageStringKeyDefault = string.Empty;

		#endregion

		#region Properties

		/// <summary>
		///     Gets the app settings.
		/// </summary>
		/// <value>The app settings.</value>
		private static ISettings AppSettings {
			get { return CrossSettings.Current; }
		}

		/// <summary>
		///     Gets or sets the device I.
		/// </summary>
		/// <value>The device I.</value>
		public static string DeviceID {
			get { return AppSettings.GetValueOrDefault (DeviceIdKey, DeviceIdKeyDefault); }
			set { AppSettings.AddOrUpdateValue (DeviceIdKey, value); }
		}

		/// <summary>
		///     Gets or sets the last login user identifier.
		/// </summary>
		/// <value>The last login user identifier.</value>
		public static long LastLoginUserId {
			get { return AppSettings.GetValueOrDefault (LastLoginUseridKey, DefaultLastLoginUserid); }
			set { AppSettings.AddOrUpdateValue (LastLoginUseridKey, value); }
		}

		/// <summary>
		///     Gets or sets the current language I.
		/// </summary>
		/// <value>The current language I.</value>
		public static long CurrentLanguageID {
			get { return AppSettings.GetValueOrDefault (LanguageIdKey, LanguageIdKeyDefault); }
			set { AppSettings.AddOrUpdateValue (LanguageIdKey, value); }
		}

		/// <summary>
		/// CurrentLanguageStrings
		/// </summary>
		public static string CurrentLanguageStrings {
			get { return AppSettings.GetValueOrDefault (LanguageStringKey, LanguageStringKeyDefault); }
			set { AppSettings.AddOrUpdateValue (LanguageStringKey, value); }
		}

		public static RecordingMode RecordingMode { get; set; }

		#endregion
	}
}