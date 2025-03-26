using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Zorro.Settings;

namespace NodeToggler;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private static readonly Harmony Patcher = new(MyPluginInfo.PLUGIN_GUID);
    
    private void Awake()
    {
        Logger = base.Logger;
        Patcher.PatchAll();
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}

public class EncounterNodesSetting : OffOnSetting, IExposedSetting
{
    public override void ApplyValue() { }
    
    public string GetDisplayName() => "[NodeToggler] Enable Encounter nodes";
    
    protected override OffOnMode GetDefaultValue() => OffOnMode.OFF;
    public SettingCategory GetCategory() => SettingCategory.Controls;

    public static bool GetValue()
    {
        return GameHandler.Instance.SettingsHandler.GetSetting<EncounterNodesSetting>().Value == OffOnMode.ON;
    }
}

public class DefaultNodesSetting : OffOnSetting, IExposedSetting
{
    public override void ApplyValue() { }
    
    public string GetDisplayName() => "[NodeToggler] Enable Default nodes";
    
    protected override OffOnMode GetDefaultValue() => OffOnMode.OFF;
    public SettingCategory GetCategory() => SettingCategory.Controls;

    public static bool GetValue()
    {
        return GameHandler.Instance.SettingsHandler.GetSetting<DefaultNodesSetting>().Value == OffOnMode.ON;
    }
}

public class RestStopNodesSetting : OffOnSetting, IExposedSetting
{
    public override void ApplyValue() { }
    
    public string GetDisplayName() => "[NodeToggler] Enable Rest Stop nodes";
    
    protected override OffOnMode GetDefaultValue() => OffOnMode.OFF;
    public SettingCategory GetCategory() => SettingCategory.Controls;

    public static bool GetValue()
    {
        return GameHandler.Instance.SettingsHandler.GetSetting<RestStopNodesSetting>().Value == OffOnMode.ON;
    }
}
public class ShopNodesSetting : OffOnSetting, IExposedSetting
{
    public override void ApplyValue() { }
    
    public string GetDisplayName() => "[NodeToggler] Enable Shop nodes";
    
    protected override OffOnMode GetDefaultValue() => OffOnMode.OFF;
    public SettingCategory GetCategory() => SettingCategory.Controls;

    public static bool GetValue()
    {
        return GameHandler.Instance.SettingsHandler.GetSetting<ShopNodesSetting>().Value == OffOnMode.ON;
    }
}
public class ChallengeNodesSetting : OffOnSetting, IExposedSetting
{
    public override void ApplyValue() { }
    
    public string GetDisplayName() => "[NodeToggler] Enable Challenge nodes";
    
    protected override OffOnMode GetDefaultValue() => OffOnMode.ON;
    public SettingCategory GetCategory() => SettingCategory.Controls;

    public static bool GetValue()
    {
        return GameHandler.Instance.SettingsHandler.GetSetting<ChallengeNodesSetting>().Value == OffOnMode.ON;
    }
}

public class ReplacementNodeSetting : EnumSetting<LevelSelectionNode.NodeType>, IExposedSetting
{
    public override void ApplyValue() { }
    
    public string GetDisplayName() => "[NodeToggler] What to replace disabled nodes with";
    
    protected override LevelSelectionNode.NodeType GetDefaultValue() => LevelSelectionNode.NodeType.Default;
    public SettingCategory GetCategory() => SettingCategory.Controls;

    public static LevelSelectionNode.NodeType GetValue()
    {
        return GameHandler.Instance.SettingsHandler.GetSetting<ReplacementNodeSetting>().Value;
    }
}

[HarmonyPatch(typeof(LevelSelectionMapGenerator))]
internal static class LevelSelectionMapGeneratorPatch
{
    [HarmonyPatch(nameof(LevelSelectionMapGenerator.Generate))]
    [HarmonyPostfix]
    internal static void Generate()
    {
        foreach (LevelSelectionNode node in GameObject.FindObjectsOfType<LevelSelectionNode>())
        {
            if (node.ID == 0) continue;
            if (
                (node.Type == LevelSelectionNode.NodeType.Encounter && !EncounterNodesSetting.GetValue()) ||
                (node.Type == LevelSelectionNode.NodeType.RestStop && !RestStopNodesSetting.GetValue()) ||
                (node.Type == LevelSelectionNode.NodeType.Shop && !ShopNodesSetting.GetValue()) ||
                (node.Type == LevelSelectionNode.NodeType.Challenge && !ChallengeNodesSetting.GetValue()) ||
                (node.Type == LevelSelectionNode.NodeType.Default && !DefaultNodesSetting.GetValue()))
            {
                node.SetType(ReplacementNodeSetting.GetValue());
            }
        }
    }
}

[HarmonyPatch(typeof(GameHandler))]
internal static class GameHandlerPatch
{
    [HarmonyPatch(nameof(GameHandler.Awake))]
    [HarmonyPostfix]
    internal static void Awake(GameHandler __instance)
    {
        __instance.SettingsHandler.AddSetting(new DefaultNodesSetting());
        __instance.SettingsHandler.AddSetting(new EncounterNodesSetting());
        __instance.SettingsHandler.AddSetting(new RestStopNodesSetting());
        __instance.SettingsHandler.AddSetting(new ShopNodesSetting());
        __instance.SettingsHandler.AddSetting(new ChallengeNodesSetting());
        __instance.SettingsHandler.AddSetting(new ReplacementNodeSetting());
    }
}