using Microsoft.Maui.Platform;

namespace Mcce.SmartOffice.Client.Common
{
    public static class PlatformHelpers
    {
        public static void HideKeyboard()
        {
#if ANDROID
            if (Platform.CurrentActivity.CurrentFocus != null)
            {
                Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
            }
#endif
        }
    }
}
