using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Smushi_AP_Client
{
    public class LoginHandler : MonoBehaviour
    {
        public string Server = "";
        public string Port = "";
        public string Slot = "";
        public string Password = ""; 
        public List<TMP_InputField> InputFieldList = new List<TMP_InputField>();
        public int CurrentIndex = 0;
        public bool IsTyping = false;
        public Color NormalColor = Color.white;
        public Color SelectedColor = new Color(1, 0.85f, 0.4f);
        public static bool AllowFileSelect;
        
        private static GameObject CreateLabeledInput(GameObject referenceButton, Transform parent, string labelText, out TextMeshProUGUI text)
        {
            var rowObject = Instantiate(referenceButton);
            rowObject.name = labelText + "Row";
            rowObject.transform.SetParent(parent, false);

            var saveStatsTransform = rowObject.transform.Find("Save Stats");
            if (saveStatsTransform != null)
                Object.Destroy(saveStatsTransform.gameObject);

            text = rowObject.transform.Find("Slot Image/Slot Text").GetComponent<TextMeshProUGUI>();
            text.text = labelText;
            text.enabled = false;
            text.enabled = true;

            var inputFieldObject = new GameObject("InputField");
            inputFieldObject.transform.SetParent(rowObject.transform, false);

            var inputFieldRectTransform = inputFieldObject.AddComponent<RectTransform>();
            inputFieldRectTransform.anchorMin = new Vector2(0, 0);
            inputFieldRectTransform.anchorMax = new Vector2(1, 1);
            inputFieldRectTransform.offsetMin = new Vector2(10, 10);
            inputFieldRectTransform.offsetMax = new Vector2(-10, -10);

            var inputFieldComponent = inputFieldObject.AddComponent<TMP_InputField>();
            inputFieldComponent.navigation = new Navigation { mode = Navigation.Mode.None };

            var textObject = new GameObject("Text");
            textObject.transform.SetParent(inputFieldObject.transform, false);

            var textComponent = textObject.AddComponent<TextMeshProUGUI>();
            textComponent.fontSize = 24;
            textComponent.enableWordWrapping = false;
            textComponent.text = "";
            textComponent.color = new Color(0, 0, 0, 1.0f);

            var textRectTransform = textObject.GetComponent<RectTransform>();
            textRectTransform.anchorMin = new Vector2(0, 0);
            textRectTransform.anchorMax = new Vector2(1, 1);
            textRectTransform.offsetMin = new Vector2(10, 6);
            textRectTransform.offsetMax = new Vector2(-10, -6);

            inputFieldComponent.textComponent = textComponent;
            inputFieldComponent.pointSize = 24;

            var targetGraphicImage = inputFieldObject.AddComponent<Image>();
            targetGraphicImage.color = new Color(0, 0, 0, 0);

            inputFieldComponent.targetGraphic = targetGraphicImage;

            badTexts.Add(rowObject.transform.Find("New Save").gameObject);
            
            return rowObject;
        }
        
        void BeginTyping(TMP_InputField inputField)
        {
            inputField.ActivateInputField();
            IsTyping = true;
            Cursor.visible = true;
        }

        void EndTyping(GameObject row, TMP_InputField inputField)
        {
            inputField.DeactivateInputField();
            IsTyping = false;
            EventSystem.current.SetSelectedGameObject(row);
            Cursor.visible = false;
        }

        private static GameObject loginMenuObject;
        private static GameObject firstButton;
        private static TextMeshProUGUI connectText;
        private static TextMeshProUGUI serverLabel;
        private static TextMeshProUGUI portLabel;
        private static TextMeshProUGUI slotLabel;
        private static TextMeshProUGUI passwordLabel;
        private static List<GameObject> badTexts = new List<GameObject>();
        public void CreateMenu(OptionsMainMenu menuInstance)
        {
            ConnectionInfoHandler.Load(ref Server, ref Port, ref Slot, ref Password);
            if (loginMenuObject != null)
            {
                loginMenuObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstButton);
                connectText.text = "Connect";
                connectText.enabled = false;
                connectText.enabled = true;
                serverLabel.text = "Server";
                serverLabel.enabled = false;
                serverLabel.enabled = true;
                portLabel.text = "Port";
                portLabel.enabled = false;
                portLabel.enabled = true;
                slotLabel.text = "Slot";
                slotLabel.enabled = false;
                slotLabel.enabled = true;
                passwordLabel.text = "Password";
                passwordLabel.enabled = false;
                passwordLabel.enabled = true;
                foreach (var text in badTexts)
                    text.SetActive(false);
                return;
            }
            
            var referenceButton = menuInstance.saveSlot1Button;
            
            loginMenuObject = new GameObject("LoginMenu");
            loginMenuObject.transform.SetParent(menuInstance.MainCanvas.transform);

            var loginMenuRect = loginMenuObject.AddComponent<RectTransform>();
            loginMenuRect.position = new Vector3(260.2f, 294.0f);
            loginMenuRect.anchorMin = new Vector2(0, 0.5f);
            loginMenuRect.anchorMax = new Vector2(0, 0.5f);
            loginMenuRect.offsetMin = new Vector2(-589.85f, -291.950f);
            loginMenuRect.offsetMax = new Vector2(-210.75f, 40.55f);
            loginMenuRect.sizeDelta = new Vector2(379.10f, 400.50f);

            var layoutGroup = loginMenuObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 20f;

            var serverRow = CreateLabeledInput(referenceButton, loginMenuObject.transform, "Server", out serverLabel);
            var portRow = CreateLabeledInput(referenceButton, loginMenuObject.transform, "Port", out portLabel);
            var slotRow = CreateLabeledInput(referenceButton, loginMenuObject.transform, "Slot", out slotLabel);
            var passwordRow = CreateLabeledInput(referenceButton, loginMenuObject.transform, "Password", out passwordLabel);

            foreach (var text in badTexts)
                text.SetActive(false);
            
            var serverField = serverRow.transform.Find("InputField").GetComponent<TMP_InputField>();
            var portField = portRow.transform.Find("InputField").GetComponent<TMP_InputField>();
            var slotField = slotRow.transform.Find("InputField").GetComponent<TMP_InputField>();
            var passwordField = passwordRow.transform.Find("InputField").GetComponent<TMP_InputField>();
            passwordField.contentType = TMP_InputField.ContentType.Password;
            
            var fontAsset = referenceButton.GetComponentInChildren<TextMeshProUGUI>().font;
            serverField.text = Server;
            serverField.fontAsset = fontAsset;
            serverField.transform.localPosition = new Vector3(0, -5, 0);
            portField.text = Port;
            portField.fontAsset = fontAsset;
            portField.transform.localPosition = new Vector3(0, -5, 0);
            slotField.text = Slot;
            slotField.fontAsset = fontAsset;
            slotField.transform.localPosition = new Vector3(0, -5, 0);
            passwordField.text = Password;
            passwordField.fontAsset = fontAsset;
            passwordField.transform.localPosition = new Vector3(0, -5, 0);

            var serverButton = serverRow.GetComponent<Button>();
            serverButton.onClick.AddListener(() => { PluginMain.LoginHandler.BeginTyping(serverField); });
            serverField.onEndEdit.AddListener(text =>
            {
                Server = text;
                PluginMain.LoginHandler.EndTyping(serverRow, serverField);
            });
            
            var portButton = portRow.GetComponent<Button>();
            portButton.onClick.AddListener(() => { PluginMain.LoginHandler.BeginTyping(portField); });
            portField.onEndEdit.AddListener(text =>
            {
                Port = text;
                PluginMain.LoginHandler.EndTyping(portRow, portField);
            });
            
            var slotButton = slotRow.GetComponent<Button>();
            slotButton.onClick.AddListener(() => { PluginMain.LoginHandler.BeginTyping(slotField); });
            slotField.onEndEdit.AddListener(text =>
            {
                Slot = text;
                PluginMain.LoginHandler.EndTyping(slotRow, slotField);
            });
            
            var passwordButton = passwordRow.GetComponent<Button>();
            passwordButton.onClick.AddListener(() => { PluginMain.LoginHandler.BeginTyping(passwordField); });
            passwordField.onEndEdit.AddListener(text =>
            {
                Password = text;
                PluginMain.LoginHandler.EndTyping(passwordRow, passwordField);
            });

            var referenceConnectButton = menuInstance.saveFiles.transform.Find("Delete");
            var connectButtonObject = Instantiate(referenceConnectButton);
            connectButtonObject.name = "Connect Button";
            connectButtonObject.transform.SetParent(loginMenuObject.transform, false);
            var connectTextComponent = connectButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            connectText = connectTextComponent;
            connectTextComponent.text = "Connect";
            connectTextComponent.enabled = false;
            connectTextComponent.enabled = true;
            var connectButton = connectButtonObject.GetComponent<Button>();
            connectButton.onClick.AddListener(Connect);
            firstButton = serverRow;
            EventSystem.current.SetSelectedGameObject(serverRow);
        }

        public void Connect()
        {
            APConsole.Instance.Log("Connecting to Archipelago");
            PluginMain.ArchipelagoHandler = new ArchipelagoHandler(Server, int.Parse(Port), Slot, Password);
            PluginMain.ArchipelagoHandler.OnConnect += (seed, slot) =>
            {
                PluginMain.SaveDataHandler.InitSave(seed, slot);
            };
            PluginMain.ArchipelagoHandler.InitConnect();
        }

        [HarmonyPatch(typeof(DeleteSlots))]
        public class DeleteSlots_Patch
        {
            [HarmonyPatch("EnterDeleteMode")]
            [HarmonyPrefix]
            public static bool EnterDeleteMode()
            {
                return false;
            }
        }
        
        [HarmonyPatch(typeof(OptionsMainMenu))]
        public class OptionsMainMenu_Patch
        {
            [HarmonyPatch("DisplaySaveFiles")]
            [HarmonyPrefix]
            public static bool DisplaySaveFiles(OptionsMainMenu __instance)
            {
                if (ArchipelagoHandler.IsConnected)
                {
                    PluginMain.SaveDataHandler.InitSave(PluginMain.ArchipelagoHandler.Seed, PluginMain.ArchipelagoHandler.Slot);
                    return false;
                }
                __instance.startingMenuButtons.SetActive(false);
                __instance.saveFiles.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
                __instance.uiState = OptionsMainMenu.UI_State.SaveSlots;
                PluginMain.LoginHandler.CreateMenu(__instance);
                return false;
            }
            
            [HarmonyPatch("CurrentPageState")]
            [HarmonyPrefix]
            private static void CurrentPageState(OptionsMainMenu __instance)
            {
                if (__instance.uiState == OptionsMainMenu.UI_State.SaveSlots)
                {
                    loginMenuObject.SetActive(false);
                }
            }
            
            [HarmonyPatch("SelectSaveFile")]
            [HarmonyPrefix]
            public static bool SelectSaveFile(int slotIndex)
            {
                if (!AllowFileSelect) 
                    return false;
                AllowFileSelect = false;
                return true;
            }
        }
    }
}