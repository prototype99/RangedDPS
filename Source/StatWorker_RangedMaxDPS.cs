using RimWorld;

namespace RangedDPS
{
    public class StatWorker_RangedMaxDPS : StatWorker_RangedDPSBase
    {
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!ShouldShowFor(req))
            {
                return 0f;
            }

            return DPSCalculator.GetRawRangedDPS(GetWeaponThing(req));
        }

    }
}
