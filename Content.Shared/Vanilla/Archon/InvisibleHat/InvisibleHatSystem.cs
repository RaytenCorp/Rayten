using Content.Shared.Stealth;
using Content.Shared.Stealth.Components;
using Content.Shared.Clothing;
using Content.Shared.Speech.Muting;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Interaction.Events;
using Robust.Shared.Timing;
namespace Content.Shared.Vanilla.Archon.InvisibleHat;

public sealed class InvisibleHatSystem : EntitySystem
{
    [Dependency] private readonly SharedStealthSystem _stealth = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<InvisibleHatComponent, ClothingGotEquippedEvent>(OnClothingEquip);
        SubscribeLocalEvent<InvisibleHatComponent, ClothingGotUnequippedEvent>(OnClothingUnequip);
        SubscribeLocalEvent<BlockInteractionComponent, InteractionAttemptEvent>(OnInteractAttempt);
    }

    private void OnInteractAttempt(Entity<BlockInteractionComponent> ent, ref InteractionAttemptEvent args)
    {
        args.Cancelled = true;
    }

    private void OnClothingEquip(Entity<InvisibleHatComponent> ent, ref ClothingGotEquippedEvent args)
    {
        var stealh = EnsureComp<StealthComponent>(args.Wearer);
        _stealth.SetVisibility(args.Wearer, -1f, stealh);

        if (!_timing.IsFirstTimePredicted)
            return;

        var pacified = EnsureComp<PacifiedComponent>(args.Wearer);
        pacified.DisallowDisarm = true;
        pacified.DisallowAllCombat  = true;
        EnsureComp<MutedComponent>(args.Wearer);
        EnsureComp<BlockInteractionComponent>(args.Wearer);
    }

    private void OnClothingUnequip(Entity<InvisibleHatComponent> ent, ref ClothingGotUnequippedEvent args)
    {
        if (TryComp<StealthComponent>(args.Wearer, out var stealh))
            _stealth.SetVisibility(args.Wearer, 1f, stealh);

        if (!_timing.IsFirstTimePredicted)
            return;

        RemComp<PacifiedComponent>(args.Wearer);
        RemComp<MutedComponent>(args.Wearer);
        RemComp<BlockInteractionComponent>(args.Wearer);
    }
}
