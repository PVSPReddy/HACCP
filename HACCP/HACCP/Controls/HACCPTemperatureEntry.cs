using Xamarin.Forms;

namespace HACCP
{
    public class HACCPTemperatureEntry : Entry
    {
        /// <summary>
        /// HACCPTemperatureEntry Constructor
        /// </summary>
        public HACCPTemperatureEntry()
        {
            Keyboard = Keyboard.Numeric;
        }

        /// <summary>
        /// ShowPlusMinus
        /// </summary>
        public bool ShowPlusMinus { get; set; }
    }
}