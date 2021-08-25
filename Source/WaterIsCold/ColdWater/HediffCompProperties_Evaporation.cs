using Verse;

namespace WaterIsCold
{
    public class HediffCompProperties_Evaporation : HediffCompProperties_SeverityPerDay
    {
        public HediffCompProperties_Evaporation()
        {
            compClass = typeof(HediffComp_Evaporation);
        }
    }
}