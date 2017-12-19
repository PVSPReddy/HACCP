using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class RecordViewModel : BaseViewModel
    {
        private Command correctiveActionCommand;
        private ObservableCollection<CorrectiveAction> correctiveActions;
        private Command naCommand;
        private Command saveCommand;


        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.RecordViewModel" /> class.
        /// </summary>
        public RecordViewModel(IPage page) : base(page)
        {
        }

        #region Properties

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public Command SaveCommand
        {
            get
            {
                return saveCommand ??
                       (saveCommand = new Command(async () => await ExecuteSaveCommand(), () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the corrective action command.
        /// </summary>
        /// <value>The corrective action command.</value>
        public Command CorrectiveActionCommand
        {
            get
            {
                return correctiveActionCommand ??
                       (correctiveActionCommand =
                           new Command(ExecuteCorrectiveActionCommand, () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the NA command.
        /// </summary>
        /// <value>The NA command.</value>
        public Command NACommand
        {
            get
            {
                return naCommand ??
                       (naCommand = new Command(async () => await ExecuteNACommand(), () => !IsBusy));
            }
        }

        public ObservableCollection<CorrectiveAction> CorrectiveActions
        {
            get { return correctiveActions; }
            set { SetProperty(ref correctiveActions, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes the save command.
        /// </summary>
        /// <returns>The save command.</returns>
        protected virtual Task ExecuteSaveCommand()
        {
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Executes the corrective action command.
        /// </summary>
        /// <returns>The corrective action command.</returns>
        protected virtual void ExecuteCorrectiveActionCommand()
        {
        }

        /// <summary>
        ///     Executes the NA command.
        /// </summary>
        /// <returns>The NA command.</returns>
        protected virtual Task ExecuteNACommand()
        {
            return Task.FromResult(0);
        }

        #endregion
    }
}