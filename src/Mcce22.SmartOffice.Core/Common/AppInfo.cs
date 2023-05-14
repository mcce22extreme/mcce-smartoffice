﻿using System.Reflection;

namespace Mcce22.SmartOffice.Core.Common
{
    public interface IAppInfo
    {
        public string AppName { get; }

        public string AppVersion { get; }
    }

    public class AppInfo : IAppInfo
    {
        public string AppName { get; }

        public string AppVersion { get; }

        public AppInfo( string appName, string appVersion)
        {
            AppName = appName;
            AppVersion = appVersion;
        }

        public AppInfo(AssemblyName assemblyName)
            :this(assemblyName?.Name, $"{assemblyName?.Version?.Major}.{assemblyName?.Version?.Minor}.{assemblyName?.Version?.Build}")
        {
        }
    }
}
