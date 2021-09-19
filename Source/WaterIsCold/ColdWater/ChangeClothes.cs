using HarmonyLib;
using RimWorld;
using Verse;

namespace WaterIsCold.WetHediff
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved")]
    public class ChangeClothes
    {
        public static void Postfix(Apparel apparel, Pawn ___pawn)
        {
            //Log.Message(___pawn.Name + " changed clothes");
            var firstHediffOfDef = ___pawn.health?.hediffSet?.GetFirstHediffOfDef(DefOf_WaterIsCold.WetCold);
            if (firstHediffOfDef == null)
            {
                return;
            }

            if (!apparel?.def?.apparel?.layers?.Contains(ApparelLayerDefOf.OnSkin) ?? false)
            {
                return;
            }

            firstHediffOfDef.Severity *= .5f;
            if (___pawn.needs?.mood?.thoughts?.memories != null)
            {
                ___pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("SoakingWet"));
            }
        }
    }
}