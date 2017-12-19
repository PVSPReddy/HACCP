using System;
using System.Net;
using SystemConfiguration;
using CoreFoundation;
using HACCP.Core;
using HACCP.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(NetworkConnection))]

namespace HACCP.iOS
{
    public class NetworkConnection : INetworkConnection
    {
        private NetworkReachability adHocWiFiNetworkReachability;

        private NetworkReachability defaultRouteReachability;
        public bool IsConnected { get; set; }

        public void CheckNetworkConnection()
        {
            InternetConnectionStatus();
        }

        private event EventHandler ReachabilityChanged;

        private void OnChange(NetworkReachabilityFlags flags)
        {
            var h = ReachabilityChanged;
            if (h != null)
                h(null, EventArgs.Empty);
        }

        private bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
        {
            if (defaultRouteReachability == null)
            {
                defaultRouteReachability = new NetworkReachability(new IPAddress(0));
                defaultRouteReachability.SetNotification(OnChange);
                defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }
            if (!defaultRouteReachability.TryGetFlags(out flags))
                return false;
            return IsReachableWithoutRequiringConnection(flags);
        }

        private bool IsAdHocWiFiNetworkAvailable(out NetworkReachabilityFlags flags)
        {
            if (adHocWiFiNetworkReachability == null)
            {
                adHocWiFiNetworkReachability = new NetworkReachability(new IPAddress(new byte[] {169, 254, 0, 0}));
                adHocWiFiNetworkReachability.SetNotification(OnChange);
                adHocWiFiNetworkReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }

            if (!adHocWiFiNetworkReachability.TryGetFlags(out flags))
                return false;

            return IsReachableWithoutRequiringConnection(flags);
        }

        public static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
        {
            // Is it reachable with the current network configuration?
            var isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

            // Do we need a connection to reach it?
            var noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0 || (flags & NetworkReachabilityFlags.IsWWAN) != 0;

            // Since the network stack will automatically try to get the WAN up,
            // probe that

            return isReachable && noConnectionRequired;
        }

        private void InternetConnectionStatus()
        {
            NetworkReachabilityFlags flags;
            var defaultNetworkAvailable = IsNetworkAvailable(out flags);
            if (defaultNetworkAvailable && ((flags & NetworkReachabilityFlags.IsDirect) != 0))
            {
                return;
            }
            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
            {
                return;
            }
            if (flags == 0)
            {
            }
        }

        private bool LocalWifiConnectionStatus()
        {
            NetworkReachabilityFlags flags;
            if (IsAdHocWiFiNetworkAvailable(out flags))
            {
                if ((flags & NetworkReachabilityFlags.IsDirect) != 0)
                    return true;
            }
            return false;
        }
    }
}