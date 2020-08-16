using RimWorld;
using Verse;

namespace RangedDPS
{

    public class StatWorker_TurretShooterDPS : StatWorker_TurretDPSBase
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Building_TurretGun turret = GetTurret(req);
            var shootVerb = DPSCalculator.GetShootVerb(turret.gun.def);
            float bestRange = DPSCalculator.FindOptimalRange(shootVerb, turret.gun, turret);

            return DPSCalculator.GetRawRangedDPS(turret.gun) * DPSCalculator.GetAdjustedHitChanceFactor(bestRange, shootVerb, turret.gun, turret);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            Building_TurretGun turret = GetTurret(optionalReq);
            var shootVerb = DPSCalculator.GetShootVerb(turret.gun.def);

            int optimalRange = (int)DPSCalculator.FindOptimalRange(shootVerb, turret.gun, turret);

            return string.Format("{0} ({1})",
                value.ToStringByStyle(stat.toStringStyle, numberSense),
                string.Format("StatsReport_RangedDPSOptimalRange".Translate(), optimalRange));
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
            {
                return "";
            }

            Building_TurretGun turret = GetTurret(req);
            return DPSRangeBreakdown(turret.gun, turret);
        }

    }
}
