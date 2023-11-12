using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.Client.Services;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public interface IListViewModel
    {
        Task Reload();
    }

    public abstract partial class ListViewModelBase<T> : ViewModelBase, IListViewModel
    {
        [ObservableProperty]
        private ObservableCollection<T> _items = new ObservableCollection<T>();

        [ObservableProperty]
        private T _selectedItem;

        protected IDialogService DialogService { get; }

        public ListViewModelBase(IDialogService dialogService)
        {
            DialogService = dialogService;
        }

        protected override void UpdateCommandStates()
        {
            AddCommand.NotifyCanExecuteChanged();
            EditCommand.NotifyCanExecuteChanged();
            ReloadCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanAdd))]
        protected async Task Add()
        {
            if (CanAdd())
            {
                await OnAdd();

                await Reload();
            }
        }

        protected bool CanAdd()
        {
            return !IsBusy;
        }

        protected virtual Task OnAdd()
        {
            return Task.CompletedTask;
        }

        [RelayCommand(CanExecute = nameof(CanEdit))]
        protected async Task Edit()
        {
            if (CanEdit())
            {
                await OnEdit();

                await Reload();
            }
        }

        protected virtual bool CanEdit()
        {
            return !IsBusy && SelectedItem != null;
        }

        protected virtual Task OnEdit()
        {
            return Task.CompletedTask;
        }


        [RelayCommand(CanExecute = nameof(CanDelete))]
        protected virtual async Task Delete()
        {
            if (CanDelete())
            {
                try
                {
                    IsBusy = true;

                    var confirmDelete = new ConfirmDeleteViewModel("Delete selected entry?", "Do you really want to delete the selected entry?", DialogService);

                    await DialogService.ShowDialog(confirmDelete);

                    if (confirmDelete.Confirmed)
                    {
                        await OnDelete();
                    }
                }
                finally
                {
                    IsBusy = false;
                }

                await Reload();
            }
        }

        protected virtual bool CanDelete()
        {
            return !IsBusy && SelectedItem != null;
        }

        protected virtual Task OnDelete()
        {
            return Task.CompletedTask;
        }

        [RelayCommand(CanExecute = nameof(CanReload))]
        public async Task Reload()
        {
            if (CanReload())
            {
                try
                {
                    IsBusy = true;

                    var items = await OnReload();
                    Items = new ObservableCollection<T>(items);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        protected bool CanReload()
        {
            return !IsBusy;
        }

        protected virtual Task<T[]> OnReload()
        {
            return Task.FromResult(Array.Empty<T>());
        }
    }
}
