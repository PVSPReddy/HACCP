using System.Collections.Generic;
using System.Linq;
using SQLite.Net;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class SQLiteDataStore : IDataStore
    {
        #region Member Variables

        private static readonly object Locker = new object();
        private static readonly SQLiteConnection Database;

        #endregion

        /// <summary>
        ///     Initializes the <see cref="HACCP.Core.SQLiteDataStore" /> class.
        /// </summary>
        static SQLiteDataStore()
        {
            Database = DependencyService.Get<ISQLite>().GetConnection();
            Database.CreateTable<User>();
            Database.CreateTable<DeviceSettings>();
            Database.CreateTable<SiteSettings>();
            Database.CreateTable<MenuLocation>();
            Database.CreateTable<LocationMenuItem>();
            Database.CreateTable<CorrectiveAction>();
            Database.CreateTable<Category>();
            Database.CreateTable<CheckListResponse>();
            Database.CreateTable<Question>();
            Database.CreateTable<ItemTemperature>();
            Database.CreateTable<Checklist>();
            Database.CreateTable<Menu>();
        }

        #region Users

        /// <summary>
        ///     Inserts the user.
        /// </summary>
        public void SaveUsers(List<User> users)
        {
            lock (Locker)
            {
                Database.DeleteAll<User>();
                Database.InsertAll(users);
            }
        }

        /// <summary>
        ///     Gets the users.
        /// </summary>
        /// <returns>The users.</returns>
        public IEnumerable<User> GetUsers()
        {
            lock (Locker)
            {
                return from user in Database.Table<User>()
                    select user;
            }
        }

        /// <summary>
        ///     Checks the users exist.
        /// </summary>
        /// <returns><c>true</c>, if users exist was checked, <c>false</c> otherwise.</returns>
        public bool CheckUsersExist()
        {
            lock (Locker)
            {
                return Database.Table<User>().Any();
            }
        }

        #endregion

        #region Site / Device Settings

        /// <summary>
        ///     Loads the app settings.
        /// </summary>
        /// <param name="appSetting">App setting.</param>
        public void LoadAppSettings(HaccpAppSettings appSetting)
        {
            lock (Locker)
            {
                var deviceSettings = (from result in Database.Table<DeviceSettings>()
                    select result).FirstOrDefault();
                if (deviceSettings != null)
                {
                    appSetting.DeviceSettings.AllowManualTemp = deviceSettings.AllowManualTemp;
                    appSetting.DeviceSettings.AutoAdvance = deviceSettings.AutoAdvance;
                    appSetting.DeviceSettings.AutoOffTime = deviceSettings.AutoOffTime;
                    appSetting.DeviceSettings.DateFormat = deviceSettings.DateFormat;
                    appSetting.DeviceSettings.RequirePin = deviceSettings.RequirePin;
                    appSetting.DeviceSettings.SkipRecordPreview = deviceSettings.SkipRecordPreview;
                    appSetting.DeviceSettings.TempScale = deviceSettings.TempScale;
                    appSetting.DeviceSettings.TimeFormat = deviceSettings.TimeFormat;
                    appSetting.DeviceSettings.AllowTextMemo = deviceSettings.AllowTextMemo;
                    appSetting.DeviceSettings.CustomProbDescription = deviceSettings.CustomProbDescription;
                    appSetting.DeviceSettings.Line1 = deviceSettings.Line1;
                    appSetting.DeviceSettings.Line2 = deviceSettings.Line2;
                }

                var siteSettings = (from result in Database.Table<SiteSettings>()
                    select result).FirstOrDefault();
                if (siteSettings != null)
                {
                    appSetting.SiteSettings.ServerAddress = siteSettings.ServerAddress;
                    appSetting.SiteSettings.ServerDirectory = siteSettings.ServerDirectory;
                    appSetting.SiteSettings.ServerPort = siteSettings.ServerPort;
                    appSetting.SiteSettings.SiteId = siteSettings.SiteId;
                    appSetting.SiteSettings.SiteName = siteSettings.SiteName;
                    appSetting.SiteSettings.MenuId = siteSettings.MenuId;
                    appSetting.SiteSettings.MenuName = siteSettings.MenuName;
                    appSetting.SiteSettings.CheckListId = siteSettings.CheckListId;
                    appSetting.SiteSettings.CheckListName = siteSettings.CheckListName;
                    appSetting.SiteSettings.LastBatchNumber = siteSettings.LastBatchNumber;
                    appSetting.SiteSettings.TimeZoneId = siteSettings.TimeZoneId;
                    appSetting.SiteSettings.LastUploaded = siteSettings.LastUploaded ?? "";
                }
            }
        }

        /// <summary>
        ///     Saves the device settings.
        /// </summary>
        public void SaveDeviceSettings(DeviceSettings deviceSettings)
        {
            lock (Locker)
            {
                Database.DeleteAll<DeviceSettings>();
                Database.Insert(deviceSettings);
            }
        }

        /// <summary>
        ///     Saves the site settings.
        /// </summary>
        public void SaveSiteSettings(SiteSettings siteSettings)
        {
            lock (Locker)
            {
                Database.DeleteAll<SiteSettings>();
                Database.Insert(siteSettings);
            }
        }

        /// <summary>
        ///     Erases all data.
        /// </summary>
        public void EraseAllData()
        {
            lock (Locker)
            {
                Database.DeleteAll<User>();
                Database.DeleteAll<DeviceSettings>();
                Database.DeleteAll<SiteSettings>();
                Database.DeleteAll<MenuLocation>();
                Database.DeleteAll<LocationMenuItem>();
                Database.DeleteAll<CorrectiveAction>();
                Database.DeleteAll<Category>();
                Database.DeleteAll<CheckListResponse>();
                Database.DeleteAll<Question>();
                Database.DeleteAll<ItemTemperature>();
                Database.DeleteAll<Checklist>();
                Database.DeleteAll<Menu>();
            }
        }

        #endregion

        #region CheckLists

        /// <summary>
        ///     Checks the category exist.
        /// </summary>
        /// <returns><c>true</c>, if category exist was checked, <c>false</c> otherwise.</returns>
        public bool CheckCategoryExist()
        {
            lock (Locker)
            {
                var count = (from cat in Database.Table<Category>()
                    select cat).Count();
                return count > 0;
            }
        }

        /// <summary>
        ///     Checks the menu exist.
        /// </summary>
        /// <returns><c>true</c>, if menu exist was checked, <c>false</c> otherwise.</returns>
        public bool CheckMenuExist()
        {
            lock (Locker)
            {
                return Database.Table<Menu>().Any();
            }
        }

        /// <summary>
        ///     Checks the checklist exist.
        /// </summary>
        /// <returns><c>true</c>, if checklist exist was checked, <c>false</c> otherwise.</returns>
        public bool CheckChecklistExist()
        {
            lock (Locker)
            {
                return Database.Table<Checklist>().Any();
            }
        }

        /// <summary>
        ///     Saves the checklists.
        /// </summary>
        /// <param name="checklists">Checklists.</param>
        public void SaveChecklists(List<Checklist> checklists)
        {
            lock (Locker)
            {
                Database.DeleteAll<Checklist>();
                Database.InsertAll(checklists);
            }
        }


        /// <summary>
        ///     Saves the menus.
        /// </summary>
        /// <param name="menus">Menus.</param>
        public void SaveMenus(List<Menu> menus)
        {
            lock (Locker)
            {
                Database.DeleteAll<Menu>();
                Database.InsertAll(menus);
            }
        }


        /// <summary>
        ///     Gets the menus.
        /// </summary>
        /// <returns>The menus.</returns>
        public IEnumerable<Menu> GetMenus()
        {
            lock (Locker)
            {
                return from menu in Database.Table<Menu>()
                    select menu;
            }
        }

        /// <summary>
        ///     Gets the checklists.
        /// </summary>
        /// <returns>The checklists.</returns>
        public IEnumerable<Checklist> GetChecklists()
        {
            lock (Locker)
            {
                return from checklist in Database.Table<Checklist>()
                    select checklist;
            }
        }


        /// <summary>
        ///     Saves the category.
        /// </summary>
        /// <param name="categories">Categories.</param>
        /// <param name="questions"></param>
        /// <param name="corrActions"></param>
        public void SaveCategory(List<Category> categories, List<Question> questions, List<CorrectiveAction> corrActions)
        {
            lock (Locker)
            {
                var query = "DELETE FROM CorrectiveAction WHERE QuestionID > 0";
                Database.Execute(query);
                Database.DeleteAll<Question>();
                Database.DeleteAll<Category>();
                Database.InsertAll(categories);
                Database.InsertAll(questions);
                Database.InsertAll(corrActions);
            }
        }


        /// <summary>
        ///     Gets the categories.
        /// </summary>
        /// <returns>The categories.</returns>
        public IEnumerable<Category> GetCategories()
        {
            lock (Locker)
            {
                return from category in Database.Table<Category>()
                    select category;
            }
        }

        /// <summary>
        ///     Saves the questions.
        /// </summary>
        /// <param name="questions">Questions.</param>
        public void SaveQuestions(List<Question> questions)
        {
            lock (Locker)
            {
                Database.InsertAll(questions);
            }
        }

        /// <summary>
        ///     Deletes the questions.
        /// </summary>
        public void DeleteQuestions()
        {
            lock (Locker)
            {
                Database.DeleteAll<Question>();
            }
        }

        /// <summary>
        ///     Gets the questions.
        /// </summary>
        /// <returns>The questions.</returns>
        /// <param name="categoryId">Catergory identifier.</param>
        /// <param name="onlyCompletedItem"></param>
        public IList<Question> GetQuestions(long categoryId, bool onlyCompletedItem)
        {
            lock (Locker)
            {
                var parameters = new object[1];
                parameters[0] = categoryId;
                var query = onlyCompletedItem
                    ? "SELECT * FROM Question WHERE CategoryID = ? AND RecordStatus=1"
                    : "SELECT * FROM Question WHERE CategoryID = ?";
                IList<Question> ret = Database.Query<Question>(query, parameters);
                return ret;
            }
        }


        /// <summary>
        ///     Gets the question record status.
        /// </summary>
        /// <returns>The question record status.</returns>
        /// <param name="categoryId">Category I.</param>
        public short GetQuestionRecordStatus(long categoryId)
        {
            lock (Locker)
            {
                var parameters = new object[1];
                parameters[0] = categoryId;

                var query = "SELECT COUNT(*) from CheckListResponse where CategoryID =?";

                short recordStatus;
                if (Database.ExecuteScalar<int>(query, parameters) != 0)
                {
                    query =
                        "SELECT CASE WHEN (select count(*) from CheckListResponse where CategoryID =?)=(select count(*) from Question where CategoryID =?) THEN 1 ELSE 2 END AS RowCountResult";
                    recordStatus = (short) Database.ExecuteScalar<int>(query, parameters);
                }
                else
                    recordStatus = 0;
                return recordStatus;
            }
        }

        /// <summary>
        ///     Updates the location item record status.
        /// </summary>
        public void UpdateQuestionRecordStatus(Question question)
        {
            lock (Locker)
            {
                Database.Update(question);
            }
        }

        /// <summary>
        ///     Updates the category record status.
        /// </summary>
        /// <param name="catId">Cat I.</param>
        public void UpdateCategoryRecordStatus(long catId)
        {
            lock (Locker)
            {
                var category = Database.Table<Category>().FirstOrDefault(x => x.CategoryId == catId);
                var questions = Database.Table<Question>().Where(x => x.CategoryId == catId);
                var totalcount = questions.Count();
                var recordcount = questions.Count(x => x.RecordStatus == (short) 1);
                if (recordcount == 0)
                    category.RecordStatus = 0;
                else if (recordcount == totalcount)
                    category.RecordStatus = 1;
                else
                    category.RecordStatus = 2;
                Database.Update(category);
            }
        }


        /// <summary>
        ///     Gets the category record status.
        /// </summary>
        /// <returns>The category record status.</returns>
        /// <param name="catId">Cat I.</param>
        public short GetCategoryRecordStatus(long catId)
        {
            lock (Locker)
            {
                var category = Database.Table<Category>().FirstOrDefault(x => x.CategoryId == catId);
                return category.RecordStatus;
            }
        }


        /// <summary>
        ///     Saves the check list response.
        /// </summary>
        public void SaveCheckListResponse(CheckListResponse checklistResponse)
        {
            lock (Locker)
            {
//				var response = database.Table<CheckListResponse> ().FirstOrDefault (x => x.QuestionID == checklistResponse.QuestionID);
//				if (response != null) {
//					checklistResponse.RecordNo = response.RecordNo;
//					database.Update (checklistResponse);
//				} else
                Database.Insert(checklistResponse);
            }
        }


        /// <summary>
        ///     Gets the corrective action for question.
        /// </summary>
        /// <returns>The corrective action for question.</returns>
        /// <param name="questionId">Question identifier.</param>
        public IEnumerable<CorrectiveAction> GetCorrectiveActionForQuestion(long questionId)
        {
            lock (Locker)
            {
                return (from corrActions in Database.Table<CorrectiveAction>()
                    select corrActions).Where(i => i.QuestionId == questionId);
            }
        }

        /// <summary>
        ///     Gets the checklist response by identifier.
        /// </summary>
        /// <returns>The checklist response by identifier.</returns>
        /// <param name="id">Identifier.</param>
        public CheckListResponse GetChecklistResponseById(long id)
        {
            lock (Locker)
            {
                return Database.Table<CheckListResponse>().LastOrDefault(x => x.QuestionId == id);
            }
        }

        /// <summary>
        ///     Gets the checklist response collection by identifier.
        /// </summary>
        /// <returns>The checklist response collection by identifier.</returns>
        /// <param name="id">Identifier.</param>
        public IList<CheckListResponse> GetChecklistResponseCollectionById(long id)
        {
            lock (Locker)
            {
                return Database.Table<CheckListResponse>().Where(x => x.CategoryId == id).ToList();
            }
        }

        /// <summary>
        ///     Gets the checklist response by record number.
        /// </summary>
        /// <returns>The checklist response by record number.</returns>
        /// <param name="recordNum">Record number.</param>
        public CheckListResponse GetChecklistResponseByRecordNumber(int recordNum)
        {
            lock (Locker)
            {
                return Database.Table<CheckListResponse>().FirstOrDefault(x => x.RecordNo == recordNum);
            }
        }


        /// <summary>
        ///     Checks the question exists.
        /// </summary>
        /// <returns><c>true</c>, if question exists was checked, <c>false</c> otherwise.</returns>
        /// <param name="categoryId">Category identifier.</param>
        public bool CheckQuestionExists(long categoryId)
        {
            lock (Locker)
            {
                return (from questions in Database.Table<Question>()
                    select questions).Any(i => i.CategoryId == categoryId);
            }
        }

        #endregion

        #region Location

        /// <summary>
        ///     Saves the locations.
        /// </summary>
        /// <param name="locations">Locations.</param>
        /// <param name="items"></param>
        public void SaveLocations(List<MenuLocation> locations, List<LocationMenuItem> items)
        {
            lock (Locker)
            {
                Database.DeleteAll<MenuLocation>();
                Database.DeleteAll<LocationMenuItem>();
                Database.InsertAll(locations);
                Database.InsertAll(items);
            }
        }

        /// <summary>
        ///     Gets the locations.
        /// </summary>
        /// <returns>The locations.</returns>
        public IEnumerable<MenuLocation> GetLocations()
        {
            lock (Locker)
            {
                return (from locations in Database.Table<MenuLocation>()
                    select locations).OrderBy(i => i.RowId);
            }
        }

        /// <summary>
        ///     Checks the locations exist in the database.
        /// </summary>
        /// <returns><c>true</c>, if locations exist was checked, <c>false</c> otherwise.</returns>
        public bool CheckLocationsExist()
        {
            lock (Locker)
            {
                return Database.Table<MenuLocation>().Any();
            }
        }

        /// <summary>
        ///     Gets the location by Id
        /// </summary>
        /// <returns>The location by Id</returns>
        /// <param name="locationId">Location identifier.</param>
        public MenuLocation GetLocationById(long locationId)
        {
            lock (Locker)
            {
                return (from locations in Database.Table<MenuLocation>()
                    where locations.LocationId == locationId
                    select locations).FirstOrDefault();
            }
        }


        /// <summary>
        ///     Checks the item exists.
        /// </summary>
        /// <returns><c>true</c>, if item exists was checked, <c>false</c> otherwise.</returns>
        /// <param name="locationId">Location identifier.</param>
        public bool CheckItemExists(long locationId)
        {
            lock (Locker)
            {
                return (from items in Database.Table<LocationMenuItem>()
                    select items).Any(i => i.LocationId == locationId);
            }
        }

        #endregion

        #region Corrective Actions

        /// <summary>
        ///     Saves the corrective action.
        /// </summary>
        public void SaveCorrectiveAction(List<CorrectiveAction> corrActions)
        {
            lock (Locker)
            {
                var query = "Delete from CorrectiveAction where QuestionID < 1";
                Database.Execute(query);
                Database.InsertAll(corrActions);
            }
        }

        //		/// <summary>
        //		/// Saves the corrective action for question.
        //		/// </summary>
        //		/// <param name="corrActions">Corr actions.</param>
        //		/// <param name="questionId">Question identifier.</param>
        //		public void SaveCorrectiveActionForQuestion (List<CorrectiveAction> corrActions, long questionId)
        //		{
        //			lock (locker) {
        //				object[] parameters = new object[1];
        //				parameters [0] = questionId;
        //				var	query = "Delete from CorrectiveAction where QuestionID=?";
        //				database.Execute (query, parameters);
        //				database.InsertAll (corrActions);
        //			}
        //		}

        /// <summary>
        ///     Gets the users.
        /// </summary>
        /// <returns>The users.</returns>
        public IEnumerable<CorrectiveAction> GetCorrectiveAction()
        {
            lock (Locker)
            {
                return from actions in Database.Table<CorrectiveAction>()
                    where actions.QuestionId < 1
                    select actions; //.OrderBy (i => i.QuestionID);
            }
        }

        #endregion

        #region Location Item

        /// <summary>
        ///     Gets the items based on Location ID and listCompletedItems filter flag.
        /// </summary>
        /// <returns>The items.</returns>
        /// <param name="locationId">Location I.</param>
        /// <param name="onlyCompletedItem"></param>
        public IList<LocationMenuItem> GetItems(long locationId, bool onlyCompletedItem)
        {
            lock (Locker)
            {
                var parameters = new object[1];
                parameters[0] = locationId;
                var query = onlyCompletedItem
                    ? "SELECT * FROM LocationMenuItem WHERE LocationID = ? AND RecordStatus = 1"
                    : "SELECT * FROM LocationMenuItem WHERE LocationID = ?";
                IList<LocationMenuItem> ret = Database.Query<LocationMenuItem>(query, parameters);
                return ret;
            }
        }

        /// <summary>
        ///     Updates the location item record status.
        /// </summary>
        /// <param name="locationId">Location I.</param>
        public void UpdateLocationItemRecordStatus(long locationId)
        {
            lock (Locker)
            {
                var location = Database.Table<MenuLocation>().FirstOrDefault(x => x.LocationId == locationId);
                var locationItems = Database.Table<LocationMenuItem>().Where(x => x.LocationId == locationId);
                var totalcount = locationItems.Count();
                var recordcount = locationItems.Count(x => x.RecordStatus == (short) 1);
                if (recordcount == 0)
                    location.RecordStatus = 0;
                else if (recordcount == totalcount)
                    location.RecordStatus = 1;
                else
                    location.RecordStatus = 2;
                Database.Update(location);
            }
        }

        /// <summary>
        ///     Gets the location item record status.
        /// </summary>
        /// <returns>The location item record status.</returns>
        /// <param name="locationId">Location I.</param>
        public short GetLocationItemRecordStatus(long locationId)
        {
            lock (Locker)
            {
                var location = Database.Table<MenuLocation>().FirstOrDefault(x => x.LocationId == locationId);
                return location.RecordStatus;
            }
        }

        /// <summary>
        ///     Checks the locations exist in the database.
        /// </summary>
        /// <returns><c>true</c>, if locations exist was checked, <c>false</c> otherwise.</returns>
        public bool CheckItemExists()
        {
            lock (Locker)
            {
                return Database.Table<LocationMenuItem>().Any();
            }
        }

        /// <summary>
        ///     Gets the item temperature.
        /// </summary>
        /// <returns>The item temperature.</returns>
        /// <param name="item">Item.</param>
        public ItemTemperature GetItemTemperature(LocationMenuItem item)
        {
            lock (Locker)
            {
                return Database.Table<ItemTemperature>().FirstOrDefault(x => x.ItemID == item.ItemId);
            }
        }

        /// <summary>
        ///     Updates the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void RecordTemperature(ItemTemperature item)
        {
            lock (Locker)
            {
                Database.Update(item);
            }
        }

        /// <summary>
        ///     Adds the temperature.
        /// </summary>
        /// <param name="item">Item.</param>
        public void AddTemperature(ItemTemperature item)
        {
            lock (Locker)
            {
                Database.Insert(item);
            }
        }

        /// <summary>
        ///     Gets the temperature record identifier.
        /// </summary>
        /// <returns>The temperature record identifier.</returns>
        public ItemTemperature GetTemperatureRecordById(long itemId, long locationId)
        {
            lock (Locker)
            {
                return
                    Database.Table<ItemTemperature>()
                        .LastOrDefault(x => x.ItemID == itemId && x.LocationID == locationId);
            }
        }

        /// <summary>
        ///     Gets the temperature record collection by identifier.
        /// </summary>
        /// <returns>The temperature record collection by identifier.</returns>
        /// <param name="locationId">Location I.</param>
        public IList<ItemTemperature> GetTemperatureRecordCollectionById(long locationId)
        {
            lock (Locker)
            {
                return Database.Table<ItemTemperature>().Where(x => x.LocationID == locationId).ToList();
            }
        }

        /// <summary>
        ///     Gets the temperature record by record number.
        /// </summary>
        /// <returns>The temperature record by record number.</returns>
        public ItemTemperature GetTemperatureRecordByRecordNumber(int recordNummber)
        {
            lock (Locker)
            {
                return Database.Table<ItemTemperature>().FirstOrDefault(x => x.RecordNo == recordNummber);
            }
        }

        /// <summary>
        ///     Checks the temperatures exists.
        /// </summary>
        /// <returns><c>true</c>, if temperatures exists was checked, <c>false</c> otherwise.</returns>
        public bool CheckTemperaturesExists()
        {
            lock (Locker)
            {
                var loc = (from locations in Database.Table<MenuLocation>()
                    select locations).Where(l => l.RecordStatus > 0);

                return loc.Any();
            }
        }

        /// <summary>
        ///     Checks the check lists.
        /// </summary>
        /// <returns><c>true</c>, if check lists was checked, <c>false</c> otherwise.</returns>
        public bool CheckCheckListsExists()
        {
            lock (Locker)
            {
                var categories = (from category in Database.Table<Category>()
                    select category).Where(l => l.RecordStatus > 0);
                return categories.Any();
            }
        }

        /// <summary>
        ///     Clears the temperatures.
        /// </summary>
        /// <param name="isUpload">If set to <c>true</c> is upload.</param>
        public void ClearTemperatures(bool isUpload)
        {
            lock (Locker)
            {
                var query = "Update LocationMenuItem set RecordStatus = 0";
                Database.Execute(query);
                query = "Update MenuLocation set RecordStatus = 0";
                Database.Execute(query);
                if (isUpload)
                {
                    Database.DeleteAll<ItemTemperature>();
                }
            }
        }

        /// <summary>
        ///     Clears the check list.
        /// </summary>
        /// <param name="isUpload">If set to <c>true</c> is upload.</param>
        public void ClearCheckList(bool isUpload)
        {
            lock (Locker)
            {
                var query = "Update Category set RecordStatus = 0";
                Database.Execute(query);
                query = "Update Question set RecordStatus = 0";
                Database.Execute(query);
                if (isUpload)
                {
                    Database.DeleteAll<CheckListResponse>();
                }
            }
        }


        /// <summary>
        ///     Records the location item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void RecordLocationItem(LocationMenuItem item)
        {
            lock (Locker)
            {
                Database.Update(item);
            }
        }


        /// <summary>
        ///     Gets the check list responses.
        /// </summary>
        /// <returns>The check list responses.</returns>
        public List<CheckListResponse> GetCheckListResponses()
        {
            lock (Locker)
            {
                return Database.Table<CheckListResponse>().ToList();
            }
        }

        /// <summary>
        ///     Gets the item temperatures.
        /// </summary>
        /// <returns>The item temperatures.</returns>
        public List<ItemTemperature> GetItemTemperatures()
        {
            lock (Locker)
            {
                return Database.Table<ItemTemperature>().ToList();
            }
        }

        /// <summary>
        /// ResetDataForSiteChange
        /// </summary>
        public void ResetDataForSiteChange()
        {
            lock (Locker)
            {
                Database.DeleteAll<User>();
                Database.DeleteAll<MenuLocation>();
                Database.DeleteAll<LocationMenuItem>();
                Database.DeleteAll<CorrectiveAction>();
                Database.DeleteAll<Category>();
                Database.DeleteAll<Question>();
            }
        }

        #endregion
    }
}