using Content.Client.Overlays;
using Content.Shared.Damage;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Overlays;
using Content.Shared.Inventory.Events;
using Content.Shared.Vanilla.Overlays;
using Robust.Shared.Prototypes;
using System.Linq;

namespace Content.Client.Vanilla.Overlays;

//Rayten-start
public sealed class ShowDominantDamageGroupIconSystem : EquipmentHudSystem<ShowDominantDamageGroupIconComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    [ViewVariables]
    private readonly HashSet<string> _damageContainers = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShowDominantDamageGroupIconComponent, AfterAutoHandleStateEvent>(OnHandleState);
    }

    protected override void UpdateInternal(RefreshEquipmentHudEvent<ShowDominantDamageGroupIconComponent> component)
    {
        base.UpdateInternal(component);

        foreach (var damageContainer in component.Components.SelectMany(x => x.DamageContainers))
        {
            _damageContainers.Add(damageContainer);
        }
    }

    protected override void DeactivateInternal()
    {
        base.DeactivateInternal();
        _damageContainers.Clear();
    }

    private void OnHandleState(Entity<ShowDominantDamageGroupIconComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        RefreshOverlay();
    }

    private void OnGetStatusIconsEvent(Entity<DamageableComponent> entity, ref GetStatusIconsEvent args)
    {
        if (!IsActive)
            return;

        var icons = GetDamageIcons(entity.Comp);
        args.StatusIcons.AddRange(icons);
    }

    public IReadOnlyList<DamageIconPrototype> GetDamageIcons(DamageableComponent damageable)
    {
        var icons = new List<DamageIconPrototype>();

        if (damageable.DamageContainerID == null || !_damageContainers.Contains(damageable.DamageContainerID))
            return icons;

        var dominant = damageable.DamagePerGroup
            .Where(kvp => kvp.Value > FixedPoint2.Zero)
            .OrderByDescending(kvp => kvp.Value)
            .FirstOrDefault();

        if (!dominant.Equals(default(KeyValuePair<string, FixedPoint2>)))
        {
            var groupId = dominant.Key;
            if (_prototype.TryIndex<DamageIconPrototype>(groupId, out var icon))
                icons.Add(icon);
        }

        if (damageable.Bleeding)
        {
            if (_prototype.TryIndex<DamageIconPrototype>("DamageIconBleeding", out var bleedIcon))
                icons.Add(bleedIcon);
        }

        return icons;
    }

    public void AddDominantDamageIcons(Entity<DamageableComponent> entity, ref GetStatusIconsEvent args)
    {
        if (!IsActive)
            return;

        var icons = GetDamageIcons(entity.Comp);
        args.StatusIcons.AddRange(icons);
    }
}
//Rayten-end