using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedDPS : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!(req.Def is ThingDef thingDef))
            {
                return false;
            }
            if (!thingDef.IsWeapon && !thingDef.isTechHediff)
            {
                return false;
            }
            return thingDef.IsRangedWeapon;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return 0f;
            }

            Pawn currentWeaponUser = GetCurrentWeaponUser(req.Thing);
            var shootVerb = GetShootVerb(thingDef);

            return GetRawDPS(shootVerb, currentWeaponUser, req.Thing);
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return null;
            }

            Pawn currentWeaponUser = GetCurrentWeaponUser(req.Thing);
            var shootVerb = GetShootVerb(thingDef);

            float rawDps = GetRawDPS(shootVerb, currentWeaponUser, req.Thing);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

            for (int i = 5; i <= shootVerb.verbProps.range; i += 5)
            {
                float hitChance = shootVerb.verbProps.GetHitChanceFactor(req.Thing, i);
                float damage = rawDps * hitChance;
                stringBuilder.AppendLine(string.Format("{0} {1,2}: {2:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    i, damage, hitChance));
            }

            // Max Range
            float maxRangeHitChance = shootVerb.verbProps.GetHitChanceFactor(req.Thing, shootVerb.verbProps.range);
            float maxRangeDamage = rawDps * maxRangeHitChance;
            stringBuilder.AppendLine(string.Format("{0} {1,2}:  {2:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    shootVerb.verbProps.range, maxRangeDamage, maxRangeHitChance));

            return stringBuilder.ToString();
        }

        private static VerbUtility.VerbPropertiesWithSource GetShootVerb(ThingDef thingDef)
        {
            // Note - the game uses the first shoot verb and ignores the rest for whatever reason.  Do the same here
            return (from x in VerbUtility.GetAllVerbProperties(thingDef.Verbs, thingDef.tools)
                    where !x.verbProps.IsMeleeAttack
                    select x).First();
        }
        
        private float GetRawDPS(VerbUtility.VerbPropertiesWithSource shootVerb, Pawn currentWeaponUser, Thing thing)
        {
            float fullCycleTime = shootVerb.verbProps.warmupTime + shootVerb.verbProps.AdjustedCooldown(shootVerb.tool, currentWeaponUser, thing)
                    + ((shootVerb.verbProps.burstShotCount - 1) * shootVerb.verbProps.ticksBetweenBurstShots).TicksToSeconds();

            int totalDamage = shootVerb.verbProps.burstShotCount * shootVerb.verbProps.defaultProjectile.projectile.GetDamageAmount(thing);

            return totalDamage / fullCycleTime;
        }

        private static Pawn GetCurrentWeaponUser(Thing weapon)
        {
            if (weapon == null)
            {
                return null;
            }
            Pawn_EquipmentTracker pawn_EquipmentTracker = weapon.ParentHolder as Pawn_EquipmentTracker;
            if (pawn_EquipmentTracker != null)
            {
                return pawn_EquipmentTracker.pawn;
            }
            return (weapon.ParentHolder as Pawn_ApparelTracker)?.pawn;
        }

    }
}
