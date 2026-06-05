using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random; // Corvax-Localization
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed partial class FrontalLispSystem : EntitySystem
{
    // @formatter:off
    private static readonly Regex RegexUpperTh = new(@"[T]+[Ss]+|[S]+[Cc]+(?=[IiEeYy]+)|[C]+(?=[IiEeYy]+)|[P][Ss]+|([S]+[Tt]+|[T]+)(?=[Ii]+[Oo]+[Uu]*[Nn]*)|[C]+[Hh]+(?=[Ii]*[Ee]*)|[Z]+|[S]+|[X]+(?=[Ee]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexLowerTh = new(@"[t]+[s]+|[s]+[c]+(?=[iey]+)|[c]+(?=[iey]+)|[p][s]+|([s]+[t]+|[t]+)(?=[i]+[o]+[u]*[n]*)|[c]+[h]+(?=[i]*[e]*)|[z]+|[s]+|[x]+(?=[e]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexUpperEcks = new(@"[E]+[Xx]+[Cc]*|[X]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexLowerEcks = new(@"[e]+[x]+[c]*|[x]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    // @formatter:on

    // Corvax-Localization Regex
    private static readonly Regex RegexRuSmallEs = new(@"с", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexRuCapitalEs = new(@"С", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexRuSmallChe = new(@"ч", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexRuCapitalChe = new(@"Ч", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexRuSmallTse = new(@"ц", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexRuCapitalTse = new(@"Ц", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexRuSmallTe = new(@"\B[т](?![АЕЁИОУЫЭЮЯаеёиоуыэюя])", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexRuCapitalTe = new(@"\B[Т](?![АЕЁИОУЫЭЮЯаеёиоуыэюя])", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexRuSmallZe = new(@"з", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex RegexRuCapitalZe = new(@"З", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [Dependency] private IRobustRandom _random = default!; // Corvax-Localization

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FrontalLispComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, FrontalLispComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // handles ts, sc(i|e|y), c(i|e|y), ps, st(io(u|n)), ch(i|e), z, s
        message = RegexUpperTh.Replace(message, "TH");
        message = RegexLowerTh.Replace(message, "th");
        // handles ex(c), x
        message = RegexUpperEcks.Replace(message, "EKTH");
        message = RegexLowerEcks.Replace(message, "ekth");

        // Corvax-Localization Start
        // с - ш
        message = RegexRuSmallEs.Replace(message, _random.Prob(0.90f) ? "ш" : "с");
        message = RegexRuCapitalEs.Replace(message, _random.Prob(0.90f) ? "Ш" : "С");
        // ч - ш
        message = RegexRuSmallChe.Replace(message, _random.Prob(0.90f) ? "ш" : "ч");
        message = RegexRuCapitalChe.Replace(message, _random.Prob(0.90f) ? "Ш" : "Ч");
        // ц - ч
        message = RegexRuSmallTse.Replace(message, _random.Prob(0.90f) ? "ч" : "ц");
        message = RegexRuCapitalTse.Replace(message, _random.Prob(0.90f) ? "Ч" : "Ц");
        // т - ч
        message = RegexRuSmallTe.Replace(message, _random.Prob(0.90f) ? "ч" : "т");
        message = RegexRuCapitalTe.Replace(message, _random.Prob(0.90f) ? "Ч" : "Т");
        // з - ж
        message = RegexRuSmallZe.Replace(message, _random.Prob(0.90f) ? "ж" : "з");
        message = RegexRuCapitalZe.Replace(message, _random.Prob(0.90f) ? "Ж" : "З");
        // Corvax-Localization End

        args.Message = message;
    }
}
