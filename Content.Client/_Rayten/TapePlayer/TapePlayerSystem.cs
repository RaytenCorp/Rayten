using Content.Shared._Rayten.RaytenCCVars;
using Content.Shared._Rayten.TapePlayer;
using Robust.Client.GameObjects;
using Robust.Shared.Configuration;

namespace Content.Client._Rayten.TapePlayer
{
    public sealed class TapePlayerSystem : SharedTapePlayerSystem
    {
        [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;
        [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
        [Dependency] private readonly IConfigurationManager _cfg = default!;

        public override void Initialize()
        {   base.Initialize();
            SubscribeLocalEvent<TapePlayerComponent, AfterAutoHandleStateEvent>(OnTapePlayerAfterState);
            _cfg.OnValueChanged(RaytenCCVars.TapePlayerClientEnabled, OnTapePlayerClientOptionChanged, true);
        }

        public override void Shutdown()
        {   base.Shutdown();
            _cfg.UnsubValueChanged(RaytenCCVars.TapePlayerClientEnabled, OnTapePlayerClientOptionChanged);
        }

        private void OnTapePlayerClientOptionChanged(bool option)
        {RaiseNetworkEvent(new ClientOptionTapePlayerEvent(option));}
        private void OnTapePlayerAfterState(Entity<TapePlayerComponent> ent, ref AfterAutoHandleStateEvent args)
        {   if (!_uiSystem.TryGetOpenUi<TapePlayerBoundUserInterface>(ent.Owner, TapePlayerUiKey.Key, out var bui))
                return;
            bui.Reload();
        }
    }
}
