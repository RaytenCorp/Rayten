using Content.Shared.Damage;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Vanilla.BloodSucker.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using System.Linq;

namespace Content.Shared.Vanilla.BloodSucker.Systems;

public sealed class BloodSuckerSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BloodSuckerComponent, ToggleHealingActionEvent>(ToggleHealing);
        SubscribeLocalEvent<BloodSuckerComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(EntityUid uid, BloodSuckerComponent component, MapInitEvent args)
    {
        _actions.AddAction(uid, ref component.HealingActionEntity, component.HealingAction);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var currentTime = _gameTiming.CurTime;
        //добавляем действие переключения ремонта

        //проходим по всем сущностям с компонентом кровосакера
        var query = EntityQueryEnumerator<BloodSuckerComponent>();
        while (query.MoveNext(out var uid, out var bloodSucker))
        {
            if (currentTime < bloodSucker.NextUpdate)
                continue;
            bloodSucker.NextUpdate = currentTime + bloodSucker.UpdateInterval;

            // Проверяем, проинициализированы ли Heal и BloodlessPenalty
            if (bloodSucker.Heal.DamageDict.Count == 0 || bloodSucker.BloodlessPenalty.DamageDict.Count == 0)
                continue;

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
            if (damageable.TotalDamage > 0 && bloodSucker.CanHeal == true)
            {
                var AmountToHeal = bloodSucker.Heal * bloodSucker.UnitsRestoreToHealPerInterval;
                _damageableSystem.TryChangeDamage(uid, AmountToHeal, ignoreResistances: true);

                bloodSucker.AmountOfBloodInStorage -= bloodSucker.UnitsRestoreToHealPerInterval;
                Dirty(uid, bloodSucker);
                return;
            }

            // Если повреждений нет, просто тратим кровь
            bloodSucker.AmountOfBloodInStorage -= bloodSucker.UnitsDecayPerInterval;
            Dirty(uid, bloodSucker);
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

                float UnitsToSuck = ((float)bloodReagent.Quantity < bloodSucker.UnitsPerInterval) ? (float)bloodReagent.Quantity : bloodSucker.UnitsPerInterval;
                
                UnitsToSuck = UnitsToSuck + bloodSucker.AmountOfBloodInStorage < bloodSucker.BloodStorage ? UnitsToSuck : bloodSucker.BloodStorage - bloodSucker.AmountOfBloodInStorage;

                var amountToRemove = FixedPoint2.New(UnitsToSuck);

                _solutionContainerSystem.RemoveReagent(bloodSolutionEnt.Value, bloodReagent.Reagent.Prototype, amountToRemove);

                bloodSucker.AmountOfBloodInStorage += UnitsToSuck;
                
                Dirty(uid, bloodSucker);
                return;
            }
        }
    }

    private void ToggleHealing(EntityUid uid, BloodSuckerComponent bloodSucker, ToggleHealingActionEvent args)
    {
        args.Handled = true;
        _actions.SetToggled(bloodSucker.HealingActionEntity, !bloodSucker.CanHeal);

        bloodSucker.CanHeal = !bloodSucker.CanHeal;
        Dirty(uid, bloodSucker);
    }

}
