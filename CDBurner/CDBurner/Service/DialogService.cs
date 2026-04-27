using CDBurner.Service.Common;
using CDBurner.View;
using CDBurner.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CDBurner.Service
{
    public class DialogService : IDialogService
    {
        public void ShowError(string message)
        {
            var vm = new DialogViewModel
            {
                Title = Application.Current.Resources["Error"] as string,
                Message = message,
                IsOkVisible = Visibility.Visible,
                IsYesNoVisible = Visibility.Collapsed,
                IsProgressVisible = Visibility.Collapsed,
                IsCancelVisible = Visibility.Collapsed
            };

            var dialog = new DialogWindow(vm);
            dialog.ShowDialog();
        }

        public void ShowInformation(string message)
        {
            var vm = new DialogViewModel
            {
                Title = Application.Current.Resources["Information"] as string,
                Message = message,
                IsOkVisible = Visibility.Visible,
                IsYesNoVisible = Visibility.Collapsed,
                IsProgressVisible = Visibility.Collapsed,
                IsCancelVisible = Visibility.Collapsed
            };

            var dialog = new DialogWindow(vm);
            dialog.ShowDialog();
        }

        public bool ShowConfirmation(string message)
        {
            var vm = new DialogViewModel
            {
                Title = Application.Current.Resources["Confirmation"] as string,
                Message = message,
                IsOkVisible = Visibility.Collapsed,
                IsYesNoVisible = Visibility.Visible,
                IsProgressVisible = Visibility.Collapsed,
                IsCancelVisible = Visibility.Collapsed
            };

            var dialog = new DialogWindow(vm);
            dialog.ShowDialog();

            return vm.Result;
        }

        public DialogViewModel ShowProgress(string message, Visibility isProgressVisible = Visibility.Visible)
        {
            var vm = new DialogViewModel
            {
                Title = Application.Current.Resources["BurnOnCdProgress"] as string,
                Message = message,
                IsOkVisible = Visibility.Collapsed,
                IsYesNoVisible = Visibility.Collapsed,
                IsProgressVisible = isProgressVisible,
                IsCancelVisible = Visibility.Visible,
                Progress = 0
            };

            var dialog = new DialogWindow(vm);
            dialog.Show();

            return vm;
        }
    }
}
