using System;
using Mcce.SmartOffice.Client.Enums;

namespace Mcce.SmartOffice.Client.Services
{
    public interface INavigationService
    {
        event EventHandler<NavigationRequestedArgs> NavigationRequested;

        void Navigate(NavigationType type);
    }

    public class NavigationService : INavigationService
    {
        public event EventHandler<NavigationRequestedArgs> NavigationRequested;

        public void Navigate(NavigationType type)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestedArgs(type));
        }
    }

    public class NavigationRequestedArgs : EventArgs
    {
        public NavigationType Type { get; }

        public NavigationRequestedArgs(NavigationType type)
        {
            Type = type;
        }        
    }
}
