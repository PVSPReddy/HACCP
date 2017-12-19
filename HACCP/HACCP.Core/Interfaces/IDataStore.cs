using System.Collections.Generic;

namespace HACCP.Core
{
    public interface IDataStore
    {
        void LoadAppSettings(HaccpAppSettings appSetting);

        bool CheckUsersExist();
        bool CheckCategoryExist();
        bool CheckLocationsExist();
        bool CheckItemExists(long locationId);
        bool CheckQuestionExists(long categoryId);
        bool CheckTemperaturesExists();
        bool CheckCheckListsExists();


        void SaveDeviceSettings(DeviceSettings appsetting);
        void SaveSiteSettings(SiteSettings appsetting);
        void SaveCorrectiveAction(List<CorrectiveAction> action);
        void SaveUsers(List<User> users);
        void SaveCategory(List<Category> categories, List<Question> questions, List<CorrectiveAction> corrActions);
        void SaveLocations(List<MenuLocation> locations, List<LocationMenuItem> items);

        void SaveCheckListResponse(CheckListResponse response);


        IEnumerable<User> GetUsers();
        IEnumerable<Category> GetCategories();
        IEnumerable<MenuLocation> GetLocations();
        IEnumerable<CorrectiveAction> GetCorrectiveAction();
        IEnumerable<CorrectiveAction> GetCorrectiveActionForQuestion(long questionId);

        IList<Question> GetQuestions(long catergoryId, bool onlyCompletedItem);
        IList<LocationMenuItem> GetItems(long locationId, bool onlyCompletedItem);


        short GetLocationItemRecordStatus(long locationId);
        short GetCategoryRecordStatus(long catId);

        void UpdateQuestionRecordStatus(Question question);
        void UpdateCategoryRecordStatus(long catId);

        MenuLocation GetLocationById(long locationId);


        void RecordTemperature(ItemTemperature item);
        void AddTemperature(ItemTemperature item);
        void RecordLocationItem(LocationMenuItem item);
        ItemTemperature GetItemTemperature(LocationMenuItem item); //should we need???

        void UpdateLocationItemRecordStatus(long locationId);


        void EraseAllData();
        void ClearTemperatures(bool isUpload);
        void ClearCheckList(bool isUpload);
        void ResetDataForSiteChange();

        List<CheckListResponse> GetCheckListResponses();
        List<ItemTemperature> GetItemTemperatures();

        CheckListResponse GetChecklistResponseById(long id);
        ItemTemperature GetTemperatureRecordById(long itemId, long locationId);
        IList<ItemTemperature> GetTemperatureRecordCollectionById(long locationId);
        ItemTemperature GetTemperatureRecordByRecordNumber(int recordNummber);
        IList<CheckListResponse> GetChecklistResponseCollectionById(long id);
        CheckListResponse GetChecklistResponseByRecordNumber(int recordNum);
        //string  GetSelectedResourceString (int languageId, string key);
        //	void SaveLanguageResources (List<ResourceStrings> resources);
    }
}