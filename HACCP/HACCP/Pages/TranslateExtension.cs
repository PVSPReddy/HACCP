using System;
using System.Diagnostics;
using HACCP.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HACCP
{
    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return null;
            Debug.WriteLine("Provide: " + Text);
            // Do your translation lookup here, using whatever method you require
            //if (translated == string.Empty)
            //if(HACCPAppSettings.SharedInstance.SiteSettings.ServerAddress !=  null && HACCPAppSettings.SharedInstance.SiteSettings.ServerAddress != string.Empty )
            var translated = HACCPUtil.GetResourceString(Text);

            //else
            //translated = Localization.Localize (Text, Text); 
            return translated;
        }
    }
}