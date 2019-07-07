using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Utils
{
    static class RzHelper
    {
        public static bool IsSdkEnabled()
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    var key = hklm.OpenSubKey(@"Software\Razer Chroma SDK");
                    return (int)key?.GetValue("Enable", 0) == 1;
                }
            }
            catch
            {
                return false;
            }
        }

        public static RzSdkVersion GetSdkVersion()
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    var key = hklm.OpenSubKey(@"Software\Razer Chroma SDK");
                    var major = (int)key?.GetValue("MajorVersion", 0);
                    var minor = (int)key?.GetValue("MinorVersion", 0);
                    var revision = (int)key?.GetValue("RevisionNumber", 0);

                    return new RzSdkVersion(major, minor, revision);
                }
            }
            catch
            {
                return new RzSdkVersion(0, 0, 0);
            }
        }
    }

    /// <summary>
    /// Singleton class proxy to access the RzHelper methods through WPF bindings easily.
    /// </summary>
    public class RzSDKStatus {
        private static RzSDKStatus instance;
        public static RzSDKStatus Instance => instance ?? (instance = new RzSDKStatus());

        public RzSdkVersion SdkVersion { get; } = RzHelper.GetSdkVersion();
        public bool SdkEnabled { get; } = RzHelper.IsSdkEnabled();
        public string SdkEnabledText { get; } = RzHelper.IsSdkEnabled() ? "Enabled" : "Disabled";

        private RzSDKStatus() { }
    }

    public readonly struct RzSdkVersion
    {
        public readonly int Major;
        public readonly int Minor;
        public readonly int Revision;

        public RzSdkVersion(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public override bool Equals(object obj)
        {
            if (obj is RzSdkVersion ver)
                return Major == ver.Major && Minor == ver.Minor && Revision == ver.Revision;

            if (obj is ValueTuple<int, int, int> tuple)
                return Major == tuple.Item1 && Minor == tuple.Item2 && Revision == tuple.Item3;

            return false;
        }

        public override string ToString() => $"{Major}.{Minor}.{Revision}";

        public override int GetHashCode()
        {
            var hashCode = -327234472;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + Minor.GetHashCode();
            hashCode = hashCode * -1521134295 + Revision.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(RzSdkVersion first, RzSdkVersion second) => first.Equals(second);
        public static bool operator !=(RzSdkVersion first, RzSdkVersion second) => !first.Equals(second);
        public static bool operator ==(RzSdkVersion first, ValueTuple<int, int, int> second) => first.Equals(second);
        public static bool operator !=(RzSdkVersion first, ValueTuple<int, int, int> second) => !first.Equals(second);
    }
}

