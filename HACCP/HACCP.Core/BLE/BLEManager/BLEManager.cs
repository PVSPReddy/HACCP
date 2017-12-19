//iOS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
	public class BLEManager
	{

		#region Member Variables

		public IAdapter Adapter;
		public IDevice SelectedDevice;

		private static volatile BLEManager _instance;
		private ICharacteristic _autooffIntervalChar;
		private IService _batteryService;
		private bool _batteryServieOn;
		private IService _deviceInfoService;
		private bool _deviceServieOn;
		private ICharacteristic _disConnectPeripheralChar;
		private ICharacteristic _measurementScaleChar;
		private ICharacteristic _measurementTimeChar;
		private ICharacteristic _probeNameChar;
		private ICharacteristic _resetAutoOffChar;
		private List<IService> _services;
		private ICharacteristic _sleepTimeChar;
		private IService _temperatureService;
		private bool _temperatureServieOn;
		public bool StopTemperatureReading;

		#endregion

		/// <summary>
		/// BLEManager Constructor
		/// </summary>
		private BLEManager ()
		{
		}

		#region Properties

		public static BLEManager SharedInstance {
			get { return _instance ?? (_instance = new BLEManager ()); }
		}

		public List<IDevice> Devices { get; set; }

		public bool IsConnected { get; set; }

		public double LastReading { get; set; }

		public TemperatureUnit LastUnit { get; set; }

		public string ReadingSlope { get; set; }

		public bool IsSleeping { get; set; }

		public bool IsHigh { get; set; }

		public bool IsLow { get; set; }

		public bool IsBatteryLow { get; set; }

		public bool ReadingBlue2Data { get; set; }

		public BleSettings Settings { get; set; }

		public bool IsBLERefrehing { get; set; }

		public bool GotTemperartureReading { get; set; }

		public bool ScanTimeOutElapsed { get; set; }

		#endregion

		#region Methods

		/// <summary>
		///     Searchs the blue2 devices.
		/// </summary>
		public void SearchBlue2Devices ()
		{
			if (Device.OS == TargetPlatform.Android) {
				if (Adapter.IsBluetoothEnabled) {
					Devices = new List<IDevice> ();
					ScanTimeOutElapsed = false;

					Adapter.DeviceDiscovered += DeviceDiscovered;
					Adapter.ScanTimeoutElapsed += ScanTimeoutElapsed;
					Adapter.DeviceConnected += DeviceConnected;
					Adapter.DeviceDisconnected += DeviceDisConnected;
					Adapter.ConnectTimeoutElapsed += ConnectTimeoutElapsed;
					Adapter.BluetoothStateChanged += BluetooothStateChanged;
					StartScanning ();
				} else {
					ScanTimeOutElapsed = true;
				}
			} else {
				Devices = new List<IDevice> ();
				ScanTimeOutElapsed = false;
				Adapter.DeviceDiscovered += DeviceDiscovered;
				Adapter.ScanTimeoutElapsed += ScanTimeoutElapsed;
				Adapter.DeviceConnected += DeviceConnected;
				Adapter.DeviceDisconnected += DeviceDisConnected;
				Adapter.ConnectTimeoutElapsed += ConnectTimeoutElapsed;
				Adapter.BluetoothStateChanged += BluetooothStateChanged;
				StartScanning ();
			}
			ReadingBlue2Data = false;
			GotTemperartureReading = false;
		}

		/// <summary>
		///     Bluetoooths the state changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void BluetooothStateChanged (object sender, EventArgs e)
		{
			if (!Adapter.IsBluetoothEnabled) {
				ScanTimeOutElapsed = true;
				IsSleeping = IsLow = IsHigh = IsBatteryLow = false;
				IsConnected = false;
				ReadingBlue2Data = false;
				GotTemperartureReading = false;
				UnRegisterEvents ();
				await StopScanning ();
				MessagingCenter.Send (new BleConnectionStatusMessage (), HaccpConstant.BleconnectionStatus);
			} else {
				if (Adapter.ConnectedDevices != null)
					Adapter.ConnectedDevices.Clear ();
			}
		}

		/// <summary>
		///     Starts the scanning.
		/// </summary>
		public void StartScanning ()
		{
			if (Adapter.IsScanning) {
				Adapter.StopScanningForDevices ();
				Debug.WriteLine ("adapter.StopScanningForDevices()");
			} else {
				Devices.Clear ();
				Adapter.StartScanningForDevices ();
				Debug.WriteLine ("adapter.StartScanningForDevices()");
			}

			GotTemperartureReading = false;
		}

		/// <summary>
		///     Stops the scanning.
		/// </summary>
		/// <returns>The scanning.</returns>
		public async Task StopScanning ()
		{
			//			new Task (() => {
			if (Adapter.IsScanning) {
				Debug.WriteLine ("Still scanning, stopping the scan");
				Adapter.StopScanningForDevices ();
			}
			ScanTimeOutElapsed = true;
			//			}).Start ();
		}

		/// <summary>
		///     Connects to wand.
		/// </summary>
		/// <param name="device">Device.</param>
		public void ConnectToWand (IDevice device)
		{
			SelectedDevice = device;
			Adapter.ConnectToDevice (SelectedDevice);
		}

		/// <summary>
		///     Dises the connect from wand.
		/// </summary>
		public void DisConnectFromWand ()
		{
			try {
				if (_disConnectPeripheralChar != null) {
					_disConnectPeripheralChar.Write (new byte[] { Convert.ToByte (0) });
				}
			} catch (Exception ex) {
				Debug.WriteLine ("Error on DisConnectFromWand : {0}", ex.Message);
			}
		}

		/// <summary>
		///     Updates the probe.
		/// </summary>
		public void UpdateProbe ()
		{
		}

		/// <summary>
		///     Updates the BLE settings.
		/// </summary>
		public async void UpdateBLESettings (BLEChar blechar)
		{
			try {
				if (Settings != null && _temperatureService != null && _measurementScaleChar != null &&
				    _measurementTimeChar != null && _autooffIntervalChar != null && _sleepTimeChar != null) {
					switch (blechar) {
					case BLEChar.Scale:
						_measurementScaleChar.Write (Settings.Scale == 0 ? new byte[] { 0x43 } : new byte[] { 0x46 });
						break;
					case BLEChar.Time:
						StopTemperatureReading=true;
						_measurementTimeChar.Write (new[] { Convert.ToByte (Settings.MeasurementLevel) });
						break;
					case BLEChar.AutoOff:
						_autooffIntervalChar.Write (new[] { Convert.ToByte (Settings.AutoOff) });
						break;
					case BLEChar.Sleep:
						_sleepTimeChar.Write (new[] { Convert.ToByte (Settings.Sleep) });
						break;
					case BLEChar.Prob:
						var bytes = HACCPUtil.GetBytesFromString (Settings.Prob);
						_probeNameChar.Write (bytes);
						break;
					}
				}
			} catch (Exception) {
				Debug.WriteLine ("Error on updating BLE settings");
			}
		}

		/// <summary>
		///     Resets the blue2 auto off.
		/// </summary>
		public void ResetBlue2AutoOff ()
		{
			if (_resetAutoOffChar != null)
				_resetAutoOffChar.Write (new byte[] { Convert.ToByte (0) });
			IsSleeping = false;
		}

		/// <summary>
		///     Devices  discoveared.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void DeviceDiscovered (object sender, DeviceDiscoveredEventArgs e)
		{
			if (Devices == null)
				Devices = new List<IDevice> ();


			if (e.Device != null && !string.IsNullOrEmpty (e.Device.Name) &&
			    e.Device.Name.ToLower () == HaccpConstant.Blue2DeviceName) {
				{
					Devices.Add (e.Device);

					if (IsConnected || Devices.Count != 1)
						return;
					if (Device.OS == TargetPlatform.Android) {
						Device.BeginInvokeOnMainThread (() => {
							ConnectToWand (e.Device);
						});
					} else {
						ConnectToWand (e.Device);
					}
				}
			}
		}

		/// <summary>
		///     Scans the timeout elapsed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void ScanTimeoutElapsed (object sender, EventArgs e)
		{
			if (!IsConnected) {
				MessagingCenter.Send (new BleScanCompleteMessage (Devices), HaccpConstant.Blue2ScanComplete);
			}
			ScanTimeOutElapsed = true;
		}

		/// <summary>
		///     Devices connected.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void DeviceConnected (object sender, DeviceConnectionEventArgs e)
		{
			SelectedDevice = e.Device;

			IsConnected = true;
			ReadingBlue2Data = true;
			GotTemperartureReading = false;
			Settings = new BleSettings ();
			SelectedDevice.ServicesDiscovered += ServicesDiscovered;
			SelectedDevice.DiscoverServices ();
			MessagingCenter.Send (new BleConnectionStatusMessage (), HaccpConstant.BleconnectionStatus);
			ScanTimeOutElapsed = true;
			await StopScanning ();
		}


		/// <summary>
		///     Connection timeout elapsed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void ConnectTimeoutElapsed (object sender, EventArgs e)
		{
			//IsBusy = false;
			//page.DismissPopup ();
			//MessagingCenter.Send<BLEConnectionTimeOutMessage> (new BLEConnectionTimeOutMessage (), HACCPConstant.BLECONNECTION_TIMEOUT);
			if (Adapter.ConnectedDevices != null)
				Adapter.ConnectedDevices.Clear ();
		}

		/// <summary>
		///     Resets the connection.
		/// </summary>
		public async void ResetConnection ()
		{
			try {
				if (IsConnected)
                {
					DisConnectFromWand ();
				}
                else if (!ScanTimeOutElapsed)
                {
					Adapter.CancelScanning ();
					DisConnectFromWand ();
					UnRegisterEvents ();
					await StopScanning ();
				}
                else
                {
					UnRegisterEvents ();
					SearchBlue2Devices ();
				}
			} catch (Exception ex) {
				Debug.WriteLine ("Error om reset connection {0}", ex.Message);
			}
		}


		/// <summary>
		///     Unregister events.
		/// </summary>
		private void UnRegisterEvents ()
		{
			Debug.WriteLine ("start UnRegisterEvents");
			ResetServiceEvents ();
			if (Adapter != null) {
				Adapter.DeviceDiscovered -= DeviceDiscovered;
				Adapter.ScanTimeoutElapsed -= ScanTimeoutElapsed;
				Adapter.DeviceConnected -= DeviceConnected;
				Adapter.DeviceDisconnected -= DeviceDisConnected;
				Adapter.ConnectTimeoutElapsed -= ConnectTimeoutElapsed;
			}


			Debug.WriteLine ("stop UnRegisterEvents");
		}

		/// <summary>
		///     Resets the service events.
		/// </summary>
		private void ResetServiceEvents ()
		{
			DisConnectFromWand ();
			if (_batteryService != null)
				_batteryService.CharacteristicsDiscovered -= ReadCharecteristics;
			if (SelectedDevice != null)
				SelectedDevice.ServicesDiscovered -= ServicesDiscovered;

			Settings = new BleSettings ();
			SelectedDevice = null;
			if (_services != null)
				_services.Clear ();

			_temperatureService = null;
			_deviceInfoService = null;
			_batteryService = null;

			_measurementTimeChar = null;
			_measurementScaleChar = null;
			_autooffIntervalChar = null;
			_sleepTimeChar = null;
			_resetAutoOffChar = null;
			_disConnectPeripheralChar = null;

			_batteryServieOn = false;
			_temperatureServieOn = false;
			_deviceServieOn = false;
			Debug.WriteLine ("ResetServiceEvents..");
		}

		/// <summary>
		///     Device dis connected event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void DeviceDisConnected (object sender, DeviceConnectionEventArgs e)
		{
			IsConnected = false;
			IsSleeping = IsLow = IsHigh = IsBatteryLow = false;
			ReadingBlue2Data = false;
			MessagingCenter.Send (new BleConnectionStatusMessage (), HaccpConstant.BleconnectionStatus);
			UnRegisterEvents ();



		}


		/// <summary>
		///     Services discovered.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void ServicesDiscovered (object sender, EventArgs e)
		{
			if (SelectedDevice != null) {
				_services = new List<IService> ();
				foreach (var service in SelectedDevice.Services) {
					_services.Add (service);
				}
				//await Task.Delay(200);
				_batteryService =
                    _services.FirstOrDefault (x => x.Name.ToLower ().Contains (HaccpConstant.BatteryServiceName));
				if (_batteryService != null) {
					_batteryService.CharacteristicsDiscovered += ReadCharecteristics;
					_batteryServieOn = true;
					_batteryService.DiscoverCharacteristics ();
				}
			}
		}

		/// <summary>
		///     Reads the charecteristics.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		public async void ReadCharecteristics (object sender, EventArgs args)
		{
			try {
				if (_batteryServieOn) {
					Debug.WriteLine ("batteryServieOn ReadBatteryCharecteristics");
					await ReadBatteryCharecteristics ();

					_batteryServieOn = false;

					_deviceInfoService =
                        _services.FirstOrDefault (x => x.Name == HaccpConstant.DeviceInformationServiceName);
					if (_deviceInfoService != null) {
						//=================================================================
						if (Device.OS == TargetPlatform.Android)
							_deviceInfoService.CharacteristicsDiscovered += ReadCharecteristics;
						//=================================================================

						_deviceServieOn = true;
						_deviceInfoService.DiscoverCharacteristics ();
					}
				} else if (_deviceServieOn) {
					await ReadBlue2Id ();
					_deviceServieOn = false;

					_temperatureService =
                        _services.FirstOrDefault (x => x.ID.ToString ().Contains (HaccpConstant.TemperatureServiceUuid));
					if (_temperatureService != null) {
						//=================================================================
						if (Device.OS == TargetPlatform.Android)
							_temperatureService.CharacteristicsDiscovered += ReadCharecteristics;
						//=================================================================

						_temperatureServieOn = true;
						_temperatureService.DiscoverCharacteristics ();
					}
				} else if (_temperatureServieOn) {
					await TemperatureCharacteristicsDiscovered ();
					_temperatureServieOn = false;

					MessagingCenter.Send (new BLEBlue2SettingsUpdatedMessage (), HaccpConstant.Bleblue2SettingsUpdate);
				}
			} catch (Exception ex) {
				Debug.WriteLine ("Error on reading battery service {0}", ex.Message);
			}
		}

		/// <summary>
		///     Reads the battery charecteristics.
		/// </summary>
		/// <returns>The battery charecteristics.</returns>
		public async Task ReadBatteryCharecteristics ()
		{
			try {
				if (_batteryService != null && _batteryService.Characteristics != null &&
				    _batteryService.Characteristics.Count > 0) {
					var batterylevelChar = _batteryService.Characteristics.FirstOrDefault ();

					batterylevelChar = await batterylevelChar.ReadAsync ();

					if (batterylevelChar != null && batterylevelChar.Value != null) {
						var array = (from i in batterylevelChar.Value
						             select i.ToString ("X")).ToArray ();
						var hexstring = string.Join (string.Empty, array);

						Settings.BatteryLevel = Convert.ToInt32 (hexstring, 16);
						Debug.WriteLine ("battery leavel : {0}", Settings.BatteryLevel);
					}
				} else {
					Debug.WriteLine ("couldn't read battery service");
				}
			} catch (Exception ex) {
				Debug.WriteLine ("Error on reading battery service {0}", ex.Message);
			}
		}

		/// <summary>
		///     Reads the blue2 identifier.
		/// </summary>
		/// <returns>The blue2 identifier.</returns>
		public async Task ReadBlue2Id ()
		{
			try {
				if (_deviceInfoService != null && _deviceInfoService.Characteristics != null &&
				    _deviceInfoService.Characteristics.Count > 0) {
					var serialNumberCharacteristics =
						_deviceInfoService.Characteristics.FirstOrDefault (
							x => x.Uuid.ToLower ().Contains (HaccpConstant.SerialNumberUuid));
					serialNumberCharacteristics = await serialNumberCharacteristics.ReadAsync ();
					if (serialNumberCharacteristics != null) {
						Settings.SNo = serialNumberCharacteristics.StringValue;
						Debug.WriteLine ("Blue2 Id is {0}", Settings.SNo);
					}
				} else {
					Debug.WriteLine ("Couldn't read device service");
				}
			} catch (Exception ex) {
				Debug.WriteLine ("Error on reading device info service : {0}", ex.Message);
			}
		}


		/// <summary>
		///     Temperature characteristics discovered.
		/// </summary>
		/// <returns>The characteristics discovered.</returns>
		public async Task TemperatureCharacteristicsDiscovered ()
		{
			try {
				if (_temperatureService != null && _temperatureService.Characteristics != null &&
				    _temperatureService.Characteristics.Count > 0) {
					_measurementTimeChar =
                        _temperatureService.Characteristics.FirstOrDefault (
						x => x.ID.ToString ().Contains (HaccpConstant.MeasurementtimingCharacteristicsUuid));
					_measurementScaleChar =
                        _temperatureService.Characteristics.FirstOrDefault (
						x => x.ID.ToString ().Contains (HaccpConstant.MeasurementScaleUuid));
					_autooffIntervalChar =
                        _temperatureService.Characteristics.FirstOrDefault (
						x => x.ID.ToString ().Contains (HaccpConstant.AutoOffIntervalUuid));
					_sleepTimeChar =
                        _temperatureService.Characteristics.FirstOrDefault (
						x => x.ID.ToString ().Contains (HaccpConstant.SleepTimeUuid));
					_probeNameChar =
                        _temperatureService.Characteristics.FirstOrDefault (
						x => x.ID.ToString ().Contains (HaccpConstant.ProbeUuid));
					_resetAutoOffChar =
                        _temperatureService.Characteristics.FirstOrDefault (
						x => x.ID.ToString ().Contains (HaccpConstant.ResetAutooffUuid));
					_disConnectPeripheralChar =
                        _temperatureService.Characteristics.FirstOrDefault (
						x => x.ID.ToString ().Contains (HaccpConstant.DiscoonectPeripheralUuid));

					_measurementTimeChar = await _measurementTimeChar.ReadAsync ();
					_measurementScaleChar = await _measurementScaleChar.ReadAsync ();
					_autooffIntervalChar = await _autooffIntervalChar.ReadAsync ();
					_sleepTimeChar = await _sleepTimeChar.ReadAsync ();


					if (_measurementScaleChar != null && !string.IsNullOrEmpty (_measurementScaleChar.StringValue)) {
						var unit = _measurementScaleChar.StringValue;
						Settings.Scale = unit == HaccpConstant.C ? (short)0 : (short)1;
						Debug.WriteLine ("Temperature unit : {0}", unit);
					}

					if (_autooffIntervalChar != null && _autooffIntervalChar.Value != null) {
						var hexstring = string.Join (string.Empty, _autooffIntervalChar.Value);
						Settings.AutoOff = Convert.ToInt32 (hexstring);

						if (Settings.AutoOff > 30)
							Settings.AutoOff = 30;
						if (Settings.AutoOff < 1)
							Settings.AutoOff = 1;

						Debug.WriteLine ("autooffIntervalChar : {0}", Settings.AutoOff);
					}

					if (_sleepTimeChar != null && _sleepTimeChar.Value != null) {
						var hexstring = string.Join (string.Empty, _sleepTimeChar.Value);
						Settings.Sleep = Convert.ToInt32 (hexstring);
						if (Settings.Sleep > 30)
							Settings.Sleep = 30;
						if (Settings.Sleep < 1)
							Settings.Sleep = 1;
						Debug.WriteLine ("sleepTimeChar : {0}", Settings.Sleep);
					}


					if (_measurementTimeChar != null && _measurementTimeChar.Value != null) {
						var hexstring = string.Join (string.Empty, _measurementTimeChar.Value);

						Debug.WriteLine ("measurementTimeChar : {0}", hexstring);
						var measurementTiming = Convert.ToInt32 (hexstring) * 1000;
						Settings.MeasurementLevel = Convert.ToInt32 (hexstring);
						if (Settings.MeasurementLevel > 5)
							Settings.MeasurementLevel = 5;
						if (Settings.MeasurementLevel < 1)
							Settings.MeasurementLevel = 1;

						//get the temperature ascii characteristics .Contains temperature string value
						var characts =
							_temperatureService.Characteristics.FirstOrDefault (
								x => x.ID.ToString ().Contains (HaccpConstant.AsciiTemperatureUuid));
						if (characts != null && characts.CanRead) {
							ReadTemperatureValue (characts, measurementTiming);
						}
					}

					if (_probeNameChar != null) {
						_probeNameChar = await _probeNameChar.ReadAsync ();
						if (_probeNameChar.Value != null) {
							var hexstring = HACCPUtil.GetStringFromBytes (_probeNameChar.Value);
							Settings.Prob = hexstring;
							Debug.WriteLine ("probeNameChar : {0}", Settings.Prob);
						}
					}

					//============================================================================

//					if (Device.OS == TargetPlatform.Android) {
//						probeNameChar = temperatureService.Characteristics.FirstOrDefault (x => x.ID.ToString ().Contains (HACCPConstant.PROBE_UUID));
//						if (probeNameChar != null) {
//							probeNameChar = await probeNameChar.ReadAsync ();
//							if (probeNameChar.Value != null) {
//								var hexstring = HACCPUtil.GetStringFromBytes (probeNameChar.Value);
//								Settings.Prob = hexstring;
//								Debug.WriteLine ("probeNameChar : {0}", Settings.Prob);
//							}
//						}
//					}

					//============================================================================
				} else {
					Debug.WriteLine ("Couldn't read temperature service");
				}
			} catch (Exception ex) {
				Debug.WriteLine ("CharacteristicsDicovered exception {0}", ex.Message);
			} finally {
				ReadingBlue2Data = false;
			}
		}

		/// <summary>
		/// Restarts the temperarture reading.
		/// </summary>
		public void RestartTemperartureReading ()
		{
			if (IsConnected && _temperatureService != null && Settings != null ) {
				var characts =
					_temperatureService.Characteristics.FirstOrDefault (
						x => x.ID.ToString ().Contains (HaccpConstant.AsciiTemperatureUuid));
				if (characts != null && characts.CanRead) {
					ReadTemperatureValue (characts, Settings.MeasurementLevel * 1000);
				}
			}
		}

		/// <summary>
		///     Reads the temperature value.
		/// </summary>
		/// <param name="characteristic">Characteristic.</param>
		/// <param name="measurementTiming">Measurement timing.</param>
		public void ReadTemperatureValue (ICharacteristic characteristic, int measurementTiming)
		{			
			// read the ascii value at measurement timing interval
			Device.StartTimer (new TimeSpan (0, 0, 0, 0, measurementTiming), () => {

				if (StopTemperatureReading) {					
					return false;
				}
								
				Task.Factory.StartNew (async () => {
					try {		
						var val = await characteristic.ReadAsync ();

						var temperatureString = val.StringValue.Replace (" ", string.Empty); //eg value: 28.4�C
						IsHigh = IsLow = IsSleeping = IsBatteryLow = false;

						Debug.WriteLine ("Temperature String:{0}", temperatureString);
						if (!string.IsNullOrEmpty (temperatureString)) {
							if (temperatureString.ToLower () == HaccpConstant.Blue2SleepStateValue) {
								IsSleeping = true;
								MessagingCenter.Send (new BleTemperatureReadingMessage {
									IsSleeping = true
								}, HaccpConstant.BletemperatureReading);
								Debug.WriteLine ("Blue2 device in light sleep mode...");
							} else if (temperatureString.ToLower ().Contains (HaccpConstant.Blue2HighStateValue)) {
								IsHigh = true;
								MessagingCenter.Send (new BleTemperatureReadingMessage {
									IsHigh = true
								}, HaccpConstant.BletemperatureReading);
								Debug.WriteLine ("Blue2 temperarture reading is high...");
							} else if (temperatureString.ToLower ().Contains (HaccpConstant.Blue2LowStateValue)) {
								IsLow = true;
								MessagingCenter.Send (new BleTemperatureReadingMessage {
									IsLow = true
								}, HaccpConstant.BletemperatureReading);
								Debug.WriteLine ("Blue2 temperarture reading is low...");
							} else if (temperatureString.ToLower ().Contains (HaccpConstant.Blue2BatteryLowState)) {
								IsBatteryLow = true;
								MessagingCenter.Send (new BleTemperatureReadingMessage {
									IsBatteryLow = true
								}, HaccpConstant.BletemperatureReading);
								Debug.WriteLine ("Blue2 device battery is low...");
							} else if (temperatureString.Length > 2) {
								IsHigh = IsLow = IsSleeping = IsBatteryLow = false;
								var lastchar = temperatureString [temperatureString.Length - 1];
								//last character will be 'S' if blue2 button press
								string temperature;
								short unit;
								bool shouldRecord;

								if (lastchar == 'S') {
									// if last char is 'S' the temperature should be automatically record
									temperature = temperatureString.Substring (0, temperatureString.Length - 3);
									// remove the last 2 character to obtain the actual value
									unit = temperatureString [temperatureString.Length - 2].ToString ().ToUpper () == "C"
                                        ? (short)0
                                        : (short)1;
									shouldRecord = true;
								} else {
									temperature = temperatureString.Substring (0, temperatureString.Length - 2);
									// remove the last 2 character to obtain the actual value
									unit = temperatureString [temperatureString.Length - 1].ToString ().ToUpper () == "C"
                                        ? (short)0
                                        : (short)1;
									shouldRecord = false;
								}


								var doublevalue = HACCPUtil.ConvertToDouble (temperature);

								LastReading = doublevalue;
								LastUnit = unit == (short)0 ? TemperatureUnit.Celcius : TemperatureUnit.Fahrenheit;

								MessagingCenter.Send (new BleTemperatureReadingMessage {
									TempUnit = unit,
									TempValue = doublevalue,
									ShouldRecord = shouldRecord
								}, HaccpConstant.BletemperatureReading);

								GotTemperartureReading = true;
							}
						}
					} catch (Exception ex) {
						Debug.WriteLine ("Error occured while reading temperature characteristics {0}", ex.Message);
					}
				});
					
				return IsConnected;
			});
		}

		#endregion
	}
}