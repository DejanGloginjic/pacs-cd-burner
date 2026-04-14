using CDBurner.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CDBurner.Service.Common
{
    public interface IDialogService
    {
        void ShowError(string message);
        void ShowInformation(string message);
        bool ShowConfirmation(string message);
        DialogViewModel ShowProgress(string message, Visibility isProgressVisible = Visibility.Visible);
    }
}
