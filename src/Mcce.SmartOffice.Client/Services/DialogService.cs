using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.SimpleChildWindow;
using Mcce.SmartOffice.Client.ViewModels;

namespace Mcce.SmartOffice.Client.Services
{
    public interface IDialogService
    {
        Control DialogContainer { get; set; }

        Task ShowDialog(IDialogViewModel content);

        void CloseDialog(IDialogViewModel content);
    }

    public class DialogService : IDialogService
    {
        private readonly List<ChildWindow> _childWindows = new List<ChildWindow>();

        public Control DialogContainer { get; set; }

        public Task ShowDialog(IDialogViewModel content)
        {
            var childWindow = new ChildWindow
            {
                Content = content,
                Title = content.Title,
                Foreground = Brushes.White
            };

            _childWindows.Add(childWindow);

            return ChildWindowManager.ShowChildWindowAsync(DialogContainer, childWindow);
        }

        public void CloseDialog(IDialogViewModel content)
        {
            var childWindow = _childWindows.FirstOrDefault(x => x.Content == content);

            if (childWindow != null)
            {
                childWindow.Close();
                _childWindows.Remove(childWindow);
            }
        }
    }
}
