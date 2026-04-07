using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Service.Common
{
    public interface IDialogService
    {
        void ShowError(string message);
        void ShowInformation(string message);
        bool ShowConfirmation(string message);
    }
}
