using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Models;
using Mcce22.SmartOffice.Client.Services;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public partial class BookingListViewModel : ListViewModelBase<BookingModel>
    {
        private readonly IBookingManager _bookingManager;

        [ObservableProperty]
        private bool _isAdmin;

        public BookingListViewModel(
            IBookingManager bookingManager,
            IDialogService dialogService)
            : base(dialogService)
        {
            _bookingManager = bookingManager;
        }

        protected override bool CanEdit()
        {
            return !IsBusy && SelectedItem != null && !SelectedItem.InvitationSent;
        }

        protected override async Task OnDelete()
        {
            await _bookingManager.Delete(SelectedItem.Id);
        }

        protected override async Task<BookingModel[]> OnReload()
        {
            return await _bookingManager.GetList();
        }
    }
}
