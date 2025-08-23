using Content.Shared._Rayten.TapePlayer;
using Robust.Client.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;

namespace Content.Client._Rayten.TapePlayer;

public sealed class TapePlayerBoundUserInterface : BoundUserInterface
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [ViewVariables]
    private TapePlayerMenu? _menu;
    public TapePlayerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        IoCManager.InjectDependencies(this);
    }
    protected override void Open()
    {
        base.Open();
        _menu = new TapePlayerMenu();
        _menu.OnClose += Close;
        _menu.OpenCentered();
        _menu.OnPlayPressed += args =>
        {
            if (args)
            {
                SendMessage(new TapePlayerPlayingMessage());
            }
            else
            {
                SendMessage(new TapePlayerPauseMessage());
            }
        };

        _menu.OnStopPressed += () =>
        {
            SendMessage(new TapePlayerStopMessage());
        };

        _menu.SetCD += SetCD;
        _menu.SetVolume += SetVolume;
        Reload();
    }
    public void Reload()
    {
        if (_menu == null || !EntMan.TryGetComponent(Owner, out TapePlayerComponent? tapePlayer))
            return;

        _menu.SetAudio(tapePlayer.AudioStream);
        _menu.SetVolumeSlider(tapePlayer.Volume * 100f);

        if (_entityManager.TryGetComponent<MusicTapeComponent>(tapePlayer.InsertedTape, out var musicTapeComponent))
        {
            var audio = EntMan.System<AudioSystem>();
            var length = audio.GetAudioLength(audio.GetSound(musicTapeComponent.Sound));
            _menu.SetSelectedSong((float) length.TotalSeconds);
        }
        else
        {
            _menu.SetSelectedSong(0f);
        }
    }
    public void SetVolume(float volume)
    {
        SendMessage(new TapePlayerSetVolumeMessage(volume));
    }
    public void SetCD(float CD)
    {
        var sentCD = CD;
        if (EntMan.TryGetComponent(Owner, out TapePlayerComponent? tapePlayer) &&
            EntMan.TryGetComponent(tapePlayer.AudioStream, out AudioComponent? audioComp))
        {
            audioComp.PlaybackPosition = CD;
        }
        SendMessage(new TapePlayerSetTimeMessage(sentCD));
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

        if (_menu == null)
            return;

        _menu.OnClose -= Close;
        _menu.Dispose();
        _menu = null;
    }
}

