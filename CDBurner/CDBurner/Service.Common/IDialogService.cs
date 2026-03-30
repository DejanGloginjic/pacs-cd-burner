using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Service.Common
{
    public interface IDialogService
    {
        void ShowInfo(string message);
        void ShowError(string message);
        bool ShowConfirmation(string message);
    }
}
