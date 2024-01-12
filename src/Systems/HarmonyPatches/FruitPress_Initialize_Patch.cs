// using HarmonyLib;
// using Vintagestory.API.Client;
// using Vintagestory.API.Common;
// using Vintagestory.API.MathTools;
// using Vintagestory.GameContent;

// namespace WoodFruitPresses;

// [HarmonyPatch(typeof(BlockEntityFruitPress), nameof(BlockEntityFruitPress.Initialize))]
// public static class FruitPress_Initialize_Patch
// {
//     public static void Postfix(BlockEntityFruitPress __instance, ICoreAPI api)
//     {
//         if (__instance.Block?.Attributes?["isWFPFruitPress"].AsBool() == true)
//         {
//             return;
//             BlockFruitPress ownBlock = __instance.GetField<BlockFruitPress>("ownBlock");
//             BlockEntityAnimationUtil animUtil = __instance.GetField<BlockEntityAnimationUtil>("animUtil");
//             // FruitpressContentsRenderer renderer = __instance.GetField<FruitpressContentsRenderer>("renderer");

//             ICoreClientAPI capi = api as ICoreClientAPI;

//             if (ownBlock != null)
//             {
//                 Shape shape = Shape.TryGet(api, "shapes/block/wood/fruitpress/part-movable.json");
//                 if (api.Side == EnumAppSide.Client)
//                 {
//                     capi.Tesselator.TesselateShape(ownBlock, shape, out MeshData _meshMovable, new Vec3f(0f, ownBlock.Shape.rotateY, 0f));
//                     __instance.SetField("meshMovable", _meshMovable);

//                     animUtil.InitializeAnimator("fruitpress", shape, null, new Vec3f(0f, ownBlock.Shape.rotateY, 0f));
//                 }
//                 else
//                 {
//                     animUtil.InitializeAnimatorServer("fruitpress", shape);
//                 }
//                 // if (api.Side == EnumAppSide.Client)
//                 // {
//                 //     renderer = new FruitpressContentsRenderer(api as ICoreClientAPI, base.Pos, this);
//                 //     (api as ICoreClientAPI).Event.RegisterRenderer(renderer, EnumRenderStage.Opaque, "fruitpress");
//                 //     renderer.reloadMeshes(getJuiceableProps(mashStack), mustReload: true);
//                 //     genBucketMesh();

//                 //     __instance.SetField("renderer", renderer);
//                 // }
//             }
//         }
//     }
// }