using Xamarin.Forms;

namespace HACCP
{
    /// <summary>
    ///     Custom entry control class
    /// </summary>
    public class HACCPEntry : Entry
    {
        public static readonly BindableProperty MaxLengthProperty =
            BindableProperty.Create<HACCPEntry, int>(p => p.MaxLength, 0);

        /// <summary>
        /// HACCPEntry
        /// </summary>
        public HACCPEntry()
        {
            TextChanged += EnforceMaxLength;
        }

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this entry is last item.
        /// </summary>
        /// <value><c>true</c> if this instance is last item; otherwise, <c>false</c>.</value>
        public bool IsLastItem { get; set; }

        public bool ShowPlusMinus { get; set; }

        public bool IsSearchbox { get; set; }

        public bool NeedDot { get; set; }

        public int MaxLength
        {
            get { return (int) GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// EnforceMaxLength
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void EnforceMaxLength(object sender, TextChangedEventArgs args)
        {
            if (MaxLength > 0)
            {
                var e = sender as Entry;
                if (e != null)
                {
                    var val = e.Text;
                    if (!string.IsNullOrEmpty(val) && val.Length > MaxLength)
                    {
                        val = val.Remove(val.Length - 1);
                    }
                    e.Text = val;
                }
            }
        }

        #endregion
    }
}