using System;
using System.Globalization;
using System.Threading;
using Foundation;
using HACCP.Core;
using HACCP.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(Locale_iOS))]

namespace HACCP.iOS
{
    public class Locale_iOS : ILocale
    {
        public void SetLocale()
        {
            var iosLocaleAuto = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier;
            var netLocale = iosLocaleAuto.Replace("_", "-");
            CultureInfo ci;
            try
            {
                ci = new CultureInfo(netLocale);
            }
            catch
            {
                ci = GetCurrent();
            }
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            Console.WriteLine("SetLocale: " + ci.Name);
        }

        /// <remarks>
        ///     Not sure if we can cache this info rather than querying every time
        /// </remarks>
        public CultureInfo GetCurrent()
        {
            var netLanguage = "en";
            var prefLanguageOnly = "en";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];

                // HACK: Apple treats portuguese fallbacks in a strange way
                // https://developer.apple.com/library/ios/documentation/MacOSX/Conceptual/BPInternational/LocalizingYourApp/LocalizingYourApp.html
                // "For example, use pt as the language ID for Portuguese as it is used in Brazil and pt-PT as the language ID for Portuguese as it is used in Portugal"
                prefLanguageOnly = pref.Substring(0, 2);
                if (prefLanguageOnly == "pt")
                {
                    pref = pref == "pt" ? "pt-BR" : "pt-PT";
                }
                netLanguage = pref.Replace("_", "-");
                Console.WriteLine("preferred language:" + netLanguage);
            }

            // this gets called a lot - try/catch can be expensive so consider caching or something
            CultureInfo ci;
            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch
            {
                // iOS locale not valid .NET culture (eg. "en-ES" : English in Spain)
                // fallback to first characters, in this case "en"
                ci = new CultureInfo(prefLanguageOnly);
            }

            return ci;
        }
    }
}