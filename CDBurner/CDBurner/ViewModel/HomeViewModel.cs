using CDBurner.Core;
using CDBurner.Model;
using CDBurner.Service.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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

        private ObservableCollection<Study> _studies;
        public ObservableCollection<Study> Studies
        {
            get => _studies;
            set { _studies = value; OnPropertyChanged(); }
        }

        private Study _selectedStudy;
        public Study SelectedStudy { get => _selectedStudy; set { _selectedStudy = value; OnPropertyChanged(); } }

        public ICommand SearchCommand { get; }
        public ICommand BurnOnCDCommand { get; }
        public HomeViewModel(INavigationService navigationService, IApiService apiService) {
            NavigationService = navigationService;

            SearchCommand = new RelayCommand(_ =>
            {
                apiService.GetStudiesAsync(); // treba dodati parametre pretrage ovdje
            });

            BurnOnCDCommand = new RelayCommand(_ =>
            {
                // Logika za narezivanje
            });
        }
    }
}
