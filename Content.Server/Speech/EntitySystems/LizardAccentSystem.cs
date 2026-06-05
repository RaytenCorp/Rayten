using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed partial class LizardAccentSystem : EntitySystem
{
    private static readonly Regex RegexLowerS = new("s+", RegexOptions.Compiled);
    private static readonly Regex RegexUpperS = new("S+", RegexOptions.Compiled);
    private static readonly Regex RegexInternalX = new(@"(\w)x", RegexOptions.Compiled);
    private static readonly Regex RegexLowerEndX = new(@"\bx([\-|r|R]|\b)", RegexOptions.Compiled);
    private static readonly Regex RegexUpperEndX = new(@"\bX([\-|r|R]|\b)", RegexOptions.Compiled);

    // Corvax-Localization Regex
    private static readonly Regex RegexRuSmallC = new(@"с+", RegexOptions.Compiled);
    private static readonly Regex RegexRuCapitalC = new(@"С+", RegexOptions.Compiled);
    private static readonly Regex RegexRuSmallZ = new(@"з+", RegexOptions.Compiled);
    private static readonly Regex RegexRuCapitalZ = new(@"З+", RegexOptions.Compiled);
    private static readonly Regex RegexRuSmallSh = new(@"ш+", RegexOptions.Compiled);
    private static readonly Regex RegexRuCapitalSh = new(@"Ш+", RegexOptions.Compiled);
    private static readonly Regex RegexRuSmallCh = new(@"ч+", RegexOptions.Compiled);
    private static readonly Regex RegexRuCapitalCh = new(@"Ч+", RegexOptions.Compiled);

    [Dependency] private IRobustRandom _random = default!; // Corvax-Localization

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LizardAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, LizardAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // hissss
        message = RegexLowerS.Replace(message, "sss");
        // hiSSS
        message = RegexUpperS.Replace(message, "SSS");
        // ekssit
        message = RegexInternalX.Replace(message, "$1kss");
        // ecks
        message = RegexLowerEndX.Replace(message, "ecks$1");
        // eckS
        message = RegexUpperEndX.Replace(message, "ECKS$1");

        // Corvax-Localization-Start
        // с => сс / ссс
        message = RegexRuSmallC.Replace(message, _random.Pick(new List<string>() { "сс", "ссс" }));
        // С => CC / CCC
        message = RegexRuCapitalC.Replace(message, _random.Pick(new List<string>() { "СС", "ССС" }));
        // з => сс / ссс
        message = RegexRuSmallZ.Replace(message, _random.Pick(new List<string>() { "сс", "ссс" }));
        // З => CC / CCC
        message = RegexRuCapitalZ.Replace(message, _random.Pick(new List<string>() { "СС", "ССС" }));
        // ш => шш / шшш
        message = RegexRuSmallSh.Replace(message, _random.Pick(new List<string>() { "шш", "шшш" }));
        // Ш => ШШ / ШШШ
        message = RegexRuCapitalSh.Replace(message, _random.Pick(new List<string>() { "ШШ", "ШШШ" }));
        // ч => щщ / щщщ
        message = RegexRuSmallCh.Replace(message, _random.Pick(new List<string>() { "щщ", "щщщ" }));
        // Ч => ЩЩ / ЩЩЩ
        message = RegexRuCapitalCh.Replace(message, _random.Pick(new List<string>() { "ЩЩ", "ЩЩЩ" }));
        // Corvax-Localization-End

        args.Message = message;
    }
}
