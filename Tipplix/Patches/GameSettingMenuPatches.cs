using System.Linq;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Reactor;
using Reactor.Extensions;
using Tipplix.Enums;
using Tipplix.Extensions;
using Tipplix.Options;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;
using Object = UnityEngine.Object;

namespace Tipplix.Patches;

[HarmonyPatch]
public static class GameSettingMenuPatches
{
    private static StringNames RevealOnExile = CustomStringName.Register("Reveal On Exile");
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.ValueChanged))]
    public static bool ValueChangedPrefix(OptionBehaviour option)
    {
        if (option.Title != RevealOnExile) return true;
        var type = (RevealTypes) option.GetInt();
        
        PlixOptions.ExileRevealType = type;
        PlayerControl.GameOptions.ConfirmImpostor = type switch 
        {
            RevealTypes.None => false,
            RevealTypes.Team => true,
            RevealTypes.Role => true,
            _ => false
        };
        
        OptionsManager.SyncSettings();
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.InitializeOptions))]
    public static void InitializeOptionsPostfix(GameSettingMenu __instance)
    {
        var mapName = FindObject<KeyValueOption>("MapName");
        var confirmEjects = FindObject<ToggleOption>("ConfirmEjects");

        var newConfirmEject = Object.Instantiate(mapName, confirmEjects!.transform.parent);
        newConfirmEject!.transform.localPosition = confirmEjects.transform.localPosition;
        confirmEjects.gameObject.Destroy();

        var keyValuePair = new List<KeyValuePair<string, int>>();
        keyValuePair.Add(new KeyValuePair<string, int> { key = "None", value = 0 });
        keyValuePair.Add(new KeyValuePair<string, int> { key = "Team", value = 1 });
        keyValuePair.Add(new KeyValuePair<string, int> { key = "Role", value = 2 });

        newConfirmEject.name = "RevealExile";

        newConfirmEject.Title = RevealOnExile;
        newConfirmEject.TitleText.SetText("Reveal On Exile");
        newConfirmEject.Values = keyValuePair;
        newConfirmEject.Selected = (int) PlixOptions.ExileRevealType;
    }

    private static T? FindObject<T>(string name)
    {
        return GameSettingMenu.Instance.AllItems
            .FirstOrDefault(x => x.gameObject.activeSelf && x.name.Equals(name))!
            .GetComponent<T>();
    }
}