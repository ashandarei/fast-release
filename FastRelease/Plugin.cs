using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FastRelease;

public sealed unsafe class Plugin : IDalamudPlugin
{
    private static readonly string[] TargetAddonNames = ["SelectYesno", "SelectYesNo"];
    private static readonly AddonEvent[] TargetEvents =
    [
        AddonEvent.PostSetup,
        AddonEvent.PostRefresh,
        AddonEvent.PostRequestedUpdate,
        AddonEvent.PostOpen,
        AddonEvent.PostShow,
    ];

    private const float InstantHoldDurationSeconds = 0.01f;

    [PluginService]
    internal static IAddonLifecycle AddonLifecycle { get; private set; } = null!;

    [PluginService]
    internal static IPluginLog Log { get; private set; } = null!;

    public string Name => "Fast Release";

    public Plugin()
    {
        foreach (var eventType in TargetEvents)
        {
            AddonLifecycle.RegisterListener(eventType, TargetAddonNames, OnSelectYesNoLifecycleEvent);
        }

        Log.Information("Fast Release loaded.");
    }

    public void Dispose()
    {
        foreach (var eventType in TargetEvents)
        {
            AddonLifecycle.UnregisterListener(eventType, TargetAddonNames, OnSelectYesNoLifecycleEvent);
        }
    }

    private static void OnSelectYesNoLifecycleEvent(AddonEvent eventType, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon.Address;
        if (addon == null || !IsRevivePromptActive())
        {
            return;
        }

        var selectYesNo = (AddonSelectYesno*)addon;
        var applied = ApplyInstantHold(selectYesNo->AtkComponentHoldButton278);
        applied |= ApplyInstantHold(selectYesNo->AtkComponentHoldButton280);
        applied |= ApplyInstantHold(selectYesNo->AtkComponentHoldButton288);

        if (applied)
        {
            Log.Verbose("Applied instant hold duration to revive prompt on {EventType}.", eventType);
        }
    }

    private static bool IsRevivePromptActive()
    {
        var agentModule = AgentModule.Instance();
        if (agentModule == null)
        {
            return false;
        }

        var reviveAgentInterface = agentModule->GetAgentByInternalId(AgentId.Revive);
        if (reviveAgentInterface == null || !reviveAgentInterface->IsAgentActive())
        {
            return false;
        }

        var reviveAgent = (AgentRevive*)reviveAgentInterface;
        return reviveAgent->Revive != null;
    }

    private static bool ApplyInstantHold(AtkComponentHoldButton* holdButton)
    {
        if (holdButton == null || holdButton->OwnerNode == null)
        {
            return false;
        }

        if (holdButton->Duration <= InstantHoldDurationSeconds)
        {
            return false;
        }

        holdButton->Duration = InstantHoldDurationSeconds;
        return true;
    }
}
