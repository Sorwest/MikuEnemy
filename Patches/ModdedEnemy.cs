using HarmonyLib;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace Sorwest.MikuEnemy;

internal sealed class ModdedEnemyManager
{
    public ModdedEnemyManager()
    {
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(MapFirst), nameof(MapFirst.GetEnemyPools)),
            postfix: new HarmonyMethod(GetType(), nameof(MapFirst_GetEnemyPools_Postfix))
        );
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Character), nameof(Character.GetDisplayName), new[] { typeof(string), typeof(State) }),
            postfix: new HarmonyMethod(GetType(), nameof(Character_GetDisplayName_Postfix))
        );
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Character), nameof(Character.DrawFace)),
            postfix: new HarmonyMethod(GetType(), nameof(Character_DrawFace_Postfix))
        );
    }
    static void MapFirst_GetEnemyPools_Postfix(
        ref MapBase.MapEnemyPool __result)
    {
        __result.elites.Add(new MikuAI());
    }
    static void Character_GetDisplayName_Postfix(
        string charId,
        ref string __result)
    {
        if (charId == "miku")
            __result = ModEntry.Instance.Localizations.Localize(["character", "miku", "name"]);
    }
    static void Character_DrawFace_Postfix(
        Character __instance,
        double x,
        double y)
    {
        if (__instance.type == "miku")
        {
            Spr? id5 = ModEntry.Instance.MikuNeutral.Sprite;
            double x6 = x + 60.0;
            double y6 = y + 1.0;
            Vec? originRel = new Vec(1.0);
            Draw.Sprite(id5, x6, y6, flipX: false, flipY: false, 0.0, null, originRel, null, null, Colors.white);
        }
    }
}
