using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_TurretRangedDPSBase : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            return req.Def is ThingDef thingDef && ThingDefIsShooty(thingDef?.building?.turretGunDef);
        }

        protected static Thing GetTurretThing(StatRequest req)
        {
            if (req.Thing is Building_TurretGun turret)
                return turret.gun;
            return (req.Def as ThingDef)?.building?.turretGunDef?.GetConcreteExample();
        }
    }
}
