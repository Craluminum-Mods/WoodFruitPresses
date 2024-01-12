using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace WoodFruitPresses;

public class WFPBlockFruitPress : BlockFruitPress
{
    private string[] wood;
    private string[] strainer;
    private string[] metal;
    private Dictionary<string, CompositeTexture> textures;
    private CompositeShape cshape;
    private CompositeShape invcshape;

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);
        LoadTypes();
    }

    public void LoadTypes()
    {
        cshape = Attributes["shape"].AsObject<CompositeShape>();
        invcshape = Attributes["invshape"].AsObject<CompositeShape>();
        textures = Attributes["textures"].AsObject<Dictionary<string, CompositeTexture>>();

        wood = api.ResolveVariants(this, "wood");
        strainer = api.ResolveVariants(this, "strainer");
        metal = api.ResolveVariants(this, "metal");

        List<JsonItemStack> stacks = new();
        foreach ((string wood, string strainer, string metal) in wood.SelectMany(wood => strainer.SelectMany(strainer => metal.Select(metal => (wood, strainer, metal)))))
        {
            JsonItemStack jstack = api.CreateJStack(this, $"{{ \"materials\": {{ \"wood\": \"{wood}\", \"strainer\": \"{strainer}\", \"metal\": \"{metal}\" }} }}");
            stacks.Add(jstack);
        }

        if (Attributes["addToCreativeInventory"].AsBool())
        {
            CreativeInventoryStacks = new CreativeTabAndStackList[1]
            {
                new CreativeTabAndStackList
                {
                    Stacks = stacks.ToArray(),
                    Tabs = new string[2] { "general", "decorative" }
                }
            };
        }
    }

    public virtual MeshData GetOrCreateMesh(WoodStrainerMetal materials, bool isInventory = false, ITexPositionSource overrideTexturesource = null)
    {
        Dictionary<string, MeshData> cMeshes = ObjectCacheUtil.GetOrCreate(base.api, this + "Meshes", () => new Dictionary<string, MeshData>());
        ICoreClientAPI capi = base.api as ICoreClientAPI;
        string key = Code + "-" + materials;
        if (overrideTexturesource != null || !cMeshes.TryGetValue(key, out MeshData mesh))
        {
            mesh = new MeshData(4, 3);
            // CompositeShape rcshape = cshape.Clone();
            // CompositeShape invrcshape = invcshape.Clone();
            if (isInventory)
            {
                ShapeInventory.Base.Path = ShapeInventory.Base.Path.Replace("{wood}", materials.Wood).Replace("{strainer}", materials.Strainer).Replace("{metal}", materials.Metal);
                ShapeInventory.Base.WithPathAppendixOnce(".json").WithPathPrefixOnce("shapes/");
            }
            else
            {
                Shape.Base.Path = Shape.Base.Path.Replace("{wood}", materials.Wood).Replace("{strainer}", materials.Strainer).Replace("{metal}", materials.Metal);
                Shape.Base.WithPathAppendixOnce(".json").WithPathPrefixOnce("shapes/");
            }

            Shape shape = capi.Assets.TryGet(isInventory ? ShapeInventory.Base : Shape.Base)?.ToObject<Shape>();
            ITexPositionSource texSource = overrideTexturesource;
            if (texSource == null)
            {
                ShapeTextureSource stexSource = new(capi, shape, isInventory ? ShapeInventory.Base.ToString() : Shape.Base.ToString());
                texSource = stexSource;
                foreach (KeyValuePair<string, CompositeTexture> val in textures)
                {
                    CompositeTexture ctex = val.Value.Clone();
                    ctex.Base.Path = ctex.Base.Path.Replace("{wood}", materials.Wood).Replace("{strainer}", materials.Strainer).Replace("{metal}", materials.Metal);
                    ctex.Bake(capi.Assets);
                    stexSource.textures[val.Key] = ctex;
                }
            }
            if (shape == null)
            {
                return mesh;
            }
            capi.Tesselator.TesselateShape("Fruitpress block", shape, out mesh, texSource, null, 0, 0, 0);
            if (overrideTexturesource == null)
            {
                cMeshes[key] = mesh;
            }
        }
        return mesh;
    }

    public override void OnBeforeRender(ICoreClientAPI capi, ItemStack itemstack, EnumItemRenderTarget target, ref ItemRenderInfo renderinfo)
    {
        base.OnBeforeRender(capi, itemstack, target, ref renderinfo);
        Dictionary<string, MultiTextureMeshRef> meshRefs = ObjectCacheUtil.GetOrCreate(capi, "ScrollrackMeshesInventory", () => new Dictionary<string, MultiTextureMeshRef>());
        string wood = itemstack.Attributes.GetOrAddTreeAttribute("materials").GetString("wood");
        string strainer = itemstack.Attributes.GetOrAddTreeAttribute("materials").GetString("strainer");
        string metal = itemstack.Attributes.GetOrAddTreeAttribute("materials").GetString("metal");
        WoodStrainerMetal materials = new(wood, strainer, metal);
        string key = Code + "-" + materials;
        if (!meshRefs.TryGetValue(key, out MultiTextureMeshRef meshref))
        {
            MeshData mesh = GetOrCreateMesh(materials, isInventory: true);
            meshref = meshRefs[key] = capi.Render.UploadMultiTextureMesh(mesh);
        }
        renderinfo.ModelRef = meshref;
    }

    public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
    {
        ItemStack stack = base.OnPickBlock(world, pos);
        if (world.BlockAccessor.GetBlockEntity(pos) is WFPBlockEntityFruitPress befp)
        {
            stack.Attributes.GetOrAddTreeAttribute("materials").SetString("wood", befp.Materials.Wood);
            stack.Attributes.GetOrAddTreeAttribute("materials").SetString("strainer", befp.Materials.Strainer);
            stack.Attributes.GetOrAddTreeAttribute("materials").SetString("metal", befp.Materials.Metal);
        }
        return stack;
    }

    public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        return new ItemStack[1] { OnPickBlock(world, pos) };
    }

    public override BlockDropItemStack[] GetDropsForHandbook(ItemStack handbookStack, IPlayer forPlayer)
    {
        BlockDropItemStack[] drops = base.GetDropsForHandbook(handbookStack, forPlayer);
        drops[0] = drops[0].Clone();
        drops[0].ResolvedItemstack.SetFrom(handbookStack);
        return drops;
    }

    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

        string wood = inSlot.Itemstack.Attributes.GetOrAddTreeAttribute("materials").GetString("wood");
        string strainer = inSlot.Itemstack.Attributes.GetOrAddTreeAttribute("materials").GetString("strainer");
        string metal = inSlot.Itemstack.Attributes.GetOrAddTreeAttribute("materials").GetString("metal");
        WoodStrainerMetal materials = new(wood, strainer, metal);

        materials.OutputTranslatedDescription(dsc);
    }
}
