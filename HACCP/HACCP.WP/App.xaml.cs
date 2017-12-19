using System;
using System.Diagnostics;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using HACCP.WP.BLE;
using HACCP.WP.Localization;
using WinRTXamlToolkit.Controls.DataVisualization;
using Xamarin.Forms;
using Application = Windows.UI.Xaml.Application;
using Frame = Windows.UI.Xaml.Controls.Frame;
using NavigationEventArgs = Windows.UI.Xaml.Navigation.NavigationEventArgs;
using HACCP.Core;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace HACCP.WP
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private bool _firstLaunch;
        private TransitionCollection transitions;

        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            _firstLaunch = true;
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used when the application is launched to open a specific file, to display
        ///     search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (_firstLaunch)
            {
                // Initialize
                GlobalSettings.Initialize();

                var settings = new GlobalSettings();
                HACCP.App.SetBLESettings(settings);

                _firstLaunch = false;
            }


#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            UnhandledException += App_UnhandledException;


            var type = typeof(Legend).GetTypeInfo()
                .Assembly.GetType("WinRTXamlToolkit.Controls.DataVisualization.Properties.Resources");
            WindowsRuntimeResourceManager.InjectIntoResxGeneratedApplicationResourcesClass(
                typeof(HACCP.Core.WP.ResourceFiles.AppResources));
            WindowsRuntimeResourceManager.InjectIntoResxGeneratedApplicationResourcesClass(type);


            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame {CacheSize = 1};

                // TODO: change this value to a cache size that is appropriate for your application

                Forms.Init(e);
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            //Check application coming from background 
            // if (e.PreviousExecutionState == ApplicationExecutionState.Running)
            // {

            // }

            // Ensure the current window is active
            Window.Current.Activate();

            //   var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            //statusBar.

            Window.Current.VisibilityChanged -= Current_VisibilityChanged;
            Window.Current.VisibilityChanged += Current_VisibilityChanged;
        }

        private void Current_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (!WindowsBLEManager.SharedInstance.OpenedBluetoothSettings)
                HaccpAppSettings.SharedInstance.CheckPendingRecords = e.Visible;

            if (e.Visible && WindowsBLEManager.SharedInstance.OpenedBluetoothSettings)
                WindowsBLEManager.SharedInstance.OpenedBluetoothSettings = false;

            if (e.Visible)
                HACCP.App.RefreshDeviceList();
            else
                HACCP.App.UnRegisterDevice();
        }


        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        /// <summary>
        ///     Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            if (rootFrame != null)
            {
                rootFrame.ContentTransitions = transitions ?? new TransitionCollection {new NavigationThemeTransition()};
                rootFrame.Navigated -= RootFrame_FirstNavigated;
            }
            // rootFrame.FontSize = 1;
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}