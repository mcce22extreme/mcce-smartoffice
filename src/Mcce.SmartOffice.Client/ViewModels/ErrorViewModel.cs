using System;
using Mcce.SmartOffice.Client.Services;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public class ErrorViewModel : DialogViewModelBase
    {
        public string Message { get; }

        public ErrorViewModel(string message, IDialogService dialogService)
            : base(dialogService)
        {
            Title = "Error occurred...";
            Message = message;
        }

        public ErrorViewModel(Exception exception, IDialogService dialogService)
            : this(exception?.Message, dialogService)
        {
        }
    }
}
