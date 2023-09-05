using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartOffice.Client.Services;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public interface IDialogViewModel
    {
        string Title { get; }

        void Load();
    }

    public abstract partial class DialogViewModelBase : ViewModelBase, IDialogViewModel
    {
        protected IDialogService DialogService { get; }

        public string Title { get; protected set; }

        public bool Confirmed { get; protected set; }

        public DialogViewModelBase(IDialogService dialogService)
        {
            DialogService = dialogService;
        }

        public virtual void Load()
        {
        }

        protected override void UpdateCommandStates()
        {
            SaveCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }

        protected virtual bool CanSave()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        protected async void Save()
        {
            if (CanSave())
            {
                try
                {
                    IsBusy = true;

                    await OnSave();

                    Confirmed = true;
                    DialogService.CloseDialog(this);
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    await DialogService.ShowDialog(new ErrorViewModel(ex, DialogService));
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        protected virtual Task OnSave()
        {
            return Task.CompletedTask;
        }

        protected bool CanCancel()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanCancel))]
        protected void Cancel()
        {
            if (CanCancel())
            {
                DialogService.CloseDialog(this);
            }
        }
    }
}
