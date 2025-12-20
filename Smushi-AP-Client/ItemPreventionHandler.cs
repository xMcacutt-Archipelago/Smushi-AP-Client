using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace Smushi_AP_Client;

public class ItemPreventionHandler
{
    public static bool AllowPowder;
    
    [HarmonyPatch(typeof(CoinPuzzleHandler))]
    public class CoinPuzzleHandler_Patch
    {
        [HarmonyPatch("SetCoinTrue")]
        [HarmonyPrefix]
        public static bool SetCoinTrue(int index)
        {
            return false;
        }
    }

    [HarmonyPatch((typeof(PlayerData)))]
    public class PlayerData_Patch
    {
        [HarmonyPatch("AddPowderCount")]
        [HarmonyPrefix]
        public static bool OnAddPowderCount(PlayerData __instance)
        {
            if (!AllowPowder) return false;
            AllowPowder = false;
            return true;
        }
    }

    [HarmonyPatch(typeof(PencilSpawner))]
    public class PencilSpawner_Patch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static bool Start(PencilSpawner __instance)
        {
            if (!PluginMain.SaveDataHandler.CustomPlayerData.HasPencil)
                return false;
            __instance.gameObject.SetActive(false);
            return false;
        }

    }
    
    // Stop removing these items:
    [HarmonyPatch(typeof(StrawberryActivityHandler))]
    public class StrawberryActivityHandler_Patch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static bool OnStart(StrawberryActivityHandler __instance) { return false; }
        
        [HarmonyPatch("KnockdownStrawberry")]
        [HarmonyPrefix]
        public static bool OnKnockdownStrawberry(StrawberryActivityHandler __instance)
        {
            --__instance.strawberryCount;
            if (__instance.strawberryCount > 0)
                return false;
            __instance.currentRoutine ??= __instance.StartCoroutine(EndActivity(__instance));
            return false;
        }

        private static IEnumerator EndActivity(StrawberryActivityHandler __instance)
        {
            __instance.fader.FadeIn();
            yield return (object) new WaitForSeconds(1f);
            __instance.controller.DisableCatapult();
            __instance.strawberries.SetActive(true);
            __instance.finishedStrawberries.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            __instance.fader.FadeOut();
            yield return new WaitForSeconds(0.5f);
            __instance.dialogue.StartDialogue(__instance.nodeName);
            __instance.pd.dialogBools["finishStrawb"] = true;
        }
    }
    
    [HarmonyPatch(typeof(BuddhaHandDetect))]
    public class BuddhaHandDetect_Patch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static bool Start(BuddhaHandDetect __instance)
        {
            __instance.boxCollider = __instance.GetComponent<BoxCollider>();
            __instance.pickupHands[0].SetActive(true);
            __instance.pickupHands[1].SetActive(true);

            if (__instance.isSolved)
            {
                __instance.boxCollider.enabled = false;
                __instance.sparklePS.Stop();
            }
            else
            {
                __instance.handsAdded = -1;
                __instance.handsUsed[0] = false;
                __instance.handsUsed[1] = false;
                __instance.boxCollider.enabled = true;
                __instance.sparklePS.Play();
            }
            __instance.SetHandModels();
            return false;
        }
    }

    [HarmonyPatch(typeof(AcornPuzzleHandler))]
    public class AcornPuzzleHandler_Patch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void OnStart(AcornPuzzleHandler __instance)
        {
            __instance.acorns[0].SetActive(true);
            __instance.acorns[1].SetActive(true);
        }
    }

    [HarmonyPatch(typeof(BombDoorPuzzle))]
    public class BombDoorPuzzle_Patch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void OnStart(BombDoorPuzzle __instance)
        {
            __instance.bombPickup.SetActive(value: true);
        }
    }

    [HarmonyPatch(typeof(CatapultActivityHandler))]
    public class CatapultActivityHandler_Patch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static bool Start(CatapultActivityHandler __instance)
        {
            if (__instance.saver.nodeTracker.CheckVisitedNode("CatapultEnd"))
            {
                __instance.chunks.SetActive(value: false);
                __instance.bandaid.SetActive(value: true);
                __instance.finishedSapphires.SetActive(value: true);
                __instance.sapphireModels.SetActive(value: true);
                __instance.ikTrigger.enabled = true;
                __instance.TriggerBongoIdle();
                __instance.bombPickup.enabled = true;
                __instance.bombSparkle.Play();
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(KeyDetection))]
    public class KeyDetection_Patch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(KeyDetection __instance)
        {
            __instance.keyPickups[0].SetActive(true);
            __instance.keyPickups[1].SetActive(true);
        }
    }
}