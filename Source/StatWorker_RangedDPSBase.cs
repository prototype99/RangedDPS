using System;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_RangedDPSBase : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            return req.Def is ThingDef thingDef && ThingDefIsShooty(thingDef);
        }

        protected static bool ThingDefIsShooty(ThingDef thingDef)
        {
            if (thingDef == null) return false;
            return (from v in thingDef.Verbs
                    where !v.IsMeleeAttack
                    select v).Any();
        }

        protected static VerbProperties GetShootVerb(ThingDef thingDef)
        {
            // Note - the game uses the first shoot verb and ignores the rest for whatever reason.  Do the same here
            return (from v in thingDef.Verbs
                    where !v.IsMeleeAttack
                    select v).FirstOrDefault();
        }

        protected static Thing GetWeaponThing(StatRequest req)
        {
            return req.Thing ?? (req.Def as ThingDef)?.GetConcreteExample();
        }

        protected static float GetRawDPS(Thing thing)
        {
            var shootVerb = GetShootVerb(thing.def);

            // Get the damage from the loaded projectile first (for loadable weapons) or the default projectile otherwise
            var projectile = thing.TryGetComp<CompChangeableProjectile>()?.Projectile?.projectile
                    ?? shootVerb?.defaultProjectile?.projectile;
            // Default to zero damage if we can't find a projectile.
            // Not an error as unloaded mortars don't have projectiles
            int damage = projectile?.GetDamageAmount(thing) ?? 0; 

            float fullCycleTime = shootVerb.warmupTime + thing.GetStatValue(StatDefOf.RangedWeapon_Cooldown, true)
                    + ((shootVerb.burstShotCount - 1) * shootVerb.ticksBetweenBurstShots).TicksToSeconds();
            int totalDamage = shootVerb.burstShotCount * damage;

            return totalDamage / fullCycleTime;
        }

        protected static string DPSRangeBreakdown(Thing gun)
        {
            float rawDps = GetRawDPS(gun);
            var shootVerb = GetShootVerb(gun.def);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

            // Min Range
            float minRange = Math.Max(shootVerb.minRange, 3f);
            float minRangeHitChance = shootVerb.GetHitChanceFactor(gun, minRange);
            float minRangeDps = rawDps * minRangeHitChance;
            stringBuilder.AppendLine(FormatDPSRangeString(minRange, minRangeDps, minRangeHitChance));

            // Ranges between Min - Max, in steps of 5
            float startRange = (float)Math.Ceiling(minRange / 5) * 5;
            for (float i = startRange; i <= shootVerb.range; i += 5)
            {
                float hitChance = shootVerb.GetHitChanceFactor(gun, i);
                float dps = rawDps * hitChance;
                stringBuilder.AppendLine(FormatDPSRangeString(i, dps, hitChance));
            }

            // Max Range
            float maxRangeHitChance = shootVerb.GetHitChanceFactor(gun, shootVerb.range);
            float maxRangeDps = rawDps * maxRangeHitChance;
            stringBuilder.AppendLine(FormatDPSRangeString(shootVerb.range, maxRangeDps, maxRangeHitChance));

            return stringBuilder.ToString();
        }

        protected static string FormatDPSRangeString(float range, float dps, float hitChance)
        {
            return string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    range, dps, hitChance);
        }

    }
}
