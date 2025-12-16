using System.Collections;
using System.Collections.Generic;
using CMF;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Playables;
using Yarn;

namespace Smushi_AP_Client
{
    public class LocationHandler
    {
        // ITEM CHECKS
        
        // Tools of the Explorer Found 
        [HarmonyPatch(typeof(ExplorerDialogue))]
        public class ExplorerDialogue_Patch
        {
            [HarmonyPatch("GiveTools")]
            [HarmonyPrefix]
            public static bool OnGiveTools(ExplorerDialogue __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x100);
                __instance.tools.SetActive(false);
                __instance.objectUI.SetText(__instance.item[0]);
                __instance.objectUI.SetUI(true, 0);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
        }
        
        // Tool of Mining Found        
        [HarmonyPatch(typeof(HexkeyPickup))]
        public class HexkeyPickup_Patch
        {
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            private static bool OnStart(HexkeyPickup __instance)
            {
                __instance.playerMask = (LayerMask) (1 << __instance.layer);
                return false;
            }
            
            [HarmonyPatch("OnTriggerStay")]
            [HarmonyPrefix]
            public static bool OnOnTriggerStay(HexkeyPickup __instance, Collider other)
            {
                if ((__instance.playerMask.value & 1 << other.gameObject.layer) <= 0 
                    || !__instance.tpcControls.IsObjInteractButtonPressed() 
                    || __instance.isPickedUp 
                    || __instance.pauseMenu.isOpen)
                    return false;
                PluginMain.ArchipelagoHandler.CheckLocation(0x101);
                __instance.isPickedUp = true;
                __instance.SetDialoguePopup(false);
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, __instance.itemIndex);
                __instance.gameObject.SetActive(false);
                return false;
            }
        }
        
        // Garden Map Found           
        [HarmonyPatch(typeof(MapPickup))]
        public class MapPickup_Patch
        {
            [HarmonyPatch("TakeMap")]
            [HarmonyPrefix]
            public static bool OnTakeMap(MapPickup __instance)
            {
                switch (__instance.pd.currentSceneName)
                {
                    case "Zone 1_F":
                        PluginMain.ArchipelagoHandler.CheckLocation(0x200);
                        break;
                    case "Zone 2_F":
                        PluginMain.ArchipelagoHandler.CheckLocation(0x300);
                        break;
                    case "Zone 3":
                        PluginMain.ArchipelagoHandler.CheckLocation(0x400);
                        break;
                    case "Zone 5":
                        PluginMain.ArchipelagoHandler.CheckLocation(0x500);
                        break;
                }
                return true;
            }
        }
        
        // Puzzle Spores:
        // Yellow Shrine Energy Spore - yellow
        // Pink Shrine Energy Spore - pink
        // Blue Shrine Energy Spore - green
        // Maple Sanctuary Energy Spore - sporeIncense
        // Restless Stream Energy Spore - acorn
        [HarmonyPatch(typeof(SporePickup))]
        public class SporePickup_Patch
        {
            [HarmonyPatch("PickupObject")]
            [HarmonyPrefix]
            public static bool OnPickupObject(SporePickup __instance)
            {
                if (__instance.isPuzzleReward)
                {
                    switch (__instance.puzzleName)
                    {
                        case "yellow":
                            PluginMain.ArchipelagoHandler.CheckLocation(0x201);
                            break;
                        case "pink":
                            PluginMain.ArchipelagoHandler.CheckLocation(0x202);
                            break;
                        case "green":
                            PluginMain.ArchipelagoHandler.CheckLocation(0x203);
                            break;
                        case "sporeIncense":
                            PluginMain.ArchipelagoHandler.CheckLocation(0x308);
                            break;
                        case "acorn":
                            PluginMain.ArchipelagoHandler.CheckLocation(0x30B);
                            break;

                    }
                }
                __instance.isPickedUp = true;
                __instance.SetDialoguePopup(false);
                if (__instance.isPuzzleReward)
                    __instance.pd.solvedPuzzles[__instance.puzzleName] = true;
                __instance.model.SetActive(false);
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, __instance.objectIndex);
                return false;
            }
        }
        
        // Dark Cave Energy Spore  
        [HarmonyPatch(typeof(LostCaveInteraction))]
        public class LostCaveInteraction_Patch
        {
            [HarmonyPatch("GiveBlueMushroom")]
            [HarmonyPrefix]
            public static bool OnGiveBlueMushroom(LostCaveInteraction __instance)
            {
                if (__instance.pd.dialogBools["foundLost2"] && __instance.pd.dialogBools["foundLost1"])
                {
                    PluginMain.ArchipelagoHandler.CheckLocation(0x705);
                    __instance.pd.skinMats["pelagic"] = true;
                    __instance.objectUI.SetTextAugmenter(__instance.augmenter);
                    __instance.objectUI.SetUI(true, 3);
                    __instance.StartCoroutine(__instance.ContinueDialogue());
                }
                else
                {
                    PluginMain.ArchipelagoHandler.CheckLocation(0x30E);
                    __instance.objectUI.SetText(__instance.item);
                    __instance.objectUI.SetUI(true, 0);
                    __instance.StartCoroutine(__instance.ContinueDialogue());
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(OnionDialogue))]
        public class OnionDialogue_Patch
        {
            [HarmonyPatch("TradeBerry")]
            [HarmonyPrefix]
            public static bool OnTradeBerry(OnionDialogue __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x703);
                __instance.objectUI.SetTextAugmenter(__instance.augmenter);
                __instance.objectUI.SetUI(true, 10);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
        }

        // Indigo Island Energy Spore  0x405
        // Indigo Island Wind Essence  0x406
        [HarmonyPatch(typeof(Level3Shop))]
        public class Level3Shop_Patch
        {
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            public static void OnStart(Level3Shop __instance)
            {
                __instance.signPost.SetActive(false);
                __instance.modelRoot.SetActive(true);
                __instance.dtb.enabled = true;
            }
            
            [HarmonyPatch("TradeSpore")]
            [HarmonyPrefix]
            public static bool OnTradeSpore(Level3Shop __instance)
            {
                __instance.pd.rockCount -= __instance.price;
                __instance.pd.dialogBools["boughtSpore"] = true;
                __instance.rockUI.UpdateCount();
                __instance.itemModels[0].SetActive(false);
                __instance.objectUI.SetText(__instance.sporeItem);
                __instance.objectUI.SetUI(true, 0);
                PluginMain.ArchipelagoHandler.CheckLocation(0x405);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
            
            [HarmonyPatch("TradeEssence")]
            [HarmonyPrefix]
            public static bool OnTradeEssence(Level3Shop __instance)
            {
                __instance.pd.rockCount -= __instance.price;
                __instance.pd.dialogBools["boughtEssence"] = true;
                __instance.rockUI.UpdateCount();
                __instance.itemModels[1].SetActive(false);
                __instance.objectUI.SetText(__instance.essenceItem);
                __instance.objectUI.SetUI(true, 1);
                PluginMain.ArchipelagoHandler.CheckLocation(0x406);
                __instance.StartCoroutine(__instance.ContinueDialogue2());
                return false;
            }
        }
        
        // Myrtle Pools Wind Essence - Wisp1EssenceGained  
        // Anemone Woods Wind Essence - Wisp2EssenceGained
        // Cryptic Caverns Wind Essence - Wisp3EssenceGained
        // Brick Chimney Wind Essence - Wisp4EssenceGained
        [HarmonyPatch(typeof(WispInteraction))]
        public class WispInteraction_Patch
        {
            [HarmonyPatch("UpgradeEssence")]
            [HarmonyPrefix]
            public static bool OnUpgradeEssence(WispInteraction __instance)
            {
                switch (__instance.nodeName)
                {
                    case "Wisp1EssenceGained":
                        PluginMain.ArchipelagoHandler.CheckLocation(0x208);
                        break;
                    case "Wisp2EssenceGained":
                        PluginMain.ArchipelagoHandler.CheckLocation(0x205);
                        break;
                    case "Wisp3EssenceGained":
                        PluginMain.ArchipelagoHandler.CheckLocation(0x309);
                        break;
                    case "Wisp4EssenceGained":
                        PluginMain.ArchipelagoHandler.CheckLocation(0x30A);
                        break;
                }
                __instance.pd.dialogBools["spokenToWisp"] = true;
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, __instance.essenceObjectIndex);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
        }
        
        // Blade of Power Purchase     
        [HarmonyPatch(typeof(HopperDialogue))]
        public class HopperDialogue_Patch
        {
            [HarmonyPatch("TradeNeedle")]
            [HarmonyPrefix]
            public static bool OnTradeNeedle(HopperDialogue __instance)
            {
                __instance.pd.rockCount -= __instance.cost;
                __instance.rockUI.UpdateCount();
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 9);
                PluginMain.ArchipelagoHandler.CheckLocation(0x204);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                Debug.Log("needle traded");
                return false;
            }
        }
        
        // Mycology Journal Unlock  
        [HarmonyPatch(typeof(MycologistBehavior))]
        public class MycologistBehavior_Patch
        {
            [HarmonyPatch("EnableJournal")]
            [HarmonyPrefix]
            public static bool OnEnableJournal(MycologistBehavior __instance)
            {
                __instance.inv.RemoveItem(__instance.item);
                PluginMain.ArchipelagoHandler.CheckLocation(0x206);
                return false;
            }
            
            [HarmonyPatch("DisplayInstructions")]
            [HarmonyPrefix]
            public static bool OnDisplayInstructions(MycologistBehavior __instance)
            {
                return false;
            }
        }


        
        // GENERIC ITEMS
        // Band of Elasticity Found
        // Ancient Relic 1 Found       
        // Ancient Relic 2 Found       
        // Ring of Youth Found         
        // Ring of Love Found          
        // Ring of Prosperity Found    
        // Ring of Spirit Found    
        // Band Aid Found       
        // Sacred Orb Found   
        // Explosive Powder 1 Found  
        // Explosive Powder 2 Found  
        // Container of Light Found 
        // Secret Opener Found         
        // Sacred Streamer 2 Obtained
        // Sacred Streamer 3 Obtained  
        [HarmonyPatch(typeof(ItemPickupGeneric))]
        public class ItemPickupGeneric_Patch
        {
            private static Dictionary<string, int> _genericIds = new()
            {
                { "rubberBand", 0x207 },
                { "coin1", 0x20A },
                { "coin2", 0x20B },
                { "sapphire", 0x503 },
                { "emerald", 0x504 },
                { "diamond", 0x505 },
                { "citrine", 0x506 },
                { "amethyst", 0x507 },
                { "hasBandaid", 0x502 },
                { "clubhousePowder", 0x303 },
                { "cavePowder", 0x304 },
                { "lightbulb", 0x30C },
                { "secretKey", 0x407 },
                { "statueShide", 0x509 },
                { "underwaterShide", 0x50A },
                { "hasPencil", 0x20D }
            };
            
            [HarmonyPatch("PickupObject")]
            [HarmonyPrefix]
            public static bool OnPickupObject(ItemPickupGeneric __instance)
            {
                if (!_genericIds.ContainsKey(__instance.dialogeBoolName))
                    return true;
                __instance.isPickedUp = true;
                __instance.SetDialoguePopup(false);
                // TODO Set bool somewhere for check completed
                PluginMain.ArchipelagoHandler.CheckLocation(_genericIds[__instance.dialogeBoolName]);
                if (__instance.dialogeBoolName == "hasPencil")
                {
                    PluginMain.SaveDataHandler.CustomPlayerData.HasPencil = true;
                    PluginMain.SaveDataHandler.SaveGame();
                }
                __instance.model.SetActive(false);
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, __instance.objectIndex);
                __instance.gameObject.SetActive(false);
                __instance.OnPickup.Invoke();
                return false;
            }
            
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            public static bool OnStart(ItemPickupGeneric __instance)
            {
                __instance.tpc = Object.FindObjectOfType<AdvancedWalkerController>();
                __instance.pauseMenu = Object.FindObjectOfType<PauseMenuController>();
                __instance.playerMask = 1 << __instance.layer;
                // TODO Remove items that have been checked
                return false;
            }
        }
        
        // Myrtle Pools Blueberry      
        [HarmonyPatch(typeof(BlueberryTraderInteraction))]
        public class BlueberryTraderInteraction_Patch
        {
            [HarmonyPatch("TradeBerry")]
            [HarmonyPrefix]
            public static bool OnTradeBerry (BlueberryTraderInteraction __instance)
            {
                __instance.pd.rockCount -= __instance.berryCost;
                __instance.pd.dialogBools["boughtBerry"] = true;
                __instance.rockUI.UpdateCount();
                __instance.berries[0].SetActive(false);
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 2);
                PluginMain.ArchipelagoHandler.CheckLocation(0x209);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                Debug.Log("berry traded");
                return false;
            }
        }
        
        // Crystal Cave Blueberry      
        [HarmonyPatch(typeof(BlueberryPickup))]
        public class BlueberryPickup_Patch
        {
            [HarmonyPatch("PickupObject")]
            [HarmonyPrefix]
            public static bool OnPickupObject(BlueberryPickup __instance)
            {
                __instance.isPickedUp = true;
                __instance.SetDialoguePopup(false);
                __instance.model.SetActive(false);
                __instance.pd.dialogBools["caveBerry"] = true;
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, __instance.objectIndex);
                PluginMain.ArchipelagoHandler.CheckLocation(0x20C);
                __instance.gameObject.SetActive(false);
                return false;
            }
        }
        
        // Sturdy Hooks Purchase       
        [HarmonyPatch(typeof(ClimberDialogue))]
        public class ClimberDialogue_Patch
        {
            [HarmonyPatch("TradeCrystals")]
            [HarmonyPrefix]
            public static bool OnTradeCrystals(ClimberDialogue __instance)
            {
                __instance.pd.rockCount -= __instance.upgradeCost;
                __instance.rockUI.UpdateCount();
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 10);
                PluginMain.ArchipelagoHandler.CheckLocation(0x301);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                Debug.Log("First Hook Upgrade");
                return false;
            }
        }

        // Nectar Collector  
        [HarmonyPatch(typeof(NectarEventHandler))]
        public class NectarEventHandler_Patch
        {
            [HarmonyPatch("NectarReward")]
            [HarmonyPrefix]
            public static bool OnNectarReward(NectarEventHandler __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x302);
                return true;
            }
            
            [HarmonyPatch("NectarSkinReward")]
            [HarmonyPrefix]
            public static bool NectarSkinReward(NectarEventHandler __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x706);
                __instance.objectUI.SetTextAugmenter(__instance.augmenter);
                __instance.objectUI.SetUI(true, 8);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
        }
  
        // Secret Password Found       
        [HarmonyPatch(typeof(ObstacleEventHandler))]
        public class ObstacleEventHandler_Patch
        {
            [HarmonyPatch("GiveGreenSkin")]
            [HarmonyPrefix]
            public static bool OnGiveGreenSkin(ObstacleEventHandler __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x704);
                __instance.objectUI.SetTextAugmenter(__instance.augmenter);
                __instance.objectUI.SetUI(true, 1);
                __instance.StartCoroutine(__instance.ContinueDialogue2());
                return false;
            }
            
            [HarmonyPatch("GivePassword")]
            [HarmonyPrefix]
            public static bool OnGivePassword(ObstacleEventHandler __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x305);
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 5);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
            
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            private static void Start(ObstacleEventHandler __instance)
            {
                if (SceneChangeVars.fromObstacleCourse)
                {
                    __instance.pauser.PauseThePlayer();
                    __instance.StartCoroutine(LoadsceneFromCourseCo(__instance, false));
                    SceneChangeVars.fromObstacleCourse = false;
                }
                else if (SceneChangeVars.fromQuittingObstacleCourse)
                {
                    __instance.pauser.PauseThePlayer();
                    __instance.StartCoroutine(LoadsceneFromCourseCo(__instance, true));
                    SceneChangeVars.fromQuittingObstacleCourse = false;
                }
                else
                {
                    if (!__instance.col1 || !(__instance.col2 != null))
                        return;
                    __instance.col1.enabled = true;
                    __instance.col2.enabled = true;
                }
            }
            
            private static IEnumerator LoadsceneFromCourseCo(ObstacleEventHandler obs, bool fromQuitting)
            {
                obs.exMember.position = obs.exPOS.position;
                obs.npcIK.SetAimTarget();
                yield return (object) new WaitForSeconds(1f);
                if (!fromQuitting)
                {
                    if (!obs.pd.dialogBools["obstacle1_beaten"])
                    {
                        obs.dtb.StartDialogue("GivePassword");
                        obs.pd.dialogBools["obstacle1_beaten"] = true;
                    }
                    else if (!obs.pd.dialogBools["obstacle2_beaten"])
                    {
                        obs.dtb.StartDialogue("GiveObstacle2Reward");
                        obs.pd.dialogBools["obstacle2_beaten"] = true;
                    }
                    else
                        obs.dtb.StartDialogue("Obstacle2NoReward");
                }
                else if (!obs.pd.dialogBools["knowsPass"])
                    obs.dtb.StartDialogue("QuitObstacle");
                else
                    obs.dtb.StartDialogue("QuitObstacle2");
            }
        }
        
        // Super Essence Found   
        [HarmonyPatch(typeof(WindObstacle))]
        public class WindObstacle_Patch
        {
            [HarmonyPatch("RewardSuperEssence")]
            [HarmonyPrefix]
            public static bool OnRewardSuperEssence(WindObstacle __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x306);
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 6);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
        }

        // Firestarter Kit Found     
        [HarmonyPatch(typeof(FlintSteelPurchase))]
        public class FlintSteelPurchase_Patch
        {
            [HarmonyPatch("TradeFlint")]
            [HarmonyPrefix]
            public static bool OnTradeFlint(FlintSteelPurchase __instance)
            {
                __instance.pd.rockCount -= __instance.flintCost;
                __instance.rockUI.UpdateCount();
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 9);
                PluginMain.ArchipelagoHandler.CheckLocation(0x307);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                Debug.Log("flint traded");
                return false;
            }
        }
        
        // Headlamp Acquired    
        [HarmonyPatch(typeof(RileyInteraction))]
        public class RileyInteraction_Patch
        {
            [HarmonyPatch("GiveHeadlamp")]
            [HarmonyPrefix]
            public static bool OnGiveHeadlamp(RileyInteraction __instance)
            {
                __instance.objectUI.SetText(__instance.headlampItem);
                __instance.objectUI.SetUI(true, 4);
                __instance.inv.RemoveItem(__instance.lightbulb);
                PluginMain.ArchipelagoHandler.CheckLocation(0x30D);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
        }
        
        // Essence of Water Purchase   
        [HarmonyPatch(typeof(DiverInteraction))]
        public class DiverInteraction_Patch
        {
            [HarmonyPatch("TradeWaterEssence")]
            [HarmonyPrefix]
            public static bool OnTradeWaterEssence(DiverInteraction __instance)
            {
                __instance.pd.rockCount -= __instance.cost;
                __instance.rockUI.UpdateCount();
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 3);
                PluginMain.ArchipelagoHandler.CheckLocation(0x401);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                Debug.Log("Water traded");
                return false;
            }
        }
        
        // Old String Found   
        [HarmonyPatch(typeof(WaterSporeCollecting))]
        public class WaterSporeCollecting_Patch
        {
            [HarmonyPatch("RopeReward")]
            [HarmonyPrefix]
            public static bool OnRopeReward(WaterSporeCollecting __instance)
            {
                __instance.pd.dialogBools["collectingSpores"] = false;
                __instance.pd.dialogBools["hasCollectedAllSpores"] = true;
                __instance.dialogue.StartNodeName = "ChojoStart";
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 5);
                __instance.thimblePickup.DropItem();
                __instance.thimblePickup.gameObject.SetActive(false);
                __instance.thimbleModel.SetActive(true);
                __instance.pickup.UnsetObjectOnPlayer();
                __instance.pickup.isNearItem = false;
                __instance.pickup.detectedObject = null;
                __instance.rope.SetActive(false);
                PluginMain.ArchipelagoHandler.CheckLocation(0x402);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
            
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            private static void OnStart(WaterSporeCollecting __instance) { __instance.rope.SetActive(true); }
        }
        
        // Chungy Saved          
        [HarmonyPatch(typeof(StringEvent))]
        public class StringEvent_Patch
        {
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            public static bool OnStart(StringEvent __instance)
            {
                __instance.director = Object.FindObjectOfType<PlayableDirector>();
                __instance.rockTrigger = __instance.GetComponent<BoxCollider>();
                if (__instance.pd.dialogBools["hasString"] && !__instance.pd.dialogBools["rescuedChungy"] && __instance.tpc.hookEnabled)
                    __instance.rockTrigger.enabled = true;
                if (!__instance.pd.dialogBools["rescuedChungy"])
                    return false;
                __instance.chungyObject.gameObject.SetActive(false);
                __instance.stringModel.SetActive(true);
                return false;
            }
            
            [HarmonyPatch("GiveChungyHooksCo")]
            [HarmonyPrefix]
            public static void OnGiveChungyHooksCo(StringEvent __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x403);
            }
        }
        
        // Screwdriver Purchase           
        [HarmonyPatch(typeof(ScavengerInteraction))]
        public class ScavengerInteraction_Patch
        {
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            public static bool OnStart(ScavengerInteraction __instance)
            {
                return false;
            }
            
            [HarmonyPatch("TradeScrew")]
            [HarmonyPrefix]
            public static bool OnTradeScrew(ScavengerInteraction __instance)
            {
                __instance.pd.rockCount -= __instance.cost;
                __instance.rockUI.UpdateCount();
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 0);
                __instance.screwModel.SetActive(false);
                PluginMain.ArchipelagoHandler.CheckLocation(0x501);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                __instance.dialogue.StartNodeName = "ScavengerChat";
                return false;
            }
        }

        [HarmonyPatch(typeof(CoinPuzzleHandler))]
        public class CoinPuzzleHandler_Patch
        {
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            private static void OnStart(CoinPuzzleHandler __instance)
            {
                __instance.director = __instance.GetComponent<PlayableDirector>();
                var pd = __instance.pd;
                __instance.dialogueRunner.AddFunction("CheckCoins", 1, boolName => pd.dialogBools["coin1"] && pd.dialogBools["coin2"]);
                if (PluginMain.ArchipelagoHandler.IsLocationChecked(0x20A))
                    __instance.coinObjects[1].SetActive(false);
                if (PluginMain.ArchipelagoHandler.IsLocationChecked(0x20B))
                    __instance.coinObjects[0].SetActive(false);
                if (__instance.hasDroppedPlatform)
                {
                    __instance.oilCan.position = new Vector3(942.88f, 43.69f, 1.17f);
                    __instance.rope.localScale = new Vector3(1f, 1.95f, 1f);
                    __instance.crystal.SetActive(false);
                }
                else
                    __instance.crystal.SetActive(true);
                if (!__instance.pd.dialogBools["completedCoinActivity"])
                    return;
                __instance.envCoinObjects[0].SetActive(true);
                __instance.envCoinObjects[1].SetActive(true);
                __instance.envCoinObjects[2].SetActive(true);
                __instance.envCoinObjects[3].SetActive(false);
            }
        }
        
        // Sacred Streamer 1 Obtained  
        [HarmonyPatch(typeof(RicoInteraction))]
        public class RicoInteraction_Patch
        {
            [HarmonyPatch("TradeShide")]
            [HarmonyPrefix]
            public static bool OnTradeShide(RicoInteraction __instance)
            {
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(true, 7);
                PluginMain.ArchipelagoHandler.CheckLocation(0x508);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                __instance.dialogue.StartNodeName = "RicoPost";
                return false;
            }
            
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            public static bool Start(RicoInteraction __instance)
            {
                if (__instance.isZone0)
                    __instance.gameObject.SetActive(true);
                return false;
            }
            
            [HarmonyPatch("GiveSuperSpore")]
            [HarmonyPrefix]
            public static bool GiveSuperSpore(RicoInteraction __instance)
            { 
                __instance.objectUI.SetText(__instance.superSpore);
                __instance.objectUI.SetUI(true, 2);
                PluginMain.ArchipelagoHandler.CheckLocation(0x102);
                __instance.StartCoroutine(__instance.ContinueDialogue2());
                return false;
            }
        }
        
        // Sacred Streamer 4 Obtained  
        [HarmonyPatch(typeof(RingTradeInteraction))]
        public class RingTradeInteraction_Patch
        {
            [HarmonyPatch("TriggerRingReward")]
            [HarmonyPrefix]
            public static bool OnTriggerRingReward(RingTradeInteraction __instance)
            {
                if (__instance.pd.ringCount < 3)
                    return false;
                if (__instance.pd.ringCount == 3)
                {
                    PluginMain.ArchipelagoHandler.CheckLocation(0x50B);
                    __instance.objectUI.SetText(__instance.shideItem);
                    __instance.objectUI.SetUI(isOpening: true, 7);
                    __instance.StartCoroutine(__instance.ContinueDialogue());
                }
                else
                {
                    PluginMain.ArchipelagoHandler.CheckLocation(0x70B);
                    __instance.dialogue.StartNodeName = "GweenyAllRingsPost";
                    __instance.objectUI.SetTextAugmenter(__instance.aug);
                    __instance.objectUI.SetUI(isOpening: true, 8);
                    __instance.StartCoroutine(__instance.ContinueDialogue());
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(StrawberryActivityHandler))]
        public class StrawberryActivityHandler_Patch
        {
            [HarmonyPatch("StrawberryReward")]
            [HarmonyPrefix]
            public static bool OnStrawberryReward(StrawberryActivityHandler __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x701);
                __instance.objectUI.SetTextAugmenter(__instance.augmenter);
                __instance.objectUI.SetUI(true, 8);
                __instance.inv.RemoveItem(__instance.rubberBand);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
        }

        [HarmonyPatch(typeof(DaisyInteraction))]
        public class DaisyInteraction_Patch
        {
            [HarmonyPatch("GiveGreenSkin")]
            [HarmonyPrefix]
            public static bool GiveGreenSkin(DaisyInteraction __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x702);
                __instance.objectUI.SetTextAugmenter(__instance.augmenter);
                __instance.objectUI.SetUI(true, 11);
                __instance.StartCoroutine(__instance.ContinueDialogue2());
                return false;
            }
        }

        [HarmonyPatch(typeof(ShimuHandler))]
        public class ShimuHandler_Patch
        {
            [HarmonyPatch("GiveConch")]
            [HarmonyPrefix]
            public static bool GiveConch(ShimuHandler __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x103);
                __instance.objectUI.SetText(__instance.item);
                __instance.objectUI.SetUI(isOpening: true, 0);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
        }


        [HarmonyPatch(typeof(WaterRockActivity))]
        public class WaterRockActivity_Patch
        {
            [HarmonyPatch("SkinReward")]
            [HarmonyPrefix]
            public static bool SkinReward(WaterRockActivity __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x708);
                __instance.objectUI.SetTextAugmenter(__instance.augmenter);
                __instance.objectUI.SetUI(true, 4);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }

            [HarmonyPatch("EndActivity")]
            [HarmonyPrefix]
            public static bool EndActivity(WaterRockActivity __instance)
            {
                __instance.StartCoroutine(EndRockActivity(__instance));
                return false;
            } 

            private static IEnumerator EndRockActivity(WaterRockActivity __instance)
            {
                __instance.pauser.PauseThePlayer();
                __instance.counter.gameObject.SetActive(false);
                __instance.dtb.gameObject.SetActive(true);
                __instance.dtb.dialogueIsRunning = true;
                __instance.uiFade.FadeIn();
                yield return new WaitForSeconds(1f);
                __instance.SetupRockLocations(false);
                __instance.capy.position = __instance.capyTeleportReturnPOS.position;
                __instance.goalParticle.Stop();
                foreach (GameObject placeholder in __instance.placeholders)
                    placeholder.SetActive(false);
                if (__instance.pickup.isHoldingItem)
                {
                    __instance.pickup.DropItem();
                    __instance.pickup.UnsetObjectOnPlayer();
                }
                __instance.pickup.isNearItem = false;
                __instance.pickup.detectedObject = null;
                __instance.player.position = __instance.positions[0].position;
                __instance.npcIK.SetAimTarget();
                if (__instance.hook.isClimbing)
                    __instance.hook.DisableHookFromCutscene();
                if (__instance.tpc.IsSwimming())
                {
                    __instance.tpc.isDiving = false;
                    __instance.tpc.ResetSwimTeleport();
                    __instance.diveController.ResetParticlesFromActivity();
                }
                __instance.dtb.SetPlayerRotation();
                __instance.dtb.SwitchToDialogueCam(false);
                yield return new WaitForSeconds(1f);
                __instance.diveController.DisableLowpass();
                __instance.uiFade.FadeOut();
                yield return new WaitForSeconds(0.5f);
                __instance.hook.enabled = PluginMain.SaveDataHandler.CustomPlayerData.HasHooks;
                __instance.miner.canMine = PluginMain.SaveDataHandler.CustomPlayerData.HasHexKey;
                __instance.pd.hasHexkey = PluginMain.SaveDataHandler.CustomPlayerData.HasHexKey;
                if (__instance.rockCount == 2)
                {
                    __instance.dtb.StartDialogue("DiverEndSuccess");
                    __instance.pd.dialogBools["beatDiver"] = true;
                }
                else
                    __instance.dtb.StartDialogue("DiverEndFail");
            }
        }


        [HarmonyPatch(typeof(GlowbugInteraction))]
        public class GlowbugInteraction_Patch
        {
            [HarmonyPatch("GiveRainbowAug")]
            [HarmonyPrefix]
            public static bool GiveRainbowAug(GlowbugInteraction __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x70C);
                __instance.objectUI.SetTextAugmenter(__instance.augmenter);
                __instance.objectUI.SetUI(true, 1);
                __instance.StartCoroutine(__instance.ContinueDialogue());
                return false;
            }
        }
       
        
        // AUGMENTER CHECKS
        [HarmonyPatch(typeof(SkinObjectPickup))]
        public class SkinObjectPickup_Patch
        {
            private static Dictionary<string, int> _augmenterTypes = new()
            {
                { "PurpleAugmenter_Name", 0x0 },
                { "StrawberryAugmenter_Name", 0x1 },
                { "FlowerAugmenter_Name", 0x2 },
                { "OnionAug_Name", 0x3 },
                { "GreenAugmenter_Name", 0x4 },
                { "PelagicAugmenter_Name", 0x5 },
                { "HoneyAugmenter_Name", 0x6 },
                { "SparkleAug_Name", 0x7 },
                { "SpikeyAug_Name", 0x8 },
                { "InkcapAug_Name", 0x9 },
                { "SharpAug_Name", 0xA },
                { "CrystalAug_Name", 0xB },
                { "RainbowAug_Name", 0xC },
                { "VeilAug_Name", 0xD },
                { "SacredAug_Name", 0xE },
            };
            
            [HarmonyPatch("PickupObject")]
            [HarmonyPrefix]
            public static bool OnPickupObject(SkinObjectPickup __instance)
            {
                __instance.isPickedUp = true;
                __instance.SetDialoguePopup(false);
                __instance.objectUI.SetTextAugmenter(__instance.item);
                __instance.objectUI.SetUI(true, __instance.objectIndex);
                PluginMain.ArchipelagoHandler.CheckLocation(0x700 + _augmenterTypes[__instance.item.tableEntryName]);
                __instance.pd.skinMats[__instance.materialName] = true;
                if (__instance.isPuzzleReward && __instance.pd.solvedPuzzles.ContainsKey(__instance.puzzleName))
                    __instance.pd.solvedPuzzles[__instance.puzzleName] = true;
                __instance.OnPickup.Invoke();
                __instance.gameObject.SetActive(false);
                return false;
            }
        }
        
        // MYCOLOGY CHECKS
        [HarmonyPatch(typeof(MushButtonBehavior))]
        public class MushButtonBehavior_Patch
        {
            private static Dictionary<string, int> _mushTypes = new()
            {
                { "mycena", 0x0 },
                { "volvo", 0x1 },
                { "capno", 0x2 },
                { "macro_proc", 0x3 },
                { "ento_hochs", 0x4 },
                { "chalc_pip", 0x5 },
                { "canth_cib", 0x8 },
                { "hygroflav", 0x9 },
                { "daetri", 0xA },
                { "psath_aqua", 0x10 },
                { "amanitaMusc", 0xB },
                { "bolete", 0xC },
                { "calv_zol", 0xD },
                { "cop_comat", 0x6 },
                { "hygro_conica", 0xE },
                { "LactIndigo", 0xF },
                { "chloro", 0x7 },
                { "fairy", 0x12 },
                { "hygrominiata", 0x11 },
                { "cop_dome", 0x15 },
                { "phallus", 0x13 },
                { "lacc_ameth", 0x14 },
                { "heart", 0x16 },
            };
            
            [HarmonyPatch("ResearchMushroom")]
            [HarmonyPrefix]
            public static void OnResearchMushroom(MushButtonBehavior __instance)
            {
                PluginMain.ArchipelagoHandler.CheckLocation(0x600 + _mushTypes[__instance.mushType]);
            }
        }
    }
}