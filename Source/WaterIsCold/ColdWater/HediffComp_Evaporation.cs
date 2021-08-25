using UnityEngine;
using Verse;

namespace WaterIsCold
{
    public class HediffComp_Evaporation : HediffComp_SeverityPerDay
    {
        private HediffCompProperties_Evaporation Props => (HediffCompProperties_Evaporation)props;

        public override string CompTipStringExtra
        {
            get
            {
                if (props is HediffCompProperties_SeverityPerDay && Props.showDaysToRecover &&
                    SeverityChangePerDay() < 0f)
                {
                    return "DaysToRecover".Translate(
                        (parent.Severity / Mathf.Abs(SeverityChangePerDay())).ToString("0.0"));
                }

                return null;
            }
        }

        public override float SeverityChangePerDay()
        {
            var heatFactor = 1f + ((Pawn.AmbientTemperature - 21) / 50);
            return base.SeverityChangePerDay() * heatFactor;
        }
    }
}