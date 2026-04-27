using CDBurner.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CDBurner.ViewModel
{
    public class DialogViewModel : ViewModelBase
    {
        private string _title;
        public string Title 
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        private Visibility _isOkVisible;
        public Visibility IsOkVisible 
        {
            get => _isOkVisible;
            set { _isOkVisible = value; OnPropertyChanged(); }
        }

        private Visibility _isYesNoVisible;
        public Visibility IsYesNoVisible 
        { 
            get => _isYesNoVisible;
            set { _isYesNoVisible = value; OnPropertyChanged(); }
        }

        private Visibility _isCancelVisible;
        public Visibility IsCancelVisible
        {
            get => _isCancelVisible;
            set { _isCancelVisible = value; OnPropertyChanged(); }
        }

        public ICommand OkCommand { get; }
        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }
        public ICommand CancelCommand { get; }

        public bool Result { get; private set; }

        public Action CloseAction { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        private double _progress;
        public double Progress
        {
            get => _progress;
            set { _progress = value; OnPropertyChanged(); }
        }

        private Visibility _isProgressVisible;
        public Visibility IsProgressVisible
        {
            get => _isProgressVisible;
            set { _isProgressVisible = value; OnPropertyChanged(); }
        }

        public DialogViewModel()
        {
            OkCommand = new RelayCommand(_ =>
            {
                Result = true;
                CloseAction?.Invoke();
            });

            YesCommand = new RelayCommand(_ =>
            {
                Result = true;
                CloseAction?.Invoke();
            });

            NoCommand = new RelayCommand(_ =>
            {
                Result = false;
                CloseAction?.Invoke();
            });
            CancelCommand = new RelayCommand(_ =>
            {
                CancellationTokenSource?.Cancel();
                Result = false;
                CloseAction?.Invoke();
            });
        }
    }
}
