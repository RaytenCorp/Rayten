using Content.Shared._Rayten.TapePlayer;
using Content.Shared.Containers.ItemSlots;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Player;

namespace Content.Server._Rayten.TapePlayer;

public sealed class TapePlayerSystem : SharedTapePlayerSystem
{
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;

    private readonly List<ICommonSession> _ignoredRecipients = [];

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TapePlayerComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<TapePlayerComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<TapePlayerComponent, EntInsertedIntoContainerMessage>(OnInsertedItem);
        SubscribeLocalEvent<TapePlayerComponent, EntRemovedFromContainerMessage>(OnRemovedItem);

        SubscribeLocalEvent<TapePlayerComponent, TapePlayerPlayingMessage>(OnPlay);
        SubscribeLocalEvent<TapePlayerComponent, TapePlayerPauseMessage>(OnPause);
        SubscribeLocalEvent<TapePlayerComponent, TapePlayerStopMessage>(OnStop);
        SubscribeLocalEvent<TapePlayerComponent, TapePlayerSetTimeMessage>(OnSetTime);
        SubscribeLocalEvent<TapePlayerComponent, TapePlayerSetVolumeMessage>(OnSetVolume);

        SubscribeNetworkEvent<ClientOptionTapePlayerEvent>(OnOptionTapePlayer);
    }
    private async void OnOptionTapePlayer(ClientOptionTapePlayerEvent ev, EntitySessionEventArgs args)
    {
        if (ev.Enabled)
            _ignoredRecipients.Remove(args.SenderSession);
        else
            _ignoredRecipients.Add(args.SenderSession);
    }
    private void OnInsertedItem(EntityUid uid, TapePlayerComponent component, EntInsertedIntoContainerMessage args)
    {
        component.InsertedTape = args.Entity;
        Dirty(uid, component);
    }
    private void OnRemovedItem(EntityUid uid, TapePlayerComponent component, EntRemovedFromContainerMessage args)
    {
        _audioSystem.Stop(component.AudioStream);
        component.AudioStream = null;
        component.InsertedTape = null;
        Dirty(uid, component);
    }
    private void OnMapInit(EntityUid uid, TapePlayerComponent component, MapInitEvent args)
    {
        _itemSlotsSystem.AddItemSlot(uid, TapePlayerComponent.TapeSlotId, component.TapeSlot);
    }
    private void OnPlay(EntityUid uid, TapePlayerComponent component, ref TapePlayerPlayingMessage args)
    {
        _audioSystem.PlayPvs(component.ButtonSound, uid);
        if (Exists(component.AudioStream))
        {
            Audio.SetState(component.AudioStream, AudioState.Playing);
        }
        else
        {
            component.AudioStream = Audio.Stop(component.AudioStream);

            if (!TryComp<MusicTapeComponent>(component.TapeSlot.Item, out var musicTapeComponent))
            {
                return;
            }

            var volume = SharedAudioSystem.GainToVolume(component.Volume) + component.IncreaceVolume;

            var audioParams = AudioParams.Default
                .WithVolume(volume)
                .WithMaxDistance(component.MaxDistance)
                .WithRolloffFactor(component.RolloffFactor)
                .WithLoop(component.Loop);
            var filter = Filter.Pvs(uid).RemovePlayers(_ignoredRecipients);
            var audio = Audio.PlayEntity(
                musicTapeComponent.Sound,
                filter,
                uid,
                false,
                audioParams);
            if (audio != null)
                component.AudioStream = audio.Value.Entity;
            Dirty(uid, component);
        }
    }
    private void OnPause(Entity<TapePlayerComponent> ent, ref TapePlayerPauseMessage args)
    {
        _audioSystem.PlayPvs(ent.Comp.ButtonSound, ent.Owner);
        Audio.SetState(ent.Comp.AudioStream, AudioState.Paused);
    }
    private void OnSetTime(EntityUid uid, TapePlayerComponent component, TapePlayerSetTimeMessage args)
    {
        if (TryComp(args.Actor, out ActorComponent? actorComp))
        {
            var offset = actorComp.PlayerSession.Channel.Ping * 1.5f / 1000f;
            Audio.SetPlaybackPosition(component.AudioStream, args.SongTime + offset);
        }
    }
    private void OnSetVolume(EntityUid uid, TapePlayerComponent component, TapePlayerSetVolumeMessage args)
    {
        component.Volume = args.Volume;
        var volume = SharedAudioSystem.GainToVolume(component.Volume) + component.IncreaceVolume;
        Audio.SetVolume(component.AudioStream, volume);
        Dirty(uid, component);
    }
    private void OnStop(Entity<TapePlayerComponent> ent, ref TapePlayerStopMessage args)
    {
        _audioSystem.PlayPvs(ent.Comp.ButtonSound, ent.Owner);
        Stop(ent);
    }
    private void Stop(Entity<TapePlayerComponent> entity)
    {
        Audio.SetState(entity.Comp.AudioStream, AudioState.Stopped);
        Dirty(entity);
    }
    private void OnShutdown(EntityUid uid, TapePlayerComponent component, ComponentShutdown args)
    {
        component.AudioStream = Audio.Stop(component.AudioStream);
    }
}
