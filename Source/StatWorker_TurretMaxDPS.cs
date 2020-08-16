using RimWorld;
using Verse;

namespace RangedDPS
{

    public class StatWorker_MaxTurretRangedDPS : StatWorker_TurretRangedDPSBase
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            return GetRawDPS(GetTurretThing(req));
        }
    }
}
