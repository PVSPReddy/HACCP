using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public interface IPage
    {
        bool IsPopupActive { get; }
        void DismissPopup();
        Task ShowAlert(string title, string message);
        Task<bool> ShowConfirmAlert(string title, string message);
        void DisplayAlertMessage(string title);
        void DisplayAlertMessage(string title, string message);
        void DisplayAlertMessage(string title, string message, string buttonText);
        Task PopPage();
        Task PopModal();
        void RemoveRecordItemPage();
        Task PushModal(PageEnum pageEnum, bool animated, object selectedObject);
        Task NavigateTo(PageEnum pageEnum, bool animated);
        Task NavigateToWithSelectedObject(PageEnum pageEnum, bool animated, object selectedObject);
        void LoadHomePage();
        void ReloadHomePage();
        void LoadServerSettingsPage();
        void EndEditing();
        void AddToolBarButton(string icon, Command cmd);
        void ClearAllToolBarButtons();
        Task PopPage(bool animated);
        void ShowProgressIndicator();
        void ShowUploadProgress();
        void DismissUploadProgress();
        Task<bool> PopPages(int count);
        void UnSubscribeMessage();
    }
}