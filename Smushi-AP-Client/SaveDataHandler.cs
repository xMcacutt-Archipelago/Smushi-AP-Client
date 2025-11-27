using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

namespace Smushi_AP_Client
{
    public class CustomPlayerData
    {
        public int ItemIndex { get; set; }
        public bool HasLeaf { get; set; }
        public bool HasHooks { get; set; }
        public bool HasHexKey { get; set; }
        public bool HasGoneFall { get; set; }
        public bool HasGoneLake { get; set; }
        public bool HasGoneHome { get; set; }
        public bool HasGoneGrove { get; set; }
        public bool HasConch { get; set; }
        public int LotusCount { get; set; }
        public int LotusCountCurrent { get; set; }
        public bool HasFaceThing { get; set; }
    }
    
    public class SaveDataHandler : MonoBehaviour
    {
        public CustomPlayerData CustomPlayerData = new();
        private const bool DebugCapys = false;
        
        public void InitSave(string seed, string slot)
        {
            var slotIndex = 1;
            var fileName = $"SCHAP_{slot}{seed}.save";
            var saveSystem = SaveSystem.Get();
            saveSystem._slotNames[slotIndex] = fileName;
            saveSystem._fileSettings[slotIndex].path = fileName;
            saveSystem._slots[slotIndex] = new ES3File(saveSystem._fileSettings[slotIndex]);
            saveSystem.SyncCache(slotIndex, true);
            saveSystem.SetActiveSaveSlot(slotIndex);
            var currentSceneExists = saveSystem._slots[slotIndex].KeyExists("currentScene");
            var sceneIsBad = false;
            if (currentSceneExists)
            {
                var scene = saveSystem._slots[slotIndex].Load<string>("currentScene");
                if (scene.Equals("home", StringComparison.InvariantCultureIgnoreCase))
                    sceneIsBad = true;
                if (scene.StartsWith("Zone 0"))
                    sceneIsBad = true;
            }
            if (sceneIsBad || !currentSceneExists)
            {
                SaveSystem.Get().Save("currentScene", "Zone 1_F");
                SaveSystem.Get().Save("playerPosition", new Vector3(867.511169f, 19.8680038f, -49.1699677f));
                if (DebugCapys)
                {
                    SaveSystem.Get().Save("currentScene", "Zone 3");
                    SaveSystem.Get().Save("playerPosition", new Vector3(0, 0, 0));
                }
            }
            saveSystem._gameSettings.isSpeedrunnerMode = true;
            var mainMenu = FindObjectOfType<OptionsMainMenu>();
            mainMenu.LoadGame();
        }

        [HarmonyPatch(typeof(SaveSystem))]
        public class SaveSystem_Patch
        {
            [HarmonyPatch(typeof(SaveSystem))]
            [HarmonyPatch("SaveAllData", typeof(int), typeof(SaveLoadManager), typeof(bool))]
            [HarmonyPostfix]
            static void SaveAllData(SaveSystem __instance, int slot, SaveLoadManager manager, 
                bool ignoreFrequencyRestrictions = false)
            {
                if (manager.pd)
                {
                    __instance._slots[slot].Save("itemIndex", PluginMain.SaveDataHandler.CustomPlayerData.ItemIndex);
                    __instance._slots[slot].Save("hasLeaf", PluginMain.SaveDataHandler.CustomPlayerData.HasLeaf);
                    __instance._slots[slot].Save("hasHooks", PluginMain.SaveDataHandler.CustomPlayerData.HasHooks);
                    __instance._slots[slot].Save("hasTools", false);
                    __instance._slots[slot].Save("canMine", PluginMain.SaveDataHandler.CustomPlayerData.HasHexKey);
                    __instance._slots[slot].Save("hasGoneFall", PluginMain.SaveDataHandler.CustomPlayerData.HasGoneFall);
                    __instance._slots[slot].Save("hasGoneLake", PluginMain.SaveDataHandler.CustomPlayerData.HasGoneLake);
                    __instance._slots[slot].Save("hasGoneHome", PluginMain.SaveDataHandler.CustomPlayerData.HasGoneHome);
                    __instance._slots[slot].Save("hasGoneGrove", PluginMain.SaveDataHandler.CustomPlayerData.HasGoneGrove);
                    __instance._slots[slot].Save("hasConch", PluginMain.SaveDataHandler.CustomPlayerData.HasConch);
                    __instance._slots[slot].Save("lotusCount", PluginMain.SaveDataHandler.CustomPlayerData.LotusCount);
                    __instance._slots[slot].Save("lotusCountCurrent", PluginMain.SaveDataHandler.CustomPlayerData.LotusCountCurrent);
                    __instance._slots[slot].Save("hasFaceThing", PluginMain.SaveDataHandler.CustomPlayerData.HasFaceThing);
                }
                __instance.SyncCache(slot, ignoreFrequencyRestrictions);
            }
            
            [HarmonyPatch("LoadAllData", typeof(int), typeof(SaveLoadManager))]
            [HarmonyPostfix]
            static void LoadAllData(SaveSystem __instance, int slot, SaveLoadManager manager)
            {
                if (__instance._slots[slot].KeyExists("itemIndex"))
                    PluginMain.SaveDataHandler.CustomPlayerData.ItemIndex
                        = __instance._slots[slot].Load<int>("itemIndex");;
                if (__instance._slots[slot].KeyExists("hasLeaf"))
                    PluginMain.SaveDataHandler.CustomPlayerData.HasLeaf 
                        = __instance._slots[slot].Load<bool>("hasLeaf");
                if (__instance._slots[slot].KeyExists("hasHooks"))
                    PluginMain.SaveDataHandler.CustomPlayerData.HasHooks
                        = __instance._slots[slot].Load<bool>("hasHooks");
                if (__instance._slots[slot].KeyExists("canMine"))
                    PluginMain.SaveDataHandler.CustomPlayerData.HasHexKey
                        = __instance._slots[slot].Load<bool>("canMine");
                if (__instance._slots[slot].KeyExists("hasGoneFall"))
                    PluginMain.SaveDataHandler.CustomPlayerData.HasGoneFall
                        = __instance._slots[slot].Load<bool>("hasGoneFall");
                if (__instance._slots[slot].KeyExists("hasGoneLake"))
                    PluginMain.SaveDataHandler.CustomPlayerData.HasGoneLake
                        = __instance._slots[slot].Load<bool>("hasGoneLake");
                if (__instance._slots[slot].KeyExists("hasGoneHome"))
                    PluginMain.SaveDataHandler.CustomPlayerData.HasGoneHome
                        = __instance._slots[slot].Load<bool>("hasGoneHome");
                if (__instance._slots[slot].KeyExists("hasGoneGrove"))
                    PluginMain.SaveDataHandler.CustomPlayerData.HasGoneGrove
                        = __instance._slots[slot].Load<bool>("hasGoneGrove");
                if (__instance._slots[slot].KeyExists("hasConch"))
                    PluginMain.SaveDataHandler.CustomPlayerData.HasConch
                        = __instance._slots[slot].Load<bool>("hasConch");
                if (__instance._slots[slot].KeyExists("lotusCount"))
                    PluginMain.SaveDataHandler.CustomPlayerData.LotusCount
                        = __instance._slots[slot].Load<int>("lotusCount");
                if  (__instance._slots[slot].KeyExists("lotusCountCurrent"))
                    PluginMain.SaveDataHandler.CustomPlayerData.LotusCountCurrent
                        = __instance._slots[slot].Load<int>("lotusCountCurrent");
                if (__instance._slots[slot].KeyExists("hasFaceThing"))
                    PluginMain.SaveDataHandler.CustomPlayerData.HasFaceThing
                        = __instance._slots[slot].Load<bool>("hasFaceThing");
                manager.tpc.gliderEnabled = PluginMain.SaveDataHandler.CustomPlayerData.HasLeaf;
                manager.tpc.hookClimber.enabled = PluginMain.SaveDataHandler.CustomPlayerData.HasHooks;
                manager.pd.hasHexkey = PluginMain.SaveDataHandler.CustomPlayerData.HasHexKey;
            }
        }
        
        [HarmonyPatch(typeof(Application), "persistentDataPath", MethodType.Getter)]
        public class PersistentDataPatch
        {
            static bool Prefix(ref string __result)
            {
                __result = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/ArchipelagoSaves/";
                if (!Directory.Exists(__result))
                    Directory.CreateDirectory(__result);
                return false;
            }
        }
    }
}