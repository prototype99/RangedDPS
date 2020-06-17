using System;
using System.Linq;
using System.Text;
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

            float rawDps = GetRawDPS(req.Thing);

            float bestAccuracy = new[] {
                req.Thing.GetStatValue(StatDefOf.AccuracyTouch),
                req.Thing.GetStatValue(StatDefOf.AccuracyShort),
                req.Thing.GetStatValue(StatDefOf.AccuracyMedium),
                req.Thing.GetStatValue(StatDefOf.AccuracyLong)
            }.Max();

            return rawDps * bestAccuracy;
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
            {
                return "";
            }

            return DPSRangeBreakdown(req.Thing);
        }

    }
}
