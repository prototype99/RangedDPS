using System;
using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedWeaponDPS : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (base.ShouldShowFor(req))
            {
                return req.Def is ThingDef thingDef && thingDef.IsRangedWeapon;
            }
            return false;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Thing weapon = GetWeaponThing(req);
            float rawDps = DPSCalculator.GetRawRangedDPS(weapon);

            float bestAccuracy = new[] {
                weapon.GetStatValue(StatDefOf.AccuracyTouch),
                weapon.GetStatValue(StatDefOf.AccuracyShort),
                weapon.GetStatValue(StatDefOf.AccuracyMedium),
                weapon.GetStatValue(StatDefOf.AccuracyLong)
            }.Max();

            return rawDps * Math.Min(bestAccuracy, 1);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            Thing weapon = GetWeaponThing(optionalReq);

            (float, int) optimalRange = new[] {
                (weapon.GetStatValue(StatDefOf.AccuracyTouch), (int) ShootTuning.DistTouch),
                (weapon.GetStatValue(StatDefOf.AccuracyShort), (int) ShootTuning.DistShort),
                (weapon.GetStatValue(StatDefOf.AccuracyMedium), (int) ShootTuning.DistMedium),
                (weapon.GetStatValue(StatDefOf.AccuracyLong), (int) ShootTuning.DistLong)
            }.MaxBy(acc => acc.Item1);

            return string.Format("{0} ({1})",
                value.ToStringByStyle(stat.toStringStyle, numberSense),
                string.Format("StatsReport_RangedDPSOptimalRange".Translate(), optimalRange.Item2));
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
            {
                return "";
            }

            return DPSRangeBreakdown(GetWeaponThing(req));
        }

    }
}
