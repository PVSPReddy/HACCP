using Foundation;
using HACCP.Core;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace HACCP.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        // class-level declarations

        //UIWindow window ;
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UIBarButtonItem.Appearance.TintColor = UIColor.FromRGB(253, 219, 0);

            Forms.Init();


            // create a new window instance based on the screen size
            //window = new UIWindow(UIScreen.MainScreen.Bounds);
            App.SetAdapter(Adapter.Current);
            LoadApplication(new App());
            // If you have defined a root view controller, set it here:
            // Window.RootViewController = myViewController;

            // make the window visible
            return base.FinishedLaunching(application, launchOptions);
        }


        public override void OnResignActivation(UIApplication application)
        {
            // User logout 
            // HACCP.Core.HACCPAppSettings.SharedInstance.CurrentUserID = 0;

            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
        }

        public override void WillEnterForeground(UIApplication application)
        {
            HaccpAppSettings.SharedInstance.CheckPendingRecords = true;
        }
			
        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }


        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
            if (BLEManager.SharedInstance.SelectedDevice != null)
                BLEManager.SharedInstance.DisConnectFromWand();
        }
    }
}