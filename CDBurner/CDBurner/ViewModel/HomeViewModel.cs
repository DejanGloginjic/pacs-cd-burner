using CDBurner.Core;
using CDBurner.Model;
using CDBurner.Service.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

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

        private string _appliedKeyword;
        public string AppliedKeyword
        {
            get => _appliedKeyword;
            set { _appliedKeyword = value; OnPropertyChanged(); }
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
        
        private Visibility _isResetVisible = Visibility.Collapsed;
        public Visibility IsResetVisible
        {
            get => _isResetVisible;
            set
            {
                _isResetVisible = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<StudyModel> _studies;
        public ObservableCollection<StudyModel> Studies
        {
            get => _studies;
            set { _studies = value; OnPropertyChanged(); }
        }

        private DateTime? _dateFrom;
        public DateTime? DateFrom
        {
            get => _dateFrom;
            set
            {
                _dateFrom = value;
                OnPropertyChanged();
                if (SearchCommand.CanExecute(null))
                    SearchCommand.Execute(null);
            }
        }

        private DateTime? _dateTo;
        public DateTime? DateTo
        {
            get => _dateTo;
            set
            {
                _dateTo = value;
                OnPropertyChanged();
                if (SearchCommand.CanExecute(null))
                    SearchCommand.Execute(null);
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand BurnOnCDCommand { get; }

        public HomeViewModel(INavigationService navigationService, IApiService apiService, IDialogService dialogService,
                             IBurnerService burnerService) {
            NavigationService = navigationService;

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<AppConfigModel>(json);
            PageSize = config.PageSize;

            _ = LoadStudiesAsync(apiService);

            SearchCommand = new RelayCommand(async _ =>
            {
                AppliedKeyword = Keyword;
                if (!string.IsNullOrWhiteSpace(AppliedKeyword) || DateFrom != null || DateTo != null)
                    IsResetVisible = Visibility.Visible;
                CurrentPage = 1;
                await LoadStudiesAsync(apiService);
            });

            ResetCommand = new RelayCommand(async _ =>
            {
                CurrentPage = 1;

                Keyword = String.Empty;
                AppliedKeyword = String.Empty;

                DateFrom = null;
                DateTo = null;
                IsResetVisible = Visibility.Collapsed;
                await LoadStudiesAsync(apiService);
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

            BurnOnCDCommand = new RelayCommand(async obj =>
            {
                bool isConfirmed = dialogService.ShowConfirmation(Application.Current.Resources["Question"] as string);
                if (!isConfirmed)
                    return;

                if (obj is not StudyModel study)
                    return;

                var progressVM = dialogService.ShowProgress(Application.Current.Resources["DownloadingProgress"] as string);
                bool finalSuccess = false;

                try
                {
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                               "CDBurner", "DICOM");
                    bool success = await apiService.DownloadStudyAsync(study.Url, path);

                    if (!success)
                        throw new Exception(Application.Current.Resources["DownloadStudiesError"] as string);
                    
                    progressVM.Message = Application.Current.Resources["BurningProgress"] as string;
                    var progress = new Progress<double>(p =>
                    {
                        progressVM.Progress = p;
                    });

                    bool burnSuccess = await burnerService.BurnFolderAsync(path, progress);
                    if (!burnSuccess)
                        throw new Exception(Application.Current.Resources["BurnOnCdFailed"] as string);

                    Directory.Delete(path, true);
                    finalSuccess = true;
                }
                catch (Exception ex)
                {
                    progressVM.IsProgressVisible = Visibility.Collapsed;
                    progressVM.Message = ex.Message;
                    progressVM.IsOkVisible = Visibility.Visible;

                    return;
                }

                progressVM.IsProgressVisible = Visibility.Collapsed;
                progressVM.Message = Application.Current.Resources["BurnOnCdSuccessful"] as string;
                progressVM.IsOkVisible = Visibility.Visible;
            });
        }

        private async Task LoadStudiesAsync(IApiService apiService)
        {
            var list = await apiService.GetStudiesAsync(CurrentPage, PageSize, AppliedKeyword, DateFrom, DateTo);
            Studies = new ObservableCollection<StudyModel>(list);

            TotalStudiesCount = await apiService.GetTotalStudiesCountAsync(AppliedKeyword, DateFrom, DateTo);
        }
    }
}