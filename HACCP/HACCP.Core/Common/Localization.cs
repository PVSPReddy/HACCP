using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class Localization
    {
        private static readonly int langageId = (int) Languages.English;

        /// <summary>
        /// SetLocale
        /// </summary>
        public static void SetLocale()
        {
            DependencyService.Get<ILocale>().SetLocale();
        }

        /// <remarks>
        ///     Maybe we can cache this info rather than querying every time
        /// </remarks>
        public static string Locale()
        {
            if (Device.OS == TargetPlatform.Android)
                return DependencyService.Get<ILocale>().GetCurrent().TwoLetterISOLanguageName;
            return DependencyService.Get<ILocale>().GetCurrent().Name;
        }

        /// <summary>
        /// LocaleId
        /// </summary>
        /// <returns></returns>
        public static int LocaleId()
        {
            return langageId;
        }

        /// <summary>
        /// Localize
        /// </summary>
        /// <param name="key"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static string Localize(string key, string comment)
        {
            string result;
            try
            {
                var netLanguage = Locale();
                // Platform-specific
                var temp = new ResourceManager("HACCP.Core.WP.ResourceFiles.AppResources",
                    typeof(Localization).GetTypeInfo().Assembly);
                Debug.WriteLine("Localize " + key);
                result = temp.GetString(key, new CultureInfo(netLanguage));
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}