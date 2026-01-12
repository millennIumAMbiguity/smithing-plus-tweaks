namespace SmithingPlusTweaks.Config;

/// <summary>
/// should look like SmithingPlus server config.
/// Used to map the SyncConfig attributes to the real config.
/// </summary>
public class SmithingPlusConfig
{
    [SyncConfig]
    public bool RecoverBitsOnSplit { get; set; } = true;
    public bool HelveHammerBitsRecovery { get; set; } = true;
    [SyncConfig]
    public float VoxelsPerBit { get; set; } = 2.1f;
    [SyncConfig]
    public bool SmithWithBits { get; set; } = true;
    [SyncConfig]
    public bool BitsTopUp { get; set; } = true;
    public bool EnableToolRecovery { get; set; } = true;
    [SyncConfig]
    public float DurabilityPenaltyPerRepair { get; set; } = 0.05f;
    public string ToolRepairForgettableAttributes { get; set; } = "quality,maxRepair";
    [SyncConfig]
    public float RepairableToolDurabilityMultiplier { get; set; } = 1.0f;
    [SyncConfig]
    public float BrokenToolVoxelPercent { get; set; } = 0.8f;

    public string RepairableToolSelector { get; set; } =
        "@.*(pickaxe|shovel|saw|axe|hoe|knife|hammer|chisel|shears|sword|spear|bow|shield|sickle|scythe|tongs|wrench|solderingiron|cleaver|prospectingpick|crossbow|pistol|rifle|shotgun|blade|halberd|poleaxe|quarterstaff|pike).*";

    public string ToolHeadSelector { get; set; } = "@(.*)(head|blade|boss|barrel|stirrup|part)(.*)";
    public string IngotSelector { get; set; } = "@(.*):ingot-(.*)";
    public string WorkItemSelector { get; set; } = "@(.*):workitem-(.*)";
    public bool DontRepairBrokenToolHeads { get; set; } = false;
    [SyncConfig]
    public bool CanRepairForlornHopeEstoc { get; set; } = true;
    public bool ShowRepairedCount { get; set; } = true;
    public bool ShowBrokenCount { get; set; } = true;
    public bool ShowRepairSmithName { get; set; } = false;
    [SyncConfig]
    public float HelveHammerSmithingQualityModifier { get; set; } = 1;
    public bool ArrowsDropBits { get; set; } = true;
    public string ArrowSelector { get; set; } = "@(.*):arrow-(.*)";
    [SyncConfig]
    public bool MetalCastingTweaks { get; set; } = true;
    [SyncConfig]
    public float CastToolDurabilityPenalty { get; set; } = 0.1f;
    [SyncConfig]
    public bool DynamicMoldUnits { get; set; } = false;
    [SyncConfig]
    public bool HammerTweaks { get; set; } = true;
    [SyncConfig]
    public bool RotationRequiresTongs { get; set; } = false;
    public bool AnvilShowRecipeVoxels { get; set; } = true;
    public bool RememberHammerToolMode { get; set; } = true;
    public bool ShowWorkableTemperature { get; set; } = true;
    public bool HandbookExtraInfo { get; set; } = true;
    public int AnvilRecipeSelectionColumns { get; set; } = 8;
}