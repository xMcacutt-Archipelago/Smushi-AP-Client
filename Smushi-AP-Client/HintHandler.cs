using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine.EventSystems;

namespace Smushi_AP_Client;

public class HintHandler
{
    public static Dictionary<string, long> _validHints = new()
    {
        { "purple augmenter", 0x0},
        { "strawberry augmenter", 0x1 },
        { "flower augmenter", 0x2 },
        { "secret augmenter", 0x3 },
        { "verdant augmenter", 0x4 },
        { "pelagic augmenter", 0x5 },
        { "honey augmenter", 0x6 },
        { "sparkle augmenter", 0x7 },
        { "clavaria augmenter", 0x8 },
        { "ink augmenter", 0x9 },
        { "sharp augmenter", 0xA },
        { "precious augmenter", 0xB },
        { "rainbow augmenter", 0xC },
        { "veiled augmenter", 0xD },
        { "sacred augmenter", 0xE },
    };
    
    [HarmonyPatch(typeof(DialogueControls))]
    public class DialogueControls_Patch
    {
        [HarmonyPatch("SelectOption")]
        [HarmonyPrefix]
        public static void SelectOption(DialogueControls __instance)
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected == null)
                return;

            var text = selected.GetComponentInChildren<TMP_Text>()?.text;
            if (text == null)
                return;

            if (!HintHandler._validHints.TryGetValue(text, out var hint))
                return;

            PluginMain.ArchipelagoHandler.Hint(0x700 + hint);
        }
    }
}