using System.Linq;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.ServerMods;

namespace WoodFruitPresses;

public static class Extensions
{
    public static JsonItemStack CreateJStack(this ICoreAPI api, CollectibleObject obj, string attributes)
    {
        JsonItemStack jstack = new()
        {
            Code = obj.Code,
            Type = obj.ItemClass,
            Attributes = new JsonObject(JToken.Parse(attributes))
        };
        _ = jstack.Resolve(api.World, obj.Code?.ToString() + " type");
        return jstack;
    }

    public static string[] ResolveVariants(this ICoreAPI api, CollectibleObject obj, string materialAttr)
    {
        RegistryObjectVariantGroup grp = obj.Attributes["materials"][materialAttr].AsObject<RegistryObjectVariantGroup>();
        string[] materials = grp.States;
        if (grp.LoadFromProperties != null)
        {
            materials = (api.Assets.TryGet(grp
                .LoadFromProperties
                .WithPathPrefixOnce("worldproperties/")
                .WithPathAppendixOnce(".json"))?.ToObject<StandardWorldProperty>()).Variants.Select((p) => p.Code.Path).ToArray().Append(materials);
        }

        return materials;
    }
}