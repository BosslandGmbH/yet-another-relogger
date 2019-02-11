using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Helpers.Hotkeys;

namespace YetAnotherRelogger.Properties
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings
    {
        public Settings()
        {
            SettingsLoaded += Settings_SettingsLoaded;
        }

        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        [DefaultSettingValue(null)]
        public List<AutoPosition.ScreensClass> AutoPosScreens
        {
            get => ((List<AutoPosition.ScreensClass>) this["AutoPosScreens"]);
            set => this["AutoPosScreens"] = value;
        }

        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public BindingList<Hotkey> HotKeys
        {
            get => ((BindingList<Hotkey>) this["HotKeys"]);
            set => this["HotKeys"] = value;
        }

        private void Settings_SettingsLoaded(object sender, SettingsLoadedEventArgs e)
        {
            if (HotKeys == null)
                HotKeys = new BindingList<Hotkey>();
        }
    }
}
