namespace Mcce.SmartOffice.MobileApp.Services
{
    public interface IDialogService
    {
        Task ShowDialog(string title, string message);

        Task ShowErrorMessage(string message);

        Task<bool> ShowConfirmationDialog(string title, string message);
    }

    public class DialogService : IDialogService
    {
        public async Task ShowDialog(string title, string message)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "Close");
        }

        public async Task ShowErrorMessage(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Oops, something went wrong!", message, "Close");
        }

        public async Task<bool> ShowConfirmationDialog(string title, string message)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, "Yes", "No");
        }
    }
}
