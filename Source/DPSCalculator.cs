using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public static class DPSCalculator
    {

        /// <summary>
        /// Takes in a ThingDef and returns the same ranged attack verb that RimWorld will use
        /// (only the first ranged verb is used in vanilla)
        /// </summary>
        /// <returns>The shoot verb of the passed-in ThingDef</returns>
        /// <param name="thingDef">The ThingDef to get the shoot verb for</param>
        public static VerbProperties GetShootVerb(ThingDef thingDef)
        {
            if (thingDef == null)
            {
                Log.Error($"[RangedDPS] Tried to get the shoot verb of a null ThingDef");
                return null;
            }
            if (!thingDef.IsRangedWeapon)
            {
                Log.Error($"[RangedDPS] Tried to get the shoot verb of {thingDef.defName}, which is not a ranged weapon");
                return null;
            }

            // Note - the game uses the first shoot verb and ignores the rest for whatever reason.  Do the same here
            var shootVerb = (from v in thingDef.Verbs
                             where !v.IsMeleeAttack
                             select v).FirstOrDefault();
            if (shootVerb == null) Log.Error($"[RangedDPS] Could not find a valid shoot verb for ThingDef {thingDef.defName}");
            return shootVerb;
        }

        /// <summary>
        /// Gets the raw ranged DPS of a Thing (The DPS assuming all shots hit their target)
        /// Logs an error and returns 0 if the thing is null or is not a ranged weapon
        /// </summary>
        /// <returns>The raw ranged DPS of the weapon</returns>
        /// <param name="weapon">The Thing to get the DPS of</param>
        public static float GetRawRangedDPS(Thing weapon)
        {
            if (weapon == null)
            {
                Log.Error($"[RangedDPS] Tried to calculate the ranged DPS of a null Thing");
                return 0f;
            }
            var shootVerb = GetShootVerb(weapon.def);
            if (shootVerb == null)
            {
                Log.Error($"[RangedDPS] Tried to calculate the ranged DPS of {weapon.def.defName}, which has no valid shoot verb");
                return 0f;
            }

            // Get the damage from the loaded projectile if the weapon is loadable or the default projectile otherwise
            var projectile = weapon.TryGetComp<CompChangeableProjectile>()?.Projectile?.projectile
                    ?? shootVerb?.defaultProjectile?.projectile;
            // Default to zero damage if we can't find a projectile.
            // Not an error as unloaded mortars don't have projectiles
            int damage = projectile?.GetDamageAmount(weapon) ?? 0;

            float fullCycleTime = shootVerb.warmupTime + weapon.GetStatValue(StatDefOf.RangedWeapon_Cooldown, true)
                    + ((shootVerb.burstShotCount - 1) * shootVerb.ticksBetweenBurstShots).TicksToSeconds();
            int totalDamage = shootVerb.burstShotCount * damage;

            return totalDamage / fullCycleTime;
        }

        /// <summary>
        /// Gets the raw ranged DPS of a Thing (The DPS assuming all shots hit their target)
        /// Logs an error and returns 0 if the thing is null or is not a ranged weapon
        /// </summary>
        /// <returns>The raw ranged DPS of the weapon</returns>
        /// <param name="weapon">The Thing to get the DPS of</param>
        public static float GetAdjustedHitChanceFactor(float range, VerbProperties shootVerb, Thing gun, Thing shooter = null)
        {
            float hitChance = shootVerb.GetHitChanceFactor(gun, range);
            if (shooter != null)
            {
                //TODO
            }

            return hitChance;
        }
    }
}
