using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_TurretRangedDPSBase : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            return req.Thing is Building_TurretGun turret && ThingDefIsShooty(turret?.gun?.def);
        }
    }
}
