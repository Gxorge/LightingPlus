using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace LightingPlus.HarmonyPatches
{
   
    public static class MPLightingPatch
    {
        public static bool staticLights = false;
        public static bool loadedLights = false;
        public static bool isInMultiplayer = false;
        public static bool loadError = false;

        public static ColorScheme colours;
        public static Color colour0;
        public static Color colour1;

        public static GameObject lasers;
        public static TubeBloomPrePassLight laserLeft;
        public static TubeBloomPrePassLight laserRight;
        public static TubeBloomPrePassLight backLaserLeft;
        public static TubeBloomPrePassLight backLaserRight;
        public static TubeBloomPrePassLight behindLaserLeft;
        public static TubeBloomPrePassLight behindLaserRight;
        public static TubeBloomPrePassLight farLaserLeft;
        public static TubeBloomPrePassLight farLaserRight;
        public static List<TubeBloomPrePassLight> allLasers;

        public static GameObject ringLights;
        public static DirectionalLight ringLightBehind;
        public static DirectionalLight ringLightFront;
        public static DirectionalLight ringLightLeft;
        public static DirectionalLight ringLightRight;
        public static List<DirectionalLight> allRings;
        public static bool ringErr = false;

        public static void LoadLights()
        {
            bool laserNull = false;
            bool ringNull = false;
            Plugin.Log.Info("Loading lights...");

            try
            {
                lasers = GameObject.Find("MultiplayerLocalActivePlayerController(Clone)/IsActiveObjects/Lasers");
                ringLights = GameObject.Find("MultiplayerLocalActivePlayerController(Clone)/IsActiveObjects/DirectionalLights");

                laserLeft = lasers.transform.Find("LaserL").gameObject.GetComponent<TubeBloomPrePassLight>();
                laserRight = lasers.transform.Find("LaserR").gameObject.GetComponent<TubeBloomPrePassLight>();
                backLaserLeft = lasers.transform.Find("LaserFrontL").gameObject.GetComponent<TubeBloomPrePassLight>();
                backLaserRight = lasers.transform.Find("LaserFrontR").gameObject.GetComponent<TubeBloomPrePassLight>();
                behindLaserLeft = lasers.transform.Find("LaserBackL").gameObject.GetComponent<TubeBloomPrePassLight>();
                behindLaserRight = lasers.transform.Find("LaserBackR").gameObject.GetComponent<TubeBloomPrePassLight>();
                farLaserLeft = lasers.transform.Find("LaserFarL").gameObject.GetComponent<TubeBloomPrePassLight>();
                farLaserRight = lasers.transform.Find("LaserFarR").gameObject.GetComponent<TubeBloomPrePassLight>();
                allLasers = new List<TubeBloomPrePassLight>() { laserLeft, laserRight, backLaserLeft, backLaserRight, behindLaserLeft, behindLaserRight, farLaserLeft, farLaserRight };

                laserLeft.gameObject.SetActive(false);
                laserRight.gameObject.SetActive(false);
                backLaserLeft.gameObject.SetActive(false);
                backLaserRight.gameObject.SetActive(false);
                behindLaserLeft.gameObject.SetActive(false);
                behindLaserRight.gameObject.SetActive(false);
                farLaserLeft.gameObject.SetActive(false);
                farLaserRight.gameObject.SetActive(false);

                ringLightBehind = ringLights.transform.Find("DirectionalLight1").gameObject.GetComponent<DirectionalLight>();
                ringLightFront = ringLights.transform.Find("DirectionalLight2").gameObject.GetComponent<DirectionalLight>();
                ringLightLeft = ringLights.transform.Find("DirectionalLight4").gameObject.GetComponent<DirectionalLight>();
                ringLightRight = ringLights.transform.Find("DirectionalLight3").gameObject.GetComponent<DirectionalLight>();
                allRings = new List<DirectionalLight>() { ringLightBehind, ringLightFront, ringLightLeft, ringLightRight };

                LightingController.HandleRingEvent(new LightArrangement() { enabled = false, color = null });

                Plugin.Log.Info("Lasers: " + (laserNull == true || lasers == null ? "null" : "working"));
                Plugin.Log.Info("Ring Lights: " + (ringNull == true || ringLights == null ? "null" : "working"));


                loadedLights = true;
            } catch (Exception e)
            {
                Plugin.Log.Critical("Error loading lights: " + e.Message);
                loadError = true;
            }
        }

        public static ColorSO GetCSO(Color color)
        {
            SimpleColorSO so = ScriptableObject.CreateInstance(typeof(SimpleColorSO)) as SimpleColorSO;
            so.SetColor(color);
            return so;
        }

        [HarmonyPatch]
        public static class NonMultiplayerSetupPatch
        {
            [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), nameof(StandardLevelScenesTransitionSetupDataSO.Init))]
            [HarmonyPostfix]
            public static void StandardPatch()
            {
                isInMultiplayer = false;
                Plugin.Log.Info("Not in multiplayer.");
            }

            [HarmonyPatch(typeof(MissionLevelScenesTransitionSetupDataSO), nameof(MissionLevelScenesTransitionSetupDataSO.Init))]
            [HarmonyPostfix]
            public static void CampaignPatch()
            {
                isInMultiplayer = false;
                Plugin.Log.Info("Not in multiplayer.");
            }
        }

        [HarmonyPatch(typeof(MultiplayerLevelScenesTransitionSetupDataSO), nameof(MultiplayerLevelScenesTransitionSetupDataSO.Init))]
        public static class SceneSetupPatch
        {
            [HarmonyPostfix]
            public static void Postfix(IDifficultyBeatmap difficultyBeatmap, PlayerSpecificSettings playerSpecificSettings, ref ColorScheme overrideColorScheme)
            {
                loadedLights = false;
                loadError = false;
                isInMultiplayer = true;
                Plugin.Log.Info("In multiplayer.");

                EnvironmentEffectsFilterPreset defaultPreset = playerSpecificSettings.environmentEffectsFilterDefaultPreset;
                EnvironmentEffectsFilterPreset ePlusPreset = playerSpecificSettings.environmentEffectsFilterExpertPlusPreset;

                if (difficultyBeatmap.difficulty == BeatmapDifficulty.ExpertPlus && ePlusPreset == EnvironmentEffectsFilterPreset.NoEffects)
                {
                    staticLights = true;
                }
                else if (difficultyBeatmap.difficulty != BeatmapDifficulty.ExpertPlus && defaultPreset == EnvironmentEffectsFilterPreset.NoEffects)
                {
                    staticLights = true;
                }
                else
                {
                    staticLights = false;
                }

                EnvironmentInfoSO eiso = difficultyBeatmap.GetEnvironmentInfo();
                colours = overrideColorScheme ?? new ColorScheme(eiso.colorScheme);
                ColorScheme mapColor = new ColorScheme("CustomColourScheme", "CustomColourScheme", false, colours.saberAColor, colours.saberBColor, colours.environmentColor0, colours.environmentColor1, true, new Color(0.18823529411f, 0.59607843137f, 1f), new Color(0.53333333333f, 0.0862745098f, 1f), colours.obstaclesColor);
                overrideColorScheme = mapColor;
                colours = mapColor;
                colour0 = colours.environmentColor0;
                colour1 = colours.environmentColor1;
                ringErr = false;
            }
        }

        [HarmonyPatch(typeof(BeatmapObjectCallbackController), nameof(BeatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent))]
        public static class LightingController
        {
            [HarmonyPostfix]
            public static void Postfix([HarmonyArgument(0)] BeatmapEventData data)
            {
                if (loadError) return;
                if (!isInMultiplayer) return;
                if (staticLights) return;
                if (!loadedLights) LoadLights();

                LightArrangement arr = CreateArrangement(data);

                switch (data.type)
                {
                    case BeatmapEventType.Event0:
                        {
                            HandleLightEvent(backLaserLeft, arr);
                            HandleLightEvent(backLaserRight, arr);
                            break;
                        }
                    case BeatmapEventType.Event1:
                        {
                            HandleRingEvent(arr);
                            break;
                        }
                    case BeatmapEventType.Event2:
                        {
                            HandleLightEvent(laserLeft, arr);
                            break;
                        }
                    case BeatmapEventType.Event3:
                        {
                            HandleLightEvent(laserRight, arr);
                            break;
                        }
                    case BeatmapEventType.Event4:
                        {
                            HandleLightEvent(behindLaserLeft, arr);
                            HandleLightEvent(behindLaserRight, arr);
                            HandleLightEvent(ringLightLeft, arr);
                            HandleLightEvent(ringLightRight, arr);
                            break;
                        }
                    case BeatmapEventType.Event5:
                        {
                            HandleBoost(data.value);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            public static void HandleBoost(int value)
            {
                if (value == 0)
                {
                    colour0 = colours.environmentColor0;
                    colour1 = colours.environmentColor1;
                    UpdateBoost(false);
                } 
                else
                {
                    colour0 = colours.environmentColor0Boost;
                    colour1 = colours.environmentColor1Boost;
                    UpdateBoost(true);
                }
            }

            private static void UpdateBoost(bool enabled)
            {
                if (enabled)
                {
                    foreach (TubeBloomPrePassLight l in allLasers)
                    {
                        if (!l.enabled) continue;

                        if (l.color == GetCSO(colours.environmentColor0))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour0) }) ;
                        else if (l.color == GetCSO(colours.environmentColor1))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour1) });
                    }

                    foreach (DirectionalLight l in allRings)
                    {
                        if (!l.enabled) continue;

                        if (l.color == GetCSO(colours.environmentColor0))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour0) });
                        else if (l.color == GetCSO(colours.environmentColor1))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour1) });
                    }
                } else
                {
                    foreach (TubeBloomPrePassLight l in allLasers)
                    {
                        if (!l.enabled) continue;

                        if (l.color == GetCSO(colours.environmentColor0Boost))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour0) });
                        else if (l.color == GetCSO(colours.environmentColor1Boost))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour1) });
                    }

                    foreach (DirectionalLight l in allRings)
                    {
                        if (!l.enabled) continue;

                        if (l.color == GetCSO(colours.environmentColor0Boost))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour0) });
                        else if (l.color == GetCSO(colours.environmentColor1Boost))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour1) });
                    }
                }
            }

            public static void HandleRingEvent(LightArrangement arr)
            {
                if (ringErr) return;
                try
                {
                    HandleLightEvent(ringLightBehind, arr);
                    HandleLightEvent(ringLightFront, arr);
                    HandleLightEvent(farLaserLeft, arr);
                    HandleLightEvent(farLaserRight, arr);
                } 
                catch (Exception e)
                {
                    ringErr = true;
                    Plugin.Log.Critical("Ring error: " + e.Message);
                }
            }

            public static void HandleLightEvent(TubeBloomPrePassLight light, LightArrangement arr)
            {
                try
                {
                    light.gameObject.SetActive(arr.enabled);
                    light.enabled = arr.enabled;
                    light.color = arr.color;
                } 
                catch (Exception e)
                {
                    Plugin.Log.Critical("Laser error: " + e.Message);
                }
            }

            public static void HandleLightEvent(DirectionalLight light, LightArrangement arr)
            {
                try
                {
                    light.gameObject.SetActive(arr.enabled);
                    light.enabled = arr.enabled;
                    light.color = arr.color;
                }
                catch (Exception e)
                {
                    Plugin.Log.Critical("Ring error 2: " + e.Message);
                }
            }

            public static LightArrangement CreateArrangement(BeatmapEventData data)
            {
                LightArrangement arr = new LightArrangement();

                switch (data.value)
                {
                    case 0:
                        arr.enabled = false;
                        arr.color = null;
                        break;
                    case 1:
                    case 2:
                    case 3:
                        arr.color = GetCSO(colour1);
                        arr.enabled = true;
                        break;
                    case 5:
                    case 6:
                    case 7:
                        arr.color = GetCSO(colour0);
                        arr.enabled = true;
                        break;
                    default:
                        arr.enabled = false;
                        arr.color = null;
                        break;
                }

                return arr;
            }
        }
    }
}
