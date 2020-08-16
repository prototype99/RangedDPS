using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedShooterDPS : StatWorker_RangedDPSBase
    {
        public override bool IsDisabledFor(Thing thing)
        {
            if (!base.IsDisabledFor(thing))
            {
                return StatDefOf.MeleeHitChance.Worker.IsDisabledFor(thing);
            }
            return true;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Thing thing = GetWeaponThing(req);
            float rawDps = DPSCalculator.GetRawRangedDPS(thing);

            float bestAccuracy = new[] {
                thing.GetStatValue(StatDefOf.AccuracyTouch),
                thing.GetStatValue(StatDefOf.AccuracyShort),
                thing.GetStatValue(StatDefOf.AccuracyMedium),
                thing.GetStatValue(StatDefOf.AccuracyLong)
            }.Max();

            return rawDps * bestAccuracy;
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
