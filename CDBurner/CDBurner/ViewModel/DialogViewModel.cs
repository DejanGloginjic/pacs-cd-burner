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
        public string Title { get; set; }
        public string Message { get; set; }

        public Visibility IsOkVisible { get; set; } = Visibility.Visible;
        public Visibility IsYesNoVisible { get; set; } = Visibility.Collapsed;

        public ICommand OkCommand { get; }
        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }

        public bool Result { get; private set; }

        public Action CloseAction { get; set; }

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
        }
    }
}
