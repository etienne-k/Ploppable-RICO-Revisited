using ICities;
using ColossalFramework.UI;
using CitiesHarmony.API;


namespace PloppableRICO
{
    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public class PloppableRICOMod : IUserMod
    {
        public static string ModName => "RICO Revisited";
        public static string Version => "2.2.2";

        public string Name => ModName + " " + Version;
        public string Description => Translations.Translate("PRR_DESCRIPTION");


        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            // Apply Harmony patches via Cities Harmony.
            // Called here instead of OnCreated to allow the auto-downloader to do its work prior to launch.
            HarmonyHelper.DoOnHarmonyReady(() => Patcher.PatchAll());

            // Load settings file.
            SettingsUtils.LoadSettings();
        }


        /// <summary>
        /// Called by the game when the mod is disabled.
        /// </summary>
        public void OnDisabled()
        {
            // Unapply Harmony patches via Cities Harmony.
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.UnpatchAll();
            }
        }


        /// <summary>
        /// Called by the game when the mod options panel is setup.
        /// </summary>
        public void OnSettingsUI(UIHelperBase helper)
        {
            // General options.
            UIHelperBase otherGroup = helper.AddGroup(" ");

            UIDropDown translationDropDown = (UIDropDown)otherGroup.AddDropdown(Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index, (value) =>
            {
                Translations.Index = value;
                SettingsUtils.SaveSettings();
            });
            translationDropDown.autoSize = false;
            translationDropDown.width = 270f;

            // Add logging checkbox.
            otherGroup.AddCheckbox(Translations.Translate("PRR_OPTION_MOREDEBUG"), ModSettings.debugLogging, isChecked =>
            {
                ModSettings.debugLogging = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add reset on load checkbox.
            otherGroup.AddCheckbox(Translations.Translate("PRR_OPTION_FORCERESET"), ModSettings.resetOnLoad, isChecked =>
            {
                ModSettings.resetOnLoad = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add thumbnail background dropdown.
            otherGroup.AddDropdown(Translations.Translate("PRR_OPTION_THUMBACK"), ModSettings.ThumbBackNames, ModSettings.thumbBacks, (value) =>
            {
                ModSettings.thumbBacks = value;
                SettingsUtils.SaveSettings();
            });

            // Add regenerate thumbnails button.
            otherGroup.AddButton(Translations.Translate("PRR_OPTION_REGENTHUMBS"), () => PloppableTool.Instance.RegenerateThumbnails());

            // Add speed boost checkbox.
            UIHelperBase speedGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SPDHDR"));
            speedGroup.AddCheckbox(Translations.Translate("PRR_OPTION_SPEED"), ModSettings.speedBoost, isChecked =>
            {
                ModSettings.speedBoost = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add fast thumbnails checkbox.
            UIHelperBase fastGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_FASTHDR"));
            fastGroup.AddCheckbox(Translations.Translate("PRR_OPTION_FASTHUMB"), ModSettings.fastThumbs, isChecked =>
            {
                ModSettings.fastThumbs = isChecked;
                SettingsUtils.SaveSettings();
            });
        }
    }
}
