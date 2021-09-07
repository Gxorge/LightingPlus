using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;
using BeatSaberMarkupLanguage.Settings;
using LightingPlus.UI;

namespace LightingPlus
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private static Harmony harmony;

        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Config Config { get; private set; }
        internal static BoostColour Boost { get; set; }

        [Init]
        public void Init(IPALogger logger, Conf conf)
        {
            Instance = this;
            Log = logger;
            Config = conf.Generated<Config>();
            Log.Info("Lighting+ initialized.");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            /*if (Config.BoostColoursEnabled == null)
                Config.BoostColoursEnabled = true;

            if (Config.MultiPlayerLightingEnabled == null)
                Config.MultiPlayerLightingEnabled = true;

            if (Config.StaticLightsColoursEnabled == null)
                Config.StaticLightsColoursEnabled = true;

            if (Config.StaticLightsColours == null)
            {
                StaticLights sl = new StaticLights(140f, 140f, 140f);
                Config.StaticLightsColours = sl;
            }

            if (Config.BoostColours.Count == 0)
            {
                Log.Info("BoostColours config is empty!! Added default!");
                BoostColour bc = new BoostColour("Default", 48f, 152f, 225f, 136f, 22f, 225f);
                Config.BoostColours.Insert(0, bc);
                Config.SelectedBoostId = "Default";
            }*/

            if (!Config.BoostColours.Any(x=> x.name == Config.SelectedBoostId))
            {
                Log.Info("SelectedId isnt in the BoostColours list. Default to the 0th item in the list.");
                Config.SelectedBoostId = Config.BoostColours[0].name;
            }


            foreach (BoostColour bc in Config.BoostColours)
            {
                if (bc.name == Config.SelectedBoostId)
                {
                    Boost = bc;
                    Log.Info("Loaded BoostColour set '" + bc.name + "'!");
                    break;
                }
            }

            Log.Info("Loaded the config!");

            BSMLSettings.instance.AddSettingsMenu("Lighting+", "LightingPlus.UI.settings.bsml", SettingsUI.instance);

            harmony = new Harmony("moe.gabriella.LightingPlus");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Info("Ready!");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            harmony.UnpatchAll("moe.gabriella.LightingPlus");
        }
    }
}
