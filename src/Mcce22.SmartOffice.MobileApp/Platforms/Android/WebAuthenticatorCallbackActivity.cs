using Android.App;
using Android.Content;
using Android.Content.PM;

namespace Mcce22.SmartOffice.MobileApp.Platforms.Windows
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = CALLBACK_SCHEME)]
    public class MauiWebAuthenticatorCallbackActivity : WebAuthenticatorCallbackActivity
    {
        const string CALLBACK_SCHEME = "smartofficemobileapp";
    }
}
