using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class MothAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!; // Corvax-Localization

    private static readonly Regex RegexLowerBuzz = new Regex("z{1,3}", RegexOptions.Compiled);
    private static readonly Regex RegexUpperBuzz = new Regex("Z{1,3}", RegexOptions.Compiled);

    // Corvax-Localization Regex
    private static readonly Regex RegexRuSmallZh = new Regex("ж+", RegexOptions.Compiled);
    private static readonly Regex RegexRuCapitalZh = new Regex("Ж+", RegexOptions.Compiled);
    private static readonly Regex RegexRuSmallZ = new Regex("з+", RegexOptions.Compiled);
    private static readonly Regex RegexRuCapitalZ = new Regex("З+", RegexOptions.Compiled);

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MothAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, MothAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // buzzz
        message = RegexLowerBuzz.Replace(message, "zzz");
        // buZZZ
        message = RegexUpperBuzz.Replace(message, "ZZZ");

        // Corvax-Localization-Start
        // ж => жж / жжж
        message = RegexRuSmallZh.Replace(message, _random.Pick(new List<string>() { "жж", "жжж" }));
        // Ж => ЖЖ / ЖЖЖ
        message = RegexRuCapitalZh.Replace(message, _random.Pick(new List<string>() { "ЖЖ", "ЖЖЖ" }));
        // з => зз / ззз
        message = RegexRuSmallZ.Replace(message, _random.Pick(new List<string>() { "зз", "ззз" }));
        // З => ЗЗ / ЗЗЗ
        message = RegexRuCapitalZ.Replace(message, _random.Pick(new List<string>() { "ЗЗ", "ЗЗЗ" }));
        // Corvax-Localization-End

        args.Message = message;
    }
}
