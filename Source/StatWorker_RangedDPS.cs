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
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return 0f;
            }

            var shootVerb = GetShootVerb(thingDef);
            if (shootVerb == null)
            {
                return 0f;
            }

            float rawDps = GetRawDPS(shootVerb, req.Thing);

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
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return null;
            }

            var shootVerb = GetShootVerb(thingDef);
            if (shootVerb == null)
            {
                return null;
            }

            float rawDps = GetRawDPS(shootVerb, req.Thing);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

            for (int i = 5; i <= shootVerb.range; i += 5)
            {
                float hitChance = shootVerb.GetHitChanceFactor(req.Thing, i);
                float damage = rawDps * hitChance;
                stringBuilder.AppendLine(string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    i, damage, hitChance));
            }

            // Max Range
            float maxRangeHitChance = shootVerb.GetHitChanceFactor(req.Thing, shootVerb.range);
            float maxRangeDamage = rawDps * maxRangeHitChance;
            stringBuilder.AppendLine(string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    shootVerb.range, maxRangeDamage, maxRangeHitChance));

            return stringBuilder.ToString();
        }

    }
}
