using System;
using Android.App;
using Android.Appwidget;
using Android.Bluetooth;
using Android.Content;
using HACCP.Core;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace BluetoothToggleWidget
{
    [BroadcastReceiver(Label = "Bluetooth Toggle Widget")]
    [IntentFilter(new[]
    {
        "android.appwidget.action.APPWIDGET_UPDATE",
        BluetoothAdapter.ActionStateChanged,
        BluetoothAdapter.ActionConnectionStateChanged
    })]
    //	[MetaData("android.appwidget.provider", Resource = "@xml/bt_widget")]
    public class BTToggleWidget : AppWidgetProvider
    {
        public bool IsBluetoothEnabled { get; set; }

        public event EventHandler BluetoothStateChanged;


        /// <summary>
        ///     This event fires for every intent you're filtering for. There can be lots of them,
        ///     and they can arrive very quickly, so spend as little time as possible processing them
        ///     on the UI thread.
        /// </summary>
        /// <param name="context">The Context in which the receiver is running.</param>
        /// <param name="intent">The Intent being received.</param>
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == BluetoothAdapter.ActionStateChanged)
            {
                var prevState = (State) intent.GetIntExtra(BluetoothAdapter.ExtraPreviousState, -1);
                var newState = (State) intent.GetIntExtra(BluetoothAdapter.ExtraState, -1);

                IsBluetoothEnabled = newState == State.On;


                if (BLEManager.SharedInstance.IsBLERefrehing && newState == State.TurningOff)
                {
                    var appContext = Application.Context;
                    var manager = (BluetoothManager) appContext.GetSystemService("bluetooth");

                    manager.Adapter.Enable();

                    return;
                }
                if (newState == State.On)
                {
                    BLEManager.SharedInstance.IsBLERefrehing = false;

                    MessagingCenter.Send(new AndroidBluetoothStatusMessage {IsEnabled = IsBluetoothEnabled},
                        DroidConstant.DEVICE_BLUETOOTH_ONOFF_MESSAGE);
                }
                else if (newState == State.Off && !BLEManager.SharedInstance.IsBLERefrehing)
                {
                    MessagingCenter.Send(new AndroidBluetoothStatusMessage {IsEnabled = IsBluetoothEnabled},
                        DroidConstant.DEVICE_BLUETOOTH_ONOFF_MESSAGE);
                }
            }
        }
    }

    public class AndroidBluetoothStatusMessage
    {
        public bool IsEnabled { get; set; }
    }

    public static class DroidConstant
    {
        //Message Center Notiificaiotn keys
        public const string DEVICE_BLUETOOTH_ONOFF_MESSAGE = "Bluetooth on/off message";
    }
}