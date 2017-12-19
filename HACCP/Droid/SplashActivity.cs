using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Environment = System.Environment;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Droid
{
	/// <summary>
	///     Splash activity.
	/// </summary>
	[Activity (Theme = "@style/Theme.Splash", Icon = "@drawable/icon", MainLauncher = true, NoHistory = false,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		ScreenOrientation = ScreenOrientation.Behind)]
	public class SplashActivity : Activity
	{

		public static Activity Instance;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Forms.Init(this, bundle);
			var dpWidth = Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density;

			Instance = this;

			RequestedOrientation = dpWidth > 700 ? ScreenOrientation.Unspecified : ScreenOrientation.Portrait;

			LoadActivity ();
		}

		private void LoadActivity ()
		{
			//System.Threading.Thread.Sleep(1000); 
			StartActivity (typeof(MainActivity));
			//Finish();

			//Intent.SetFlags (Android.Content.ActivityFlags.NoHistory);
		}


	}
}