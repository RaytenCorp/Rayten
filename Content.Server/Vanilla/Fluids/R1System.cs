using Content.Shared.Damage;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Shared.Vanilla.BloodSucker;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using System.Linq;

namespace Content.Server.Vanilla.BloodSucker;

public sealed class BloodSuckerSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SolutionContainerSystem _solutionContainerSystem = default!;


    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var currentTime = _gameTiming.CurTime;

        //проходим по всем сущностям с компонентом кровосакера
        var query = EntityQueryEnumerator<BloodSuckerComponent>();
        while (query.MoveNext(out var uid, out var bloodSucker))
        {
            if (currentTime < bloodSucker.NextUpdate)
                continue;
            bloodSucker.NextUpdate = currentTime + bloodSucker.UpdateInterval;

            Sucksomebloodfrompuddle(uid, bloodSucker);
            UseBloodInStorage(uid, bloodSucker);

        }
    }
    
    private void UseBloodInStorage(EntityUid uid, BloodSuckerComponent bloodSucker)
    {
        if (bloodSucker.AmountOfBloodInStorage > 0)
        {
            if (!TryComp<DamageableComponent>(uid, out var damageable))
                return;

            // Если у персонажа есть повреждения, начинаем лечить его
            if (damageable.TotalDamage > 0)
            {
                var AmountToHeal = bloodSucker.Heal * bloodSucker.UnitsRestoreToHealPerInterval;
                _damageableSystem.TryChangeDamage(uid, AmountToHeal, ignoreResistances: true);

                bloodSucker.AmountOfBloodInStorage -= bloodSucker.UnitsRestoreToHealPerInterval;

                return;
            }

            // Если повреждений нет, просто тратим кровь
            bloodSucker.AmountOfBloodInStorage -= bloodSucker.UnitsDecayPerInterval;
        }
        else
        {
            _damageableSystem.TryChangeDamage(uid, bloodSucker.BloodlessPenalty, ignoreResistances: true);
        }
    }



    /// <summary>
    /// метод сосет кровь из луж и заполняет хранилище кровью
    /// </summary>
    /// <param name="uid">сущность кровососущая обыкновенная</param>
    /// <param name="bloodSucker"> компонент кровососущности</param>
    private void Sucksomebloodfrompuddle(EntityUid uid, BloodSuckerComponent bloodSucker)
    {
        if (bloodSucker.AmountOfBloodInStorage >= bloodSucker.BloodStorage)
            return;

        var transform = Transform(uid);
        var entitiesInRange = _lookup.GetEntitiesInRange(transform.Coordinates, bloodSucker.Range);

        foreach (var entity in entitiesInRange)
        {
            foreach (var solutionName in bloodSucker.Solutions)
            {
                if (!_solutionContainerSystem.TryGetSolution(entity, solutionName, out var bloodSolutionEnt, out var bloodSolution))
                    continue;

                var bloodReagent = bloodSolution.Contents
                    .FirstOrDefault(rq => rq.Reagent.Prototype == "Blood");

                if (bloodReagent.Quantity <= FixedPoint2.Zero)
                    continue;

                var amountToRemove = FixedPoint2.New(bloodSucker.UnitsPerInterval);
                _solutionContainerSystem.RemoveReagent(bloodSolutionEnt.Value, bloodReagent.Reagent.Prototype, amountToRemove);

                bloodSucker.AmountOfBloodInStorage += bloodSucker.UnitsPerInterval;

                return;
            }
        }
    }

}
