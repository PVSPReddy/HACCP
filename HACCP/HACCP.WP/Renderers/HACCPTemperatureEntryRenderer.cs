﻿using System;
using System.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using HACCP.WP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
using HACCP;
using HACCP.Core;

[assembly: ExportRenderer(typeof(HACCPTemperatureEntry), typeof(HACCPTemperatureEntryRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPTemperatureEntryRenderer : EntryRenderer
    {
        public double Max
        {
            get { return HaccpAppSettings.SharedInstance.DeviceSettings.TempScale == 0 ? 9999.9 : 5537.7; }
        }

        public double Min
        {
            get { return HaccpAppSettings.SharedInstance.DeviceSettings.TempScale == 0 ? -9999.9 : -5537.7; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element != null)
            {
                Element.BackgroundColor = Xamarin.Forms.Color.Transparent;
                Element.TextColor = Xamarin.Forms.Color.White;
                Control.BorderBrush = new SolidColorBrush(Colors.White);
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(1);
                Control.Height = 33;


                Control.GotFocus += Control_GotFocus;
                Control.LostFocus += Control_LostFocus;
                Control.KeyDown += Control_KeyDown;
                var scope = new InputScope();
                var name = new InputScopeName { NameValue = InputScopeNameValue.TelephoneNumber };

                scope.Names.Add(name);

                Control.InputScope = scope;
                Control.Focus(FocusState.Programmatic);
            }
        }

        private void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            // Control.Foreground = new SolidColorBrush(Colors.White);
        }

        private void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            // Control.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void Control_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (Control.Text != null)
            {
                var text = Control.Text.Trim();

                if (e.Key == VirtualKey.Space || e.Key == VirtualKey.Number3 || e.Key == VirtualKey.Number8 ||
                    e.Key.ToString() == "187" || e.Key.ToString() == "186" || e.Key.ToString() == "188" ||
                    e.Key == VirtualKey.Number9 || e.Key == VirtualKey.Number0 || e.Key == VirtualKey.X)
                    e.Handled = true;
                else if (e.Key.ToString() == "189" && (text.Contains('-') || text.Length >= 1))
                    e.Handled = true;
                else if (e.Key.ToString() == "190" && text.Contains('.'))
                    e.Handled = true;

                else
                {
                    try
                    {
                        switch (e.Key)
                        {
                            case VirtualKey.NumberPad0:
                                text += "0";
                                break;
                            case VirtualKey.NumberPad1:
                                text += "1";
                                break;
                            case VirtualKey.NumberPad2:
                                text += "2";
                                break;
                            case VirtualKey.NumberPad3:
                                text += "3";
                                break;
                            case VirtualKey.NumberPad4:
                                text += "4";
                                break;
                            case VirtualKey.NumberPad5:
                                text += "5";
                                break;
                            case VirtualKey.NumberPad6:
                                text += "6";
                                break;
                            case VirtualKey.NumberPad7:
                                text += "7";
                                break;
                            case VirtualKey.NumberPad8:
                                text += "8";
                                break;
                            case VirtualKey.NumberPad9:
                                text += "9";
                                break;
                            default:
                                if (e.Key.ToString() == "190")
                                    text += ".";
                                break;
                        }

                        var doublevalue = Convert.ToDouble(text);
                        if (doublevalue > Max || doublevalue < Min)
                            e.Handled = true;
                        else
                        {
                            var arr = text.Split('.');
                            if (arr != null && arr.Length > 1 && arr[1].Length > 1)
                                e.Handled = true;
                        }
                    }
                    catch (Exception)
                    {
                        //e.Handled = true;                        
                    }
                }

            }
            else if (e.Key == VirtualKey.Space || e.Key == VirtualKey.Number3 || e.Key == VirtualKey.Number8 ||
                     e.Key.ToString() == "187" || e.Key.ToString() == "186" || e.Key.ToString() == "188" ||
                     e.Key == VirtualKey.Number9 || e.Key == VirtualKey.Number0 || e.Key == VirtualKey.X)
                e.Handled = true;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Element != null && Control != null)
            {
                Element.BackgroundColor = Xamarin.Forms.Color.Transparent;
                Element.TextColor = Xamarin.Forms.Color.White;
                Control.BorderBrush = new SolidColorBrush(Colors.White);
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(1);
                Control.VerticalContentAlignment = VerticalAlignment.Center;
            }
        }
    }
}