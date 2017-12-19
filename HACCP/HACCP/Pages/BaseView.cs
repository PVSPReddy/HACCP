using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
	public class BaseView : ContentPage, IPage
	{
		public BaseView()
		{
			if (HaccpAppSettings.SharedInstance.IsWindows)
				NavigationPage.SetHasNavigationBar(this, false);
		}

		/// <summary>
		///     Dismisses the popup.
		/// </summary>
		public void DismissPopup()
		{
			var baseLayout = Content as BaseLayout;
			if (baseLayout != null) baseLayout.DismissPopup();

			if (BindingContext != null && BindingContext is BaseViewModel)
			{
				var vm = (BaseViewModel)BindingContext;
				vm.IsProgressVisible = false;
			}
		}


		/// <summary>
		///     Loads the home page.
		/// </summary>
		public void LoadHomePage()
		{
			//if (HACCPAppSettings.SharedInstance.IsWindows) {
			//    await NavigateTo (PageEnum.Home, true);
			//    //Navigation.NavigationStack.ToList().RemoveAt(0);
			//    var page = Navigation.NavigationStack.FirstOrDefault ();
			//    Navigation.RemovePage (page);

			//} else {

			Application.Current.MainPage = new NavigationPage(new Home())
			{
				BarBackgroundColor = Color.FromRgb(20, 34, 43),
				BarTextColor = Color.FromRgb(225, 225, 225),
				HeightRequest = 41
			};
			//}
		}

		/// <summary>
		/// UnSubscribeMessage
		/// </summary>
		public void UnSubscribeMessage()
		{
			try
			{
				var page = Navigation.NavigationStack.FirstOrDefault() as Home;
				if (page != null)
				{
					var vm = page.BindingContext as HomeViewModel;
					if (vm != null) vm.UnSubscribe();
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}

		/// <summary>
		/// LoadServerSettingsPage
		/// </summary>
		public void LoadServerSettingsPage()
		{
			Application.Current.MainPage = new NavigationPage(new ServerSettings())
			{
				BarBackgroundColor = Color.FromRgb(20, 34, 43),
				BarTextColor = Color.FromRgb(225, 225, 225),
				HeightRequest = 41
			};
		}

		/// <summary>
		///     Reloads the home page.
		/// </summary>
		public async void ReloadHomePage()
		{
			try
			{
				if (HaccpAppSettings.SharedInstance.IsWindows)
				{
					var page = Navigation.NavigationStack.First();

					while (page.Navigation.NavigationStack.Count > 1)
					{
						await page.Navigation.PopAsync();
					}
				}
				else
				{
					await Navigation.PopToRootAsync(false);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error ReloadHomePage {0}", ex.Message);
			}
		}

		/// <summary>
		/// PopPages
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public async Task<bool> PopPages(int count)
		{
			var page = Navigation.NavigationStack.First();

			for (var i = 1; i <= count; i++)
			{
				await page.Navigation.PopAsync();
			}

			return true;
		}

		/// <summary>
		///     Shows the alert.
		/// </summary>
		/// <returns>The alert.</returns>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		public async Task ShowAlert(string title, string message)
		{
			await DisplayAlert(title, message, HACCPUtil.GetResourceString("OK"));
		}


		/// <summary>
		///     Shows the confirm alert.
		/// </summary>
		/// <returns>The confirm alert.</returns>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		public async Task<bool> ShowConfirmAlert(string title, string message)
		{
			return
				await
					DisplayAlert(title, message, HACCPUtil.GetResourceString("Yes"), HACCPUtil.GetResourceString("No"));
		}

		/// <summary>
		///     Displaies the alert message.
		/// </summary>
		/// <param name="message">Message.</param>
		public void DisplayAlertMessage(string message)
		{
			DisplayAlertMessage(string.Empty, message, HACCPUtil.GetResourceString("OK"));
		}

		/// <summary>
		///     Displaies the alert message.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		public void DisplayAlertMessage(string title, string message)
		{
			DisplayAlertMessage(title, message, HACCPUtil.GetResourceString("OK"));
		}


		/// <summary>
		///     Displaies the alert message.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="buttonText">Button text.</param>
		public void DisplayAlertMessage(string title, string message, string buttonText)
		{
			Device.BeginInvokeOnMainThread(async () => await DisplayAlert(title, message, buttonText));
		}

		/// <summary>
		///     Pops the page.
		/// </summary>
		/// <returns>The page.</returns>
		public Task PopPage()
		{
			return PopPage(true);
		}

		/// <summary>
		/// PopPage
		/// </summary>
		/// <param name="animated"></param>
		/// <returns></returns>
		public Task PopPage(bool animated)
		{
			return Navigation.PopAsync(animated);
		}


		/// <summary>
		///     Pops the modal.
		/// </summary>
		/// <returns>The modal.</returns>
		public Task PopModal()
		{
			return Navigation.PopModalAsync();
		}

		/// <summary>
		///     Removes the page.
		/// </summary>
		/// <returns>The page.</returns>
		public void RemoveRecordItemPage()
		{
			var page = Navigation.NavigationStack.FirstOrDefault(x => x.GetType() == typeof(RecordItem));
			Navigation.RemovePage(page);
		}


		/// <summary>
		///     Gets a value indicating whether this instance is popup active.
		/// </summary>
		/// <value><c>true</c> if this instance is popup active; otherwise, <c>false</c>.</value>
		public bool IsPopupActive
		{
			get
			{
				var baseLayout = Content as BaseLayout;
				return baseLayout != null && baseLayout.IsPopupActive;
			}
		}

		/// <summary>
		///     Navigates to.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="pageEnum">Page enum.</param>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public Task NavigateTo(PageEnum pageEnum, bool animated)
		{
			switch (pageEnum)
			{
				case PageEnum.WirelessTask:
					return Navigation.PushAsync(new WirelessTask(), animated);
				case PageEnum.SelectUser:
					return Navigation.PushAsync(new SelectUser(), animated);
				case PageEnum.ServerSettings:
					return Navigation.PushAsync(new ServerSettings(), animated);
				case PageEnum.PerformCheckList:
					return Navigation.PushAsync(new PerformCheckList(), animated);
				case PageEnum.SelectLocations:
					return Navigation.PushAsync(new SelectLocations(), animated);
				case PageEnum.Blue2Settings:
					return Navigation.PushAsync(new Blue2Settings(), animated);
				case PageEnum.ViewStatus:
					return Navigation.PushAsync(new ViewStatus(), animated);
				case PageEnum.ThermometerMode:
					return Navigation.PushAsync(new ThermometerMode(), animated);
				case PageEnum.ClearCheckmarks:
					return Navigation.PushAsync(new ClearCheckmarks(), animated);
				case PageEnum.ViewStatusLines:
					return Navigation.PushAsync(new ViewStatusLines(), animated);
				case PageEnum.Home:
					return Navigation.PushAsync(new Home(), animated);
				default:
					return Task.FromResult(0);
			}
		}


		/// <summary>
		///     Navigates to with selected object.
		/// </summary>
		/// <returns>The to with selected object.</returns>
		/// <param name="pageEnum">Page enum.</param>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		/// <param name="selectedObject">Selected object.</param>
		public Task NavigateToWithSelectedObject(PageEnum pageEnum, bool animated, object selectedObject)
		{
			switch (pageEnum)
			{
				case PageEnum.SelectQuestion:
					return Navigation.PushAsync(new SelectQuestion(selectedObject), animated);
				case PageEnum.RecordAnswer:
					return Navigation.PushAsync(new RecordAnswer(selectedObject), animated);
				case PageEnum.SelectLocationItem:
					return Navigation.PushAsync(new SelectLocationItem(selectedObject), animated);
				case PageEnum.RecordItem:
					return Navigation.PushAsync(new RecordItem(selectedObject), animated);
				case PageEnum.RecordComplete:
					return Navigation.PushAsync(new RecordItemComplete(selectedObject), animated);
				case PageEnum.ServerSettingsConfirmation:
					return Navigation.PushAsync(new ServerSettingsConfirmation(selectedObject), animated);
				case PageEnum.MenuChecklist:
					return Navigation.PushAsync(new MenuChecklist(selectedObject), animated);
				case PageEnum.Temperaturereview:
					return Navigation.PushAsync(new TemperatureReview(selectedObject), animated);
				case PageEnum.CategoryReview:
					return Navigation.PushAsync(new CategoryReview(selectedObject), animated);
				default:
					return Task.FromResult(0);
			}
		}


		/// <summary>
		///     Pushs the modal.
		/// </summary>
		/// <returns>The modal.</returns>
		/// <param name="pageEnum">Page enum.</param>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		/// <param name="selectedObject">Selected object.</param>
		public Task PushModal(PageEnum pageEnum, bool animated, object selectedObject)
		{
			switch (pageEnum)
			{
				case PageEnum.RecordComplete:
					return Navigation.PushModalAsync(new NavigationPage(new RecordItemComplete(selectedObject))
					{
						BarBackgroundColor = Color.FromRgb(20, 34, 43),
						BarTextColor = Color.FromRgb(225, 225, 225),
						HeightRequest = 41
					}, animated);

				default:
					return Task.FromResult(0);
			}
		}

		/// <summary>
		///     Ends the editing.
		/// </summary>
		public virtual void EndEditing()
		{
			Unfocus();
		}

		/// <summary>
		///     Adds the tool bar button.
		/// </summary>
		/// <param name="icon">Icon.</param>
		/// <param name="cmd">Cmd.</param>
		public void AddToolBarButton(string icon, Command cmd)
		{
			ToolbarItems.Add(new ToolbarItem
			{
				Icon = icon,
				Order = ToolbarItemOrder.Primary,
				Command = cmd
			});
		}

		/// <summary>
		///     Clears all tool bar buttons.
		/// </summary>
		public void ClearAllToolBarButtons()
		{
			ToolbarItems.Clear();
		}

		/// <summary>
		/// To dismiss the upload progress bar
		/// </summary>
		public void DismissUploadProgress()
		{
			//var pagetype = App.CurrentPageType;
			//var page = Navigation.NavigationStack.LastOrDefault(x => x.GetType() == pagetype) as ContentPage;
			//if (page != null)
			//{
			//    var baseLayout = page.Content as BaseLayout;
			//    BaseViewModel baseViewModel = page.BindingContext as BaseViewModel;
			//    if (baseViewModel != null) baseViewModel.IsBusy = false;
			//    if (baseLayout != null) baseLayout.DismissPopup();
			//}

			if (BindingContext != null && BindingContext is BaseViewModel)
			{
				var vm = (BaseViewModel)BindingContext;
				vm.IsProgressVisible = false;
				MessagingCenter.Send(new UploadProgressMessage(false), HaccpConstant.UploadRecordProgress);
			}
		}

		/// <summary>
		/// Show Upload Progress
		/// </summary>
		public void ShowUploadProgress()
		{
			//var pagetype = App.CurrentPageType;
			//var page = Navigation.NavigationStack.LastOrDefault(x => x.GetType() == pagetype) as ContentPage;
			//if (page != null)
			//{
			//    var baseLayout = page.Content as BaseLayout;

			//    if (baseLayout != null && baseLayout.IsPopupActive)
			//    {
			//        baseLayout.DismissPopup();
			//    }
			//    else
			//    {
			//        BaseViewModel baseViewModel = page.BindingContext as BaseViewModel;
			//        if (baseViewModel != null)
			//            baseViewModel.IsBusy = true;
			//        var loadingIndicator = new HACCPProgressView();
			//        loadingIndicator.HeightRequest = page.Height;
			//        loadingIndicator.WidthRequest = page.Width;
			//        if (baseLayout != null) baseLayout.ShowPopup(loadingIndicator);
			//    }
			//}


			if (BindingContext != null && BindingContext is BaseViewModel)
			{
				var vm = (BaseViewModel)BindingContext;
				vm.IsProgressVisible = true;
				MessagingCenter.Send(new UploadProgressMessage(true), HaccpConstant.UploadRecordProgress);
			}
		}

		/// <summary>
		/// ShowProgressIndicator
		/// </summary>
		//public void ShowProgressIndicator()
		//{
		//    var baseLayout = Content as BaseLayout;
		//    if (baseLayout != null && baseLayout.IsPopupActive)
		//    {
		//        baseLayout.DismissPopup();
		//    }
		//    else
		//    {
		//        var loadingIndicator = new HACCPProgressView
		//        {
		//            HeightRequest = Height,
		//            WidthRequest = Width
		//        };
		//        if (baseLayout != null) baseLayout.ShowPopup(loadingIndicator);
		//    }
		//}


		public void ShowProgressIndicator()
		{

			if (BindingContext != null && BindingContext is BaseViewModel)
			{
				var vm = (BaseViewModel)BindingContext;
				vm.IsProgressVisible = true;
			}
		}



		/// <summary>
		///     Passes the event of the view appearing through to the view model.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();


			if (BindingContext != null && BindingContext is BaseViewModel)
			{
				var vm = (BaseViewModel)BindingContext;
				vm.OnViewAppearing();

				MessagingCenter.Subscribe<UploadProgressMessage>(this, HaccpConstant.UploadRecordProgress, sender =>
		   		{
					   vm.IsProgressVisible = (sender as UploadProgressMessage).IsVisible; ;
		  	 	});
			}


		}

		/// <summary>
		///     Passes the event of the view disappearing through to the view model.
		/// </summary>
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			if (BindingContext != null && BindingContext is BaseViewModel)
			{
				var vm = (BaseViewModel)BindingContext;
				vm.OnViewDisappearing();
				MessagingCenter.Unsubscribe<UploadProgressMessage>(this, HaccpConstant.UploadRecordProgress);
			}
		}

		/// <summary>
		///     Application developers can override this method to provide behavior when the back button is pressed.
		/// </summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		protected override bool OnBackButtonPressed()
		{
			if (IsPopupActive)
			{
				DismissPopup();
				return false;
			}
			return base.OnBackButtonPressed();
		}
	}
}