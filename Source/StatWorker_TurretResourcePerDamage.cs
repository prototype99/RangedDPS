using System;
using System.Linq;
using RangedDPS.StatUtilities;
using RimWorld;
using Verse;

namespace RangedDPS
{

    public class StatWorker_TurretResourcePerDamage : StatWorker_TurretDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!base.ShouldShowFor(req)) return false;

            // Don't show resource usage for turrets without ammo
            var comp = GetTurretWeapon(req).TryGetComp<CompReloadable>();
            return comp != null;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Building_TurretGun turret = GetTurret(req);
            RangedWeaponStats weaponStats = GetTurretStats(req);

            float optimalRange = weaponStats.FindOptimalRange(turret);
            return weaponStats.GetAdjustedDPS(optimalRange, turret);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            Building_TurretGun turret = GetTurret(optionalReq);
            RangedWeaponStats weaponStats = GetTurretStats(optionalReq);

            int optimalRange = (int)weaponStats.FindOptimalRange(turret);

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
            RangedWeaponStats weaponStats = GetTurretStats(req);

            return DPSRangeBreakdown(weaponStats, turret);
        }
    }
}