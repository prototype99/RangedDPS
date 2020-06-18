using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{

    public class StatWorker_TurretRangedDPS : StatWorker_TurretRangedDPSBase
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Thing turretGun = (req.Thing as Building_TurretGun).gun;

            float rawDps = GetRawDPS(turretGun);

            float bestAccuracy = new[] {
                turretGun.GetStatValue(StatDefOf.AccuracyTouch),
                turretGun.GetStatValue(StatDefOf.AccuracyShort),
                turretGun.GetStatValue(StatDefOf.AccuracyMedium),
                turretGun.GetStatValue(StatDefOf.AccuracyLong)
            }.Max();

            return rawDps * bestAccuracy;
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
            {
                return "";
            }

            Thing turretGun = (req.Thing as Building_TurretGun).gun;

            return DPSRangeBreakdown(turretGun);
        }

    }
}
