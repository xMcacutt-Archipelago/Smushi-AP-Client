using HarmonyLib;

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
}