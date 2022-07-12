using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace WoodFruitPresses.Content
{
  public class WFPBlockFruitPress : BlockFruitPress, ITexPositionSource, IContainedMeshSource
  {
    private ICoreClientAPI capi;
    private ITextureAtlasAPI targetAtlas;
    private readonly Dictionary<string, AssetLocation> tmpTextures = new();
    public Size2i AtlasSize => targetAtlas.Size;
    private Dictionary<int, MeshRef> Meshrefs => ObjectCacheUtil.GetOrCreate(api, "wfpfruitpressmeshrefs", () => new Dictionary<int, MeshRef>());
    public TextureAtlasPosition this[string textureCode] => GetOrCreateTexPos(tmpTextures[textureCode]);

    protected TextureAtlasPosition GetOrCreateTexPos(AssetLocation texturePath)
    {
      var texAsset = capi.Assets.TryGet(texturePath.Clone().WithPathPrefixOnce("textures/").WithPathAppendixOnce(".png"));
      var texPos = targetAtlas[texturePath];

      if (texPos != null) return texPos;

      if (texAsset != null)
      {
        targetAtlas.GetOrInsertTexture(texturePath, out var _, out texPos, () => texAsset.ToBitmap(capi));
      }
      else
      {
        capi.World.Logger.Warning("For render in fruitpress {0}, require texture {1}, but no such texture found.", Code, texturePath);
      }

      return texPos;
    }

    public override void OnLoaded(ICoreAPI api)
    {
      base.OnLoaded(api);
      capi = api as ICoreClientAPI;

      AddAllTypesToCreativeInventory();
    }

    public void AddAllTypesToCreativeInventory()
    {
      var stacks = new List<JsonItemStack>();
      var variantGroups = Attributes["variantGroups"].AsObject<Dictionary<string, string[]>>();

      if (LastCodePart() != "ns") return;

      foreach (var wood in variantGroups["wood"])
      {
        foreach (var strainer in variantGroups["strainer"])
        {
          foreach (var metal in variantGroups["metal"])
          {
            stacks.Add(GenJstack(string.Format("{{ wood: \"{0}\", strainer: \"{1}\", metal: \"{2}\" }}", wood, strainer, metal)));
          }

          CreativeInventoryStacks = new CreativeTabAndStackList[]
          {
          new CreativeTabAndStackList() { Stacks = stacks.ToArray(), Tabs = new string[]{ "general", "cralwv" } }
          };
        }
      }
    }

    private JsonItemStack GenJstack(string json)
    {
      var jstack = new JsonItemStack()
      {
        Code = Code,
        Type = EnumItemClass.Block,
        Attributes = new JsonObject(JToken.Parse(json))
      };

      jstack.Resolve(api.World, "fruitpress type");

      return jstack;
    }

    public override void OnBeforeRender(ICoreClientAPI capi, ItemStack itemstack, EnumItemRenderTarget target, ref ItemRenderInfo renderinfo)
    {
      var meshrefid = itemstack.TempAttributes.GetInt("meshRefId", 0);
      if (meshrefid == 0 || !Meshrefs.TryGetValue(meshrefid, out renderinfo.ModelRef))
      {
        var num = Meshrefs.Count + 1;
        var value = capi.Render.UploadMesh(GenMesh(itemstack, capi.BlockTextureAtlas));
        renderinfo.ModelRef = Meshrefs[num] = value;
        itemstack.TempAttributes.SetInt("meshRefId", num);
      }
      base.OnBeforeRender(capi, itemstack, target, ref renderinfo);
    }

    public MeshData GenMesh(ItemStack itemstack, ITextureAtlasAPI targetAtlas)
    {
      this.targetAtlas = targetAtlas;
      tmpTextures.Clear();

      var wood = itemstack.Attributes.GetString("wood");
      var strainer = itemstack.Attributes.GetString("strainer");
      var metal = itemstack.Attributes.GetString("metal");

      if (wood == null && strainer == null && metal == null) return new MeshData();

      tmpTextures["wood"] = new AssetLocation("block/wood/debarked/" + wood + ".png");
      tmpTextures["strainer"] = new AssetLocation("block/wood/debarked/" + strainer + ".png");
      tmpTextures["metal"] = new AssetLocation("block/metal/ingot/" + metal + ".png");

      var shape = capi.Assets.Get(new AssetLocation("woodfp:shapes/block/inventory.json")).ToObject<Shape>();

      capi.Tesselator.TesselateShape(typeForLogging: "fruitpress", shape, out var modeldata, this);
      return modeldata;
    }

    public override string GetHeldItemName(ItemStack itemStack) => Lang.GetMatching("game:block-fruitpress-*");

    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
      base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

      var woodDsc = Lang.Get("material-" + inSlot.Itemstack.Attributes.GetString("wood"));
      var strainerDsc = Lang.Get("material-" + inSlot.Itemstack.Attributes.GetString("strainer"));
      var metalDsc = Lang.Get("material-" + inSlot.Itemstack.Attributes.GetString("metal"));

      dsc.Append('\n').Append(Lang.Get("Wood")).Append(": ").AppendLine(woodDsc)
        .Append("Strainer: ").AppendLine(strainerDsc)
        .Append(Lang.Get("Metal")).Append(": ").AppendLine(metalDsc);
    }

    public MeshData GenMesh(ItemStack itemstack, ITextureAtlasAPI targetAtlas, BlockPos atBlockPos) => GenMesh(itemstack, targetAtlas);

    public string GetMeshCacheKey(ItemStack itemstack)
    {
      var wood = itemstack.Attributes.GetString("wood");
      var strainer = itemstack.Attributes.GetString("strainer");
      var metal = itemstack.Attributes.GetString("metal");
      return $"{Code.ToShortString()}-{wood}-{strainer}-{metal}";
    }

    public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
    {
      var stack = new ItemStack(world.GetBlock(CodeWithVariant("orientation", "ns")));

      if (api.World.BlockAccessor.GetBlockEntity(pos) is WFPBlockEntityFruitPress be)
      {
        stack.Attributes.SetString("wood", be.woodType);
        stack.Attributes.SetString("strainer", be.strainerType);
        stack.Attributes.SetString("metal", be.metalType);
      }
      return stack;
    }

    public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
    {
      return new ItemStack[] { OnPickBlock(world, pos) };
    }
  }
}