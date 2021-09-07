using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;

namespace LightingPlus.UI
{
    internal class SettingsUI : PersistentSingleton<SettingsUI>
    {
        [UIValue("boostcolourchooser")]
        private readonly List<object> _presetList = new List<object>(Plugin.Config.BoostColours);

        [UIValue("BoostColoursEnabled")]
        public bool BoostColoursEnabled
        {
            get => Plugin.Config.BoostColoursEnabled;
            set => Plugin.Config.BoostColoursEnabled = value;
        }

        [UIValue("MultiPlayerLightingEnabled")]
        public bool MultiPlayerLightingEnabled
        {
            get => Plugin.Config.MultiPlayerLightingEnabled;
            set => Plugin.Config.MultiPlayerLightingEnabled = value;
        }

        [UIValue("StaticLightsColoursEnabled")]
        public bool StaticLightsColoursEnabled
        {
            get => Plugin.Config.StaticLightsColoursEnabled;
            set => Plugin.Config.StaticLightsColoursEnabled = value;
        }

        [UIValue("SelectedBoostId")]
        public BoostColour SelectedBoostId
        {
            get => Plugin.Boost;
            set => Plugin.Config.Update(value);
        }

        [UIAction("boostcolourformatter")]
        private string BCFormat(BoostColour bc)
        {
            return bc.name;
        }
    }
}
