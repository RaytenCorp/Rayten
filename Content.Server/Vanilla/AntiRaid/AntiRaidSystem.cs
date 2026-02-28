namespace Content.Server.Vanilla.AntiRaid;

using Content.Server.Players.PlayTimeTracking;
using Content.Server.Administration.Managers;
using Content.Server.GameTicking.Rules;
using Content.Server.Damage.Components;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Server.Database;

using Content.Shared.Movement.Components;
using Content.Shared.Vanilla.AntiRaid;
using Content.Shared.Damage.Systems;
using Content.Shared.GameTicking;
using Content.Shared.Damage;
using Content.Shared.Mind;

using Robust.Shared.Player;
using Robust.Shared.Timing;
public sealed class AntiRaidSystem : EntitySystem
{
    private double _minimum_time_to_be_trusted = 60;
    private int _max_warns_to_ban = 3;

    [Dependency] private readonly PlayTimeTrackingManager _playtime = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly IChatManager _chat = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    //[Dependency] private readonly IBanManager _ban = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PotentialRaiderComponent, TryDamageOnToolInteract>(TryDamageInteract);
        SubscribeLocalEvent<InputMoverComponent, DamageChangedEvent>(OnDamaged);
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawn);

    }

    // Выдаёт роль потенциального набегера при спавне новичка
    private async void OnPlayerSpawn(PlayerSpawnCompleteEvent args)
    {
        //var has_playtime = _playtime.TryGetTrackerTime(args.Player, "Overall", out var playtime);
        //if (has_playtime && playtime > _minimum_time_to_be_trusted)
            //return;

        var record = await _db.GetPlayerRecordByUserId(args.Player.UserId);
        if (record == null)
            return;
        var accountAge = (DateTime.UtcNow - record.FirstSeenTime).TotalMinutes;

        if (accountAge > _minimum_time_to_be_trusted)
            return;

        _chat.SendAdminAnnouncement($"АвтоГегло: {args.Player.Name} - помечен как потенциальный набегатор");

        var potentialRaiderComp = EnsureComp<PotentialRaiderComponent>(args.Mob);
        potentialRaiderComp.Session = args.Player;
        Timer.Spawn(TimeSpan.FromSeconds(10), () =>
        {
          var message = "Так как у вас недостаточно наигранного времени (1 час) вы попали в группу потенциальных набегаторов. Вам запрещено взрывать топливные баки и бить невиновных людей(если вам нанесли 10 урона или если это антагонист, вы имеет право на атаку), в ином же случае вы будете получать варны, при достижении 3 штук вы получите перманентный бан. При ошибочной блокировке обратитесь в дискорд сервер проекта.";
          _chat.DispatchServerMessage(args.Player, message);
        });
    }

    // Логика при атаке потенциального набегера или от него
    private void OnAttack(EntityUid attacker, EntityUid victim, float damage)
    {
        TryComp<PotentialRaiderComponent>(attacker, out var attackerPotentialRaiderComp);
        TryComp<PotentialRaiderComponent>(victim, out var victimPotentialRaiderComp);

        if (attackerPotentialRaiderComp != null)
        {
            // Если атакующий потенциальный набегер
            if (CheckAttackLegitimacy(attacker, attackerPotentialRaiderComp, victim))
                return;

            if (attackerPotentialRaiderComp.Session != null)
                _chat.SendAdminAnnouncement($"АвтоГегло: {attackerPotentialRaiderComp.Session.Name} - получил 1 варн за нападение на другого игрока. Всего варнов: {attackerPotentialRaiderComp.Warns}");
            AddWarn(attackerPotentialRaiderComp, 1);
        }

        if (victimPotentialRaiderComp != null)
        {
            // Если жертва потенциальный набегер
            if (victimPotentialRaiderComp.Attackers.ContainsKey(attacker))
                victimPotentialRaiderComp.Attackers[attacker] += damage;
            else
                victimPotentialRaiderComp.Attackers.Add(attacker, damage);
        }
    }

    // Проверяет легальность атаки ориентируясь на роли и нанесённый урон
    private bool CheckAttackLegitimacy(EntityUid attacker, PotentialRaiderComponent raiderComp, EntityUid victim)
    {
        // attacker = потенциальный набегатор
        // victim = чел которого он бьёт

        // Если набегер антаг то это легитимно
        if (TryComp(attacker, out ActorComponent? attackerActor) &&
            _mind.TryGetMind(attackerActor.PlayerSession, out var attackerMindId, out var attackerMind))
        {
            if (attackerMind == null || attackerMind.RoleType != "Neutral")
                return true;
        }
        else
            return true;

        // Если жертва антаг то это легитимно
        if (TryComp(victim, out ActorComponent? victimActor) &&
            _mind.TryGetMind(victimActor.PlayerSession, out var victimMindId, out var victimMind))
        {
            if (victimMind == null || victimMind.RoleType != "Neutral")
                return true;
        }
        else
            return true;

        // Если просто так ебашит то это незаконно
        if (!raiderComp.Attackers.ContainsKey(victim))
            return false;

        raiderComp.Attackers.TryGetValue(victim, out var damage);

        // Если жертва ебашила его до этого то это закнно
        if (damage > 10)
            return true;

        return false;
    }

    // Логика при попытке подрыва топливного бака
    private void TryDamageInteract(EntityUid uid, PotentialRaiderComponent raiderComp, TryDamageOnToolInteract args)
    {
        args.Cancel();
        AddWarn(raiderComp, 1);

        if (raiderComp.Session != null)
        {
            _chat.SendAdminAnnouncement($"АвтоГегло: {raiderComp.Session.Name} - получил 1 варн за попытку подрыва топливного бака. Всего варнов: {raiderComp.Warns}");
        }
    }

    // Даёт варны
    private void AddWarn(PotentialRaiderComponent raiderComp, int count)
    {
        if (raiderComp.Session != null)
        {
            var message = "Что делаем????? Вам варн.";
            _chat.DispatchServerMessage(raiderComp.Session, message);
        }

        raiderComp.Warns++;
        if (raiderComp.Warns >= _max_warns_to_ban)
        {
            // тут бан потом добавить
        }
    }

    private void OnDamaged(EntityUid target, InputMoverComponent comp, DamageChangedEvent args)
    {
        if (args.Origin == null || args.DamageDelta == null)
            return;
        var totalDamage = (float)args.DamageDelta.GetTotal();

        if (totalDamage <= 0)
            return;

        OnAttack(args.Origin.Value, target, totalDamage);
    }
}
