using System;
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

[assembly: ExportRenderer(typeof(HACCPEntry), typeof(HACCPEntryRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPEntryRenderer : EntryRenderer
    {
        private readonly string space = "     ";

        public double Max
        {
            get { return 9999.9; }
        }

        public double Min
        {
            get { return -9999.9; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);


            if (Control != null)
            {

                Element.BackgroundColor = Xamarin.Forms.Color.Transparent;
                Element.TextColor = Xamarin.Forms.Color.White;
                Control.BorderBrush = new SolidColorBrush(Colors.White);
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(1);
                Control.Height = 33;
                

                Control.VerticalContentAlignment = VerticalAlignment.Center;

                //Control.Padding = new Windows.UI.Xaml.Thickness(5,0, 0, 0);

                //Control.SetTextColor(Android.Graphics.Color.Rgb(255,255,255));

                var entry = Element as HACCPEntry;
                if (entry != null && entry.ShowPlusMinus)
                {
                    Control.KeyDown += Control_KeyDown;

                    var scope = new InputScope();
                    var name = new InputScopeName();
                    name.NameValue = InputScopeNameValue.TelephoneNumber;
                    scope.Names.Add(name);

                    Control.InputScope = scope;
                }
                else if (entry != null && entry.IsSearchbox)
                {
                    Control.KeyDown += Search_Control_KeyDown;
                    Control.Padding = new Windows.UI.Xaml.Thickness(30, 0, 0, 0);
                    var scope = new InputScope();
                    var name = new InputScopeName();
                    name.NameValue = InputScopeNameValue.Search;
                    scope.Names.Add(name);
                    Control.InputScope = scope;
                }

                Control.GotFocus += Control_GotFocus;
                Control.LostFocus += Control_LostFocus;
                Control.IsEnabledChanged += Control_IsEnabledChanged;


                if (Element != null && !string.IsNullOrEmpty(Element.Placeholder))
                {
                    var placeHolder = Element.Placeholder;
                    var length = Device.Idiom == TargetIdiom.Tablet ? 42 : 33;
                    if (placeHolder.Length > length)
                        Element.Placeholder = string.Format("{0}...", placeHolder.Substring(0, length));
                }

               
            }
        }

        private void Control_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Control.Foreground = new SolidColorBrush(Colors.White);
        }

        private void Search_Control_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Element.Unfocus();
            }
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
                    catch (Exception ex)
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

        private void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Control != null)
            {
                //  Control.Foreground = new SolidColorBrush(Colors.White);
                // Control.Background = new SolidColorBrush(Colors.Red);
                //     Element.BackgroundColor = Xamarin.Forms.Color.Transparent;
                // Control.Background = new SolidColorBrush(Colors.Transparent);

                //Element.BackgroundColor = Xamarin.Forms.Color.Transparent;
                //Element.TextColor = Xamarin.Forms.Color.White;

                if (Element != null && (Element as HACCPEntry).IsSearchbox == true)
                {
                    if (!Control.Text.StartsWith(space))
                    {
                        Control.Text = Control.Text.Trim();
                        if (Control.Text != "")
                        {
                            Control.Text = space + Control.Text;
                        }
                    }
                    else
                    {
                        Control.Text = "";
                    }
                }
            }
        }

        private void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Control != null)
            {
                //   Control.Foreground = new SolidColorBrush(Colors.Black);
                //   Control.Background = new SolidColorBrush(Colors.Transparent);
                // Element.BackgroundColor = Xamarin.Forms.Color.Transparent;
                //Control.BorderBrush = new SolidColorBrush(Colors.White);
                //Control.BorderThickness = new Windows.UI.Xaml.Thickness(1);

                //Element.BackgroundColor = Xamarin.Forms.Color.White;
                //Element.TextColor = Xamarin.Forms.Color.Black;
                if ((Element as HACCPEntry).IsSearchbox == true)
                {
                    Control.Text = Control.Text.Trim();
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        //    Control.Background = new SolidColorBrush(Colors.Transparent);
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