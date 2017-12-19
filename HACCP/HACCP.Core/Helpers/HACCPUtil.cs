using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Resources;
using System.Text;
using System.Xml.Linq;
using HACCP.Core.WP.ResourceFiles;
using Xamarin.Forms;

namespace HACCP.Core
{
    public static class HACCPUtil
    {
        #region Member Variables

        private static Dictionary<double, double> _slopes;

        #endregion

        #region Methods

        /// <summary>
        ///     Builds the server endpoint.
        /// </summary>
        /// <returns>The server endpoint.</returns>
        /// <param name="serverAddress">Server address.</param>
        /// <param name="port">Port.</param>
        /// <param name="serverDirectory">Server directory.</param>
        public static string BuildServerEndpoint(string serverAddress, string port, string serverDirectory)
        {
            var ret = new StringBuilder();

            if (serverAddress != null && serverAddress.Trim().Length > 0)
            {
                serverAddress = serverAddress.ToLower();
                if (serverAddress.StartsWith("http") == false)
                    ret.Append("http://");
                ret.Append(serverAddress.Trim('/', '\\', ' '));
                if (port != null && port.Trim().Length > 0)
                {
                    var portNum = ConvertToInt32NulltoZero(port.Trim('/', '\\', ' ', ':'));
                    if (portNum > 0 && portNum != 80)
                        ret.AppendFormat(":{0}", portNum);
                }
                if (serverDirectory != null && serverDirectory.Trim().Length > 0)
                    ret.AppendFormat("/{0}", serverDirectory.Trim('/', '\\', ' ', ':'));

                ret.Append("/processrequest.svc");
            }

            return ret.ToString();
        }

        /// <summary>
        ///     Converts to int32 nullto zero.
        /// </summary>
        /// <returns>The to int32 nullto zero.</returns>
        /// <param name="obj">Object.</param>
        public static int ConvertToInt32NulltoZero(object obj)
        {
            var ret = 0;
            if (obj != null)
            {
                try
                {
                    ret = Convert.ToInt32(obj);
                }
                catch
                {
                    // ignored
                }
            }
            return ret;
        }

        /// <summary>
        ///     Converts the celsius to fahrenheit.
        /// </summary>
        /// <returns>The celsius to fahrenheit.</returns>
        /// <param name="c">C.</param>
        public static double ConvertCelsiusToFahrenheit(double c)
        {
            return 9.0/5.0*c + 32;
        }

        /// <summary>
        ///     Converts the fahrenheit to celsius.
        /// </summary>
        /// <returns>The fahrenheit to celsius.</returns>
        /// <param name="f">F.</param>
        public static double ConvertFahrenheitToCelsius(double f)
        {
            return 5.0/9.0*(f - 32);
        }

        /// <summary>
        /// TruncateResourceString
        /// </summary>
        /// <param name="status"></param>
        /// <param name="isLarge"></param>
        /// <returns></returns>
        public static string TruncateResourceString(string status, bool isLarge)
        {
            var truncatedString = status;
            int length;
            if (isLarge)
            {
                length = Device.Idiom == TargetIdiom.Tablet ? 50 : 42;
            }
            else
            {
                length = Device.Idiom == TargetIdiom.Tablet ? 36 : 23;
            }
            if (status.Length > length)
                truncatedString = string.Format("{0}...", status.Substring(0, length));
            return truncatedString;
        }


        /// <summary>
        ///     Builds the service method URL.
        /// </summary>
        /// <returns>The service method URL.</returns>
        /// <param name="method">Method.</param>
        /// <param name="deviceId">Device identifier.</param>
        /// <param name="siteId">Site identifier.</param>
        /// <param name="additionalParam">Additional parameter.</param>
        /// <param name="recordToUpload">Record to upload.</param>
        public static string BuildServiceMethodUrl(WebServiceEnum method, string deviceId, string siteId,
            string additionalParam, object recordToUpload)
        {
            var methodUrl = new StringBuilder();
            //deviceId =Regex.Replace(deviceId, "[-]+", ""); 
            switch (method)
            {
                case WebServiceEnum.ActiveSiteDetails:
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}", WebServiceEnum.ActiveSiteDetails,
                        deviceId);
                    break;
                case WebServiceEnum.DeviceSettings:
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&siteId={2}",
                        WebServiceEnum.DeviceSettings, deviceId, siteId);
                    break;
                case WebServiceEnum.HandheldUsers:
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&siteId={2}",
                        WebServiceEnum.HandheldUsers, deviceId, siteId);
                    break;
                case WebServiceEnum.ListMenuName:
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&siteId={2}",
                        WebServiceEnum.ListMenuName, deviceId, siteId);
                    break;
                case WebServiceEnum.MenuDetails:
                {
                    var menuId = additionalParam;
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&menuID={2}&siteId={3}",
                        WebServiceEnum.MenuDetails, deviceId, menuId, siteId);
                    break;
                }
                case WebServiceEnum.CorrActions:
                {
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&siteId={2}",
                        WebServiceEnum.CorrActions, deviceId, siteId);
                    break;
                }
                case WebServiceEnum.ListCheckListNames:
                {
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&siteId={2}",
                        WebServiceEnum.ListCheckListNames, deviceId, siteId);
                    break;
                }
                case WebServiceEnum.ChecklistDetails:
                {
                    var checklistId = additionalParam;
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&checklistID={2}&siteId={3}",
                        WebServiceEnum.ChecklistDetails, deviceId, checklistId, siteId);
                    break;
                }
                case WebServiceEnum.OpenBatch:
                {
                    var recCount = additionalParam;
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&RecCount={2}&siteId={3}",
                        WebServiceEnum.OpenBatch, deviceId, recCount, siteId);
                    break;
                }
                case WebServiceEnum.CloseBatch:
                {
                    var batchId = recordToUpload.ToString();
                    var recordCount = additionalParam;
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&BatchID={2}&RecCount={3}&siteId={4}",
                        WebServiceEnum.CloseBatch, deviceId, batchId, recordCount, siteId);
                    break;
                }
                case WebServiceEnum.UploadChecklists:
                {
                    var checklistResponse = (CheckListResponse) recordToUpload;
                    var batchId = additionalParam;
                    methodUrl = BuildUploadCheckListRecordServiceMethodUrl(checklistResponse, batchId);
                    break;
                }
                case WebServiceEnum.UploadTemperatures:
                {
                    var itemTemp = (ItemTemperature) recordToUpload;
                    var batchId = additionalParam;
                    methodUrl = BuildUploadUploadTemperaturesServiceMethodUrl(itemTemp, batchId);
                    break;
                }
                case WebServiceEnum.LanguageList:
                {
                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}", WebServiceEnum.LanguageList,
                        deviceId);
                    break;
                }
                case WebServiceEnum.LanguageDictionary:
                {
                    var languageId = additionalParam;
                    if (languageId == "0")
                        languageId = "1";

                    methodUrl.AppendFormat("/HACCPManagerService/{0}?DeviceID={1}&LanguageID={2}",
                        WebServiceEnum.LanguageDictionary, deviceId, languageId);
                    break;
                }
            }

            return methodUrl.ToString();
        }


        /// <summary>
        ///     Builds the upload check list record service method URL.
        /// </summary>
        /// <returns>The upload check list record service method URL.</returns>
        /// <param name="checklistResponse">Checklist response.</param>
        /// <param name="batchId"></param>
        private static StringBuilder BuildUploadCheckListRecordServiceMethodUrl(CheckListResponse checklistResponse,
            string batchId)
        {
            var checkListUploadUrl = new StringBuilder();


            var questionType = GetString(checklistResponse.QuestionType) == ((short) QuestionType.YesOrNo).ToString()
                ? "y"
                : "n";
            var answer = checklistResponse.Answer;

            if (checklistResponse.QuestionType == ((short) QuestionType.NumericAnswer).ToString())
            {
                answer =
                    ((string.IsNullOrEmpty(checklistResponse.Answer) ? 0 : ConvertToDouble(checklistResponse.Answer))*10)
                        .ToString();
            }
            checkListUploadUrl.AppendFormat(
                /*"{record:\"{0}\", catname:\"{1}\", question:\"{2}\", month:\"{3}\", day:\"{4}\", year:\"{5}\", hour:\"{6}\", minute:\"{7}\", " +
				"sec:\"{8}\", username:\"{9}\", questionType:\"{10}\", min:\"{11}\", max:\"{12}\", corraction:\"{13}\", answer:\"{14}\", " +
				"device:\"{15}\", batch:\"{16}\", siteId:\"{17}\", checklistid:\"{18}\", isna:\"{19}\", tzID:\"{20}\"}",*/

                "/HACCPManagerService/UploadChecklists?record={0}&catname={1}&question={2}&month={3}&day={4}&year={5}&hour={6}&minute={7}&" +
                "sec={8}&username={9}&questionType={10}&min={11}&max={12}&corraction={13}&answer={14}&" +
                "device={15}&batch={16}&siteId={17}&checklistid={18}&isna={19}&tzID={20}",
                GetString(checklistResponse.RecordNo), GetString(checklistResponse.Catname),
                GetString(checklistResponse.Question),
                GetString(checklistResponse.Month), GetString(checklistResponse.Day), GetString(checklistResponse.Year),
                GetString(checklistResponse.Hour), GetString(checklistResponse.Minute), GetString(checklistResponse.Sec),
                GetString(checklistResponse.UserName), questionType,
                questionType == "y" ? "" : GetString(checklistResponse.Min),
                questionType == "y" ? "" : GetString(checklistResponse.Max), GetString(checklistResponse.CorrAction),
                GetString(answer),
                GetString(checklistResponse.DeviceId), batchId, GetString(checklistResponse.SiteId),
                GetString(checklistResponse.ChecklistId),
                GetString(checklistResponse.IsNa), GetString(checklistResponse.Tzid));

            return checkListUploadUrl;
        }

        /// <summary>
        ///     Builds the upload upload temperatures service method URL.
        /// </summary>
        /// <returns>The upload upload temperatures service method URL.</returns>
        /// <param name="itemTemperature">Item temperature.</param>
        /// <param name="batchId"></param>
        private static StringBuilder BuildUploadUploadTemperaturesServiceMethodUrl(ItemTemperature itemTemperature,
            string batchId)
        {
            var temperatureUploadUrl = new StringBuilder();

            var min = GetString(itemTemperature.Min);
            min = min == "0" ? "" : min;

            var max = GetString(itemTemperature.Max);
            max = max == "0" ? "" : max;

            var temperature =
                ((string.IsNullOrEmpty(itemTemperature.Temperature) ? 0 : ConvertToDouble(itemTemperature.Temperature))*
                 10).ToString();


            temperatureUploadUrl.AppendFormat(
                "/HACCPManagerService/UploadTemperatures?record={0}&item={1}&temp={2}&month={3}&day={4}&year={5}&hour={6}&minute={7}&" +
                "sec={8}&user={9}&cact={10}&min={11}&max={12}&loc={13}&ccp={14}&device={15}&batch={16}&siteId={17}&itemid={18}&" +
                "locationid={19}&ccpid={20}&menuid={21}&isna={22}&tzID={23}&isManual={24}&Blue2ID={25}&Note={26}",
                /*"record:\"{0}\", item:\"{1}\", temp:\"{2}\", month:\"{3}\", day:\"{4}\", year:\"{5}\", hour:\"{6}\", minute:\"{7}\", " +
				"sec:\"{8}\", username:\"{9}\", cact:\"{10}\", min:\"{11}\", max:\"{12}\", loc:\"{13}\", ccp:\"{14}\", device:\"{15}\", " +
				"batch:\"{16}\", siteId:\"{17}\", itemid:\"{18}\", locationid:\"{19}\", ccpid:\"{20}\", menuid:\"{21}\", isna:\"{22}\", " +
				"tzID:\"{23}\", manual:\"{24}\"", */
                GetString(itemTemperature.RecordNo), GetString(itemTemperature.ItemName), temperature,
                GetString(itemTemperature.Month), GetString(itemTemperature.Day), GetString(itemTemperature.Year),
                GetString(itemTemperature.Hour), GetString(itemTemperature.Minute), GetString(itemTemperature.Sec),
                GetString(itemTemperature.UserName), GetString(itemTemperature.CorrAction), min,
                max, GetString(itemTemperature.LocName), GetString(itemTemperature.Ccp),
                GetString(itemTemperature.DeviceId), batchId, GetString(itemTemperature.SiteID),
                GetString(itemTemperature.ItemID),
                GetString(itemTemperature.LocationID), GetString(itemTemperature.CCPID),
                GetString(itemTemperature.MenuID),
                GetString(itemTemperature.IsNA), GetString(itemTemperature.TZID),
                GetString(itemTemperature.IsManualEntry),
                GetString(itemTemperature.Blue2ID), GetString(itemTemperature.Note)
                );

            return temperatureUploadUrl;
        }

        /// <summary>
        ///     Gets URL encoded string
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="value">Value.</param>
        private static string GetString(object value)
        {
            if (value != null)
            {
                return WebUtility.UrlEncode(value.ToString().Trim());
            }
            return string.Empty;
        }


        /// <summary>
        /// Reverse
        /// </summary>
        /// <param name="stringToReverse"></param>
        /// <returns></returns>
        public static string Reverse(this string stringToReverse)
        {
            var charArray = stringToReverse.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


        /// <summary>
        /// Get Formatte dDate
        /// </summary>
        /// <param name="date"></param>
        /// <param name="multiline"></param>
        /// <returns></returns>
        public static string GetFormattedDate(DateTime date, bool multiline)
        {
            var formattedDate = string.Empty;

            if (HaccpAppSettings.SharedInstance.SiteSettings != null)
            {
                var timeformat = (TimeFormat) HaccpAppSettings.SharedInstance.DeviceSettings.TimeFormat ==
                                 TimeFormat.TwentyFourHour
                    ? "HH:mm"
                    : "hh:mm tt";


                if (HaccpAppSettings.SharedInstance.DeviceSettings != null && date != DateTime.MinValue)
                {
                    var dateFormat = (DateFormat) HaccpAppSettings.SharedInstance.DeviceSettings.DateFormat;

                    if (dateFormat == DateFormat.dd_MMMM_yy)
                    {
                        formattedDate = date.ToString("dd-MMMM-yy" + (multiline ? "\n" : " ") + timeformat,
                            CultureInfo.CurrentCulture);
                    }
                    else if (dateFormat == DateFormat.M_d_yyyy)
                    {
                        formattedDate = date.ToString("M/d/yyyy" + " " + timeformat, CultureInfo.CurrentCulture);
                    }
                    else if (dateFormat == DateFormat.yyyy_MM_dd)
                    {
                        formattedDate = date.ToString("yyyy-MM-dd" + " " + timeformat, CultureInfo.CurrentCulture);
                    }
                    else if (dateFormat == DateFormat.yy_MM_dd)
                    {
                        formattedDate = date.ToString("yy/MM/dd" + " " + timeformat, CultureInfo.CurrentCulture);
                    }
                }
            }


            return formattedDate;
        }

        /// <summary>
        ///     Gets the formatted date based on the site settings.
        /// </summary>
        /// <returns>The formatted date.</returns>
        /// <param name="date">Date.</param>
        public static string GetFormattedDate(string date)
        {
            var formattedDate = string.Empty;
            try
            {
                if (HaccpAppSettings.SharedInstance.SiteSettings != null && !string.IsNullOrEmpty(date))
                {
                    var timeformat = (TimeFormat) HaccpAppSettings.SharedInstance.DeviceSettings.TimeFormat ==
                                     TimeFormat.TwentyFourHour
                        ? "HH:mm"
                        : "hh:mm tt";

                    var _date = Convert.ToDateTime(date, CultureInfo.CurrentCulture);
                    if (HaccpAppSettings.SharedInstance.DeviceSettings != null && _date != DateTime.MinValue)
                    {
                        var dateFormat = (DateFormat) HaccpAppSettings.SharedInstance.DeviceSettings.DateFormat;

                        if (dateFormat == DateFormat.dd_MMMM_yy)
                        {
                            formattedDate = _date.ToString("dd-MMMM-yy" + " " + timeformat, CultureInfo.CurrentCulture);
                        }
                        else if (dateFormat == DateFormat.M_d_yyyy)
                        {
                            formattedDate = _date.ToString("M/d/yyyy" + " " + timeformat, CultureInfo.CurrentCulture);
                        }
                        else if (dateFormat == DateFormat.yyyy_MM_dd)
                        {
                            formattedDate = _date.ToString("yyyy-MM-dd" + " " + timeformat, CultureInfo.CurrentCulture);
                        }
                        else if (dateFormat == DateFormat.yy_MM_dd)
                        {
                            formattedDate = _date.ToString("yy/MM/dd" + " " + timeformat, CultureInfo.CurrentCulture);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return formattedDate;
        }

        /// <summary>
        ///     Gets the bytes.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="str">String.</param>
        public static byte[] GetBytesFromString(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        ///     Gets the string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="bytes">Bytes.</param>
        public static string GetStringFromBytes(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// GetServiceMessageFromResourceFile
        /// </summary>
        /// <param name="errorcode"></param>
        /// <returns></returns>
        public static string GetServiceMessageFromResourceFile(string errorcode)
        {
            var resxManager = new ResourceManager(typeof(AppResources));

            return resxManager.GetString(string.Format("{0}_{1}", HaccpConstant.ServiceResponseAckcode, errorcode),
                AppResources.Culture);
        }

        /// <summary>
        ///     Converts to double.
        /// </summary>
        /// <returns>The to double.</returns>
        /// <param name="val">Value.</param>
        public static double ConvertToDouble(string val)
        {
            var decimalChar = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            val = val.Replace(".", decimalChar);
            val = val.Replace(",", decimalChar);
            return Convert.ToDouble(val);
        }


        /// <summary>
        ///     Pres the calculate slopes.
        /// </summary>
        public static void PreCalculateSlopes()
        {
            _slopes = new Dictionary<double, double>
            {
                {2.8, -84},
                {2.7, -81},
                {2.6, -78},
                {2.5, -75},
                {2.4, -72},
                {2.3, -69},
                {2.2, -66},
                {2.1, -63},
                {2, -60},
                {1.9, -57},
                {1.8, -54},
                {1.7, -51},
                {1.6, -48},
                {1.5, -45},
                {1.4, -42},
                {1.3, -39},
                {1.2, -36},
                {1.1, -33},
                {1, -30},
                {0.9, -27},
                {0.8, -24},
                {0.7, -21},
                {0.6, -18},
                {0.5, -15},
                {0.4, -12},
                {0.3, -9},
                {0.2, -6},
                {0.1, -3},
                {0, 0},
                {-2.8, 84},
                {-2.7, 81},
                {-2.6, 78},
                {-2.5, 75},
                {-2.4, 72},
                {-2.3, 69},
                {-2.2, 66},
                {-2.1, 63},
                {-2, 60},
                {-1.9, 57},
                {-1.8, 54},
                {-1.7, 51},
                {-1.6, 48},
                {-1.5, 45},
                {-1.4, 42},
                {-1.3, 39},
                {-1.2, 36},
                {-1.1, 33},
                {-1, 30},
                {-0.9, 27},
                {-0.8, 24},
                {-0.7, 21},
                {-0.6, 18},
                {-0.5, 15},
                {-0.4, 12},
                {-0.3, 9},
                {-0.2, 6},
                {-0.1, 3}
            };
//			var val = -84;
//			for (var i = -2.8; i <= 2.8; i += 0.1) {
//				slopes.Add (i, val);
//				val += 3;
//			}


        }


        /// <summary>
        ///     Gets the slope.
        /// </summary>
        /// <returns>The slope.</returns>
        /// <param name="delta">Delta.</param>
        public static double GetSlope(double delta)
        {
            //if(slopes.ContainsKey(delta))
            return _slopes.ContainsKey(delta) ? _slopes[delta] : 0;
        }

    
      /// <summary>
        /// GetResourceString
      /// </summary>
      /// <param name="keyname"></param>
      /// <returns></returns>
        public static string GetResourceString(string keyname)
        {
            var selectedString = AppResources.ResourceManager.GetString(keyname);

            try
            {
                if (HaccpAppSettings.SharedInstance.ResourceString != string.Empty)
                {
                    var resultXml = HaccpAppSettings.SharedInstance.ResourceString;

                    var doc = XDocument.Parse(resultXml);
                    var resourceResponse = doc.Root;

                    if (resourceResponse != null)
                        foreach (var resourceNode in resourceResponse.Descendants())
                        {
                            switch (resourceNode.Name.LocalName)
                            {
                                case "Text":
                                {
                                    foreach (var resourceattribute in resourceNode.Attributes())
                                    {
                                        switch (resourceattribute.Name.LocalName)
                                        {
                                            case "name":
                                                var resourceKey = resourceattribute.Value;
                                                if (resourceKey == keyname)
                                                {
                                                    foreach (var valueNode in resourceNode.Descendants())
                                                    {
                                                        //												if (valueNode.Name.LocalName == "Baseword") {
                                                        //													selectedString = valueNode.Value;
                                                        //												}
                                                        if (valueNode.Name.LocalName == "Transword")
                                                        {
                                                            //if (valueNode.Value != "xxxxx") {
                                                            selectedString = valueNode.Value;
                                                            //}
                                                        }
                                                    }

                                                    return selectedString;
                                                }
                                                break;
                                        }
                                    }
                                }
                                    break;
                            }
                        }
                }
            }
            catch
            {
                selectedString = AppResources.ResourceManager.GetString(keyname);
            }

            return selectedString;
        }

        #endregion
    }
}