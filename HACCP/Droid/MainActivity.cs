using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using HACCP.Core;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace HACCP.Droid
{
    [Activity(Label = "HACCP.Droid", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity, Android.Support.V4.App.ActivityCompat.IOnRequestPermissionsResultCallback
    {
        private bool hasGoneBackground;

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.SetIcon(Android.Resource.Color.Transparent);

            Forms.Init(this, bundle);


            var a = new Adapter();
            Plugin.Permissions.Abstractions.PermissionStatus status = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);
            if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                App.SetAdapter(a);
            }
            else
            {
                App.SetAdapterDontStartScanning(a);
            }

            //float widthDp = Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density;
            var IsTablet = Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet;

            RequestedOrientation = IsTablet ? ScreenOrientation.Unspecified : ScreenOrientation.Portrait;

            //if (!HACCPAppSettings.SharedInstance.CheckPendingRecords) {
            //	HACCPAppSettings.SharedInstance.CheckPendingRecords = true;
            //}

            LoadApplication(new App());
        }

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
		}

        protected override void OnResume()
        {
			base.OnResume ();
           

            if (hasGoneBackground)
            {
                if (!HaccpAppSettings.SharedInstance.CheckPendingRecords)
                {
                    HaccpAppSettings.SharedInstance.CheckPendingRecords = true;
                    //HACCPAppSettings.SharedInstance.AppResuming = true;
                }
                hasGoneBackground = false;
            }

				
        }

        protected override void OnPause()
        {
            base.OnPause();
            hasGoneBackground = true;
        }


		protected override void OnPostResume ()
		{
			base.OnPostResume ();

			SplashActivity.Instance.Finish ();
		}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}