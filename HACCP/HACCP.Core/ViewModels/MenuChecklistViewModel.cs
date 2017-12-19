using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class MenuChecklistViewModel : BaseViewModel
    {
        #region  Member Variables

        private readonly IDataStore dataStore;
        private readonly IHACCPService haccpService;
        private readonly Command loadDataCommand;
        private ObservableCollection<Checklist> checklists;
        private bool isMenu;
        private ObservableCollection<Menu> menus;

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.MenuChecklistViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        public MenuChecklistViewModel(IPage page) : base(page)
        {
            dataStore = new SQLiteDataStore();
            haccpService = new HACCPService(dataStore);
            loadDataCommand = new Command(async () => await LoadMenuChecklists(), () => !IsBusy);
        }
   

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is menu.
        /// </summary>
        /// <value><c>true</c> if this instance is menu; otherwise, <c>false</c>.</value>
        public bool IsMenu
        {
            get { return isMenu; }
            set { SetProperty(ref isMenu, value); }
        }

        /// <summary>
        ///     Gets or sets the menus.
        /// </summary>
        /// <value>The menus.</value>
        public ObservableCollection<Menu> Menus
        {
            get { return menus; }
            set { SetProperty(ref menus, value); }
        }

        /// <summary>
        ///     Gets or sets the checklists.
        /// </summary>
        /// <value>The checklists.</value>
        public ObservableCollection<Checklist> Checklists
        {
            get { return checklists; }
            set { SetProperty(ref checklists, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// OnViewAppearing
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            loadDataCommand.Execute(null);
        }

        /// <summary>
        /// LoadMenuChecklists
        /// </summary>
        /// <returns></returns>
        public async Task LoadMenuChecklists()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            Page.ShowProgressIndicator();
            if (IsMenu)
            {
                try
                {
                    var res = await haccpService.DownloadMenus();
                    if (res.IsSuccess)
                    {
                        var menuLists = (IList<Menu>) res.Results;
                        if (menuLists.Any())
                        {
                            Menus = new ObservableCollection<Menu>(menuLists);
                        }
                        else
                        {
                            IsBusy = false;
                            Page.DismissPopup();
                            await
                                Page.ShowAlert(HACCPUtil.GetResourceString("NoMenusFound"),
                                    HACCPUtil.GetResourceString("Nomenusfoundintheserver"));
                        }
                    }
                    else
                    {
                        IsBusy = false;
                        Page.DismissPopup();
                        Page.DisplayAlertMessage("", res.Message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ooops! Something went wrong while fetch menu list  from server. Exception: {0}", ex);
                }
                finally
                {
                    IsBusy = false;
                    Page.DismissPopup();
                }
            }
            else
            {
                try
                {
                    var res = await haccpService.DownloadChecklists();
                    if (res.IsSuccess)
                    {
                        var _checklists = (IList<Checklist>) res.Results;
                        if (_checklists.Any())
                        {
                            Checklists = new ObservableCollection<Checklist>(_checklists);
                        }
                        else
                        {
                            IsBusy = false;
                            Page.DismissPopup();
                            await
                                Page.ShowAlert(HACCPUtil.GetResourceString("NoChecklistsFound"),
                                    HACCPUtil.GetResourceString("Nochecklistsfoundintheserver"));
                        }
                    }
                    else
                    {
                        IsBusy = false;
                        Page.DismissPopup();
                        Page.DisplayAlertMessage("", res.Message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ooops! Something went wrong while fetch checklist from server. Exception: {0}", ex);
                }
                finally
                {
                    IsBusy = false;
                    Page.DismissPopup();
                }
            }
        }


        /// <summary>
        /// ShowSelectMenuChecklistAlert
        /// </summary>
        /// <param name="message"></param>
        /// <param name="selectedItem"></param>
        /// <returns></returns>
        public async Task ShowSelectMenuChecklistAlert(string message, object selectedItem)
        {
            if (IsBusy)
                return;
            await Task.Run(async () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = true;
                    Page.ShowProgressIndicator();
                });
                ServiceResponse res;

                try
                {
                    if (IsMenu)
                    {
                        var menu = (Menu) selectedItem;
                        res = await haccpService.DownloadLocationandItems(menu.MenuId);
                        if (res.IsSuccess)
                        {
                            HaccpAppSettings.SharedInstance.SiteSettings.MenuId = menu.MenuId;
                            HaccpAppSettings.SharedInstance.SiteSettings.MenuName = menu.Name;
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                IsBusy = false;
                                Page.DismissPopup();
                                Page.DisplayAlertMessage("", res.Message);
                            });
                        }
                    }
                    else
                    {
                        var checklist = (Checklist) selectedItem;

                        res = await haccpService.DownloadCheckList(checklist.ChecklistId);

                        if (res.IsSuccess)
                        {
                            HaccpAppSettings.SharedInstance.SiteSettings.CheckListId = checklist.ChecklistId;
                            HaccpAppSettings.SharedInstance.SiteSettings.CheckListName = checklist.Name;
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                IsBusy = false;
                                Page.DismissPopup();
                                Page.DisplayAlertMessage("", res.Message);
                            });
                        }
                    }
                    if (res.IsSuccess)
                    {
                        dataStore.SaveSiteSettings(HaccpAppSettings.SharedInstance.SiteSettings);
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            try
                            {
                                IsBusy = false;
                                Page.DismissPopup();
                                await Page.ShowAlert(string.Empty, message);
                                await Page.PopPage();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Poppage error {0}", ex);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ooops! Something went wrong ShowSelectMenuChecklistAlert. Exception: {0}", ex);
                }
                finally
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        IsBusy = false;
                        Page.DismissPopup();
                    });
                }
            });
        }

        #endregion
    }
}