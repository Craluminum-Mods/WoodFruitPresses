using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace WoodFruitPresses.Content
{
  public class WFPBlockEntityFruitPress : BlockEntityFruitPress
  {
    public string woodType;
    public string strainerType;
    public string metalType;

    public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
    {
      woodType = tree.GetString("wood");
      strainerType = tree.GetString("strainer");
      metalType = tree.GetString("metal");

      if (Api?.Side == EnumAppSide.Client)
      {
        MarkDirty(true);
      }

      base.FromTreeAttributes(tree, worldAccessForResolve);
    }

    public override void ToTreeAttributes(ITreeAttribute tree)
    {
      base.ToTreeAttributes(tree);

      tree.SetString("wood", woodType);
      tree.SetString("strainer", strainerType);
      tree.SetString("metal", metalType);
    }

    public override void OnBlockPlaced(ItemStack byItemStack = null)
    {
      if (byItemStack?.Attributes != null)
      {
        var nowWoodType = byItemStack.Attributes.GetString("wood");
        var nowStrainerType = byItemStack.Attributes.GetString("strainer");
        var nowMetalType = byItemStack.Attributes.GetString("metal");

        if (nowWoodType != woodType || nowStrainerType != strainerType || nowMetalType != metalType)
        {
          woodType = nowWoodType;
          strainerType = nowStrainerType;
          metalType = nowMetalType;
          MarkDirty();
        }
      }

      base.OnBlockPlaced(byItemStack);
    }

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
    {
      base.GetBlockInfo(forPlayer, dsc);

      var woodDsc = Lang.Get("material-" + woodType);
      var strainerDsc = Lang.Get("material-" + strainerType);
      var metalDsc = Lang.Get("material-" + metalType);

      dsc.Append('\n').Append(Lang.Get("Wood")).Append(": ").AppendLine(woodDsc)
        .Append("Strainer: ").AppendLine(strainerDsc)
        .Append(Lang.Get("Metal")).Append(": ").AppendLine(metalDsc);
    }
  }
}