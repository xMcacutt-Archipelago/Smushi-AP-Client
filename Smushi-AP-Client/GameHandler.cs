using System;
using System.Collections;
using Archipelago.MultiClient.Net.Models;
using FMODUnity;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Smushi_AP_Client
{
    public class GameHandler : MonoBehaviour
    {
        public void InitOnConnect()
        {
            SceneManager.sceneLoaded += SceneChanged;
        }

        public void SceneChanged(Scene scene, LoadSceneMode loadSceneMode)
        {
            switch (scene.name)
            {
                case "Zone 2_F":
                    PluginMain.SaveDataHandler.CustomPlayerData.HasGoneFall = true;
                    break;
                case "Zone 3":
                    PluginMain.SaveDataHandler.CustomPlayerData.HasGoneLake = true;
                    break;
                case "Zone 5":
                    PluginMain.SaveDataHandler.CustomPlayerData.HasGoneGrove = true;
                    break;
                case "Home":
                    PluginMain.SaveDataHandler.CustomPlayerData.HasGoneHome = true;
                    break;
            }
            var manager = FindObjectOfType<SaveLoadManager>();
            if (manager == null)
                return;
            SaveSystem._instance.SaveAllData(manager, true);
        }

        [HarmonyPatch(typeof(HomeLevelSetup))]
        public class HomeLevelSetup_Patch
        {
            [HarmonyPatch("SetHomeOutro")]
            [HarmonyPostfix]
            public static void OnSetHomeOutro()
            {
                if (PluginMain.ArchipelagoHandler.SlotData.Goal == Goal.SmushiGoHome)
                    PluginMain.ArchipelagoHandler.Release();
            }
        }

        [HarmonyPatch(typeof(ForestHeartCutscene))]
        public class ForestHeartCutscene_Patch
        {
            [HarmonyPatch("SetCutsceneEnd")]
            [HarmonyPostfix]
            public static void OnSetCutsceneEnd()
            {
                if (PluginMain.ArchipelagoHandler.SlotData.Goal == Goal.SmushiSaveTree)
                    PluginMain.ArchipelagoHandler.Release();
            }
        }

        [HarmonyPatch(typeof(CapyActivator))]
        public class CapyActivator_Patch
        {
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            public static void OnStart(CapyActivator __instance)
            {
                __instance.SetBoxTrigger(true);
            }
        }
        
        [HarmonyPatch(typeof(Capy1Manager))]
        public class Capy1Manager_Patch
        {
            [HarmonyPatch("MoveCapyToTarget")]
            [HarmonyPrefix]
            public static bool OnMoveCapyToTarget(Capy1Manager __instance, int index)
            {
                __instance.StartCoroutine(MoveCo(__instance, index));
                return false;
            }
            
            private static IEnumerator MoveCo(Capy1Manager manager, int index)
            {
                manager.capyRB.isKinematic = true;
                manager.activeIndex = index;
                yield return new WaitForSeconds(0.2f);
                manager.capyTransform.position = manager.targetPositions[index].position;
                manager.capyRotation.rotation = manager.targetPositions[index].rotation;
            }
        }
        
        // Capybara Hell Sandcastle
        [HarmonyPatch(typeof(IslandDialogueTrigger))]
        public class IslandDialogueTrigger_Patch
        {
            private static IEnumerator CapyHijack1(IslandDialogueTrigger island)
            {
                island.SetStateWaitingForOrb();
                island.fader.FadeIn();
                yield return new WaitForSeconds(1f);
                island.capyManager.MoveCapyToTarget(1);
                island.capySiblingDTB.ResetDialogue();
                yield return new WaitForSeconds(0.3f);
                island.fader.FadeOut();
                island.capyManager.isTrackingCapy = false;
            }

            private static IEnumerator DismountHijack(IslandDialogueTrigger island)
            {
                yield return new WaitForSeconds(2f);
                island.capyActivator.DisableUnmounting();
            }

            [HarmonyPatch("TriggerIslandIntroDialogue")]
            [HarmonyPostfix]
            private static void TriggerIslandIntroDialogue(IslandDialogueTrigger __instance)
            {
                __instance.StartCoroutine(DismountHijack(__instance));
            }
            
            [HarmonyPatch("EndIslandIntroCutscene")]
            [HarmonyPrefix]
            private static bool OnEndIslandIntroCutscene(IslandDialogueTrigger __instance)
            {
                if (__instance.coroutine2 != null)
                    __instance.StopCoroutine(__instance.coroutine2);
                __instance.coroutine2 = __instance.StartCoroutine(CapyHijack1(__instance));
                return false;
            }
            
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            private static bool OnStart(IslandDialogueTrigger __instance)
            {
                __instance.hasTriggeredHint = __instance.pd.dialogBools["hasTriggeredHint"];
                __instance.hasTriggeredBeach = __instance.pd.dialogBools["hasTriggeredBeach"];
                switch (__instance.currentIslandState)
                {
                    case IslandDialogueTrigger.SandIslandState.WaitingForOrb:
                        __instance.capySiblingDTB.ResetDialogue();
                        __instance.mountingTrigger.displayUI = true;
                        if (Vector3.Distance(__instance.player.position, __instance.targetPosition.position) > 1213.0 
                            && Vector3.Distance(__instance.player.position, __instance.targetPosition.position) < 1444.0)
                        {
                            __instance.player.position = __instance.targetPosition.position;
                            break;
                        }
                        break;
                    case IslandDialogueTrigger.SandIslandState.HasOrb:
                        __instance.islandHandler.ActivateTier2Castles();
                        __instance.mountingTrigger.displayUI = true;
                        if (Vector3.Distance(__instance.player.position, __instance.targetPosition.position) > 1213.0
                            && Vector3.Distance(__instance.player.position, __instance.targetPosition.position) < 1444.0)
                        {
                            __instance.player.position = __instance.targetPosition.position;
                            break;
                        }
                        break;
                    case IslandDialogueTrigger.SandIslandState.IslandFinished:
                        __instance.capy3.SetActive(false);
                        __instance.mountingTrigger.displayUI = true;
                        __instance.islandHandler.SetupNPCs();
                        break;
                }
                __instance.capySiblingDTB.TriggerDialogueAnim("build");
                __instance.mountingTrigger.displayUI = true;
                return false;
            }
        }

        // Capybara Hell 2 - Electric Boogaloo
        [HarmonyPatch(typeof(IslandCaveDialogueTrigger))]
        public class IslandCaveDialogueTrigger_Patch
        {
            private static IEnumerator CapyHijack2(IslandCaveDialogueTrigger island)
            {
                island.currentIslandState = IslandCaveDialogueTrigger.CaveIslandState.WaitingForChungy;
                island.fader.FadeIn();
                yield return new WaitForSeconds(1f);
                island.capyManager.MoveCapyToTarget(3);
                island.capySiblingDTB.ResetDialogue();
                yield return new WaitForSeconds(0.3f);
                island.fader.FadeOut();
                island.capyManager.isTrackingCapy = false;
            }
            
            private static IEnumerator DismountHijack(IslandCaveDialogueTrigger island)
            {
                yield return new WaitForSeconds(2f);
                island.capyActivator.DisableUnmounting();
            }

            [HarmonyPatch("TriggerIslandIntroDialogue")]
            [HarmonyPostfix]
            private static void TriggerIslandIntroDialogue(IslandCaveDialogueTrigger __instance)
            {
                __instance.StartCoroutine(DismountHijack(__instance));
            }
            
            [HarmonyPatch("EndIslandIntroCutscene")]
            [HarmonyPrefix]
            private static bool OnEndIslandIntroCutscene(IslandCaveDialogueTrigger __instance)
            {
                if (__instance.coroutine2 != null)
                    __instance.StopCoroutine(__instance.coroutine2);
                __instance.coroutine2 = __instance.StartCoroutine(CapyHijack2(__instance));
                return false;
            }
            
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            private static bool OnStart(IslandCaveDialogueTrigger __instance)
            {
                if (__instance.pd.dialogBools["rescuedChungy"] && __instance.currentIslandState != IslandCaveDialogueTrigger.CaveIslandState.IslandFinished)
                    __instance.currentIslandState = IslandCaveDialogueTrigger.CaveIslandState.RescuedChungy;
                if (__instance.currentIslandState == IslandCaveDialogueTrigger.CaveIslandState.WaitingForChungy)
                {
                    __instance.capyDTBCollider.enabled = true;
                    __instance.mountTrigger.SetUnderwaterAssets(true);
                    if (Vector3.Distance(__instance.player.position, __instance.targetPosition.position) <= 140.0 
                        || Vector3.Distance(__instance.player.position, __instance.targetPosition.position) >= 1000.0)
                        return false;
                    __instance.player.position = __instance.targetPosition.position;
                }
                else if (__instance.currentIslandState == IslandCaveDialogueTrigger.CaveIslandState.RescuedChungy)
                {
                    __instance.melons.SetActive(true);
                    __instance.chungy.SetActive(true);
                    __instance.capyDTBCollider.enabled = true;
                    __instance.mountTrigger.SetUnderwaterAssets(true);
                    if (Vector3.Distance(__instance.player.position, __instance.targetPosition.position) <= 140.0 
                        || Vector3.Distance(__instance.player.position, __instance.targetPosition.position) >= 1000.0)
                        return false;
                    __instance.player.position = __instance.targetPosition.position;
                }
                else if (__instance.currentIslandState == IslandCaveDialogueTrigger.CaveIslandState.IslandFinished)
                {
                    __instance.chungy.SetActive(false);
                    __instance.capySiblingObject.SetActive(false);
                    __instance.mountTrigger.displayUI = true;
                }
                else
                {
                    if (__instance.currentIslandState != IslandCaveDialogueTrigger.CaveIslandState.WaitingForDive)
                        return false;
                    __instance.capySiblingDTB.StartNodeName = "Capy2ChatNoDive";
                    __instance.mountTrigger.displayUI = true;
                }
                return false;
            }
            
            [HarmonyPatch("SetIslandHasDiveNow")]
            [HarmonyPrefix]
            public static bool OnSetIslandHasDiveNow(IslandCaveDialogueTrigger __instance)
            {
                __instance.currentIslandState = IslandCaveDialogueTrigger.CaveIslandState.HasDiveNow;
                __instance.mountTrigger.displayUI = true;
                __instance.capyDTBCollider.enabled = false;
                __instance.capySiblingDTB.StartNodeName = "Capy2Chat";
                return false;
            }
        }

        [HarmonyPatch(typeof(InventoryManager))]
        public class InventoryManager_Patch
        {
            [HarmonyPatch("ListItems")]
            [HarmonyPrefix]
            public static bool OnListItems(InventoryManager __instance)
            {
                foreach (Component component in __instance.ItemContent)
                    Destroy(component.gameObject);
                int num = 0;
                foreach (Transform transform in __instance.skinContent)
                {
                    ++num;
                    if (num <= 1)
                        Debug.Log("nothing");
                    else
                        Destroy(transform.gameObject);
                }

                if (PluginMain.SaveDataHandler.CustomPlayerData.HasConch && __instance.pd.currentSceneName != "Zone 3")
                {
                    GameObject gameObject = Instantiate<GameObject>(__instance.conchButton, __instance.ItemContent);
                    gameObject.transform.Find("ItemIcon").GetComponent<Image>();
                    gameObject.SetActive(true);
                }

                foreach (Item obj in __instance.Items)
                {
                    GameObject gameObject = Instantiate<GameObject>(__instance.InventoryItem, __instance.ItemContent);
                    gameObject.transform.Find("ItemIcon").GetComponent<Image>().sprite = obj.icon;
                    gameObject.GetComponent<InventoryButtonBehavior>().itemType = obj;
                }

                __instance.ListSkins();
                __instance.ListPlayerTasks();
                return false;
            }
        }

        [HarmonyPatch(typeof(FlowerCollectingZone3))]
        public class FlowerCollectingZone3_Patch
        {
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            private static bool OnStart(FlowerCollectingZone3 __instance)
            {
                __instance.islandTrigger = __instance.GetComponent<SphereCollider>();
                __instance.rollingSFX = RuntimeManager.CreateInstance("event:/Island_Summon");
                __instance.islandObjects.SetActive(true);
                __instance.lotusPickups.SetActive(false);
                __instance.islandTrigger.enabled = true;
                if (__instance.pd.solvedPuzzles.ContainsKey("lotusFlowerCapy"))
                {
                    __instance.island.SetActive(true);
                    __instance.islandAnim.Play("LotusCaveUP");
                    __instance.mapIcon.SetActive(true);
                    __instance.islandDocking.displayUI = true;
                    __instance.lotusPickups.SetActive(false);
                    foreach (GameObject flowerModel in __instance.flowerModels)
                        flowerModel.SetActive(true);
                }
                else
                {
                    var customData = PluginMain.SaveDataHandler.CustomPlayerData;
                    for (int i = 0; i < customData.LotusCount - customData.LotusCountCurrent; i++)
                    {
                        __instance.flowerModels[i].SetActive(true);
                        ++__instance.flowerCountTotal;
                    }
                }
                return false;
            }
            
            [HarmonyPatch("OnTriggerEnter")]
            [HarmonyPrefix]
            private static bool OnTriggerEnter(FlowerCollectingZone3 __instance, Collider other)
            {
                if (!other.CompareTag("capy") || !__instance.capyActivator.isActivated || !__instance.isReset || PluginMain.SaveDataHandler.CustomPlayerData.LotusCountCurrent == 0)
                    return false;
                __instance.isReset = false;
                __instance.AddFlowers();
                return false;
            }
            
            [HarmonyPatch("AddFlowers")]
            [HarmonyPrefix]
            public static bool AddFlowers(FlowerCollectingZone3 __instance)
            {
                while (PluginMain.SaveDataHandler.CustomPlayerData.LotusCountCurrent > 0)
                {
                    __instance.flowerModels[__instance.flowerCountTotal].SetActive(true);
                    --PluginMain.SaveDataHandler.CustomPlayerData.LotusCountCurrent;
                    ++__instance.flowerCountTotal;
                }
                RuntimeManager.PlayOneShot(__instance.placeFX);
                __instance.isReset = true;
                if (__instance.flowerCountTotal != 5)
                    return false;
                __instance.StartCoroutine(__instance.SolveActivity());
                return false;
            }
        }

        [HarmonyPatch(typeof(InventoryUIManager))]
        public class InventoryUIManager_Patch
        {
            [HarmonyPatch("OpenConchUI")]
            [HarmonyPostfix]
            public static void OpenConchUI(InventoryUIManager __instance)
            {
                var levelButtonParent = __instance.conchUI.transform.Find("Level Buttons/Buttons");
                foreach (var button in levelButtonParent.GetComponentsInChildren<Button>())
                {
                    switch (button.gameObject.name)
                    {
                        case "Home":
                            button.gameObject.SetActive(PluginMain.SaveDataHandler.CustomPlayerData.HasGoneHome);
                            break;
                        case "Level 2":
                            button.gameObject.SetActive(PluginMain.SaveDataHandler.CustomPlayerData.HasGoneFall);
                            break;
                        case "Level 3":
                            button.gameObject.SetActive(PluginMain.SaveDataHandler.CustomPlayerData.HasGoneLake);
                            break;
                        case "Level 5":
                            button.gameObject.SetActive(PluginMain.SaveDataHandler.CustomPlayerData.HasGoneGrove);
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerData))]
        public class PlayerData_Patch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void OnAwake(PlayerData __instance)
            {
                if (!PluginMain.SaveDataHandler.CustomPlayerData.HasFaceThing)
                    return;
                __instance.transform.Find("ModelRoot/shroom player/Armature/Hip/LowerSpine/UpperSpine/Head/mustache").gameObject.SetActive(true);
            }
        }

        [HarmonyPatch(typeof(PickupController))]
        public class PickupController_Patch
        {
            [HarmonyPatch("DropItem")]
            [HarmonyPostfix]
            public static void OnDropItem(PickupController __instance)
            {
                __instance.tpc.gliderEnabled = PluginMain.SaveDataHandler.CustomPlayerData.HasLeaf;
                __instance.hook.enabled = PluginMain.SaveDataHandler.CustomPlayerData.HasHooks;
            }
            
            [HarmonyPatch("UnsetObjectOnPlayer")]
            [HarmonyPostfix]
            public static void UnsetObjectOnPlayer(PickupController __instance)
            {
                __instance.tpc.gliderEnabled = PluginMain.SaveDataHandler.CustomPlayerData.HasLeaf;
                __instance.hook.enabled = PluginMain.SaveDataHandler.CustomPlayerData.HasHooks;
            }
            
            [HarmonyPatch("TeleportObjectOutOfPlayer")]
            [HarmonyPostfix]
            public static void TeleportObjectOutOfPlayer(PickupController __instance, Vector3 pos)
            {
                __instance.tpc.gliderEnabled = PluginMain.SaveDataHandler.CustomPlayerData.HasLeaf;
                __instance.hook.enabled = PluginMain.SaveDataHandler.CustomPlayerData.HasHooks;
            }
        }
    }
}