using Android.Content;
using Android.Net.Wifi;
using HACCP.Core;
using HACCP.Droid;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(NetworkConnection))]

namespace HACCP.Droid
{
    public class NetworkConnection : INetworkConnection
    {
        public bool IsConnected { get; set; }

        public void CheckNetworkConnection()
        {
            //			
            //				var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService (Context.WifiService);
            //				if (connectivityManager == null) {
            //					IsConnected = false;
            //					return;
            //				}
            //				var activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
            //				if (activeNetworkInfo != null && activeNetworkInfo.IsConnectedOrConnecting) {
            //					IsConnected = true;
            //				} else {
            //					IsConnected = false;
            //				}
            //
            //			

            var connectivityManager = (WifiManager) Application.Context.GetSystemService(Context.WifiService);
            IsConnected = connectivityManager != null && connectivityManager.IsWifiEnabled;
        }
    }
}