using System;
using Xamarin.Forms;

namespace HACCP
{
	/// <summary>
	/// Custom entry control class
	/// </summary>
	public class HACCPPasswordEntry:Entry 
	{

		public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create<HACCPEntry, int>(p => p.MaxLength, 0);

	    public int MaxLength
		{
			get
			{
				return (int)GetValue(MaxLengthProperty);
			}
			set
			{
				SetValue(MaxLengthProperty, value);
			}
		}

        /// <summary>
        /// HACCPPasswordEntry Constructor
        /// </summary>
        public HACCPPasswordEntry()
        {

            Keyboard = Keyboard.Numeric;
            
		}



	}


}

