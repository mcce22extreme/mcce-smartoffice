using Mcce.SmartOffice.DigitalFrameApp.Models;

namespace Mcce.SmartOffice.DigitalFrameApp.Managers
{
    public interface ISessionManager
    {
        void StartSession(WorkspaceActivatedModel model);

        string GetCurrentUserName();

        string[] GetCurrentUserImages();

        Task PrepareSession();

        Task EndSession();
    }

    public class SessionManager : ISessionManager
    {
        private string _currentUserName;
        private string[] _currentUserImages;        

        public void StartSession(WorkspaceActivatedModel model)
        {
            _currentUserName = model.UserName;
            _currentUserImages = model.UserImages;
        }

        public string GetCurrentUserName()
        {
            return _currentUserName;
        }

        public string[] GetCurrentUserImages()
        {
            return _currentUserImages;
        }

        public async Task PrepareSession()
        {
            await Task.Delay(2000);
        }

        public async Task EndSession()
        {
            _currentUserName = null;
            _currentUserImages = null;

            await Task.Delay(2000);
        }
    }
}
