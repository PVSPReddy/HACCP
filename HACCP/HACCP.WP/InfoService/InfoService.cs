using System;
using Windows.ApplicationModel;
using HACCP.Core;
using HACCP.WP.InfoService;
using Xamarin.Forms;

[assembly: Dependency(typeof(InfoService))]

namespace HACCP.WP.InfoService
{
    public class InfoService : IInfoService
    {
        public string ApplicationVersion { get; set; }

        public string GetAppVersion()
        {
            var version = new Version(Package.Current.Id.Version.Major,
                Package.Current.Id.Version.Minor,
                Package.Current.Id.Version.Revision,
                Package.Current.Id.Version.Build);


            //var context = Forms.Context;
            ApplicationVersion = string.Format("{0}.{1}", version.Major, version.Minor);
            return ApplicationVersion;
        }
    }
}