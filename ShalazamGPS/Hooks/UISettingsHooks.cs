using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ShalazamGPS.Hooks;

[HarmonyPatch(typeof(UISettings))]
[HarmonyPatch(nameof(UISettings.Awake))]
public class UISettingsHooks
{
    private static void Postfix(UISettings __instance)
    {
        // Fired in char select
        if (__instance.transform.childCount < 11)
        {
            Globals.HasSetUpUI = false;
            return;
        }
        
        if (Globals.HasSetUpUI)
        {
            return;
        }
        
        Globals.HasSetUpUI = true;
        
        var tabOther = __instance.transform.GetChild(9);
        var otherLayoutGroup = tabOther.GetChild(0);
        var spacer = otherLayoutGroup.GetChild(1);
        var resetUiButton = otherLayoutGroup.GetChild(2);

        Object.Instantiate(spacer, spacer.position, spacer.rotation, otherLayoutGroup);
        
        var copy = Object.Instantiate(resetUiButton, resetUiButton.position, resetUiButton.rotation, otherLayoutGroup);
        copy.name = "Button_OpenGPS";
        copy.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Open GPS";

        var buttonComp = copy.GetComponent<Button>();
        buttonComp.onClick = new Button.ButtonClickedEvent();
        buttonComp.onClick.AddCall(new InvokableCall(new Action(() =>
        {
            Application.OpenURL($"https://shalazam.info/maps/1?gps_websocket_url={ModMain.WebsocketAddress}:{ModMain.WebsocketPort}&gps_websocket_interval={ModMain.RefreshInterval}&zoom=8&x={Globals.LocalPlayer!.transform.position.x}&y={Globals.LocalPlayer.transform.position.z}");
        })));
    }
}