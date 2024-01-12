using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace WoodFruitPresses;

public class WFPBlockEntityFruitPress : BlockEntityFruitPress
{
    private string wood;
    private string strainer;
    private string metal;
    private MeshData mesh;

    public WoodStrainerMetal Materials => new(wood, strainer, metal);

    private void Init()
    {
        if (Api != null && Api.Side == EnumAppSide.Client && Materials.Full && Block is WFPBlockFruitPress)
        {
            mesh = (Block as WFPBlockFruitPress).GetOrCreateMesh(Materials);
            // meshMovable = (Block as WFPBlockFruitPress).GetOrCreateMeshMovable(Materials);
        }
    }

    // public BlockEntityAnimationUtil AnimUtil
    // {
    //     get => this.GetField<BlockEntityAnimationUtil>("animUtil");
    //     set => this.SetField("animUtil", value);
    // }

    // public MeshData MeshMovable
    // {
    //     get => this.GetField<MeshData>("meshMovable");
    //     set => this.SetField("meshMovable", value);
    // }

    public override void Initialize(ICoreAPI api)
    {
        base.Initialize(api);
        if (mesh == null && Materials.Full)
        {
            Init();
        }

        // ICoreClientAPI capi = api as ICoreClientAPI;
        // if (Block is WFPBlockFruitPress block)
        // {
        //     Shape shape = Shape.TryGet(api, "shapes/block/wood/fruitpress/part-movable.json");
        //     if (api.Side == EnumAppSide.Client)
        //     {
        //         capi.Tesselator.TesselateShape(block, shape, out MeshData _meshMovable, new Vec3f(0f, block.Shape.rotateY, 0f));
        //         MeshMovable = _meshMovable;
        //         AnimUtil.InitializeAnimator("fruitpress", shape, null, new Vec3f(0f, block.Shape.rotateY, 0f));
        //     }
        //     else
        //     {
        //         AnimUtil.InitializeAnimatorServer("fruitpress", shape);
        //     }
        // }
    }

    public override void OnBlockPlaced(ItemStack byItemStack = null)
    {
        base.OnBlockPlaced(byItemStack);
        wood = byItemStack?.Attributes.GetOrAddTreeAttribute("materials").GetString("wood");
        strainer = byItemStack?.Attributes.GetOrAddTreeAttribute("materials").GetString("strainer");
        metal = byItemStack?.Attributes.GetOrAddTreeAttribute("materials").GetString("metal");
        Init();
    }

    public override void ToTreeAttributes(ITreeAttribute tree)
    {
        base.ToTreeAttributes(tree);
        tree.GetOrAddTreeAttribute("materials").SetString("wood", wood);
        tree.GetOrAddTreeAttribute("materials").SetString("strainer", strainer);
        tree.GetOrAddTreeAttribute("materials").SetString("metal", metal);
    }

    public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
    {
        base.FromTreeAttributes(tree, worldForResolving);
        wood = tree.GetOrAddTreeAttribute("materials").GetString("wood");
        strainer = tree.GetOrAddTreeAttribute("materials").GetString("strainer");
        metal = tree.GetOrAddTreeAttribute("materials").GetString("metal");
        Init();
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
    {
        mesher.AddMeshData(mesh);
        base.OnTesselation(mesher, tessThreadTesselator);
        return true;
    }

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
    {
        base.GetBlockInfo(forPlayer, dsc);

        Materials.OutputTranslatedDescription(dsc);
    }
}