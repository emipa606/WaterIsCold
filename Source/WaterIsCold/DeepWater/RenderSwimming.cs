using HarmonyLib;
using RimWorld;
using Verse;

namespace WaterIsCold
{
    [HarmonyPatch]
    public class RenderSwimming
    {
        [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
        private static void Prefix(ref bool renderBody, Pawn ___pawn)
        {
            if (!renderBody)
            {
                return;
            }

            if (___pawn == null || ___pawn.Dead)
            {
                return;
            }

            if (___pawn.CarriedBy != null)
            {
                return;
            }

            if (!ModSettings_WaterIsCold.deepWater)
            {
                return;
            }

            var terrain = ___pawn.Position.GetTerrain(___pawn.Map);
            if (terrain == TerrainDefOf.WaterMovingChestDeep || terrain == TerrainDefOf.WaterOceanDeep ||
                terrain == TerrainDefOf.WaterDeep)
            {
                renderBody = false;
            }
        }
    }
}