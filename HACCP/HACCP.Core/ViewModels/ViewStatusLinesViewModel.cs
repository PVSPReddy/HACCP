using Xamarin.Forms;

namespace HACCP.Core
{
    public class ViewStatusLinesViewModel : BaseViewModel
    {
        private Command homeCommand;
        private bool showLines;

        public ViewStatusLinesViewModel(IPage page) : base(page)
        {
            if (!string.IsNullOrEmpty(Line1) || !string.IsNullOrEmpty(Line2))
            {
                ShowLines = true;
            }
        }


        /// <summary>
        ///     Gets or sets the line1.
        /// </summary>
        /// <value>The line1.</value>
        public string Line1
        {
            get { return HaccpAppSettings.SharedInstance.DeviceSettings.Line1; }
        }

        /// <summary>
        ///     Gets or sets the line1.
        /// </summary>
        /// <value>The line2.</value>
        public string Line2
        {
            get { return HaccpAppSettings.SharedInstance.DeviceSettings.Line2; }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.ViewStatusLinesViewModel" /> show lines.
        /// </summary>
        /// <value><c>true</c> if show lines; otherwise, <c>false</c>.</value>
        public bool ShowLines
        {
            get { return showLines; }
            set { SetProperty(ref showLines, value); }
        }

        #region Commands

        public Command HomeCommand
        {
            get { return homeCommand ?? (homeCommand = new Command(() => { Page.ReloadHomePage(); })); }
        }

        #endregion
    }
}