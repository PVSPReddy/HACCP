using System;
using System.Globalization;
using System.Threading;
using HACCP.Core;
using HACCP.Droid;
using Java.Util;
using Xamarin.Forms;

[assembly: Dependency(typeof(Locale_Android))]

namespace HACCP.Droid
{
    public class Locale_Android : ILocale
    {
        public void SetLocale()
        {
            var androidLocale = Locale.Default; // user's preferred locale
            var netLocale = androidLocale.ToString().Replace("_", "-");
            var ci = new CultureInfo(netLocale);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        /// <remarks>
        ///     Not sure if we can cache this info rather than querying every time
        /// </remarks>
        public CultureInfo GetCurrent()
        {
            var androidLocale = Locale.Default; // user's preferred locale

            // en, es, ja
            // en-US, es-ES, ja-JP
            var netLocale = androidLocale.ToString().Replace("_", "-");

            #region Debugging output

            var ci = new CultureInfo(netLocale);

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            Console.WriteLine(@"thread:  " + Thread.CurrentThread.CurrentCulture);
            Console.WriteLine(@"threadui:" + Thread.CurrentThread.CurrentUICulture);

            #endregion

            return ci;
        }
    }
}