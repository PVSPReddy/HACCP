using System.Collections.Generic;
using System.Threading.Tasks;

namespace HACCP.Core
{
    public interface IHACCPService
    {
        Task<bool> IsRemoteReachable(string serverAddress, string port);
        Task<bool> IsWsdlAccess(string serverAddress, string port, string serverDirectory);
        bool IsWifiConnected();
        bool IsConnected();
        Task<ServiceResponse> DownloadUsers();
        Task<ServiceResponse> DownloadSiteAndSettings(bool isActiveDeviceCheck);
        Task<ServiceResponse> DownloadCorrectiveAction();
        Task<ServiceResponse> DownloadCheckList(long checklistId);
        Task<ServiceResponse> DownloadLocationandItems(long menuId);
        Task<ServiceResponse> DownloadMenus();
        Task<ServiceResponse> DownloadChecklists();
        Task<ServiceResponse> DownloadDeviceSettings();
        Task<ServiceResponse> UploadRecords(List<CheckListResponse> checklists, List<ItemTemperature> temperatures);
        Task<ServiceResponse> DownloadLanguages();
        Task<ServiceResponse> SaveLanguageStrings(long languageId);
    }
}