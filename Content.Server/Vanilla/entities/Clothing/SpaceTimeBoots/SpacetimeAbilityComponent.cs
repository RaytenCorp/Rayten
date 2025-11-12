using Content.Shared.Damage;
using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;
using Robust.Shared.Map;
using System.Numerics;

namespace Content.Server.Vanilla.Entities.SpacetimeBoots;

[RegisterComponent]
public sealed partial class SpacetimeAbilityComponent : Component
{
    /// <summary>
    /// тот кто носит ботиночки
    /// </summary>
    [DataField]
    public EntityUid? Wearer;

    /// <summary>
    /// Прототип акшена позволяющий перемещаться во времени
    /// </summary>
    [DataField]
    public EntProtoId Action = "ActionSpacetimeJump";

    [DataField]
    public EntityUid? ActionEntity;


    [DataField]
    public SoundSpecifier? JumpSound;

    // Сколько секунд назад "отматываем"
    [DataField]
    public float HistoryLength = 5f;


    // Интервал между сэмплами (секунды)
    [DataField]
    public float SampleRate = 0.5f;

    // История состояния (позиция, здоровье)
    public readonly Queue<(Vector2 Coords, DamageSpecifier Damage, float? BloodAmount, float? BleedAmount, EntityUid SavedEnt)> History = new();

    // Таймер между сэмплами
    [DataField]
    public TimeSpan LastSampleTime = TimeSpan.Zero;

    // Максимум записей ~11-10
    public int MaxSamples => (int)(HistoryLength / SampleRate);
}