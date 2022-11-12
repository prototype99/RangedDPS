using RimWorld;

namespace RangedDPS;

public class StatWorker_TurretMaxDamagePerResource : StatWorker_TurretDPSBase
{
    public override bool ShouldShowFor(StatRequest req)
    {
        return base.ShouldShowFor(req) &&
               // Don't show resource usage for turrets without fuel
               GetTurretStats(req).NeedsFuel;
    }

    public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
    {
        if (!ShouldShowFor(req))
        {
            return 0f;
        }

        var turretStats = GetTurretStats(req);
        return turretStats.DamagePerFuel;
    }
}