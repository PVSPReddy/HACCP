using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class TemperatureReviewViewModel : BaseViewModel
    {
        private readonly IDataStore dataStore;
        private bool hasItems;
        private bool isReviewAnswerVisible;
        private readonly MenuLocation location;
        private string locationName;
        private ObservableCollection<ItemTemperature> records;
        private Command reviewItemOKCommand;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.TemperatureReviewViewModel" /> class.
        /// </summary>
        public TemperatureReviewViewModel(IPage page, MenuLocation menuLocation) : base(page)
        {
            dataStore = new SQLiteDataStore();
            location = menuLocation;
            var items = dataStore.GetTemperatureRecordCollectionById(location.LocationId);
            HasItems = items != null && items.Any();
            Records = new ObservableCollection<ItemTemperature>(items);
            IsReviewItemVisible = false;
            LocationName = menuLocation.Name;


            MessagingCenter.Subscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh, sender =>
                {
                    var list = dataStore.GetTemperatureRecordCollectionById(location.LocationId);
                    HasItems = list != null && list.Any();
                    Records = new ObservableCollection<ItemTemperature>(list);
                    IsReviewItemVisible = false;
                });
        }

        /// <summary>
        ///     Gets or sets the records.
        /// </summary>
        /// <value>The records.</value>
        public ObservableCollection<ItemTemperature> Records
        {
            get { return records; }
            set { SetProperty(ref records, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance is review item visible.
        /// </summary>
        /// <value><c>true</c> if this instance is review item visible; otherwise, <c>false</c>.</value>
        public bool IsReviewItemVisible
        {
            get { return isReviewAnswerVisible; }
            set { SetProperty(ref isReviewAnswerVisible, value); }
        }

        /// <summary>
        ///     Gets or sets the name of the location.
        /// </summary>
        /// <value>The name of the location.</value>
        public string LocationName
        {
            get { return locationName; }
            set { SetProperty(ref locationName, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance hasitems.
        /// </summary>
        /// <value><c>true</c> if this instance hasitems; otherwise, <c>false</c>.</value>
        public bool HasItems
        {
            get { return hasItems; }
            set { SetProperty(ref hasItems, value); }
        }

        /// <summary>
        ///     Gets the review answer OK command.
        /// </summary>
        /// <value>The review answer OK command.</value>
        public Command ReviewItemOKCommand
        {
            get
            {
                return reviewItemOKCommand ??
                       (reviewItemOKCommand =
                           new Command(ExecuteReviewAnswerOKCommand, () => !IsBusy));
            }
        }

        /// <summary>
        ///     Executes the review answer OK command.
        /// </summary>
        /// <returns>The review answer OK command.</returns>
        private void ExecuteReviewAnswerOKCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            ReviewItemOKCommand.ChangeCanExecute();

            IsReviewItemVisible = false;

            IsBusy = false;
            ReviewItemOKCommand.ChangeCanExecute();
        }


        public override void OnViewDisappearing()
        {
            base.OnViewDisappearing();
            MessagingCenter.Unsubscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh);
        }
    }
}