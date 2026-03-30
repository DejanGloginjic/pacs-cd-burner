using CDBurner.Service.Common;
using System;
using CDBurner.ViewModel;
using CDBurner.View;
using System.Collections.Generic;
using System.Windows;

namespace CDBurner.Service
{
    public class DialogService : IDialogService
    {
        public void ShowInfo(string message)
        {
            var vm = new DialogViewModel
            {
                Title = "Info",
                Message = message,
                IsOkVisible = Visibility.Visible,
                IsYesNoVisible = Visibility.Collapsed
            };

            var dialog = new DialogWindow(vm);
            dialog.ShowDialog();
        }

        public void ShowError(string message)
        {
            var vm = new DialogViewModel
            {
                Title = "Error",
                Message = message,
                IsOkVisible = Visibility.Visible,
                IsYesNoVisible = Visibility.Collapsed
            };

            var dialog = new DialogWindow(vm);
            dialog.ShowDialog();
        }

        public bool ShowConfirmation(string message)
        {
            var vm = new DialogViewModel
            {
                Title = "Confirm",
                Message = message,
                IsOkVisible = Visibility.Collapsed,
                IsYesNoVisible = Visibility.Visible
            };

            var dialog = new DialogWindow(vm);
            dialog.ShowDialog();

            return vm.Result;
        }
    }
}
