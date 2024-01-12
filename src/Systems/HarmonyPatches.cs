// using System.Reflection;
// using HarmonyLib;
// using Vintagestory.API.Common;
// using Vintagestory.GameContent;

// namespace WoodFruitPresses;

// public class HarmonyPatches : ModSystem
// {
//     public const string HarmonyID = "craluminum2413.WoodFruitPresses";

//     public MethodInfo FruitpressInitialize => typeof(BlockEntityFruitPress).GetMethod(nameof(BlockEntityFruitPress.Initialize));

//     public override void Start(ICoreAPI api)
//     {
//         base.Start(api);
//         new Harmony(HarmonyID).Patch(original: FruitpressInitialize, postfix: typeof(FruitPress_Initialize_Patch).GetMethod(nameof(FruitPress_Initialize_Patch.Postfix)));
//     }

//     public override void Dispose()
//     {
//         new Harmony(HarmonyID).Unpatch(original: FruitpressInitialize, HarmonyPatchType.All, HarmonyID);
//         base.Dispose();
//     }
// }