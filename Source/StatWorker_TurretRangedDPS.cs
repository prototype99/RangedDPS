using System;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RangedDPS
{

    public class StatWorker_TurretRangedDPS : StatWorker_TurretRangedDPSBase
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return 0f;
            }

            var weaponDef = thingDef?.building?.turretGunDef;
            if (weaponDef == null)
            {
                return 0f;
            }

            var shootVerb = GetShootVerb(weaponDef);
            if (shootVerb == null)
            {
                return 0f;
            }

            float rawDps = GetTurretRawDPS(shootVerb, weaponDef);

            float bestAccuracy = new[] {
                weaponDef.GetStatValueAbstract(StatDefOf.AccuracyTouch),
                weaponDef.GetStatValueAbstract(StatDefOf.AccuracyShort),
                weaponDef.GetStatValueAbstract(StatDefOf.AccuracyMedium),
                weaponDef.GetStatValueAbstract(StatDefOf.AccuracyLong)
            }.Max();

            return rawDps * bestAccuracy;
        }

        //TODO make this work given that turret guns have no Thing
        //public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        //{
        //    ThingDef thingDef = req.Def as ThingDef;
        //    if (thingDef == null)
        //    {
        //        return "";
        //    }

        //    var weaponDef = thingDef?.building?.turretGunDef;
        //    if (weaponDef == null)
        //    {
        //        return "";
        //    }

        //    var shootVerb = GetShootVerb(weaponDef);
        //    if (shootVerb == null)
        //    {
        //        return "";
        //    }

        //    float rawDps = GetTurretRawDPS(shootVerb, weaponDef);

        //    StringBuilder stringBuilder = new StringBuilder();
        //    stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

        //    int startRange = 5;
        //    if (shootVerb.minRange > 0)
        //    {
        //        float minRangeHitChance = shootVerb.GetHitChanceFactor(req.Thing, shootVerb.minRange);
        //        float minRangeDps = rawDps * minRangeHitChance;
        //        stringBuilder.AppendLine(string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
        //                "distance".Translate().CapitalizeFirst(),
        //                shootVerb.minRange, minRangeDps, minRangeHitChance));

        //        int startPoint = (int)Math.Ceiling(shootVerb.minRange / 5) * 5;
        //    }

        //    for (int i = startRange; i <= shootVerb.range; i += 5)
        //    {
        //        float hitChance = shootVerb.GetHitChanceFactor(req.Thing, i);
        //        float damage = rawDps * hitChance;
        //        stringBuilder.AppendLine();
        //    }

        //    // Max Range
        //    float maxRangeHitChance = shootVerb.GetHitChanceFactor(req.Thing, shootVerb.range);
        //    float maxRangeDps = rawDps * maxRangeHitChance;
        //    stringBuilder.AppendLine(string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
        //            "distance".Translate().CapitalizeFirst(),
        //            shootVerb.range, maxRangeDps, maxRangeHitChance));

        //    return stringBuilder.ToString();
        //}

    }
}
