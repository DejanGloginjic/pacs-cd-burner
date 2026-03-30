using CDBurner.Core;
using CDBurner.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.ViewModel
{
    public abstract class ViewModelBase : ObservableObject
    {
        private INavigationService? _navigationService;

        public INavigationService? NavigationService
        {
            get => _navigationService;
            set { _navigationService = value; OnPropertyChanged(); }
        }
    }
}
