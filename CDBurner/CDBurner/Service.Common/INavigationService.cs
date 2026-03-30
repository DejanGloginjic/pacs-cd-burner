using CDBurner.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Service.Common
{
    public interface INavigationService
    {
        ViewModelBase CurrentView { get; }
        void NavigateTo<T>() where T : ViewModelBase;
        void NavigateTo<T>(object parameters) where T : ViewModelBase;
    }
}
