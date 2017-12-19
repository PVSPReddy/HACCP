using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HACCP.Core;
using HACCP.Core.WP.ResourceFiles;
using Xamarin.Forms;
namespace HACCP
{
    public class App : Application
    {
        public static IAdapter Adapter;
        public static Type CurrentPageType { get; set; }

        /// <summary>
        /// App Constructor
        /// </summary>
        public App()
        {
            HaccpAppSettings.SharedInstance.IsWindows = Device.OS == TargetPlatform.Windows ||
                                                        Device.OS == TargetPlatform.WinPhone;


            if (HaccpAppSettings.SharedInstance.IsWindows)

                Task.Run(
                    async () =>
                    {
                        HaccpAppSettings.SharedInstance.ResourceString =
                            await DependencyService.Get<IResourceFileHelper>().LoadResourceAsync("ResourceFile.xml");
                    });

            else

                try
                {
                    HaccpAppSettings.SharedInstance.ResourceString =
                        DependencyService.Get<IResourceFileHelper>().LoadResource("ResourceFile.xml");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }


            IDataStore dataStore = new SQLiteDataStore();
            dataStore.LoadAppSettings(HaccpAppSettings.SharedInstance);
            Localization.SetLocale();
            HACCPUtil.PreCalculateSlopes();


            var netLanguage = DependencyService.Get<ILocale>().GetCurrent();
            AppResources.Culture = netLanguage;

            Styles.LoadStyles();


            if (string.IsNullOrEmpty(HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress))
            {
                MainPage = new NavigationPage(new ServerSettings())
                {
                    BarBackgroundColor = Color.FromRgb(20, 34, 43),
                    BarTextColor = Color.FromRgb(225, 225, 225),
                    HeightRequest = 41
                };
            }
            else
            {
                MainPage = new NavigationPage(new Home())
                {
                    BarBackgroundColor = Color.FromRgb(20, 34, 43),
                    BarTextColor = Color.FromRgb(225, 225, 225),
                    HeightRequest = 41
                };
            }
        }

        #region Events 

        /// <summary>
        ///Application  OnStart Event Handler
        /// </summary>
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        /// <summary>
        /// Application OnSleep Event Handler
        /// </summary>
        protected override void OnSleep()
        {
            //int a = 0;
            // Handle when your app sleeps 
        }

        /// <summary>
        ///OnResume Event Handler
        /// </summary>
        protected override void OnResume()
        {
            //int a = 0;
            // Handle when your app resumes
        }

        #endregion

        #region Methods


        /// <summary>
        /// SetAdapter
        /// </summary>
        /// <param name="adapter"></param>
        public static void SetAdapter(IAdapter adapter)
        {
            Adapter = adapter;
            BLEManager.SharedInstance.Adapter = adapter;

            BLEManager.SharedInstance.SearchBlue2Devices();
        }

        /// <summary>
        /// SetAdapterDontStartScanning
        /// For use when Android M+ doesn't have Location permission
        /// </summary>
        /// <param name="adapter"></param>
        public static void SetAdapterDontStartScanning(IAdapter adapter)
        {
            Adapter = adapter;
            BLEManager.SharedInstance.Adapter = adapter;
        }

        #region Windows

        /// <summary>
        /// RefreshDeviceList
        /// </summary>
        public static void RefreshDeviceList()
        {
            WindowsBLEManager.SharedInstance.PopulateDeviceList();
        }

        /// <summary>
        /// SetBLESettings
        /// </summary>
        /// <param name="globalSettings"></param>
        public static void SetBLESettings(IGlobalSettings globalSettings)
        {
            WindowsBLEManager.SharedInstance.GlobalSettings = globalSettings;
            WindowsBLEManager.SharedInstance.RegisterEvents();
        }

        /// <summary>
        /// UnRegisterDevice
        /// </summary>
        public static void UnRegisterDevice()
        {
        }

        #endregion

        #endregion

    }
}