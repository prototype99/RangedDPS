using System;
using System.Text;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedDPSBase : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (base.ShouldShowFor(req))
            {
                return req.Def is ThingDef thingDef && thingDef.IsRangedWeapon;
            }
            return false;
        }

        protected static Thing GetWeaponThing(StatRequest req)
        {
            Thing weapon = req.Thing ?? (req.Def as ThingDef)?.GetConcreteExample();
            if (weapon == null) Log.Error($"[RangedDPS] Could not find a valid weapon thing when trying to caluculate the stat for {req.Def.defName}");
            return weapon;
        }

        /// <summary>
        /// Returns a string that provides a breakdown of both accuracy and DPS over the full range of the given weapon.
        /// Shooter is optional, and if provided the DPS is adjusted to account for the shooter's shooting accuracy.
        /// </summary>
        /// <returns>A string providing a breakdown of the performance of the given weapon at various ranges.</returns>
        /// <param name="gun">The gun to caluclate a breakdown for.</param>
        /// <param name="shooter">(Optional) The shooter (pawn or turret) using the weapon.</param>
        protected static string DPSRangeBreakdown(Thing gun, Thing shooter = null)
        {
            float rawDps = DPSCalculator.GetRawRangedDPS(gun);
            var shootVerb = DPSCalculator.GetShootVerb(gun.def);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

            // Min Range
            float minRange = Math.Max(shootVerb.minRange, 3f);
            float minRangeHitChance = DPSCalculator.GetAdjustedHitChanceFactor(minRange, shootVerb, gun, shooter);
            float minRangeDps = rawDps * minRangeHitChance;
            stringBuilder.AppendLine(FormatDPSRangeString(minRange, minRangeDps, minRangeHitChance));

            // Ranges between Min - Max, in steps of 5
            float startRange = (float)Math.Ceiling(minRange / 5) * 5;
            for (float range = startRange; range < shootVerb.range; range += 5)
            {
                float hitChance = DPSCalculator.GetAdjustedHitChanceFactor(range, shootVerb, gun, shooter);
                float dps = rawDps * hitChance;
                stringBuilder.AppendLine(FormatDPSRangeString(range, dps, hitChance));
            }

            // Max Range
            float maxRangeHitChance = DPSCalculator.GetAdjustedHitChanceFactor(shootVerb.range, shootVerb, gun, shooter);
            float maxRangeDps = rawDps * maxRangeHitChance;
            stringBuilder.AppendLine(FormatDPSRangeString(shootVerb.range, maxRangeDps, maxRangeHitChance));

            return stringBuilder.ToString();
        }

        protected static string FormatDPSRangeString(float range, float dps, float hitChance)
        {
            return string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    range, dps, hitChance);
        }

    }
}
