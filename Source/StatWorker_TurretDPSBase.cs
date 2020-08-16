using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_TurretDPSBase : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!(req.Def is ThingDef thingDef && (thingDef?.building?.turretGunDef?.IsRangedWeapon ?? false)))
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
            Thing turretGun;
            if (req.Thing is Building_TurretGun turret)
            {
                turretGun = turret.gun;
            }
            else
            {
                turretGun = (req.Def as ThingDef)?.building?.turretGunDef?.GetConcreteExample();
            }

            if (turretGun == null) Log.Error($"[RangedDPS] Turret {req.Def.defName} has no turret gun defined");
            return turretGun;
        }
    }
}
