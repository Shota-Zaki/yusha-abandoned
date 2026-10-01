using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Yusha.Combat;

public enum BattleMode { Pve, Pvp }
public enum Outcome { Active, EnemyDefeated, PlayerWiped }
public sealed record Fighter(string Id, int MaxHp, int MaxMp, int Attack, int Defense,
    int HitBp, int DodgeBp, int CriticalBp, int IntervalMs);
public sealed record Actor(Fighter Build, int Hp, int Mp, long NextAttackMs);
public sealed record Counters(ulong Hit = 0, ulong Critical = 0, ulong Variance = 0);
public sealed record Snapshot(string CombatVersion, BattleMode Mode, string ProfileId,
    Guid ExpeditionId, uint RunIndex, uint RoomIndex, long SimMs, long EventSequence,
    Actor Player, Actor Enemy, long NextMpMs, Counters Random, Outcome Outcome,
    long? WipedAtMs = null);
public sealed record BalanceProfile(string Id, BattleMode Mode, int NormalMultiplierBp = 10000);
public sealed record CombatEvent(long Sequence, long SimMs, string Kind, string ActorId,
    string? TargetId = null, int Amount = 0);
public sealed record AdvanceResult(Snapshot Snapshot, IReadOnlyList<CombatEvent> Events);

public static class Duel
{
    public const string Version = "ya-duel-v1";

    public static int QuantizeInterval(int milliseconds) =>
        checked((Math.Max(800, milliseconds) + 99) / 100 * 100);

    public static int Damage(int attack, int multiplierBp, int defense, int varianceBp, bool critical)
    {
        Range(attack, 0, 100000); Range(defense, 0, 100000);
        Range(multiplierBp, 0, 100000); Range(varianceBp, 9500, 10500);
        checked
        {
            long value = (long)attack * multiplierBp / 10000;
            value = value * 1000 / (1000 + defense);
            value = value * varianceBp / 10000;
            if (critical) value = value * 15000 / 10000;
            return (int)Math.Clamp(value, 1, 2000000000);
        }
    }

    public static Snapshot Create(Fighter player, Fighter enemy, BalanceProfile profile,
        Guid expeditionId, uint runIndex = 0, uint roomIndex = 0)
    {
        Validate(player); Validate(enemy); Validate(profile);
        if (player.Id == enemy.Id || expeditionId == Guid.Empty) throw new ArgumentException("Invalid identities");
        return new(Version, profile.Mode, profile.Id, expeditionId, runIndex, roomIndex, 0, 0,
            new(player, player.MaxHp, player.MaxMp, QuantizeInterval(player.IntervalMs)),
            new(enemy, enemy.MaxHp, enemy.MaxMp, QuantizeInterval(enemy.IntervalMs)),
            5000, new(), Outcome.Active);
    }

    public static AdvanceResult Advance(Snapshot input, long untilSimMs, BalanceProfile profile,
        ReadOnlySpan<byte> secretKey)
    {
        Validate(input.Player.Build); Validate(input.Enemy.Build); Validate(profile);
        if (secretKey.Length != 32) throw new ArgumentException("Expected 32-byte simulation key");
        if (input.CombatVersion != Version || input.Mode != profile.Mode || input.ProfileId != profile.Id)
            throw new ArgumentException("Combat version or balance profile mismatch");
        if (input.ExpeditionId == Guid.Empty || input.Player.Build.Id == input.Enemy.Build.Id ||
            input.SimMs < 0 || input.SimMs % 100 != 0 || input.EventSequence < 0 ||
            untilSimMs < input.SimMs || untilSimMs % 100 != 0 || !Enum.IsDefined(input.Outcome))
            throw new ArgumentException("Invalid snapshot or simulation time");
        Validate(input.Player); Validate(input.Enemy);
        if (input.Outcome == Outcome.Active &&
            (input.Player.Hp == 0 || input.Enemy.Hp == 0 || input.WipedAtMs is not null ||
             input.Player.NextAttackMs <= input.SimMs || input.Enemy.NextAttackMs <= input.SimMs ||
             input.NextMpMs <= input.SimMs || input.NextMpMs % 5000 != 0))
            throw new ArgumentException("Invalid active schedules");
        if (input.Outcome == Outcome.PlayerWiped &&
            (input.Player.Hp != 0 || input.WipedAtMs is null || input.WipedAtMs < 0 || input.WipedAtMs > input.SimMs))
            throw new ArgumentException("Invalid wipe snapshot");
        if (input.Outcome == Outcome.EnemyDefeated &&
            (input.Enemy.Hp != 0 || input.Player.Hp == 0 || input.WipedAtMs is not null))
            throw new ArgumentException("Invalid victory snapshot");
        var state = input;
        var events = new List<CombatEvent>();
        void Emit(long time, string kind, string actor, string? target = null, int amount = 0)
        {
            state = state with { EventSequence = checked(state.EventSequence + 1) };
            events.Add(new(state.EventSequence, time, kind, actor, target, amount));
        }
        while (state.Outcome == Outcome.Active)
        {
            long at = Math.Min(state.NextMpMs, Math.Min(state.Player.NextAttackMs, state.Enemy.NextAttackMs));
            if (at > untilSimMs) break;
            bool playerFirst = string.CompareOrdinal(state.Player.Build.Id, state.Enemy.Build.Id) < 0;
            var order = playerFirst ? new[] { true, false } : new[] { false, true };
            if (state.NextMpMs == at)
            {
                foreach (bool isPlayer in order)
                {
                    Actor actor = isPlayer ? state.Player : state.Enemy;
                    int restored = Math.Min(actor.Build.MaxMp - actor.Mp, Math.Max(1, actor.Build.MaxMp * 50 / 10000));
                    actor = actor with { Mp = actor.Mp + restored };
                    state = isPlayer ? state with { Player = actor } : state with { Enemy = actor };
                    if (restored > 0) Emit(at, "MpRestored", actor.Build.Id, amount: restored);
                }
                state = state with { NextMpMs = checked(at + 5000) };
            }
            // KO is deferred until all completions in this group resolve.
            foreach (bool isPlayer in order)
            {
                Actor actor = isPlayer ? state.Player : state.Enemy;
                if (actor.NextAttackMs != at) continue;
                Actor target = isPlayer ? state.Enemy : state.Player;
                var counters = state.Random;
                ulong hitCounter = counters.Hit;
                bool hit = Draw(secretKey, state, "hit", ref hitCounter, 0, 9999) < actor.Build.HitBp;
                if (hit) hit = Draw(secretKey, state, "hit", ref hitCounter, 0, 9999) >= target.Build.DodgeBp;
                ulong criticalCounter = counters.Critical, varianceCounter = counters.Variance;
                int amount = 0;
                if (hit)
                {
                    bool critical = Draw(secretKey, state, "crit", ref criticalCounter, 0, 9999) < actor.Build.CriticalBp;
                    int variance = Draw(secretKey, state, "variance", ref varianceCounter, 9500, 10500);
                    amount = Math.Min(target.Hp, Damage(actor.Build.Attack, profile.NormalMultiplierBp, target.Build.Defense, variance, critical));
                    target = target with { Hp = target.Hp - amount };
                }
                actor = actor with { NextAttackMs = checked(at + QuantizeInterval(actor.Build.IntervalMs)) };
                state = isPlayer ? state with { Player = actor, Enemy = target } : state with { Player = target, Enemy = actor };
                state = state with { Random = new(hitCounter, criticalCounter, varianceCounter) };
                Emit(at, hit ? "Damage" : "Miss", actor.Build.Id, target.Build.Id, amount);
            }
            foreach (bool isPlayer in order)
            {
                Actor actor = isPlayer ? state.Player : state.Enemy;
                if (actor.Hp == 0) Emit(at, "Ko", actor.Build.Id);
            }
            if (state.Player.Hp == 0) state = state with { Outcome = Outcome.PlayerWiped, WipedAtMs = at };
            else if (state.Enemy.Hp == 0) state = state with { Outcome = Outcome.EnemyDefeated };
            if (state.Outcome != Outcome.Active) Emit(at, state.Outcome.ToString(), state.Player.Build.Id);
            state = state with { SimMs = at };
        }
        return new(state with { SimMs = untilSimMs }, events.AsReadOnly());
    }

    public static long PveCooldownUntil(Snapshot state, long officialStartMs)
    {
        if (state.Mode != BattleMode.Pve || state.Outcome != Outcome.PlayerWiped || state.WipedAtMs is null || officialStartMs < 0)
            throw new ArgumentException("Expected official PvE wipe");
        return checked(officialStartMs + state.WipedAtMs.Value + 300000);
    }

    public static int Draw(ReadOnlySpan<byte> key, Snapshot state, string stream, ref ulong counter, int min, int max)
    {
        if (key.Length != 32 || min < 0 || max < min ||
            stream is not ("hit" or "crit" or "variance" or "drop" or "gear" or "pity"))
            throw new ArgumentException("Invalid random contract");
        ulong n = (ulong)((long)max - min + 1);
        // 2^64 mod n, without representing 2^64 in a ulong.
        ulong rejectedTail = (ulong.MaxValue % n + 1) % n;
        while (true)
        {
            string message = FormattableString.Invariant($"v1|{stream}|{state.ExpeditionId:D}|{state.RunIndex}|{state.RoomIndex}|{counter}");
            counter = checked(counter + 1);
            byte[] digest = HMACSHA256.HashData(key, Encoding.ASCII.GetBytes(message));
            ulong value = BinaryPrimitives.ReadUInt64BigEndian(digest);
            if (rejectedTail == 0 || value <= ulong.MaxValue - rejectedTail)
                return checked(min + (int)(value % n));
        }
    }

    private static void Validate(Actor actor)
    {
        Range(actor.Hp, 0, actor.Build.MaxHp); Range(actor.Mp, 0, actor.Build.MaxMp);
        if (actor.NextAttackMs < 0 || actor.NextAttackMs % 100 != 0) throw new ArgumentException("Invalid attack time");
    }
    private static void Validate(Fighter fighter)
    {
        if (string.IsNullOrWhiteSpace(fighter.Id)) throw new ArgumentException("Missing entity ID");
        Range(fighter.MaxHp, 1, 2000000000); Range(fighter.MaxMp, 0, 100000);
        Range(fighter.Attack, 0, 100000); Range(fighter.Defense, 0, 100000);
        Range(fighter.HitBp, 0, 10000); Range(fighter.DodgeBp, 0, 10000); Range(fighter.CriticalBp, 0, 10000);
        Range(fighter.IntervalMs, 0, 1000000);
    }
    private static void Validate(BalanceProfile profile)
    {
        if (!Enum.IsDefined(profile.Mode) || string.IsNullOrWhiteSpace(profile.Id)) throw new ArgumentException("Invalid profile");
        Range(profile.NormalMultiplierBp, 0, 100000);
    }
    private static void Range(int value, int min, int max)
    {
        if (value < min || value > max) throw new ArgumentOutOfRangeException(nameof(value));
    }
}
