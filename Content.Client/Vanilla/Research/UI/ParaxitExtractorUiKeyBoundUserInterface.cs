using Content.Shared.Research;
using Content.Shared.Vanilla.Research;
using Content.Shared.Research.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;

namespace Content.Client.Vanilla.Research.UI
{
    public sealed class ParaxitExtractorUiKeyBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private ParaxitExtractorMenu? _menu;

        public ParaxitExtractorUiKeyBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();

            _menu = this.CreateWindow<ParaxitExtractorMenu>();

            _menu.OnServerButtonPressed += () =>
            {
                SendMessage(new ConsoleServerSelectionMessage());
            };
            _menu.OnExtractButtonPressed += () =>
            {
                SendMessage(new ParaxitExtractorExtractMessage());
            };
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);

            if (state is not ParaxitExtractorBoundUserInterfaceState msg)
                return;

            _menu?.Update(msg);
        }
    }
}