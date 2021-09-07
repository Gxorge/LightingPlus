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

            // If static lights
            if ((difficultyBeatmap.difficulty == BeatmapDifficulty.ExpertPlus && ePlusPreset == EnvironmentEffectsFilterPreset.NoEffects) || (difficultyBeatmap.difficulty != BeatmapDifficulty.ExpertPlus && defaultPreset == EnvironmentEffectsFilterPreset.NoEffects))
            {
                if (!Plugin.Config.StaticLightsColoursEnabled)
                    return;

                StaticLights sl = Plugin.Config.StaticLightsColours;
                ColorScheme mc = new ColorScheme("LPCustomColourScheme", "LPCustomColourScheme", false, "LPCustomColourScheme", false, curr.saberAColor, curr.saberBColor, new Color(sl.r / 255, sl.g / 255, sl.b / 255), new Color(sl.r / 255, sl.g / 255, sl.b / 255), false, new Color(), new Color(), curr.obstaclesColor);
                overrideColorScheme = mc;
                Plugin.Log.Info("Loaded static lights colours");
                return;
            }

            // Check if the colour scheme is being overrided, if it is, great we wanna use that. We also don't wanna use it if the overrided scheme already has boost colours
            if (overrideColorScheme == null || overrideColorScheme.supportsEnvironmentColorBoost || !Plugin.Config.BoostColoursEnabled)
                return;

            // Set the new colour scheme
            BoostColour b = Plugin.Boost;
            ColorScheme mapColor = new ColorScheme("LPCustomColourScheme", "LPCustomColourScheme", false, "LPCustomColourScheme", false, curr.saberAColor, curr.saberBColor, curr.environmentColor0, curr.environmentColor1, true, new Color(b.r0 / 255, b.g0 / 255, b.b0 / 255), new Color(b.r1 / 255, b.g1 /255, b.b1 / 255), curr.obstaclesColor);
            overrideColorScheme = mapColor;
            Plugin.Log.Info("Loaded Custom Boost Colours");
        }
    }
}
