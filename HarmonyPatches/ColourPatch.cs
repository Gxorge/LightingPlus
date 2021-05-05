using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LightingPlus
{

    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), "Init", new Type[] {typeof(string), typeof(IDifficultyBeatmap), typeof(IPreviewBeatmapLevel), typeof(OverrideEnvironmentSettings) ,typeof(ColorScheme),
            typeof(GameplayModifiers) , typeof(PlayerSpecificSettings) , typeof(PracticeSettings) , typeof(string) , typeof(bool)})]
    [HarmonyPatch("Init", MethodType.Normal)]
    class ColourPatch
    {
        [HarmonyPrefix]
        public static void Prefix(IDifficultyBeatmap difficultyBeatmap, ref ColorScheme overrideColorScheme)
        {
            EnvironmentInfoSO eiso = difficultyBeatmap.GetEnvironmentInfo();
            ColorScheme curr = overrideColorScheme ?? new ColorScheme(eiso.colorScheme);

            // the second boost colour is often used more
            // new Color(0.09411764705f, 0, 0.78431372549f) dark blue, new Color(0.58823529411f, 0, 0) dark red
            BoostColour b = Plugin.Boost;
            ColorScheme mapColor = new ColorScheme("CustomColourScheme", "CustomColourScheme", false, curr.saberAColor, curr.saberBColor, curr.environmentColor0, curr.environmentColor1, true, new Color(b.r0, b.g0, b.b0), new Color(b.r1, b.g1, b.b1), curr.obstaclesColor);
            overrideColorScheme = mapColor;
            Plugin.Log.Info("Loaded Custom Boost Colours");
        }
    }
}
