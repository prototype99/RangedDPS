using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedWeaponDPS : StatWorker_RangedDPSBase
    {
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

        //public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        //{
        //    object[] obj = new object[4] {
        //        value.ToStringByStyle (stat.toStringStyle, numberSense),
        //        null,
        //        null,
        //        null
        //    };
        //    float num = GetMeleeDamage(optionalReq, true);
        //    obj[1] = num.ToString("0.##");
        //    obj[2] = StatDefOf.MeleeHitChance.ValueToString(GetMeleeHitChance(optionalReq, true), ToStringNumberSense.Absolute, true);
        //    num = GetMeleeCooldown(optionalReq, true);
        //    obj[3] = num.ToString("0.##");
        //    return string.Format("{0} ( {1} x {2} / {3} )", obj);
        //}

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
