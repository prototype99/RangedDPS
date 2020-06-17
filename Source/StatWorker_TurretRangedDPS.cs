using System.Linq;
using RimWorld;
using Verse;

namespace RangedDPS
{

    public class StatWorker_TurretRangedDPS : StatWorker_RangedDPSBase
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!(req.Def is ThingDef thingDef))
            {
                return false;
            }

            return thingDef?.building?.IsTurret ?? false;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return 0f;
            }

            var weaponDef = thingDef?.building?.turretGunDef;

            var shootVerb = GetShootVerb(weaponDef);
            if (shootVerb == null)
            {
                return 0f;
            }

            float rawDps = GetTurretRawDPS(shootVerb, weaponDef);

            float bestAccuracy = new[] {
                weaponDef.GetStatValueAbstract(StatDefOf.AccuracyTouch),
                weaponDef.GetStatValueAbstract(StatDefOf.AccuracyShort),
                weaponDef.GetStatValueAbstract(StatDefOf.AccuracyMedium),
                weaponDef.GetStatValueAbstract(StatDefOf.AccuracyLong)
            }.Max();

            return rawDps * bestAccuracy;
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            ThingDef thingDef = req.Def as ThingDef;
            if (thingDef == null)
            {
                return "";
            }

            var weaponDef = GetWeaponDef(thingDef);
            var shootVerb = GetShootVerb(weaponDef);
            if (shootVerb == null)
            {
                return "";
            }

            float rawDps = GetRawDPS(shootVerb, weaponDef);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StatsReport_RangedDPSAccuracy".Translate());

            for (int i = 5; i <= shootVerb.range; i += 5)
            {
                float hitChance = shootVerb.GetHitChanceFactor(req.Thing, i);
                float damage = rawDps * hitChance;
                stringBuilder.AppendLine(string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    i, damage, hitChance));
            }

            // Max Range
            float maxRangeHitChance = shootVerb.GetHitChanceFactor(req.Thing, shootVerb.range);
            float maxRangeDamage = rawDps * maxRangeHitChance;
            stringBuilder.AppendLine(string.Format("{0} {1,2}: {2,5:F2} ({3:P1})",
                    "distance".Translate().CapitalizeFirst(),
                    shootVerb.range, maxRangeDamage, maxRangeHitChance));

            return stringBuilder.ToString();
        }

        protected float GetTurretRawDPS(VerbProperties shootVerb, ThingDef weaponDef)
        {
            float fullCycleTime = shootVerb.warmupTime + weaponDef.GetStatValueAbstract(StatDefOf.RangedWeapon_Cooldown)
                    + ((shootVerb.burstShotCount - 1) * shootVerb.ticksBetweenBurstShots).TicksToSeconds();

            int totalDamage = shootVerb.burstShotCount * shootVerb.defaultProjectile.projectile.GetDamageAmount_NewTmp(weaponDef, null);

            return totalDamage / fullCycleTime;
        }

    }
}
