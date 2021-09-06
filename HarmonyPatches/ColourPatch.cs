using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LightingPlus.HarmonyPatches
{

    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), "Init", new Type[] {typeof(string), typeof(IDifficultyBeatmap), typeof(IPreviewBeatmapLevel), typeof(OverrideEnvironmentSettings) ,typeof(ColorScheme),
            typeof(GameplayModifiers) , typeof(PlayerSpecificSettings) , typeof(PracticeSettings) , typeof(string) , typeof(bool)})]
    [HarmonyPatch("Init", MethodType.Normal)]
    class ColourPatch
    {
        [HarmonyPrefix]
        public static void Prefix(IDifficultyBeatmap difficultyBeatmap, PlayerSpecificSettings playerSpecificSettings, ref ColorScheme overrideColorScheme)
        {
            EnvironmentInfoSO eiso = difficultyBeatmap.GetEnvironmentInfo();
            ColorScheme curr = overrideColorScheme ?? new ColorScheme(eiso.colorScheme);

            EnvironmentEffectsFilterPreset defaultPreset = playerSpecificSettings.environmentEffectsFilterDefaultPreset;
            EnvironmentEffectsFilterPreset ePlusPreset = playerSpecificSettings.environmentEffectsFilterExpertPlusPreset;
            if ((difficultyBeatmap.difficulty == BeatmapDifficulty.ExpertPlus && ePlusPreset == EnvironmentEffectsFilterPreset.NoEffects) || (difficultyBeatmap.difficulty != BeatmapDifficulty.ExpertPlus && defaultPreset == EnvironmentEffectsFilterPreset.NoEffects))
            {
                ColorScheme mc = new ColorScheme("CustomColourScheme", "CustomColourScheme", false, "CustomColourScheme", false, curr.saberAColor, curr.saberBColor, new Color(0.54901960784f, 0.54901960784f, 0.54901960784f), new Color(0.54901960784f, 0.54901960784f, 0.54901960784f), false, new Color(), new Color(), curr.obstaclesColor);
                overrideColorScheme = mc;
                Plugin.Log.Info("Loaded static lights colours");
                return;
            }

            if (overrideColorScheme == null)
                return;
            BoostColour b = Plugin.Boost;
            ColorScheme mapColor = new ColorScheme("CustomColourScheme", "CustomColourScheme", false, "CustomColourScheme", false, curr.saberAColor, curr.saberBColor, curr.environmentColor0, curr.environmentColor1, true, new Color(b.r0, b.g0, b.b0), new Color(b.r1, b.g1, b.b1), curr.obstaclesColor);
            overrideColorScheme = mapColor;
            Plugin.Log.Info("Loaded Custom Boost Colours");
        }
    }
}
