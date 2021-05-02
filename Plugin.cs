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
using IPALogger = IPA.Logging.Logger;

namespace LightingPlus
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private static Harmony harmony;

        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        [Init]
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("Lighting+ initialized.");
        }

        [OnStart]
        public void OnApplicationStart()
        {
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
