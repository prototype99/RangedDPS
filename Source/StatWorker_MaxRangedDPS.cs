using RimWorld;
using Verse;

namespace RangedDPS
{
    public class StatWorker_MaxRangedDPS : StatWorker_RangedDPSBase
    {
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            Thing thing = req.Thing ?? (req.Def as ThingDef).GetConcreteExample();
            return GetRawDPS(thing);
        }

    }
}
