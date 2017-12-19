using System.Globalization;
using Windows.System.UserProfile;
using HACCP.Core;
using HACCP.WP.Localization;
using Xamarin.Forms;

[assembly: Dependency(typeof(Locale_WinPhone))]

namespace HACCP.WP.Localization
{
    public class Locale_WinPhone : ILocale
    {
        /// <remarks>
        ///     Not sure if we can cache this info rather than querying every time
        /// </remarks>
        public CultureInfo GetCurrent()
        {
            var ci = new CultureInfo(GlobalizationPreferences.Languages[0]);

            return ci;

            //var lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;


            //var culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            //return lang;
        }


        public void SetLocale()
        {
            //  CultureInfo ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);

            //CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture..CreateSpecificCulture("fr-FR");


            // System.Globalization.CultureInfo ci;


            //Thread.CurrentThread.CurrentCulture = ci;
            //Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}