﻿using Android.App;
using Android.Runtime;

namespace Mcce.SmartOffice.MobileApp
{
    [Application(UsesCleartextTraffic = false, AllowBackup = false)]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
          : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
