using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._Rayten.RaytenCCVars;

public sealed partial class RaytenCCVars : CVars
{
    public static readonly CVarDef<bool> TapePlayerClientEnabled =
        CVarDef.Create("tape_player.client_enabled", true, CVar.CLIENTONLY | CVar.ARCHIVE);
}
