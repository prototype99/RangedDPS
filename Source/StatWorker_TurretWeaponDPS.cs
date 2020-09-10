using System;
using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{

    public class StatWorker_TurretWeaponDPS : StatWorker_TurretDPSBase
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Thing turretGun = GetTurretWeapon(req);
            float rawDps = DPSCalculator.GetRawRangedDPS(turretGun);

            float bestAccuracy = new[] {
                turretGun.GetStatValue(StatDefOf.AccuracyTouch),
                turretGun.GetStatValue(StatDefOf.AccuracyShort),
                turretGun.GetStatValue(StatDefOf.AccuracyMedium),
                turretGun.GetStatValue(StatDefOf.AccuracyLong)
            }.Max();

            return rawDps * Math.Min(bestAccuracy, 1f);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            Thing turretGun = GetTurretWeapon(optionalReq);

            (float, int) optimalRange = new[] {
                (turretGun.GetStatValue(StatDefOf.AccuracyTouch), (int) ShootTuning.DistTouch),
                (turretGun.GetStatValue(StatDefOf.AccuracyShort), (int) ShootTuning.DistShort),
                (turretGun.GetStatValue(StatDefOf.AccuracyMedium), (int) ShootTuning.DistMedium),
                (turretGun.GetStatValue(StatDefOf.AccuracyLong), (int) ShootTuning.DistLong)
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

            return DPSRangeBreakdown(GetTurretWeapon(req));
        }

    }
}
