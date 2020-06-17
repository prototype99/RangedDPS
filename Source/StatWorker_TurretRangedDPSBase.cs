using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_TurretRangedDPSBase : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!(req.Def is ThingDef thingDef))
            {
                return false;
            }

            return thingDef?.building?.IsTurret ?? false;
        }

        protected float GetTurretRawDPS(VerbProperties shootVerb, ThingDef weaponDef)
        {
            float fullCycleTime = shootVerb.warmupTime + weaponDef.GetStatValueAbstract(StatDefOf.RangedWeapon_Cooldown)
                    + ((shootVerb.burstShotCount - 1) * shootVerb.ticksBetweenBurstShots).TicksToSeconds();

            int totalDamage = shootVerb.burstShotCount * shootVerb.defaultProjectile.projectile.GetDamageAmount_NewTmp(weaponDef, null);

            return totalDamage / fullCycleTime;
        }

    }
}
