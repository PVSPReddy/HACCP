using Xamarin.Forms;

namespace HACCP
{
    public class HACCPSwitch : Switch
    {
        /// <summary>
        /// BindableProperty TextOnProperty
        /// </summary>
        public static readonly BindableProperty TextOnProperty =
            BindableProperty.Create<HACCPSwitch, string>(p => p.TextOn, "Yes");

        /// <summary>
        /// BindableProperty TextOffProperty
        /// </summary>
        public static readonly BindableProperty TextOffProperty =
            BindableProperty.Create<HACCPSwitch, string>(p => p.TextOff, "No");

        /// <summary>
        /// TextOn
        /// </summary>
        public string TextOn
        {
            get { return (string) GetValue(TextOnProperty); }
            set { SetValue(TextOnProperty, value); }
        }

        /// <summary>
        /// TextOff
        /// </summary>
        public string TextOff
        {
            get { return (string) GetValue(TextOffProperty); }
            set { SetValue(TextOffProperty, value); }
        }
    }
}