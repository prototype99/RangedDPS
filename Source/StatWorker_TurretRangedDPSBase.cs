using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_TurretRangedDPSBase : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!(req.Def is ThingDef thingDef && ThingDefIsShooty(thingDef?.building?.turretGunDef)))
            {
                return false;
            }

            // Don't show DPS for unloaded mortars
            var comp = GetTurretThing(req).TryGetComp<CompChangeableProjectile>();
            if (comp != null)
            {
                return comp.Loaded;
            }

            return true;
        }

        protected static Thing GetTurretThing(StatRequest req)
        {
            if (req.Thing is Building_TurretGun turret)
            {
                return turret.gun;
            }
            return (req.Def as ThingDef)?.building?.turretGunDef?.GetConcreteExample();
        }
    }
}
