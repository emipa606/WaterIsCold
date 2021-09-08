using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace WaterIsCold
{
    public class JobDriver_Swim : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn.Map));
            this.FailOn(() => pawn.Map.mapTemperature.OutdoorTemp < 21f);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);

            void SwimTick()
            {
                JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.GoToNextToil);
            }

            var treadWaterToil = Toils_General.Wait(job.def.joyDuration / 3, TargetIndex.C);
            treadWaterToil.tickAction = SwimTick;

            //Swim to first spot
            var firstSwimToil = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            firstSwimToil.tickAction = SwimTick;
            firstSwimToil.FailOn(() => pawn.Position.GetTerrain(pawn.Map) == TerrainDef.Named("Marsh"));
            yield return firstSwimToil;
            yield return treadWaterToil;
            //Swim to second spot
            var secondSwimToil = Toils_Goto.GotoCell(TargetIndex.C, PathEndMode.OnCell);
            secondSwimToil.tickAction = SwimTick;
            secondSwimToil.FailOn(() => pawn.Position.GetTerrain(pawn.Map) == TerrainDef.Named("Marsh"));
            yield return secondSwimToil;
            yield return treadWaterToil;
            //Swim back to first spot
            yield return firstSwimToil;
            yield return treadWaterToil;
            //Return to shore
            var shoreReturnToil = Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            shoreReturnToil.tickAction = delegate { JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None); };
            yield return shoreReturnToil;
        }

        public override string GetReport()
        {
            var terrain = pawn.Position.GetTerrain(pawn.Map);
            if (!terrain.IsWater)
            {
                return "WIC.goingswim".Translate();
            }

            if (terrain == TerrainDefOf.WaterShallow || terrain == TerrainDefOf.WaterMovingShallow ||
                terrain == TerrainDefOf.WaterOceanShallow)
            {
                return "WIC.wading".Translate();
            }

            if (terrain == TerrainDefOf.WaterOceanDeep)
            {
                return "WIC.bodysurfing".Translate();
            }

            if (terrain == TerrainDefOf.WaterDeep)
            {
                return "WIC.treadingwater".Translate();
            }

            return "WIC.swimming".Translate();
        }
    }
}