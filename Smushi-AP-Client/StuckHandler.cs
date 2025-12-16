using System;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Smushi_AP_Client;

public class StuckHandler : MonoBehaviour
{
    [HarmonyPatch(typeof(PauseMenuController))]
    public class PauseMenuController_Patch
    {
        private static PauseMenuController instance;
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void OnStart(PauseMenuController __instance)
        {
            instance = __instance;
            
            var referenceButton = __instance.QAButton;
            var buttonsParent = referenceButton.transform.parent;
            var stuckButton = Instantiate(referenceButton, buttonsParent);
            stuckButton.name = "StuckButton";
            
            var button = stuckButton.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(OnStuckClicked);
            
            var text = stuckButton.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "I'M STUCK";
            text.enabled = false;
            text.enabled = true;
            
            stuckButton.gameObject.SetActive(true);
        }

        private static void OnStuckClicked()
        {
            var outOfBoundsTeleport = FindObjectOfType<OutOfBoundsTeleport>();
            if (outOfBoundsTeleport != null)
                outOfBoundsTeleport.player.position = outOfBoundsTeleport.teleportPlayerHere.position;
            instance.ExitPauseMenu();
            instance.isOpen = false;
            instance.blackFade.FadeOut();
        }
    }
}
