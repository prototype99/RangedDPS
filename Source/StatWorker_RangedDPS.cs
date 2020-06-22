using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedDPS : StatWorker_RangedDPSBase
    {
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Thing thing = req.Thing ?? (req.Def as ThingDef).GetConcreteExample();
            float rawDps = GetRawDPS(thing);

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

            Thing thing = req.Thing ?? (req.Def as ThingDef).GetConcreteExample();
            return DPSRangeBreakdown(thing);
        }

    }
}
