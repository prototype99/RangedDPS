using System;
using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{
    public static class DPSCalculator
    {

        /// <summary>
        /// Takes in a ThingDef and returns the same ranged attack verb that RimWorld will use.
        /// (only the first ranged verb is used in vanilla)
        /// </summary>
        /// <returns>The shoot verb of the passed-in ThingDef.</returns>
        /// <param name="thingDef">The ThingDef to get the shoot verb for.</param>
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
        /// Logs an error and returns 0 if the thing is null or is not a ranged weapon.
        /// </summary>
        /// <returns>The raw ranged DPS of the weapon.</returns>
        /// <param name="weapon">The Thing to get the DPS of.</param>
        /// <param name="shooter">The Pawn wielding the weapon, or null if we're just looking at a weapon in the abstract</param>
        public static float GetRawRangedDPS(Thing weapon, Pawn shooter = null, Building_TurretGun turret = null)
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

            float aimFactor = shooter?.GetStatValue(StatDefOf.AimingDelayFactor, true) ?? 1f;
            float warmup = turret?.def?.building?.turretBurstWarmupTime ?? shootVerb.warmupTime;
            float cooldown = (turret != null) ? TurretBurstCooldown(turret) : weapon.GetStatValue(StatDefOf.RangedWeapon_Cooldown, true);

            float fullCycleTime = (warmup * aimFactor)
                    + cooldown
                    + ((shootVerb.burstShotCount - 1) * shootVerb.ticksBetweenBurstShots).TicksToSeconds();

            if (turret != null) {
                fullCycleTime = Math.Max(fullCycleTime, 10.TicksToSeconds()); // Turrets attempt to fire at most once every 10 ticks no matter what
            }
            int totalDamage = shootVerb.burstShotCount * damage;

            return totalDamage / fullCycleTime;
        }

        /// <summary>
        /// Gets the adjusted hit chance factor of a shot.  This is equivalent to shootVerb.GetHitChanceFactor() unless
        /// a shooter is provided, in which case it will also be adjusted based on the shooter's hit chance.
        /// 
        /// This value can be greater than 1.0 in the case of weapons with overcapped accuracy.
        /// </summary>
        /// <returns>The adjusted hit chance factor.</returns>
        /// <param name="range">The range of the shot.</param>
        /// <param name="shootVerb">The shoot verb used to shoot.</param>
        /// <param name="weapon">The weapon used to shoot.</param>
        /// <param name="shooter">(Optional) The turret or pawn shooting the weapon.</param>
        public static float GetAdjustedHitChanceFactor(float range, VerbProperties shootVerb, Thing weapon, Thing shooter = null)
        {
            float hitChance = shootVerb.GetHitChanceFactor(weapon, range);
            if (shooter != null)
            {
                hitChance *= ShotReport.HitFactorFromShooter(shooter, range);
            }

            return hitChance;
        }

        /// <summary>
        /// Calculates and returns the optimal range of the given weapon (the range at which accuracy is highest).  If
        /// shooter is provided, the calculation correctly accounts for the shooter's accuracy as well as that of the
        /// weapon.
        /// </summary>
        /// <returns>
        /// The range, in cells, at which this weapon performs best (for the <paramref name="shooter"/> if provided, or
        /// in general if not).
        /// </returns>
        /// <param name="shootVerb">The shoot verb used to shoot.</param>
        /// <param name="weapon">The weapon used to shoot.</param>
        /// <param name="shooter">(Optional) The turret or pawn shooting the weapon.</param>
        public static float FindOptimalRange(VerbProperties shootVerb, Thing weapon, Thing shooter = null)
        {
            int minRange = (int) Math.Max(1.0, Math.Ceiling(shootVerb.minRange));
            int maxRange = (int) Math.Floor(shootVerb.range);
            return Enumerable.Range(minRange, maxRange).MaxBy(r => GetAdjustedHitChanceFactor(r, shootVerb, weapon, shooter));
        }

        // Copied from Building
        public static float TurretBurstCooldown(Building_TurretGun turret)
        {
            if (turret.def.building.turretBurstCooldownTime >= 0f)
            {
                return turret.def.building.turretBurstCooldownTime;
            }
            return turret.AttackVerb.verbProps.defaultCooldownTime;
        }
    }
}
