using Mcce.SmartOffice.Client.Services;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public class ConfirmDeleteViewModel : DialogViewModelBase
    {
        public string Message { get; set; }

        public ConfirmDeleteViewModel(string title, string msg, IDialogService dialogService) : base(dialogService)
        {
            Title = title;
            Message = msg;
        }
    }
}
