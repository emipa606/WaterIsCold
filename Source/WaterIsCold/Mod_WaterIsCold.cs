using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WaterIsCold
{
    public class Mod_WaterIsCold : Mod
    {
        private readonly Listing_Standard listingStandard = new Listing_Standard();

        public Mod_WaterIsCold(ModContentPack content) : base(content)
        {
            GetSettings<ModSettings_WaterIsCold>();
            var harmony = new Harmony("rimworld.varietymattersfashion");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory()
        {
            return "Water Is Cold";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var rect = new Rect(10f, 50f, inRect.width * .6f, inRect.height);
            listingStandard.Begin(rect);
            listingStandard.CheckboxLabeled("Water is cold: ", ref ModSettings_WaterIsCold.coldWater);
            listingStandard.CheckboxLabeled("Water is deep: ", ref ModSettings_WaterIsCold.deepWater);
            listingStandard.CheckboxLabeled("Water is fun: ", ref ModSettings_WaterIsCold.funWater);
            listingStandard.GapLine();
            //listingStandard.CheckboxLabeled("Always disable soaking wet thought:", ref ModSettings_WaterIsCold.disableWetAlways);
            //listingStandard.CheckboxLabeled("Disable soaking wet thought when warm (above 21C/70F):", ref ModSettings_WaterIsCold.disableWetWarm);
            if (listingStandard.RadioButton("Always disable soaking wet thought:",
                ModSettings_WaterIsCold.disableWetAlways))
            {
                ModSettings_WaterIsCold.disableWetAlways = true;
                ModSettings_WaterIsCold.disableWetWarm = false;
                ModSettings_WaterIsCold.disableWetNever = false;
            }

            if (listingStandard.RadioButton("Disable soaking wet when warm (above 26C/78.8F):",
                ModSettings_WaterIsCold.disableWetWarm))
            {
                ModSettings_WaterIsCold.disableWetAlways = false;
                ModSettings_WaterIsCold.disableWetWarm = true;
                ModSettings_WaterIsCold.disableWetNever = false;
            }

            if (listingStandard.RadioButton("Disable soaking wet when swimming:",
                !ModSettings_WaterIsCold.disableWetAlways && !ModSettings_WaterIsCold.disableWetWarm &&
                !ModSettings_WaterIsCold.disableWetNever))
            {
                ModSettings_WaterIsCold.disableWetAlways = false;
                ModSettings_WaterIsCold.disableWetWarm = false;
                ModSettings_WaterIsCold.disableWetNever = false;
            }

            if (listingStandard.RadioButton("Swimming makes me angry:", ModSettings_WaterIsCold.disableWetNever))
            {
                ModSettings_WaterIsCold.disableWetAlways = false;
                ModSettings_WaterIsCold.disableWetWarm = false;
                ModSettings_WaterIsCold.disableWetNever = true;
            }

            listingStandard.GapLine();
            var wetInsulationLabel = "Minimum insulation value of clothing when wet (%):";
            var wetInsulationBuffer = ModSettings_WaterIsCold.wetInsFactor.ToString();
            LabeledIntEntry(listingStandard.GetRect(24f), wetInsulationLabel, ref ModSettings_WaterIsCold.wetInsFactor,
                ref wetInsulationBuffer, 1, 10, 0, 100);
            var swimSearchArea = "Max swimming distance (default = 80; max = 250):";
            var swimSearchBuffer = ModSettings_WaterIsCold.swimSearchArea.ToString();
            LabeledIntEntry(listingStandard.GetRect(24f), swimSearchArea, ref ModSettings_WaterIsCold.swimSearchArea,
                ref swimSearchBuffer, 5, 20, 0, 250);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        private void LabeledIntEntry(Rect rect, string label, ref int value, ref string editBuffer, int multiplier,
            int largeMultiplier, int min, int max)
        {
            var num = rect.width / 15f;
            Widgets.Label(rect, label);
            if (multiplier != largeMultiplier)
            {
                if (Widgets.ButtonText(new Rect(rect.xMax - (num * 5f), rect.yMin, num, rect.height),
                    (-1 * largeMultiplier).ToString()))
                {
                    value -= largeMultiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToString();
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                }

                if (Widgets.ButtonText(new Rect(rect.xMax - num, rect.yMin, num, rect.height), "+" + largeMultiplier))
                {
                    value += largeMultiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToString();
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                }
            }

            if (Widgets.ButtonText(new Rect(rect.xMax - (num * 4f), rect.yMin, num, rect.height),
                (-1 * multiplier).ToString()))
            {
                value -= multiplier * GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToString();
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
            }

            if (Widgets.ButtonText(new Rect(rect.xMax - (num * 2f), rect.yMin, num, rect.height), "+" + multiplier))
            {
                value += multiplier * GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToString();
                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
            }

            Widgets.TextFieldNumeric(new Rect(rect.xMax - (num * 3f), rect.yMin, num, rect.height), ref value,
                ref editBuffer, min, max);
        }
    }
}