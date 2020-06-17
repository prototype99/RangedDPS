using RimWorld;
using Verse;

namespace RangedDPS
{

    public class StatWorker_MaxTurretRangedDPS : StatWorker_TurretRangedDPSBase
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return 0f;
            }

            var weaponDef = thingDef?.building?.turretGunDef;
            if (weaponDef == null)
            {
                return 0f;
            }

            var shootVerb = GetShootVerb(weaponDef);
            if (shootVerb == null)
            {
                return 0f;
            }

            return GetTurretRawDPS(shootVerb, weaponDef);
        }

    }
}
