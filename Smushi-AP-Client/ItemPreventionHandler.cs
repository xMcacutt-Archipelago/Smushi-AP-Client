using HarmonyLib;

namespace Smushi_AP_Client;

public class ItemPreventionHandler
{
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
    
    
}