using Microsoft.Win32;

namespace AlienCaseLights.App.Services;

public sealed class StartupRegistration
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

    private const string ValueName = "AlienCaseLights";

    public bool IsEnabled
    {
        get
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKey);

            return key?.GetValue(ValueName) is not null;
        }
        set
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(RunKey);

            if (value)
            {
                key.SetValue(ValueName, $"\"{Environment.ProcessPath}\" --minimized");
            }
            else
            {
                key.DeleteValue(ValueName, false);
            }
        }
    }
}
