using HACCP.Core;
using HACCP.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(InfoService))]

namespace HACCP.Droid
{
    public class InfoService : IInfoService
    {
        public string ApplicationVersion { get; set; }

        public string GetAppVersion()
        {
            var context = Forms.Context;
            ApplicationVersion = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            return ApplicationVersion;
        }
    }
}