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

            Thing turretGun = GetTurretThing(req);
            float rawDps = DPSCalculator.GetRawRangedDPS(turretGun);

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

            return DPSRangeBreakdown(GetTurretThing(req));
        }

    }
}
