using Arbitrage.CoreApi.Enums;
using System;

namespace Arbitrage.CoreApi.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class AppSettingsFlagAttribute : Attribute
    {
        public AppSettingsSection Section { get; set; }
        public int KeyCode { get; set; }
        public AppSettingsFlagAttribute(AppSettingsSection section, int keycode)
        {
            Section = section;
            KeyCode = keycode;
        }
    }
}
