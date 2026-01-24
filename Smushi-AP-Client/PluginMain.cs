using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using CMF;
using HarmonyLib;
using Rewired.Integration.UnityUI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;


namespace Smushi_AP_Client 
{
    [BepInPlugin("SmushiAPClient", "Smushi AP Client", "1.0.8")]
    public class PluginMain : BaseUnityPlugin
    {
        public static ConfigEntry<bool> FilterLog;
        public static LoginHandler LoginHandler;
        public static SaveDataHandler SaveDataHandler;
        public static ArchipelagoHandler ArchipelagoHandler;
        public static ItemHandler ItemHandler;
        public static LocationHandler LocationHandler;
        public static GameHandler GameHandler;
        
        private readonly Harmony _harmony = new Harmony("SmushiAPClient");
       
        private void Awake()
        {
            _harmony.PatchAll();
            var handlerObj = new GameObject("ArchipelagoLoginHandler");
            LoginHandler = handlerObj.AddComponent<LoginHandler>();
            DontDestroyOnLoad(handlerObj);
            handlerObj = new GameObject("ArchipelagoSaveDataHandler");
            SaveDataHandler = handlerObj.AddComponent<SaveDataHandler>();
            DontDestroyOnLoad(handlerObj);
            handlerObj = new GameObject("ArchipelagoGameHandler");
            GameHandler = handlerObj.AddComponent<GameHandler>();
            DontDestroyOnLoad(handlerObj);
            handlerObj = new GameObject("ArchipelagoItemHandler");
            ItemHandler = handlerObj.AddComponent<ItemHandler>();
            DontDestroyOnLoad(handlerObj);
            LocationHandler = new LocationHandler();
            APConsole.Create();
            StartCoroutine(EnableMouse());
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                var rewiredInputModule = FindObjectOfType<RewiredStandaloneInputModule>();
                if (rewiredInputModule != null)
                {
                    rewiredInputModule.allowMouseInput = true;
                    rewiredInputModule.allowMouseInputIfTouchSupported = true;
                }
            };
            
            FilterLog = Config.Bind(
                "Logging",
                "FilterLog",
                false,
                "Filter the archipelago log to only show messages relevant to you."
            );
        }

        private IEnumerator EnableMouse()
        {
            RewiredStandaloneInputModule rewiredInputModule = null;
            do
            {
                rewiredInputModule = FindObjectOfType<RewiredStandaloneInputModule>();
                yield return new WaitForSeconds(2);
            }
            while (rewiredInputModule == null);
            rewiredInputModule.allowMouseInput = true;
            rewiredInputModule.allowMouseInputIfTouchSupported = true;
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.F7))
            // {
            //     APConsole.Instance.Log("PRESSED F7");
            //     
            //     var conchUI = FindObjectOfType<ConchUI>();
            //     if (Input.GetKeyDown(KeyCode.Alpha0))
            //         conchUI.StartLevelTeleport(0);
            //     if (Input.GetKeyDown(KeyCode.Alpha1))
            //         conchUI.StartLevelTeleport(1);
            //     if (Input.GetKeyDown(KeyCode.Alpha2))
            //         conchUI.StartLevelTeleport(2);
            //     if (Input.GetKeyDown(KeyCode.Alpha3))
            //         conchUI.StartLevelTeleport(3);
            //     if (Input.GetKeyDown(KeyCode.Alpha4))
            //         conchUI.StartLevelTeleport(4);
            //     if (Input.GetKeyDown(KeyCode.Alpha5))
            //         conchUI.StartLevelTeleport(5);
            //     if (Input.GetKeyDown(KeyCode.Alpha6))
            //         conchUI.StartLevelTeleport(6);
            //     if (Input.GetKeyDown(KeyCode.Alpha7))
            //         conchUI.StartLevelTeleport(7);
            //     Debug.Log("F7 Pressed");
            // }
            //
            // if (Input.GetKeyDown(KeyCode.F8))
            // {
            //     var controller = FindObjectOfType<AdvancedWalkerController>();
            //     controller.gliderEnabled = !controller.gliderEnabled;
            // }
            //
            // if (Input.GetKeyDown(KeyCode.F9))
            // {
            //     var controller = FindObjectOfType<AdvancedWalkerController>();
            //     controller.hookClimber.enabled = !controller.hookClimber.enabled;
            // }
            //
            // if (Input.GetKeyDown(KeyCode.F10))
            // {
            //     var controller = FindObjectOfType<AdvancedWalkerController>();
            //     var playerData = FindObjectOfType<PlayerData>();
            //     controller.hookClimber.hasHookUpgrade = !controller.hookClimber.hasHookUpgrade;
            //     playerData.climbStamina =  controller.hookClimber.hasHookUpgrade ? 1f : 0f;
            //     controller.hookClimber.savedStamina = controller.hookClimber.hasHookUpgrade ? 1f : 0f;
            //     controller.hookClimber.stamina =  controller.hookClimber.hasHookUpgrade ? 1f : 0f;
            // }
            //
            // if (Input.GetKeyDown(KeyCode.F11))
            // {
            //     var controller = FindObjectOfType<AdvancedWalkerController>();
            //     var playerData = FindObjectOfType<PlayerData>();
            //     if (playerData.hasSprint)
            //     {
            //         playerData.hasSprint = false;
            //         playerData.sporeCount = 0;
            //         controller.sprinterController.CheckSporeCount();
            //     }
            //     else
            //     {
            //         playerData.hasSprint = true;
            //         playerData.sporeCount = 2;
            //         controller.sprinterController.CheckSporeCount();
            //     }
            // }
        }
    }
}