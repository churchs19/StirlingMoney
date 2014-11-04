using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Shane.Church.StirlingMoney.Universal.Data;
using Shane.Church.StirlingMoney.Universal.Common;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.Services;
using System.Threading.Tasks;
using Windows.UI.Core;
using Shane.Church.StirlingMoney.Universal.Services;
using Grace;
using Newtonsoft.Json.Linq;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace Shane.Church.StirlingMoney.Universal
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public partial class HubPage : Page
    {

        private NavigationHelper navigationHelper;
        //private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private MainViewModel _model;
        private ILoggingService _log;
        private ISettingsService _settings;
        private ILicensingService _license;
        private INavigationService _navService;

        private bool _refreshAccounts;
        private bool _refreshBudgets;
        private bool _refreshGoals;

        /// <summary>
        /// Gets the NavigationHelper used to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        ///// <summary>
        ///// Gets the DefaultViewModel. This can be changed to a strongly typed view model.
        ///// </summary>
        //public ObservableDictionary DefaultViewModel
        //{
        //    get { return this.defaultViewModel; }
        //}

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            _log = ContainerService.Container.Locate<ILoggingService>();
            _settings = ContainerService.Container.Locate<ISettingsService>();
            _license = ContainerService.Container.Locate<ILicensingService>();
            _navService = ContainerService.Container.Locate<INavigationService>();
            _model = ContainerService.Container.Locate<MainViewModel>();
            _model.BusyChanged += _model_BusyChanged;
            _model.SyncCompleted += _model_SyncCompleted;

            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            if (e.PageState == null)
            {
                //initial load
                JObject navContext = e.NavigationParameter is JObject ? (JObject)e.NavigationParameter : null;

                Task.Run(() => Initialize(navContext));
            }
            else
            {
                //restoring from saved state
            }
        }

        protected async Task Initialize(Newtonsoft.Json.Linq.JObject navContext)
        {
#if !PERSONAL
            //Dispatcher.RunAsync(() =>
            //{
            //    //Shows the trial reminder message, according to the settings of the TrialReminder.
            //    ContainerService.Container.Locate<RadTrialApplicationReminder>().Notify();
            //});

            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
            //    //Shows the rate reminder message, according to the settings of the RateReminder.
            //    (App.Current as App).rateReminder.Notify();
            //});
#endif
            try
            {
                await _model.Initialize();
            }
            catch(Exception ex)
            {

            }

            //try
            //{
            //    if (UniversalNavigationService.DecodeNavigationParameter<bool>(navContext) && _navService.CanGoBack)
            //    {
            //        NavigationService.RemoveBackEntry();
            //    }
            //}
            //catch { }

            //if (navMode == System.Windows.Navigation.NavigationMode.New && _settings.LoadSetting<bool>("EnableSync") && _settings.LoadSetting<bool>("SyncOnStartup"))
            //{
            //    _model.SyncCommand.Execute(null);
            //}
            //else
            //{
                _refreshAccounts = true;
                _refreshBudgets = true;
                _refreshGoals = true;

                await LoadData();
            //}

            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.DataContext = _model;
            });
        }

        async void _model_SyncCompleted()
        {
            _refreshAccounts = true;
            _refreshBudgets = true;
            _refreshGoals = true;

            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                //string header = Shane.Church.StirlingMoney.Strings.Resources.AccountsTitle;
                //if (pi != null)
                //{
                //    header = pi.Header.ToString();
                //}
                //if (_model != null)
                //{
                //    UpdateApplicationBar(header);
                //    if (header == Shane.Church.StirlingMoney.Strings.Resources.AccountsTitle)
                //    {
                await _model.LoadAccounts(_refreshAccounts);
                _refreshAccounts = false;
                //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{
                //if (AccountPanel.Visibility == System.Windows.Visibility.Collapsed)
                //    AccountPanel.Visibility = System.Windows.Visibility.Visible;
                //});
                //}
                //else if (header == Shane.Church.StirlingMoney.Strings.Resources.BudgetsTitle)
                //{
                await _model.LoadBudgets(_refreshBudgets);
                _refreshBudgets = false;
                //    Deployment.Current.Dispatcher.BeginInvoke(() =>
                //    {
                //        if (BudgetPanel.Visibility == System.Windows.Visibility.Collapsed)
                //            BudgetPanel.Visibility = System.Windows.Visibility.Visible;
                //    });
                //}
                //else if (header == Shane.Church.StirlingMoney.Strings.Resources.GoalsTitle)
                //{
                await _model.LoadGoals(_refreshGoals);
                _refreshGoals = false;
                //    Deployment.Current.Dispatcher.BeginInvoke(() =>
                //    {
                //        if (GoalsPanel.Visibility == System.Windows.Visibility.Collapsed)
                //            GoalsPanel.Visibility = System.Windows.Visibility.Visible;
                //    });
                //}
                //}
                //});
            }
            catch(Exception ex)
            {

            }
        }


        async void _model_BusyChanged(object sender, Core.Data.BusyEventArgs args)
        {
            if (args.IsBusy)
            {
                //await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                //    //LoadingBusy.Content = args.Message;
                //    //LoadingBusy.AnimationStyle = (Telerik.Windows.Controls.AnimationStyle)args.AnimationType;
                //    //LoadingBusy.IsRunning = true;
                //});
                return;
            }
            else if (args.IsError)
            {
                //await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                //    //LoadingBusy.IsRunning = false;
                //    _log.LogException(args.Error);
                //    //MessageBox.Show(args.Error.Message, Strings.Resources.GeneralErrorCaption, MessageBoxButton.OK);
                //});
            }
            else
            {
                //await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                //    if (!string.IsNullOrWhiteSpace(args.Message))
                //    {

                //    }
                //    //LoadingBusy.IsRunning = false;
                //});
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
//            throw new NotImplementedException();
            _model.Commit().Wait(1000);
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            //HubSection section = e.Section;
            //var group = section.DataContext;
            //this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            //var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            //this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MarkedUp.AnalyticClient.EnterPage(e.SourcePageType.Name);
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            MarkedUp.AnalyticClient.ExitPage(e.SourcePageType.Name);
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
