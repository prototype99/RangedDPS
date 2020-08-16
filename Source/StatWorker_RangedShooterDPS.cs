using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedShooterDPS : StatWorker_RangedDPSBase
    {

        public override bool ShouldShowFor(StatRequest req)
        {
            // FIXME - we can't check the base class because at the moment the base class assumes a ranged weapon
            //if (base.ShouldShowFor(req))
            //{
                return req.Thing is Pawn pawn && (pawn?.equipment?.Primary?.def?.IsRangedWeapon ?? false);
            //}
            //return false;
        }

        public override bool IsDisabledFor(Thing thing)
        {
            if (!base.IsDisabledFor(thing))
            {
                return StatDefOf.ShootingAccuracyPawn.Worker.IsDisabledFor(thing) &&
                        StatDefOf.ShootingAccuracyTurret.Worker.IsDisabledFor(thing);
            }
            return true;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Pawn pawn = req.Thing as Pawn;
            Thing weapon = GetPawnWeapon(pawn);
            if (!(weapon?.def?.IsRangedWeapon ?? false))
            {
                Log.Error($"[RangedDPS] Tried to calculate the ranged DPS of pawn {pawn.Name} that doesn't have a ranged weapon equipped");
                return 0f;
            }

            float rawDps = DPSCalculator.GetRawRangedDPS(weapon);

            // FIXME - the actual stat value doesn't yet account for shooter accuracy

            float bestAccuracy = new[] {
                weapon.GetStatValue(StatDefOf.AccuracyTouch),
                weapon.GetStatValue(StatDefOf.AccuracyShort),
                weapon.GetStatValue(StatDefOf.AccuracyMedium),
                weapon.GetStatValue(StatDefOf.AccuracyLong)
            }.Max();

            return rawDps * bestAccuracy;
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (!ShouldShowFor(req))
            {
                return "";
            }

            Pawn pawn = req.Thing as Pawn;
            return DPSRangeBreakdown(GetPawnWeapon(pawn), pawn);
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

        public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
        {
            if (statRequest.Thing is Pawn pawn && pawn?.equipment?.Primary != null)
            {
                yield return new Dialog_InfoCard.Hyperlink(pawn.equipment.Primary, -1);
            }
        }

        protected static Thing GetPawnWeapon(Pawn pawn)
        {
            return pawn?.equipment?.Primary;
        }
    }
}
