using System;
using System.Collections.Generic;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class HACCPBlue2DeviceListView : ContentView
    {
        public HACCPBlue2DeviceListView(IList<IDevice> devices)
        {
            InitializeComponent();
            devicelist.ItemSelected += (sender, e) => { DeviceListSelected(sender, e); };
            devicelist.ItemsSource = devices;
        }

        public event EventHandler<SelectedItemChangedEventArgs> DeviceListSelected = delegate { };

        public void Skip_Button_Click(object sender, EventArgs args)
        {
            IsVisible = false;
        }
    }
}