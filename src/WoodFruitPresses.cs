using WoodFruitPresses.Content;
using Vintagestory.API.Common;

[assembly: ModInfo("Wood Fruit Presses",
  Authors = new[] { "Craluminum2413" })]

namespace WoodFruitPresses.Load
{
  class WoodFruitPresses : ModSystem
  {
    public override void Start(ICoreAPI api)
    {
      base.Start(api);
      api.RegisterBlockClass("WFPBlockFruitPress", typeof(WFPBlockFruitPress));
      api.RegisterBlockEntityClass("WFPFruitPress", typeof(WFPBlockEntityFruitPress));
      api.World.Logger.Event("started 'Wood Fruit Presses' mod");
    }
  }
}