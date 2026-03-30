using CDBurner.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            NavigationService.NavigateTo<HomeViewModel>();
        }
    }
}
