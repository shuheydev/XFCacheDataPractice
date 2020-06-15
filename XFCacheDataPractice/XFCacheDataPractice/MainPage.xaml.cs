using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XFCacheDataPractice.Models.Covid19;
using XFCacheDataPractice.Services;

namespace XFCacheDataPractice
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Prefecture> _prefectures;
        public ObservableCollection<Prefecture> Prefectures
        {
            get => _prefectures;
            set
            {
                _prefectures = value;
                OnPropertyChanged();
            }
        }

        private readonly Covid19JapanApiManager _dataService;

        public ICommand RefreshDataCommand { get; }
        public MainPage()
        {
            InitializeComponent();

            this._dataService = DependencyService.Get<Covid19JapanApiManager>();

            this.RefreshDataCommand = new Command(async _ => await ExecuteRefresh());

            this.BindingContext = this;
        }

        private async Task ExecuteRefresh()
        {
            this.IsRefreshing = true;
            await RefreshData();
            //Refreshing中のぐるぐるが小さくなって消えるアニメーションをしっかり表示させる場合は
            //Delayを挟むと良い.なぜかはわからない.
            //1秒は個人的に自然な長さに見えたので
            await Task.Delay(1000);
            this.IsRefreshing = false;
        }


        private async Task RefreshData()
        {
            var prefecturesData = await this._dataService.GetPrefectures();

            if (prefecturesData != null)
                this.Prefectures = new ObservableCollection<Prefecture>(prefecturesData);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await ExecuteRefresh();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await ExecuteRefresh();
        }
    }
}
