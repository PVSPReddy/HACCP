using Foundation;
using HACCP.Core;
using HACCP.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(InfoService))]

namespace HACCP.iOS
{
    public class InfoService : IInfoService
    {
        public string ApplicationVersion { get; set; }

        public string GetAppVersion()
        {
            ApplicationVersion = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
            return ApplicationVersion;
        }
    }
}