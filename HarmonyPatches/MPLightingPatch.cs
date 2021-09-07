using System;
using System.Collections;
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
        public static bool enabled = true;
        public static bool staticLights = false;
        public static bool loadedLights = false;
        public static bool isInMultiplayer = false;
        public static bool loadError = false;

        public static ColorScheme colours;
        public static Color colour0;
        public static Color colour1;
        public static bool bostSupport;

        public static GameObject lasers;
        public static TubeBloomPrePassLight laserLeft;
        public static TubeBloomPrePassLight laserRight;
        public static TubeBloomPrePassLight backLaserLeft;
        public static TubeBloomPrePassLight backLaserRight;
        public static TubeBloomPrePassLight behindLaserLeft;
        public static TubeBloomPrePassLight behindLaserRight;
        public static TubeBloomPrePassLight farLaserLeft;
        public static TubeBloomPrePassLight farLaserRight;
        public static List<ConnectedPlayerLighting> connectedPlayerLighting;
        public static List<TubeBloomPrePassLight> allLasers;
        public static List<Transform> ringExpanded;
        public static ColorSO cpOffColour;
        public static bool playerLaserErr = false;

        public static GameObject ringLights;
        public static DirectionalLight ringLightBehind;
        public static DirectionalLight ringLightFront;
        public static DirectionalLight ringLightLeft;
        public static DirectionalLight ringLightRight;
        public static List<DirectionalLight> allRings;
        public static bool ringErr = false;

        public static GameObject consturctionLeft;
        public static GameObject consturctionRight;

        public static void LoadLights()
        {
            bool laserNull = false;
            bool ringNull = false;
            Plugin.Log.Info("Loading lights...");

            ringExpanded = new List<Transform>();
            new GameObject("LightingPlusCoRoutineController").AddComponent<CoRoutineController>();

            try
            {
                lasers = GameObject.Find("MultiplayerLocalActivePlayerController(Clone)/IsActiveObjects/Lasers");
                ringLights = GameObject.Find("MultiplayerLocalActivePlayerController(Clone)/IsActiveObjects/DirectionalLights");

                // Load player construction
                consturctionLeft = GameObject.Find("MultiplayerLocalActivePlayerController(Clone)/IsActiveObjects/Construction/ConstructionL");
                consturctionRight = GameObject.Find("MultiplayerLocalActivePlayerController(Clone)/IsActiveObjects/Construction/ConstructionR");


                // Load the lasers
                if (lasers != null)
                {
                    laserLeft = lasers.transform.Find("LaserL").gameObject.GetComponent<TubeBloomPrePassLight>();
                    laserRight = lasers.transform.Find("LaserR").gameObject.GetComponent<TubeBloomPrePassLight>();
                    backLaserLeft = lasers.transform.Find("LaserR/LaserFrontL").gameObject.GetComponent<TubeBloomPrePassLight>();

                    // Fixing for some reason front L being a child of laser r, and fixing its position
                    backLaserLeft.transform.parent = lasers.transform;
                    var v3 = backLaserLeft.transform.position;
                    backLaserLeft.transform.SetPositionAndRotation(new Vector3(v3.x, 0, v3.z), backLaserLeft.transform.rotation);

                    backLaserRight = lasers.transform.Find("LaserFrontR").gameObject.GetComponent<TubeBloomPrePassLight>();
                    behindLaserLeft = lasers.transform.Find("LaserBackL").gameObject.GetComponent<TubeBloomPrePassLight>();
                    behindLaserRight = lasers.transform.Find("LaserBackR").gameObject.GetComponent<TubeBloomPrePassLight>();
                    farLaserLeft = lasers.transform.Find("LaserFarL").gameObject.GetComponent<TubeBloomPrePassLight>();
                    farLaserRight = lasers.transform.Find("LaserFarR").gameObject.GetComponent<TubeBloomPrePassLight>();
                    allLasers = new List<TubeBloomPrePassLight>() { laserLeft, laserRight, backLaserLeft, backLaserRight, behindLaserLeft, behindLaserRight, farLaserLeft, farLaserRight };

                    foreach (TubeBloomPrePassLight l in allLasers) LightingController.HandleLightEvent(l, new LightArrangement() { enabled = false, color = null });
                    playerLaserErr = false;
                }
                else
                {
                    Plugin.Log.Error("Lasers are disabled.");
                    playerLaserErr = true;
                }

                // Loaded the other player's lasers
                connectedPlayerLighting = new List<ConnectedPlayerLighting>();
                var players = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "MultiplayerConnectedPlayerController(Clone)");
                if (players != null)
                {
                    foreach (GameObject o in players)
                    {
                        ConnectedPlayerLighting l = new ConnectedPlayerLighting();
                        GameObject _lasers = o.transform.Find("Lasers").gameObject;
                        l.laserLeft = _lasers.transform.Find("SideLaserL").gameObject.GetComponent<TubeBloomPrePassLight>();
                        l.laserRight = _lasers.transform.Find("SideLaserR").gameObject.GetComponent<TubeBloomPrePassLight>();
                        l.laserFront = _lasers.transform.Find("FrontLaserC2").gameObject.GetComponent<TubeBloomPrePassLight>();
                        l.downLaserLeft = _lasers.transform.Find("FrontLaserL").gameObject.GetComponent<TubeBloomPrePassLight>();
                        l.downLaserRight = _lasers.transform.Find("FrontLaserR").gameObject.GetComponent<TubeBloomPrePassLight>();
                        l.downLaserConnector = _lasers.transform.Find("FrontLaserC").gameObject.GetComponent<TubeBloomPrePassLight>();
                        l.extendedLaserLeft = _lasers.transform.Find("ThinLaserL").gameObject.GetComponent<TubeBloomPrePassLight>();
                        l.extendedLaserRight = _lasers.transform.Find("ThinLaserR").gameObject.GetComponent<TubeBloomPrePassLight>();
                        l.AddAllToList();

                        allLasers.AddRange(l.allLasers);
                        connectedPlayerLighting.Add(l);
                        Plugin.Log.Info("Loaded a Connected Player's lights");
                    }
                }

                cpOffColour = GetCSO(new Color(0, 0, 0));
                LightingController.HandleGlobalCPLightEvent(new LightArrangement() { enabled = false, color = cpOffColour });

                // Load the ring lights
                if (ringLights != null)
                {
                    ringLightBehind = ringLights.transform.Find("DirectionalLight1").gameObject.GetComponent<DirectionalLight>();
                    ringLightFront = ringLights.transform.Find("DirectionalLight2").gameObject.GetComponent<DirectionalLight>();
                    ringLightLeft = ringLights.transform.Find("DirectionalLight4").gameObject.GetComponent<DirectionalLight>();
                    ringLightRight = ringLights.transform.Find("DirectionalLight3").gameObject.GetComponent<DirectionalLight>();
                    allRings = new List<DirectionalLight>() { ringLightBehind, ringLightFront, ringLightLeft, ringLightRight };

                    foreach (DirectionalLight l in allRings) LightingController.HandleLightEvent(l, new LightArrangement() { enabled = false, color = null });
                    ringErr = false;
                } 
                else
                {
                    Plugin.Log.Error("Ring Lights are disabled.");
                    ringErr = true;
                }

                Plugin.Log.Info("Lasers: " + (laserNull == true || lasers == null ? "error" : "working"));
                Plugin.Log.Info("Ring Lights: " + (ringNull == true || ringLights == null ? "error" : "working"));
                Plugin.Log.Info("CP Lights: " + (connectedPlayerLighting.Count == 0 ? "none found" : "working (" + connectedPlayerLighting.Count + ")"));

                if (ringErr && playerLaserErr)
                {
                    loadError = true;
                    Plugin.Log.Critical("Both rings and lasers failed to load. Lighting has been disabled.");
                } 
                else
                {
                    loadedLights = true;
                }

                
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
                enabled = Plugin.Config.MultiPlayerLightingEnabled;
                loadedLights = false;
                loadError = false;
                playerLaserErr = false;
                ringErr = false;
                isInMultiplayer = true;
                Plugin.Log.Info("In multiplayer.");

                EnvironmentEffectsFilterPreset defaultPreset = playerSpecificSettings.environmentEffectsFilterDefaultPreset;
                EnvironmentEffectsFilterPreset ePlusPreset = playerSpecificSettings.environmentEffectsFilterExpertPlusPreset;
                // If static lights
                if ((difficultyBeatmap.difficulty == BeatmapDifficulty.ExpertPlus && ePlusPreset == EnvironmentEffectsFilterPreset.NoEffects) || (difficultyBeatmap.difficulty != BeatmapDifficulty.ExpertPlus && defaultPreset == EnvironmentEffectsFilterPreset.NoEffects))
                {
                    Plugin.Log.Info("Static lights are on");
                    staticLights = true;
                }

                EnvironmentInfoSO eiso = difficultyBeatmap.GetEnvironmentInfo();
                colours = overrideColorScheme ?? new ColorScheme(eiso.colorScheme);

                BoostColour b = Plugin.Boost;

                ColorScheme mapColor;
                if (overrideColorScheme != null || !overrideColorScheme.supportsEnvironmentColorBoost)
                {
                    mapColor = new ColorScheme("LPCustomColourScheme", "LPCustomColourScheme", false, "LPCustomColourScheme", false, colours.saberAColor, colours.saberBColor, colours.environmentColor0, colours.environmentColor1, true, new Color(b.r0 / 255, b.g0 / 255, b.b0 / 255), new Color(b.r1 / 255, b.g1 / 255, b.b1 / 255), colours.obstaclesColor);
                    overrideColorScheme = mapColor;
                }
                else
                {
                    mapColor = colours;
                }

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
                if (!enabled) return;
                if (loadError) return;
                if (!isInMultiplayer) return;
                if (staticLights) return;
                if (!loadedLights) LoadLights();

                LightArrangement arr = CreateArrangement(data);

                switch (data.type) // https://bsmg.wiki/mapping/map-format.html#events-2
                {
                    case BeatmapEventType.Event0: // Back Lasers
                        {
                            HandleLightEvent(backLaserLeft, arr);
                            HandleLightEvent(backLaserRight, arr);
                            foreach (ConnectedPlayerLighting c in connectedPlayerLighting)
                            {
                                HandleCPLightEvent(c.downLaserLeft, arr);
                                HandleCPLightEvent(c.downLaserRight, arr);
                                HandleCPLightEvent(c.downLaserConnector, arr);
                            }
                            break;
                        }
                    case BeatmapEventType.Event1: // Ring Lights
                        {
                            HandleRingEvent(arr);
                            foreach (ConnectedPlayerLighting c in connectedPlayerLighting)
                            {
                                HandleCPLightEvent(c.extendedLaserLeft, arr);
                                HandleCPLightEvent(c.extendedLaserRight, arr);
                            }
                            break;
                        }
                    case BeatmapEventType.Event2: // Left Lasers
                        {
                            HandleLightEvent(laserLeft, arr);
                            foreach (ConnectedPlayerLighting c in connectedPlayerLighting) HandleCPLightEvent(c.laserLeft, arr);
       
                            break;
                        }
                    case BeatmapEventType.Event3: // Right Lasers
                        {
                            HandleLightEvent(laserRight, arr);
                            HandleLightEvent(laserLeft, arr);
                            foreach (ConnectedPlayerLighting c in connectedPlayerLighting) HandleCPLightEvent(c.laserRight, arr);
                            break;
                        }
                    case BeatmapEventType.Event4: // Center lights
                        {
                            HandleLightEvent(behindLaserLeft, arr);
                            HandleLightEvent(behindLaserRight, arr);
                            HandleLightEvent(farLaserLeft, arr);
                            HandleLightEvent(farLaserRight, arr);
                            HandleLightEvent(ringLightLeft, arr);
                            HandleLightEvent(ringLightRight, arr);
                            foreach (ConnectedPlayerLighting c in connectedPlayerLighting) HandleCPLightEvent(c.laserFront, arr);
                            break;
                        }
                    case BeatmapEventType.Event5: // Boost colours
                        {
                            HandleBoost(data.value);
                            break;
                        }
                    case BeatmapEventType.Event9: // Ring expand
                        {
                            CoRoutineController.i().StartCoroutine(RingExpand(laserLeft.transform, new Vector3((ringExpanded.Contains(laserLeft.transform) ? -1.20f : -4.20f), laserLeft.transform.position.y, laserLeft.transform.position.z), 0.5f));
                            CoRoutineController.i().StartCoroutine(RingExpand(consturctionLeft.transform, new Vector3((ringExpanded.Contains(consturctionLeft.transform) ? 0 : -3), consturctionLeft.transform.position.y, consturctionLeft.transform.position.z), 0.5f));
                            CoRoutineController.i().StartCoroutine(RingExpand(laserRight.transform, new Vector3((ringExpanded.Contains(laserRight.transform) ? 1.20f : 4.20f), laserRight.transform.position.y, laserRight.transform.position.z), 0.5f));
                            CoRoutineController.i().StartCoroutine(RingExpand(consturctionRight.transform, new Vector3((ringExpanded.Contains(consturctionRight.transform) ? 0 : 3), consturctionRight.transform.position.y, consturctionRight.transform.position.z), 0.5f));
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            public static IEnumerator RingExpand(Transform toMove, Vector3 toMoveTo, float timeToMove)
            {
                var currentPos = toMove.position;
                var t = 0f;

                if (ringExpanded.Contains(toMove))
                    ringExpanded.Remove(toMove);
                else
                    ringExpanded.Add(toMove);

                while (t < 1)
                {
                    t += Time.deltaTime / timeToMove;
                    toMove.transform.position = Vector3.Lerp(currentPos, toMoveTo, t);
                    yield return null;
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
                        if (playerLaserErr) break;
                        if (!l.enabled) continue;

                        if (l.color == GetCSO(colours.environmentColor0))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour0) }) ;
                        else if (l.color == GetCSO(colours.environmentColor1))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour1) });
                    }

                    foreach (DirectionalLight l in allRings)
                    {
                        if (ringErr) break;
                        if (!l.enabled) continue;

                        if (l.color == GetCSO(colours.environmentColor0))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour0) });
                        else if (l.color == GetCSO(colours.environmentColor1))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour1) });
                    }
                } 
                else
                {
                    foreach (TubeBloomPrePassLight l in allLasers)
                    {
                        if (playerLaserErr) break;
                        if (!l.enabled) continue;

                        if (l.color == GetCSO(colours.environmentColor0Boost))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour0) });
                        else if (l.color == GetCSO(colours.environmentColor1Boost))
                            HandleLightEvent(l, new LightArrangement() { enabled = true, color = GetCSO(colour1) });
                    }

                    foreach (DirectionalLight l in allRings)
                    {
                        if (ringErr) break;
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
                    HandleLightEvent(ringLightLeft, arr);
                    HandleLightEvent(ringLightRight, arr);
                } 
                catch (Exception e)
                {
                    ringErr = true;
                    Plugin.Log.Critical("Ring error: " + e.Message);
                }
            }

            public static void HandleLightEvent(TubeBloomPrePassLight light, LightArrangement arr)
            {
                if (playerLaserErr) return;
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
                if (ringErr) return;
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

            public static void HandleCPLightEvent(TubeBloomPrePassLight light, LightArrangement arr)
            {
                try
                {
                    light.enabled = arr.enabled;
                    if (arr.enabled)
                    {
                        light.gameObject.SetActive(arr.enabled);
                        light.color = arr.color;
                    } 
                    else
                    {
                        light.color = cpOffColour;
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.Critical("CP Laser error: " + e.Message);
                }
            }

            public static void HandleGlobalCPLightEvent(LightArrangement arr)
            {
                foreach (ConnectedPlayerLighting c in connectedPlayerLighting)
                {
                    foreach (TubeBloomPrePassLight light in c.allLasers)
                    {
                        HandleCPLightEvent(light, arr);
                    }
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

        public class CoRoutineController : MonoBehaviour
        {
            private static CoRoutineController instance;

            public void Start()
            {
                Plugin.Log.Info("instance created");
                instance = this;
            }

            public static CoRoutineController i()
            {
                return instance;
            }
        }
    }
}
