using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;

namespace HACCP.Core
{
	public class HACCPService : IHACCPService
	{
		private readonly string _endPointAddress;
		private readonly IDataStore dataStore;
		private HttpClient _baseClient;


		/// <summary>
		///     Initializes a new instance of the <see cref="HACCP.Core.HACCPService" /> class.
		/// </summary>
		/// <param name="dataStore">Data store.</param>
		public HACCPService (IDataStore dataStore)
		{
			this.dataStore = dataStore;
			_endPointAddress = HACCPUtil.BuildServerEndpoint (HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress,
				HaccpAppSettings.SharedInstance.SiteSettings.ServerPort,
				HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory);
		}

		#region Properties

		/// <summary>
		///     Gets the base client.
		/// </summary>
		/// <value>The base client.</value>
		private HttpClient BaseClient {
			get {
				var httpClient = _baseClient ?? (_baseClient = new HttpClient ());
				if (HaccpAppSettings.SharedInstance.IsWindows)
					httpClient.DefaultRequestHeaders.Add ("Cache-Control", "no-cache");
				return httpClient;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///     Determines whether this instance is wifi connected.
		/// </summary>
		/// <returns><c>true</c> if this instance is wifi connected; otherwise, <c>false</c>.</returns>
		public bool IsWifiConnected ()
		{
			if (CrossConnectivity.Current.ConnectionTypes != null)
				foreach (var conType in CrossConnectivity.Current.ConnectionTypes)
					if (conType == ConnectionType.WiFi)
						return true;
			return false;
		}


		/// <summary>
		///     Determines whether this instance is remote reachable the specified serverAddress port.
		/// </summary>
		/// <returns><c>true</c> if this instance is remote reachable the specified serverAddress port; otherwise, <c>false</c>.</returns>
		/// <param name="serverAddress">Server address.</param>
		/// <param name="port">Port.</param>
		public async Task<bool> IsRemoteReachable (string serverAddress, string port)
		{
			string host;
			Uri uriResult;
			//return true;
			var result = Uri.TryCreate (serverAddress.Trim (), UriKind.Absolute, out uriResult);
			if (result) {
				var uri = new Uri (serverAddress);
				host = uri.Host.Replace ("www.", "").Trim ();
			} else
				host = serverAddress.Replace ("www.", "").Trim ();
			var portNum = HACCPUtil.ConvertToInt32NulltoZero (port.Trim ('/', '\\', ' ', ':'));
			if (portNum == 0)
				portNum = 80;

			return await CrossConnectivity.Current.IsRemoteReachable (host, portNum);
		}

		/// <summary>
		///     Determines whether this instance is WSDL access the specified serverAddress port serverDirectory.
		/// </summary>
		/// <returns>
		///     <c>true</c> if this instance is WSDL access the specified serverAddress port serverDirectory; otherwise,
		///     <c>false</c>.
		/// </returns>
		/// <param name="serverAddress">Server address.</param>
		/// <param name="port">Port.</param>
		/// <param name="serverDirectory">Server directory.</param>
		public async Task<bool> IsWsdlAccess (string serverAddress, string port, string serverDirectory)
		{
			if (CrossConnectivity.Current.IsConnected) {
				var serverEndPoint = HACCPUtil.BuildServerEndpoint (serverAddress, port, serverDirectory);
				if (!string.IsNullOrEmpty (serverEndPoint)) {
					try {
						var res = await BaseClient.GetAsync (string.Format (@"{0}?wsdl", serverEndPoint));
						res.EnsureSuccessStatusCode ();
						//var xml = await res.Content.ReadAsStringAsync();
						return true;
					} catch (Exception ex) {
						Debug.WriteLine ("Ooops! Something went wrong checking the WSDL access. Exception: {0}", ex);
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		///     Determines whether this instance is connected.
		/// </summary>
		/// <returns><c>true</c> if this instance is connected; otherwise, <c>false</c>.</returns>
		public bool IsConnected ()
		{
			return CrossConnectivity.Current.IsConnected;
		}

		/// <summary>
		///     Checks the is active device.
		/// </summary>
		/// <returns>The is active device.</returns>
		private async Task<bool> CheckIsActiveDevice ()
		{
			var response = await DownloadSiteAndSettings (true);
			return response.ErrorCode != HaccpConstant.AckCodeDeviceDisabled;
		}

		/// <summary>
		///     Downloads the users.
		/// </summary>
		/// <returns>The users.</returns>
		public async Task<ServiceResponse> DownloadUsers ()
		{
			var ret = new ServiceResponse ();

			if (await CheckIsActiveDevice ()) {
				if (!string.IsNullOrEmpty (_endPointAddress)) {
					try {
						var methodUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.HandheldUsers,
							                                  HaccpAppSettings.SharedInstance.DeviceId,
							                                  HaccpAppSettings.SharedInstance.SiteSettings.SiteId.ToString (), string.Empty, null);
						var res = await BaseClient.GetAsync (string.Format (@"{0}{1}", _endPointAddress, methodUrl));
						res.EnsureSuccessStatusCode ();
						var userXml = await res.Content.ReadAsStringAsync ();

						var doc = XDocument.Parse (userXml);
						var userResponse = doc.Root;

						var users = new List<User> ();
						if (userResponse != null)
							foreach (var userNode in userResponse.Descendants()) {
								switch (userNode.Name.LocalName) {
								case "AckCode":
									ret.ErrorCode = Convert.ToInt32 (userNode.Value);
									break;
								case "AckDetailedInfo":
									ret.Message = userNode.Value;
									break;
								case "AckStatus":
									ret.IsSuccess = userNode.Value == HaccpConstant.AckStatusSuccess;
									break;
								case "USERS":
									foreach (var user in userNode.Descendants()) {
										var _user = new User ();
										if (user.Name.LocalName == "User") {
											foreach (var usernode in user.Descendants()) {
												switch (usernode.Name.LocalName) {
												case "ID":
													_user.Id = Convert.ToInt64 (usernode.Value);
													break;
												case "NAME":
													_user.Name = usernode.Value;
													break;
												case "PERMISSION":
													_user.Permision = usernode.Value;
													break;
												case "PIN":
													_user.Pin = usernode.Value;
													break;
												}
											}

											users.Add (_user);
										}
									}
									break;
								}
							}

						if (ret.IsSuccess ||
						                      (ret.ErrorCode.ToString () == HaccpConstant.AckCodeSuccess)) {
							dataStore.SaveUsers (users);

							//Check current user exists in the downloaded list
							var isCurrentUserExists =
								users.Any (u => u.Id == HaccpAppSettings.SharedInstance.CurrentUserId);

							//Logout if current user not exixts in the downloaded list
							if (!isCurrentUserExists) {
								HaccpAppSettings.SharedInstance.CurrentUserId = 0;
							} else if (HaccpAppSettings.SharedInstance.CurrentUserId > 0) {
								var userPermission =
									users.Where (
										u =>
                                            u.Id == HaccpAppSettings.SharedInstance.CurrentUserId &&
										u.Permision == HaccpAppSettings.SharedInstance.Permission);

								if (!userPermission.Any ()) {
									HaccpAppSettings.SharedInstance.CurrentUserId = 0;
								}
							}
						} else if (!string.IsNullOrEmpty (ret.Message)) {
							var serviceMessage =
								HACCPUtil.GetResourceString (Regex.Replace (ret.Message, "/[^a-zA-Z0-9]*/g", ""));
							if (!string.IsNullOrEmpty (serviceMessage)) {
								ret.Message = serviceMessage;
							}
						}
					} catch (Exception ex) {
						ret.IsSuccess = false;
						ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
						Debug.WriteLine ("Ooops! Something went wrong downloading the users. Exception: {0}", ex);
					}
				}
			} else {
				ret.ErrorCode = HaccpConstant.AckCodeDeviceDisabled;
				ret.Message =
                    HACCPUtil.GetResourceString (
					"TheHACCPManagerapplicationhasbeentemporarilydisabledonthisdevicePleasecontacttheapplicationadministrator");
			}
			return ret;
		}

		/// <summary>
		///     Downloads the languages.
		/// </summary>
		/// <returns>The languages.</returns>
		public async Task<ServiceResponse> DownloadLanguages ()
		{
			var ret = new ServiceResponse ();
			var languageList = new ObservableCollection<Language> ();
			//	if (endPointAddress != null && endPointAddress.Length > 0) {
			try {
				//string methodUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.LanguageList, HACCPAppSettings.SharedInstance.DeviceID, string.Empty, string.Empty, null);
				var res = await BaseClient.GetAsync (HaccpConstant.GetLanguageServiceUrl);
				res.EnsureSuccessStatusCode ();
				var languageXml = await res.Content.ReadAsStringAsync ();

				var doc = XDocument.Parse (languageXml);

				var lanuageResponse = doc.Root;


				if (lanuageResponse != null)
					foreach (var language in lanuageResponse.Descendants()) {
						switch (language.Name.LocalName) {
						case "AckCode":
							ret.ErrorCode = 200; //Convert.ToInt32 (language.Value);						
							break;
						case "AckDetailedInfo":
							ret.Message = language.Value.Replace (" ", "");
							break;
						case "AckStatus":
							ret.IsSuccess = language.Value == HaccpConstant.AckStatusSuccess;
							break;
						case "Language":
							{
								var languageObj = new Language ();
								foreach (var lanuageAttribute in language.Attributes()) {
									switch (lanuageAttribute.Name.LocalName) {
									case "id":
										languageObj.LanguageId = Convert.ToInt64 (lanuageAttribute.Value);
										break;
									case "name":
										languageObj.LanguageName = lanuageAttribute.Value;
										break;
									}
								}

								languageObj.LanguageShortName = string.Empty;
								languageObj.IsSelected = languageObj.LanguageId ==
								HaccpAppSettings.SharedInstance.LanguageId;
								languageList.Add (languageObj);
							}
							break;
						}
					}

				ret.Results = languageList;

              
			} catch (Exception ex) {
				ret.IsSuccess = false;
				ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
				Debug.WriteLine ("Ooops! Something went wrong downloading the users. Exception: {0}", ex);
			}

			//}
			return ret;
		}

		/// <summary>
		///     Downloads the check list.
		/// </summary>
		/// <returns>The check list.</returns>
		public async Task<ServiceResponse> DownloadCheckList (long checklistId)
		{
			var ret = new ServiceResponse ();

			if (await CheckIsActiveDevice ()) {
				if (!string.IsNullOrEmpty (_endPointAddress)) {
					try {
						var methodUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.ChecklistDetails,
							                                  HaccpAppSettings.SharedInstance.DeviceId,
							                                  HaccpAppSettings.SharedInstance.SiteSettings.SiteId.ToString (), checklistId.ToString (), null);
						var res = await BaseClient.GetAsync (string.Format (@"{0}{1}", _endPointAddress, methodUrl));
						res.EnsureSuccessStatusCode ();
						var checklistXml = await res.Content.ReadAsStringAsync ();

						var doc = XDocument.Parse (checklistXml);
						var checklistResponse = doc.Root;


						var categoryList = new List<Category> ();
						var corrActionList = new List<CorrectiveAction> ();
						var questions = new List<Question> ();
						long categoryId = 0;
						long questionId = 0;

						if (checklistResponse != null)
							foreach (var categoryDetailsNode in checklistResponse.Descendants()) {
								switch (categoryDetailsNode.Name.LocalName) {
								case "AckCode":
									ret.ErrorCode = Convert.ToInt32 (categoryDetailsNode.Value);
									break;
								case "AckDetailedInfo":
									ret.Message = categoryDetailsNode.Value;
									break;
								case "AckStatus":
									ret.IsSuccess = categoryDetailsNode.Value == HaccpConstant.AckStatusSuccess;
									break;
								case "CATEGORIES":
									{
										foreach (var category in categoryDetailsNode.Descendants()) {
											if (category.Name.LocalName == "Category") {
												var _category = new Category ();
												categoryId++;
												_category.CategoryId = categoryId;
												var isQuestionExists = false;

												foreach (var categoryNode in category.Elements()) {
													switch (categoryNode.Name.LocalName) {
													case "Name":
														_category.CategoryName = categoryNode.Value;
														break;
													case "QUESTIONS":

														foreach (var question in categoryNode.Descendants()) {
															if (question.Name.LocalName == "Question") {
																var _question = new Question ();
																questionId++;
																_question.QuestionId = questionId;
																foreach (var questionNode in question.Descendants()) {
																	switch (questionNode.Name.LocalName) {
																	case "CORRACTIONS":
																		{
																			foreach (
                                                                                    var corrNode in
                                                                                        questionNode.Descendants()) {
																				if (corrNode.Name.LocalName ==
																				                                                                    "CorrAction") {
																					var corrAction =
																						new CorrectiveAction ();
																					foreach (
                                                                                            var cNode in
                                                                                                corrNode.Descendants()) {
																						switch (cNode.Name.LocalName) {
																						case "CORRACTIONNAME":
																							corrAction
                                                                                                        .CorrActionName
                                                                                                        =
                                                                                                        cNode.Value;
																							corrAction
                                                                                                        .QuestionId =
                                                                                                        _question
                                                                                                            .QuestionId;
																							corrActionList.Add (
																								corrAction);
																							break;
																						}
																					}
																				}
																			}
																		}
																		break;
																	case "MAX":
																		_question.Max = questionNode.Value;
																		break;
																	case "MIN":
																		_question.Min = questionNode.Value;
																		break;
																	case "QUESTIONNAME":
																		_question.QuestionName =
                                                                                    questionNode.Value;
																		break;
																	case "TYPE":
																		if (questionNode.Value ==
																		                                                                  HaccpConstant.YesNoQuestionType)
																			_question.QuestionType =
                                                                                        (short)QuestionType.YesOrNo;
																		else if (questionNode.Value ==
																		                                                                       HaccpConstant
                                                                                             .NumericQuestionType)
																			_question.QuestionType =
                                                                                        (short)
                                                                                            QuestionType.NumericAnswer;
																		break;
																	}
																}
																_question.CategoryId = _category.CategoryId;
																isQuestionExists = true;
																questions.Add (_question);
															}
														}
														break;
													}
												}

												if (isQuestionExists) {
													if (!categoryList.Contains (_category))
														categoryList.Add (_category);
												}
											}
										}
									}

									break;
								}
							}
						if (ret.IsSuccess ||
						                      (ret.ErrorCode.ToString () == HaccpConstant.AckCodeSuccess)) {
							if (questions != null && questions.Count > 0)
								dataStore.SaveCategory (categoryList, questions, corrActionList);
						} else if (!string.IsNullOrEmpty (ret.Message)) {
							var serviceMessage =
								HACCPUtil.GetResourceString (
									Regex.Replace (ret.Message, "/[^a-zA-Z0-9]*/g", "").Replace (" ", ""));
							if (!string.IsNullOrEmpty (serviceMessage)) {
								ret.Message = serviceMessage;
							}
						}
					} catch (Exception ex) {
						ret.IsSuccess = false;
						ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
						Debug.WriteLine ("Ooops! Something went wrong downloading the category details. Exception: {0}",
							ex);
					}
				}
			} else {
				ret.ErrorCode = HaccpConstant.AckCodeDeviceDisabled;
				ret.Message =
                    HACCPUtil.GetResourceString (
					"TheHACCPManagerapplicationhasbeentemporarilydisabledonthisdevicePleasecontacttheapplicationadministrator");
			}
			return ret;
		}

		/// <summary>
		///     Downloads the site and settings.
		/// </summary>
		/// <returns>The site and settings.</returns>
		public async Task<ServiceResponse> DownloadSiteAndSettings (bool isActiveDeviceCheck)
		{
			var currentSiteId = HaccpAppSettings.SharedInstance.SiteSettings.SiteId;
			var ret = new ServiceResponse ();
			if (!string.IsNullOrEmpty (_endPointAddress)) {
				try {
					var methodUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.ActiveSiteDetails,
						                               HaccpAppSettings.SharedInstance.DeviceId, string.Empty, string.Empty, null);
					var res = await BaseClient.GetAsync (string.Format (@"{0}{1}", _endPointAddress, methodUrl));
					res.EnsureSuccessStatusCode ();
					var siteSettingsXml = await res.Content.ReadAsStringAsync ();
					var doc = XDocument.Parse (siteSettingsXml);
					var siteSettingsResponse = doc.Root;
					var siteSettings = new SiteSettings ();
					if (siteSettingsResponse != null)
						foreach (var siteSettingsNode in siteSettingsResponse.Descendants()) {
							switch (siteSettingsNode.Name.LocalName) {
							case "AckCode":
								ret.ErrorCode = Convert.ToInt32 (siteSettingsNode.Value);
								break;
							case "AckDetailedInfo":
								ret.Message = siteSettingsNode.Value;
								break;
							case "AckStatus":
								ret.IsSuccess = siteSettingsNode.Value == "Success";
								break;
							case "ISACTIVE":
								ret.ErrorCode = siteSettingsNode.Value == "1"
                                        ? ret.ErrorCode
                                        : HaccpConstant.AckCodeDeviceDisabled;
								break;
							case "ID":
								if (siteSettingsNode.Value != string.Empty) {
									siteSettings.SiteId = Convert.ToInt64 (siteSettingsNode.Value);
								}
								break;
							case "SITENAME":
								if (siteSettingsNode.Value != string.Empty) {
									siteSettings.SiteName = siteSettingsNode.Value;
								}
								break;
							case "tzID":
								if (siteSettingsNode.Value != string.Empty) {
									siteSettings.TimeZoneId = siteSettingsNode.Value;
								}
								break;
							}
						}
					if ((ret.IsSuccess ||
					                   (ret.ErrorCode.ToString () == HaccpConstant.AckCodeSuccess)) &&
					                   !isActiveDeviceCheck) {
						HaccpAppSettings.SharedInstance.SiteSettings.SiteId = siteSettings.SiteId;
						HaccpAppSettings.SharedInstance.SiteSettings.SiteName = siteSettings.SiteName;
						HaccpAppSettings.SharedInstance.SiteSettings.TimeZoneId = siteSettings.TimeZoneId;
						Settings.RecordingMode = RecordingMode.BlueTooth;


						dataStore.SaveSiteSettings (HaccpAppSettings.SharedInstance.SiteSettings);

						//HACCPAppSettings.SharedInstance.CurrentUserID = 0; 

						if (currentSiteId != HaccpAppSettings.SharedInstance.SiteSettings.SiteId) {
							dataStore.ResetDataForSiteChange ();
							HaccpAppSettings.SharedInstance.SiteSettings.MenuId = 0;
							HaccpAppSettings.SharedInstance.SiteSettings.CheckListId = 0;
						}
						ret = await DownloadCorrectiveAction ();
						ret = await DownloadDeviceSettings ();

						//Added downloaded users with this based on the request from customer.
						//ret = await DownloadUsers ();
					} else if (!string.IsNullOrEmpty (ret.Message)) {
						var serviceMessage =
							HACCPUtil.GetResourceString (Regex.Replace (ret.Message, "/[^a-zA-Z0-9]*/g", "")
                                .Replace (" ", ""));
						if (!string.IsNullOrEmpty (serviceMessage)) {
							ret.Message = serviceMessage;
						}
					}
				} catch (Exception ex) {
					ret.IsSuccess = false;
					ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
					Debug.WriteLine ("Ooops! Something went wrong downloading the site details. Exception: {0}", ex);
				}
			}
			return ret;
		}

		/// <summary>
		///     Downloads the menus.
		/// </summary>
		/// <returns>The menus.</returns>
		public async Task<ServiceResponse> DownloadMenus ()
		{
			var serverEndPoint =
				HACCPUtil.BuildServerEndpoint (HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress,
					HaccpAppSettings.SharedInstance.SiteSettings.ServerPort,
					HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory);
			var ret = new ServiceResponse ();
			if (await CheckIsActiveDevice ()) {
				if (!string.IsNullOrEmpty (serverEndPoint)) {
					try {
						var methodUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.ListMenuName,
							                                  HaccpAppSettings.SharedInstance.DeviceId,
							                                  HaccpAppSettings.SharedInstance.SiteSettings.SiteId.ToString (), string.Empty, null);
						var res = await BaseClient.GetAsync (string.Format (@"{0}{1}", serverEndPoint, methodUrl));
						res.EnsureSuccessStatusCode ();
						var menuXml = await res.Content.ReadAsStringAsync ();

						var doc = XDocument.Parse (menuXml);
						var menuResponse = doc.Root;
						var menus = new List<Menu> ();
						if (menuResponse != null)
							foreach (var menuNode in menuResponse.Descendants()) {
								switch (menuNode.Name.LocalName) {
								case "AckCode":
									ret.ErrorCode = Convert.ToInt32 (menuNode.Value);
									break;
								case "AckDetailedInfo":
									ret.Message = menuNode.Value;
									break;
								case "AckStatus":
									ret.IsSuccess = menuNode.Value == "Success";
									break;
								case "MENUS":
									foreach (var menu in menuNode.Descendants()) {
										if (menu.Name.LocalName == "Menu") {
											var _menu = new Menu ();
											foreach (var mNode in menu.Descendants()) {
												switch (mNode.Name.LocalName) {
												case "ID":
													_menu.MenuId = Convert.ToInt64 (mNode.Value);
													break;
												case "NAME":
													_menu.Name = mNode.Value;
													break;
												}
											}
											menus.Add (_menu);
										}
									}
									break;
								}
							}


						if (ret.IsSuccess ||
						                      (ret.ErrorCode.ToString () == HaccpConstant.AckCodeSuccess)) {
							ret.Results = menus;
						} else if (!string.IsNullOrEmpty (ret.Message)) {
							var serviceMessage =
								HACCPUtil.GetResourceString (
									Regex.Replace (ret.Message, "/[^a-zA-Z0-9]*/g", "").Replace (" ", ""));
							if (!string.IsNullOrEmpty (serviceMessage)) {
								ret.Message = serviceMessage;
							}
						}
					} catch (Exception ex) {
						ret.IsSuccess = false;
						ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
						Debug.WriteLine ("Ooops! Something went wrong downloading the menus. Exception: {0}", ex);
					}
				}
			} else {
				ret.ErrorCode = HaccpConstant.AckCodeDeviceDisabled;
				ret.Message =
                    HACCPUtil.GetResourceString (
					"TheHACCPManagerapplicationhasbeentemporarilydisabledonthisdevicePleasecontacttheapplicationadministrator");
			}

			return ret;
		}

		/// <summary>
		///     Downloads the checklists.
		/// </summary>
		/// <returns>The checklists.</returns>
		public async Task<ServiceResponse> DownloadChecklists ()
		{
			var ret = new ServiceResponse ();
			if (await CheckIsActiveDevice ()) {
				var serverEndPoint =
					HACCPUtil.BuildServerEndpoint (HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress,
						HaccpAppSettings.SharedInstance.SiteSettings.ServerPort,
						HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory);
				if (!string.IsNullOrEmpty (serverEndPoint)) {
					try {
						var methodUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.ListCheckListNames,
							                                  HaccpAppSettings.SharedInstance.DeviceId,
							                                  HaccpAppSettings.SharedInstance.SiteSettings.SiteId.ToString (), string.Empty, null);
						var res = await BaseClient.GetAsync (string.Format (@"{0}{1}", serverEndPoint, methodUrl));
						res.EnsureSuccessStatusCode ();
						var checklistXml = await res.Content.ReadAsStringAsync ();

						var doc = XDocument.Parse (checklistXml);
						var checklistResponse = doc.Root;
						var checklists = new List<Checklist> ();
						if (checklistResponse != null)
							foreach (var checklistNode in checklistResponse.Descendants()) {
								switch (checklistNode.Name.LocalName) {
								case "AckCode":
									ret.ErrorCode = Convert.ToInt32 (checklistNode.Value);
									break;
								case "AckDetailedInfo":
									ret.Message = checklistNode.Value;
									break;
								case "AckStatus":
									ret.IsSuccess = checklistNode.Value == "Success";
									break;
								case "CHECKLISTS":

									foreach (var chkNode in checklistNode.Descendants()) {
										if (chkNode.Name.LocalName == "CheckList") {
											var checkList = new Checklist ();
											foreach (var cNode in chkNode.Descendants()) {
												switch (cNode.Name.LocalName) {
												case "ID":
													checkList.ChecklistId = Convert.ToInt64 (cNode.Value);
													break;
												case "NAME":
													checkList.Name = cNode.Value;
													break;
												}
											}

											checklists.Add (checkList);
										}
									}
									break;
								}
							}
						if (ret.IsSuccess ||
						                      (ret.ErrorCode.ToString () == HaccpConstant.AckCodeSuccess)) {
							ret.Results = checklists;
						} else if (!string.IsNullOrEmpty (ret.Message)) {
							var serviceMessage =
								HACCPUtil.GetResourceString (
									Regex.Replace (ret.Message, "/[^a-zA-Z0-9]*/g", "").Replace (" ", ""));
							if (!string.IsNullOrEmpty (serviceMessage)) {
								ret.Message = serviceMessage;
							}
						}
					} catch (Exception ex) {
						ret.IsSuccess = false;
						ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
						Debug.WriteLine ("Ooops! Something went wrong downloading the checklists. Exception: {0}", ex);
					}
				}
			} else {
				ret.ErrorCode = HaccpConstant.AckCodeDeviceDisabled;
				ret.Message =
                    HACCPUtil.GetResourceString (
					"TheHACCPManagerapplicationhasbeentemporarilydisabledonthisdevicePleasecontacttheapplicationadministrator");
			}
			return ret;
		}


		/// <summary>
		///     Downloads the locationand items.
		/// </summary>
		/// <returns>The locationand items.</returns>
		/// <param name="menuId">Menu identifier.</param>
		public async Task<ServiceResponse> DownloadLocationandItems (long menuId)
		{
			var ret = new ServiceResponse ();

			if (await CheckIsActiveDevice ()) {
				if (!string.IsNullOrEmpty (_endPointAddress)) {
					try {
						var methodUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.MenuDetails,
							                                  HaccpAppSettings.SharedInstance.DeviceId,
							                                  HaccpAppSettings.SharedInstance.SiteSettings.SiteId.ToString (), menuId.ToString (), null);
						var res = await BaseClient.GetAsync (string.Format (@"{0}{1}", _endPointAddress, methodUrl));
						res.EnsureSuccessStatusCode ();
						var locationXml = await res.Content.ReadAsStringAsync ();

						var doc = XDocument.Parse (locationXml);
						var locationResponse = doc.Root;

						var locations = new List<MenuLocation> ();
						var items = new List<LocationMenuItem> ();
						long itemRowId = 0;
						long locationRowId = 0;
						if (locationResponse != null)
							foreach (var locationDetailsNode in locationResponse.Descendants()) {
								switch (locationDetailsNode.Name.LocalName) {
								case "AckCode":
									ret.ErrorCode = Convert.ToInt32 (locationDetailsNode.Value);
									break;
								case "AckDetailedInfo":
									ret.Message = locationDetailsNode.Value;
									break;
								case "AckStatus":
									ret.IsSuccess = locationDetailsNode.Value == "Success";
									break;
								case "LOCATIONS":
									foreach (var location in locationDetailsNode.Elements()) {
										if (location.Name.LocalName == "Location") {
											var _location = new MenuLocation ();
											XElement itemElement = null;
											foreach (var locationnode in location.Elements()) {
												switch (locationnode.Name.LocalName) {
												case "LOCATIONID":
													locationRowId++;
													_location.RowId = locationRowId;
													_location.LocationId = Convert.ToInt64 (locationnode.Value);
													break;
												case "LOCATIONNAME":
													_location.Name = locationnode.Value;
													break;
												case "ITEMS":
													itemElement = locationnode;
													break;
												}
											}
											var isItemExists = false;
											if (itemElement != null) {
												foreach (var item in itemElement.Elements()) {
													if (item.Name.LocalName == "Item") {
														var _item = new LocationMenuItem ();
														itemRowId++;
														_item.RowId = itemRowId;
														_item.LocationId = _location.LocationId;
														foreach (var itemNode in item.Elements()) {
															switch (itemNode.Name.LocalName) {
															case "CCP":
																_item.Ccp = itemNode.Value;
																break;
															case "CCPID":
																_item.Ccpid = itemNode.Value;
																break;
															case "ITEMID":
																_item.ItemId = Convert.ToInt64 (itemNode.Value);
																break;
															case "ITEMNAME":
																_item.Name = itemNode.Value;
																break;
															case "MAX":
																_item.Max = itemNode.Value;
																break;
															case "MIN":
																_item.Min = itemNode.Value;
																break;
															}
														}
														isItemExists = true;
														items.Add (_item);
													}
												}
											}
											if (isItemExists) {
												var locCount =
													locations.Count (x => x.LocationId == _location.LocationId);
												if (locCount == 0) {
													locations.Add (_location);
												}
											}
										}
									}
									break;
								}
							}
						if (ret.IsSuccess ||
						                      (ret.ErrorCode.ToString () == HaccpConstant.AckCodeSuccess)) {
							dataStore.SaveLocations (locations, items);
						} else if (!string.IsNullOrEmpty (ret.Message)) {
							var serviceMessage =
								HACCPUtil.GetResourceString (
									Regex.Replace (ret.Message, "/[^a-zA-Z0-9]*/g", "").Replace (" ", ""));
							if (!string.IsNullOrEmpty (serviceMessage)) {
								ret.Message = serviceMessage;
							}
						}
					} catch (Exception ex) {
						ret.IsSuccess = false;
						ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
						Debug.WriteLine (
							"Ooops! Something went wrong downloading the Location and Items. Exception: {0}", ex);
					}
				}
			} else {
				ret.ErrorCode = HaccpConstant.AckCodeDeviceDisabled;
				ret.Message =
                    HACCPUtil.GetResourceString (
					"TheHACCPManagerapplicationhasbeentemporarilydisabledonthisdevicePleasecontacttheapplicationadministrator");
			}

			return ret;
		}


		/// <summary>
		///     Downloads the corrective action.
		/// </summary>
		public async Task<ServiceResponse> DownloadCorrectiveAction ()
		{
			var ret = new ServiceResponse ();

			if (await CheckIsActiveDevice ()) {
				var corrActionList = new List<CorrectiveAction> ();

				if (!string.IsNullOrEmpty (_endPointAddress)) {
					try {
						var methodUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.CorrActions,
							                                  HaccpAppSettings.SharedInstance.DeviceId,
							                                  HaccpAppSettings.SharedInstance.SiteSettings.SiteId.ToString (), string.Empty, null);
						var res = await BaseClient.GetAsync (string.Format (@"{0}{1}", _endPointAddress, methodUrl));
						res.EnsureSuccessStatusCode ();
						var corrActionXml = await res.Content.ReadAsStringAsync ();
						var doc = XDocument.Parse (corrActionXml);
						var correctiveActionResponse = doc.Root;

						if (correctiveActionResponse != null)
							foreach (var corrActionNode in correctiveActionResponse.Descendants()) {
								switch (corrActionNode.Name.LocalName) {
								case "AckCode":
									ret.ErrorCode = Convert.ToInt32 (corrActionNode.Value);
									break;
								case "AckDetailedInfo":
									ret.Message = corrActionNode.Value;
									break;
								case "AckStatus":
									ret.IsSuccess = corrActionNode.Value == "Success";
									break;
								case "CORRACTIONS":
									foreach (var corrNode in corrActionNode.Descendants()) {
										if (corrNode.Name.LocalName == "CorrAction") {
											var corrAction = new CorrectiveAction ();
											foreach (var cNode in corrNode.Descendants()) {
												switch (cNode.Name.LocalName) {
												case "CORRACTIONNAME":

													corrAction.CorrActionName = cNode.Value;
													corrAction.QuestionId = 0;
													corrActionList.Add (corrAction);
													break;
												}
											}
										}
									}
									break;
								}
							}
						if (ret.IsSuccess ||
						                      (ret.ErrorCode.ToString () == HaccpConstant.AckCodeSuccess)) {
							dataStore.SaveCorrectiveAction (corrActionList);
						} else if (!string.IsNullOrEmpty (ret.Message)) {
							var serviceMessage =
								HACCPUtil.GetResourceString (
									Regex.Replace (ret.Message, "/[^a-zA-Z0-9]*/g", "").Replace (" ", ""));
							if (!string.IsNullOrEmpty (serviceMessage)) {
								ret.Message = serviceMessage;
							}
						}
					} catch (Exception ex) {
						ret.IsSuccess = false;
						ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
						Debug.WriteLine (
							"Ooops! Something went wrong downloading the corrective actions. Exception: {0}", ex);
					}
				}
			} else {
				ret.ErrorCode = HaccpConstant.AckCodeDeviceDisabled;
				ret.Message =
                    HACCPUtil.GetResourceString (
					"TheHACCPManagerapplicationhasbeentemporarilydisabledonthisdevicePleasecontacttheapplicationadministrator");
			}
			return ret;
		}


		/// <summary>
		///     Downloads the device settings.
		/// </summary>
		/// <returns>The device settings.</returns>
		public async Task<ServiceResponse> DownloadDeviceSettings ()
		{
			var ret = new ServiceResponse ();
			if (await CheckIsActiveDevice ()) {
				if (!string.IsNullOrEmpty (_endPointAddress)) {
					try {
						var methodUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.DeviceSettings,
							                                  HaccpAppSettings.SharedInstance.DeviceId,
							                                  HaccpAppSettings.SharedInstance.SiteSettings.SiteId.ToString (), string.Empty, null);
						var res = await BaseClient.GetAsync (string.Format (@"{0}{1}", _endPointAddress, methodUrl));
						res.EnsureSuccessStatusCode ();
						var deviceSettingsXml = await res.Content.ReadAsStringAsync ();

						var doc = XDocument.Parse (deviceSettingsXml);
						var deviceSettingsResponse = doc.Root;
						var deviceSettings = new DeviceSettings ();
						if (deviceSettingsResponse != null)
							foreach (var deviceSettingsNode in deviceSettingsResponse.Descendants()) {
								switch (deviceSettingsNode.Name.LocalName) {
								case "AckCode":
									ret.ErrorCode = Convert.ToInt32 (deviceSettingsNode.Value);
									break;
								case "AckDetailedInfo":
									ret.Message = deviceSettingsNode.Value;
									break;
								case "AckStatus":
									ret.IsSuccess = deviceSettingsNode.Value == "Success";
									break;
								case "AUTOADVANCE":
									if (deviceSettingsNode.Value != string.Empty)
										deviceSettings.AutoAdvance = bool.Parse (deviceSettingsNode.Value);
									break;
								case "DATEFORMAT":
									if (deviceSettingsNode.Value != string.Empty)
										deviceSettings.DateFormat = short.Parse (deviceSettingsNode.Value);
									break;
								case "REQUIRESPIN":
									if (deviceSettingsNode.Value != string.Empty)
										deviceSettings.RequirePin = short.Parse (deviceSettingsNode.Value);
									break;
								case "SKIPRECORDPREVIEW":
									if (deviceSettingsNode.Value != string.Empty)
										deviceSettings.SkipRecordPreview = bool.Parse (deviceSettingsNode.Value);
									break;

								case "TEMPERATURESCALE":
									if (deviceSettingsNode.Value != string.Empty)
										deviceSettings.TempScale = short.Parse (deviceSettingsNode.Value);
									break;
								case "TIMEFORMAT":
									if (deviceSettingsNode.Value != string.Empty)
										deviceSettings.TimeFormat = short.Parse (deviceSettingsNode.Value);
									break;

								case "AllowManualReadings":
									if (deviceSettingsNode.Value != string.Empty)
										deviceSettings.AllowManualTemp = short.Parse (deviceSettingsNode.Value);
									break;
								case "AllowTextMemo":
									if (deviceSettingsNode.Value != string.Empty)
										deviceSettings.AllowTextMemo = short.Parse (deviceSettingsNode.Value);
									break;
								case "LINE1":
									if (deviceSettingsNode.Value != string.Empty) {
										deviceSettings.Line1 = deviceSettingsNode.Value;
									}
									break;
								case "LINE2":
									if (deviceSettingsNode.Value != string.Empty) {
										deviceSettings.Line2 = deviceSettingsNode.Value;
									}
									break;
								}
							}
						if (ret.IsSuccess ||
						                      (ret.ErrorCode.ToString () == HaccpConstant.AckCodeSuccess)) {
							HaccpAppSettings.SharedInstance.DeviceSettings.AutoAdvance = deviceSettings.AutoAdvance;
							HaccpAppSettings.SharedInstance.DeviceSettings.DateFormat = deviceSettings.DateFormat;
							HaccpAppSettings.SharedInstance.DeviceSettings.RequirePin = deviceSettings.RequirePin;
							HaccpAppSettings.SharedInstance.DeviceSettings.SkipRecordPreview =
                                deviceSettings.SkipRecordPreview;
							HaccpAppSettings.SharedInstance.DeviceSettings.TempScale = deviceSettings.TempScale;
							HaccpAppSettings.SharedInstance.DeviceSettings.TimeFormat = deviceSettings.TimeFormat;
							HaccpAppSettings.SharedInstance.DeviceSettings.AllowManualTemp =
                                deviceSettings.AllowManualTemp;
							HaccpAppSettings.SharedInstance.DeviceSettings.AllowTextMemo = deviceSettings.AllowTextMemo;
							HaccpAppSettings.SharedInstance.DeviceSettings.Line1 = deviceSettings.Line1;
							HaccpAppSettings.SharedInstance.DeviceSettings.Line2 = deviceSettings.Line2;

							dataStore.SaveDeviceSettings (HaccpAppSettings.SharedInstance.DeviceSettings);
						} else if (!string.IsNullOrEmpty (ret.Message)) {
							var serviceMessage =
								HACCPUtil.GetResourceString (
									Regex.Replace (ret.Message, "/[^a-zA-Z0-9]*/g", "").Replace (" ", ""));
							if (!string.IsNullOrEmpty (serviceMessage)) {
								ret.Message = serviceMessage;
							}
						}
					} catch (Exception ex) {
						ret.IsSuccess = false;
						ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
						Debug.WriteLine ("Ooops! Something went wrong downloading the device details. Exception: {0}", ex);
					}
				}
			} else {
				ret.ErrorCode = HaccpConstant.AckCodeDeviceDisabled;
				ret.Message =
                    HACCPUtil.GetResourceString (
					"TheHACCPManagerapplicationhasbeentemporarilydisabledonthisdevicePleasecontacttheapplicationadministrator");
			}
			return ret;
		}

		/// <summary>
		///     Uploads the records checklists and temperature to server.
		/// </summary>
		/// <returns>The records.</returns>
		public async Task<ServiceResponse> UploadRecords (List<CheckListResponse> checklists,
		                                                       List<ItemTemperature> temperatures)
		{
			var ret = new ServiceResponse ();


			if (await CheckIsActiveDevice ()) {
				try {
					var serverEndPoint =
						HACCPUtil.BuildServerEndpoint (HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress,
							HaccpAppSettings.SharedInstance.SiteSettings.ServerPort,
							HaccpAppSettings.SharedInstance.SiteSettings.ServerDirectory);
					var deviceId = HaccpAppSettings.SharedInstance.DeviceId;
					var siteId = HaccpAppSettings.SharedInstance.SiteSettings.SiteId.ToString ();

					HaccpAppSettings.SharedInstance.IsUploadingProgress = true;

					int checklistCount, temperaturesCount;

					checklistCount = checklists != null ? checklists.Count : 0;
					temperaturesCount = temperatures != null ? temperatures.Count : 0;

					var totalcount = checklistCount + temperaturesCount;
					var progresscount = 0;


					if (checklistCount > 0 || temperaturesCount > 0) {
						//START -  OPEN BATCH PROCESS
						//Creating Batch Open URL
						var openBatchUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.OpenBatch, deviceId, siteId,
							                                     (checklistCount + temperaturesCount).ToString (), null);
						Debug.WriteLine ("openBatchUrl: " + openBatchUrl);
						var res = await BaseClient.GetAsync (string.Format (@"{0}{1}", serverEndPoint, openBatchUrl));
						res.EnsureSuccessStatusCode ();
						var batchXml = await res.Content.ReadAsStringAsync ();
						Debug.WriteLine ("batchXml; " + batchXml);
						var doc = XDocument.Parse (batchXml);
						var batchResponse = doc.Root;

						//bool isBatchOpen = false;
						long batchId = 0;

						//getting batch id and open status from xml
						if (batchResponse != null)
							foreach (var batchNode in batchResponse.Descendants()) {
								switch (batchNode.Name.LocalName.ToLower ()) {
								case "batchnumber":
									batchId = Convert.ToInt64 (batchNode.Value);
									break;
								}
							}
						//END -  OPEN BATCH PROCESS


						Debug.WriteLine ("batchId; " + batchId);
						Debug.WriteLine ("checklistCount: " + checklistCount);
						Debug.WriteLine ("temperaturesCount: " + temperaturesCount);
						Debug.WriteLine ("totalcount: " + checklistCount + temperaturesCount);

						if (batchId > 0) {
							var uploadedCount = 0;


							//START - UPLOADING CHECKLISTS PROCESS
							string uploadRecordsUrl;
							for (var i = 0; i < checklists.Count; i++) {
								checklists [i].RecordNo = i + 1;
								uploadRecordsUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.UploadChecklists,
									deviceId, siteId, batchId.ToString (), checklists [i]);
								Debug.WriteLine ("uploadRecordsUrl(checklist): " + uploadRecordsUrl);
								//Post data
								var resChecklists = await BaseClient.GetAsync (serverEndPoint + uploadRecordsUrl);

								if (resChecklists.IsSuccessStatusCode) {
									uploadedCount += 1;
									Debug.WriteLine ("checklistsuccessCount: " + uploadedCount);
								}
								progresscount++;
								MessagingCenter.Send (new ProgressMessage (progresscount, totalcount),
									HaccpConstant.UploadrecordMessage);
							}

							var uploadChecklistSuccess = checklists.Count > 0 && uploadedCount == checklists.Count;
							Debug.WriteLine ("uploadChecklistSuccess: " + uploadChecklistSuccess);
							uploadedCount = 0;
							//END - UPLOADING CHECKLISTS PROCESS


							//START - UPLOADING TEMPERATURES PROCESS
							for (var i = 0; i < temperatures.Count; i++) {
								temperatures [i].RecordNo = i + 1;
								uploadRecordsUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.UploadTemperatures,
									deviceId, siteId, batchId.ToString (), temperatures [i]);
								Debug.WriteLine ("uploadRecordsUrl(temp): " + uploadRecordsUrl);

								var resTemperatures = await BaseClient.GetAsync (serverEndPoint + uploadRecordsUrl);

								if (resTemperatures.IsSuccessStatusCode) {
									uploadedCount += 1;
									Debug.WriteLine ("tempsuccessCount: " + uploadedCount);
								}

								progresscount++;
								MessagingCenter.Send (new ProgressMessage (progresscount, totalcount),
									HaccpConstant.UploadrecordMessage);
							}

							var uploadTemperatureSuccess = temperatures.Count > 0 && uploadedCount == temperatures.Count;
							Debug.WriteLine ("uploadTemperatureSuccess: " + uploadTemperatureSuccess);
							//END - UPLOADING TEMPERATURES PROCESS


							if (uploadChecklistSuccess || uploadTemperatureSuccess) {
								//START - CLOSE BATCH PROCESS
								var closeBatchUrl = HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.CloseBatch, deviceId,
									                                            siteId, (checklistCount + temperaturesCount).ToString (), batchId);
								var resCloseBatch =
									await BaseClient.GetAsync (string.Format (@"{0}{1}", serverEndPoint, closeBatchUrl));
								Debug.WriteLine ("closeBatchUrl: " + closeBatchUrl);

								ret.IsSuccess = resCloseBatch.IsSuccessStatusCode;
								Debug.WriteLine ("closebatchsuccess: " + ret.IsSuccess);
								//END - CLOSE BATCH PROCESS

								HaccpAppSettings.SharedInstance.SiteSettings.LastUploaded = DateTime.Now.ToString ("F",
									CultureInfo.CurrentCulture);
								dataStore.SaveSiteSettings (HaccpAppSettings.SharedInstance.SiteSettings);

								//if (ret.IsSuccess) {
								//START - CLEAR TABLES
								Debug.WriteLine ("uploadChecklistSuccess: " + uploadChecklistSuccess);
								if (uploadChecklistSuccess && ret.IsSuccess) {
									dataStore.ClearCheckList (true);
								} else {
									if (checklistCount > 0) {
										ret.IsSuccess = false;
										ret.Message =
                                            HACCPUtil.GetResourceString (
											"UploadedallthetemperaturerecordstoHACCPEnterpriseapplicationsuccessfullybutunabletouploadchecklistrecords");
									}
								}

								if (uploadTemperatureSuccess && ret.IsSuccess) {
									dataStore.ClearTemperatures (true);
								} else {
									if (temperaturesCount > 0) {
										ret.IsSuccess = false;
										ret.Message =
                                            HACCPUtil.GetResourceString (
											"UploadedallthechecklistrecordstoHACCPEnterpriseapplicationsuccessfullybutunabletouploadtemperaturerecords");
									}
								}
								//END - CLEAR TABLES
								//}
							}
						}
					}
				} catch (Exception ex) {
					ret.IsSuccess = false;
					ret.Message = HACCPUtil.GetResourceString ("AnunexpectederrorhasoccurredWewilllookintoitsoon");
					Debug.WriteLine ("Ooops! Something went wrong when uploading records. Exception: {0}", ex);
				}
			} else {
				ret.ErrorCode = HaccpConstant.AckCodeDeviceDisabled;
				ret.Message =
                    HACCPUtil.GetResourceString (
					"TheHACCPManagerapplicationhasbeentemporarilydisabledonthisdevicePleasecontacttheapplicationadministrator");
			}

			HaccpAppSettings.SharedInstance.IsUploadingProgress = false;
			return ret;
		}


		/// <summary>
		///     Saves the language strings.
		/// </summary>
		/// <returns>The language strings.</returns>
		public async Task<ServiceResponse> SaveLanguageStrings (long languageId)
		{
			var ret = new ServiceResponse ();
			//	if (endPointAddress != null && endPointAddress.Length > 0) {
			try {
				//	string	methodUrlLng =HACCPUtil.BuildServiceMethodUrl (WebServiceEnum.LanguageDictionary, HACCPAppSettings.SharedInstance.DeviceID, string.Empty, HACCPAppSettings.SharedInstance.LanguageId, null);
				var resLng =
					await BaseClient.GetAsync (string.Format (HaccpConstant.GetLanguagedictionaryServiceUrl, languageId));
				resLng.EnsureSuccessStatusCode ();
				var resourceXmlOth = await resLng.Content.ReadAsStringAsync ();
				await DependencyService.Get<IResourceFileHelper> ().SaveResource ("ResourceFile.xml", resourceXmlOth);
				ret.IsSuccess = true;
			} catch (Exception ex) {
				ret.IsSuccess = false;

				Debug.WriteLine ("Ooops! Something went wrong when saving language strings  . Exception: {0}", ex);
			}
			return ret;
			//	}
		}

		#endregion
	}
}