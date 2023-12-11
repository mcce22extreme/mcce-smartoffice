using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.DigitalFrameApp.Models;

namespace Mcce.SmartOffice.DigitalFrameApp.Managers
{
    public interface ISessionManager
    {
        void StartSession(WorkspaceActivatedModel model);

        string GetUserName();

        UserImageModel[] GetUserImages();

        Task PrepareSession();

        Task EndSession();
    }

    public class SessionManager : ManagerBase, ISessionManager
    {
        private List<UserImageModel> _userImages = new List<UserImageModel>();

        private string _currentUserName;
        private string[] _currentUserImages;

        public SessionManager(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage)
            : base(appConfig, httpClientFactory, secureStorage)
        {
        }

        public void StartSession(WorkspaceActivatedModel model)
        {
            _currentUserName = model.UserName;
            _currentUserImages = model.UserImages;
        }

        public string GetUserName()
        {
            return _currentUserName;
        }

        public UserImageModel[] GetUserImages()
        {
            return _userImages.ToArray();
        }

        public async Task PrepareSession()
        {
            _userImages.Clear();

            if (_currentUserImages?.Length > 0)
            {
                using var httpClient = await CreateHttpClient();

                foreach (var imageUrl in _currentUserImages)
                {
                    var bytes = await httpClient.GetByteArrayAsync(imageUrl);

                    _userImages.Add(new UserImageModel
                    {
                        ImageUrl = imageUrl,
                        Content = bytes
                    });
                }
            }
        }

        public async Task EndSession()
        {
            _currentUserName = null;
            _currentUserImages = null;
            _userImages.Clear();

            await Task.Delay(2000);
        }
    }
}
