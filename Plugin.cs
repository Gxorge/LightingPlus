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
            if (Config.BoostColours.Count == 0)
            {
                Log.Info("BoostColours config is empty!! Added default!");
                BoostColour bc = new BoostColour();
                bc.name = "Default";
                bc.r0 = 0.18823529411f;
                bc.g0 = 0.59607843137f;
                bc.b0 = 1f;
                bc.r1 = 0.53333333333f;
                bc.g1 = 0.0862745098f;
                bc.b1 = 1f;
                Config.BoostColours.Insert(0, bc);
                Config.SelectedId = "Default";
            }

            if (Config.SelectedId == null || !Config.BoostColours.Any(x=> x.name == Config.SelectedId))
            {
                Log.Info("SelectedId is null or isnt in the BoostColours list. Default to the 0th item in the list.");
                Config.SelectedId = Config.BoostColours[0].name;
            }

            foreach (BoostColour bc in Config.BoostColours)
            {
                if (bc.name == Config.SelectedId)
                {
                    Boost = bc;
                    Log.Info("Loaded BoostColour set '" + bc.name + "'!");
                    break;
                }
            }

            Log.Info("Loaded the config!");

            harmony = new Harmony("moe.gabriella.LightingPlus");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            UI.ChooseSetUI.OnLoad();

            Log.Info("Ready!");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            harmony.UnpatchAll("moe.gabriella.LightingPlus");
        }
    }
}
