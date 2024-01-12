using Vintagestory.API.Common;

[assembly: ModInfo(name: "Wood Fruit Presses", modID: "woodfp", Side = "Universal")]

namespace WoodFruitPresses;

public class Core : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        base.Start(api);

        api.RegisterBlockClass("WoodFruitPresses.BlockFruitPress", typeof(WFPBlockFruitPress));
        api.RegisterBlockEntityClass("WoodFruitPresses.FruitPress", typeof(WFPBlockEntityFruitPress));
        // api.RegisterBlockBehaviorClass("DanaTweaks:BranchCutter", typeof(BlockBehaviorBranchCutter));
        // api.RegisterBlockBehaviorClass("DanaTweaks:CrateInteractionHelp", typeof(BlockBehaviorCrateInteractionHelp));
        // api.RegisterBlockBehaviorClass("DanaTweaks:DropResinAnyway", typeof(BlockBehaviorDropResinAnyway));
        // api.RegisterBlockBehaviorClass("DanaTweaks:DropVinesAnyway", typeof(BlockBehaviorDropVinesAnyway));
        // api.RegisterBlockBehaviorClass("DanaTweaks:GuaranteedDecorDrop", typeof(BlockBehaviorGuaranteedDecorDrop));
        // api.RegisterBlockBehaviorClass("DanaTweaks:GuaranteedDrop", typeof(BlockBehaviorGuaranteedDrop));
        // api.RegisterBlockEntityBehaviorClass("DanaTweaks:RainCollector", typeof(BEBehaviorRainCollector));
        // api.RegisterCollectibleBehaviorClass("DanaTweaks:BranchCutter", typeof(CollectibleBehaviorBranchCutter));
        // api.RegisterCollectibleBehaviorClass("DanaTweaks:RemoveBookSignature", typeof(CollectibleBehaviorRemoveBookSignature));

        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }
}