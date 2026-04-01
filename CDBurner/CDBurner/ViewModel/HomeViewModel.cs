using CDBurner.Core;
using CDBurner.Model;
using CDBurner.Service.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace CDBurner.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {
        private string _keyword;
        public string Keyword
        {
            get => _keyword;
            set { _keyword = value; OnPropertyChanged(); }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PaginationText));
            }
        }

        private int _pageSize = 20;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PaginationText));
            }
        }

        private int _totalStudiesCount;
        public int TotalStudiesCount
        {
            get { return _totalStudiesCount; }
            set
            {
                _totalStudiesCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PaginationText));
            }
        }

        public string PaginationText => TotalStudiesCount == 0 ? "Nema rezultata" : $"Stranica {CurrentPage} od {(int)Math.Ceiling((double)TotalStudiesCount / PageSize)}";


        private ObservableCollection<StudyModel> _studies;
        public ObservableCollection<StudyModel> Studies
        {
            get => _studies;
            set { _studies = value; OnPropertyChanged(); }
        }

        public ICommand SearchCommand { get; }
        public ICommand DetailsCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand BurnOnCDCommand { get; }

        public HomeViewModel(INavigationService navigationService, IApiService apiService) {
            NavigationService = navigationService;

            // Ovo osigruati da ne pada aplikacija
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<AppConfigModel>(json);
            PageSize = config.PageSize;

            _ = LoadStudiesAsync(apiService);

            SearchCommand = new RelayCommand(async _ =>
            {
                await LoadStudiesAsync(apiService);
            });

            DetailsCommand = new RelayCommand(async obj =>
            {
                if (obj is StudyModel study)
                {
                    // otvori dialog box i prikazi detalje
                }
            });

            NextPageCommand = new RelayCommand(async _ =>
            {
                CurrentPage += 1;
                await LoadStudiesAsync(apiService);
            }, _ => CurrentPage * PageSize < TotalStudiesCount);

            PreviousPageCommand = new RelayCommand(async _ =>
            {
                CurrentPage -= 1;
                await LoadStudiesAsync(apiService);
            }, _ => CurrentPage > 1);

            BurnOnCDCommand = new RelayCommand(async obj => // ovdje staviti study objekat i vidjeti da li ide async
            {
                if (obj is StudyModel study)
                {
                    await apiService.DownloadStudyAsync(study, ""); // dodati putanju ovdje
                }
                // Otvoriti dialog pitati da li je korisnik siguran da to zeli
                // Logika za narezivanje
            });
        }

        private async Task LoadStudiesAsync(IApiService apiService)
        {
            var list = await apiService.GetStudiesAsync(CurrentPage, PageSize, Keyword);
            Studies = new ObservableCollection<StudyModel>(list);

            TotalStudiesCount = await apiService.GetTotalStudiesCountAsync(Keyword);
        }
    }
}