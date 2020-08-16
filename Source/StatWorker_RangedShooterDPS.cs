using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedShooterDPS : StatWorker_RangedDPSBase
    {

        public override bool ShouldShowFor(StatRequest req)
        {
            if (base.ShouldShowFor(req))
            {
                return req.Thing is Pawn pawn && (pawn?.equipment?.Primary?.def?.IsRangedWeapon ?? false);
            }
            return false;
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

            var shootVerb = DPSCalculator.GetShootVerb(weapon.def);
            float bestRange = DPSCalculator.FindOptimalRange(shootVerb, weapon, pawn);

            return DPSCalculator.GetRawRangedDPS(weapon) * DPSCalculator.GetAdjustedHitChanceFactor(bestRange, shootVerb, weapon, pawn);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            Pawn pawn = optionalReq.Thing as Pawn;
            Thing weapon = GetPawnWeapon(pawn);
            var shootVerb = DPSCalculator.GetShootVerb(weapon.def);

            int optimalRange = (int) DPSCalculator.FindOptimalRange(shootVerb, weapon, pawn);

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

            Pawn pawn = req.Thing as Pawn;
            return DPSRangeBreakdown(GetPawnWeapon(pawn), pawn);
        }

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
