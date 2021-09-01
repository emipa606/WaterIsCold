using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WaterIsCold
{
    [HarmonyPatch]
    public class AddWetHediff
    {
        //public static List<ThoughtDef> wetThoughts = new List<ThoughtDef>() {ThoughtDef.Named("SoakingWet")};

        [HarmonyPatch(typeof(MemoryThoughtHandler), "TryGainMemoryFast")]
        public static bool Prefix(ThoughtDef mem, Pawn ___pawn)
        {
            if (mem != ThoughtDef.Named("SoakingWet"))
            {
                if (ModLister.GetActiveModWithIdentifier("ReGrowth.BOTR.Core") == null ||
                    mem != ThoughtDef.Named("RG_Wet") && mem != ThoughtDef.Named("RG_ExtremelyWet"))
                {
                    return true;
                }
            }

            if (ModSettings_WaterIsCold.coldWater)
            {
                AddHediff(___pawn);
            }

            if (ModLister.GetActiveModWithIdentifier("VanillaExpanded.VanillaTraitsExpanded") != null)
            {
                if (___pawn.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail("VTE_ChildOfSea")))
                {
                    return true;
                }
            }

            if (ModSettings_WaterIsCold.disableWetAlways)
            {
                return false;
            }

            if (ModSettings_WaterIsCold.disableWetNever)
            {
                return true;
            }

            if (ModSettings_WaterIsCold.disableWetWarm)
            {
                return ___pawn.Map.mapTemperature.OutdoorTemp <= 26;
            }

            return ___pawn.jobs.curJob?.def != DefOf_WaterIsCold.WIC_Swim;
        }

        public static void AddHediff(Pawn pawn)
        {
            var def = DefOf_WaterIsCold.WetCold;
            var firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(def);
            if (firstHediffOfDef == null)
            {
                pawn.health.AddHediff(HediffMaker.MakeHediff(def, pawn));
                firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(def);
            }

            var curSeverity = firstHediffOfDef.Severity;

            //Determine level from traversing through water
            var terrain = pawn.Position.GetTerrain(pawn.Map);
            if (terrain.IsWater)
            {
                if (terrain == TerrainDefOf.WaterDeep || terrain == TerrainDefOf.WaterOceanDeep ||
                    terrain == TerrainDefOf.WaterMovingChestDeep)
                {
                    firstHediffOfDef.Severity = 1f;
                }
                else
                {
                    firstHediffOfDef.Severity = .5f;
                }

                return;
            }

            //Apply rain
            var rainRate = pawn.Map.weatherManager.RainRate;
            if (pawn.apparel == null)
            {
                return;
            }

            var outer = false;
            var middle = false;
            var skin = false;
            var layers = 0f;
            for (var i = 0; i < pawn.apparel.WornApparelCount; i++)
            {
                var apparel = pawn.apparel.WornApparel[i];
                if (apparel.def.apparel.layers.Contains(ApparelLayerDefOf.Shell) && !outer)
                {
                    outer = true;
                    layers += .5f;
                }

                if (apparel.def.apparel.layers.Contains(ApparelLayerDefOf.Middle) && !middle)
                {
                    middle = true;
                    layers += .25f;
                }

                if (!apparel.def.apparel.layers.Contains(ApparelLayerDefOf.OnSkin) || skin)
                {
                    continue;
                }

                skin = true;
                layers += .25f;
            }

            layers = 1.25f - Mathf.Max(.25f, layers);
            curSeverity += rainRate * layers / 10;
            firstHediffOfDef.Severity = Mathf.Min(curSeverity, rainRate * layers);
        }
    }
}