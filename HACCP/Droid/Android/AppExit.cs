using Android.OS;
using HACCP.Core;
using HACCP.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppExit))]

namespace HACCP.Droid
{
    public class AppExit : IAppExit
    {
        public void CloseApp()
        {
            Process.KillProcess(Process.MyPid());


        }
    }
}