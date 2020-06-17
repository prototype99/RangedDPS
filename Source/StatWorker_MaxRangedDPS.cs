using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_MaxRangedDPS : StatWorker_RangedDPSBase
    {
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return 0f;
            }

            var shootVerb = GetShootVerb(thingDef);
            if (shootVerb == null)
            {
                return 0f;
            }

            return GetRawDPS(shootVerb, req.Thing);
        }

    }
}
