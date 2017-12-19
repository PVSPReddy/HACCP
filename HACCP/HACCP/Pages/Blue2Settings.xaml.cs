using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class Blue2Settings : BaseView
    {

        /// <summary>
        /// Blue2SettingsViewModel object
        /// </summary>
        private readonly Blue2SettingsViewModel _viewModel;

        /// <summary>
        /// Blue2Settings Constructor
        /// </summary>
        public Blue2Settings()
        {
            InitializeComponent();


            Connection_Button.WidthRequest = Device.Idiom == TargetIdiom.Tablet ? 330 : 230;


            MessagingCenter.Subscribe<Blue2PlaceHolderVisibility>(this, HaccpConstant.Blue2PlaceholderVisibility,
                sender => { SetPlaceHolderVisibility(sender.IsVisible); });

            NavigationPage.SetBackButtonTitle(this, string.Empty);
            _viewModel = new Blue2SettingsViewModel(this);
            BindingContext = _viewModel;


            editorcontrol.Focused += (sender, e) => { placeholdernote.IsVisible = false; };

            editorcontrol.Unfocused += (sender, e) =>
            {
                _viewModel.UpdateProbeDescription();

                placeholdernote.IsVisible = string.IsNullOrEmpty(editorcontrol.Text);
            };

            entrycontrol.Focused += (sender, e) => { Windowsplaceholdernote.IsVisible = false; };

            entrycontrol.Unfocused += (sender, e) =>
            {
                _viewModel.UpdateProbeDescription();
                Windowsplaceholdernote.IsVisible = string.IsNullOrEmpty(editorcontrol.Text);
            };

            if (Device.OS == TargetPlatform.Windows)
            {
                editorcontrol.IsVisible = false;
                entrycontrol.IsVisible = true;
                placeholdernote.IsVisible = false;
                Windowsplaceholdernote.IsVisible = true;
            }
            else
            {
                editorcontrol.IsVisible = true;
                placeholdernote.IsVisible = true;
                entrycontrol.IsVisible = false;
                Windowsplaceholdernote.IsVisible = false;
            }
        }

        /// <summary>
        ///     Sets the place holder visibility.
        /// </summary>
        /// <param name="isVisible">If set to <c>true</c> is visible.</param>
        private void SetPlaceHolderVisibility(bool isVisible)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Device.OS == TargetPlatform.Windows)
                    Windowsplaceholdernote.IsVisible = isVisible;
                else
                    placeholdernote.IsVisible = isVisible;
            });
        }

        /// <summary>
        /// OnDisappearing Base class method
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<Blue2PlaceHolderVisibility>(this, HaccpConstant.Blue2PlaceholderVisibility);
        }

        /// <summary>
        /// On Appearing Base class method
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Connection_Button.IsVisible = true;
            App.CurrentPageType = typeof(Blue2Settings);
        }
    }
}