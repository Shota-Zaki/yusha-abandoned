using Yusha.Combat;
using System.Globalization;
using System.Text.Json;

int count = 0;
byte[] key = new byte[32];
var profile = new BalanceProfile("pve-v1", BattleMode.Pve);
var id = Guid.Parse("11111111-1111-1111-1111-111111111111");
var player = new Fighter("a", 10000, 200, 300, 200, 9500, 100, 500, 811);
var enemy = player with { Id = "b", IntervalMs = 1000 };
Snapshot Fresh() => Duel.Create(player, enemy, profile, id);
void Equal<T>(T actual, T expected)
{
    if (!EqualityComparer<T>.Default.Equals(actual, expected)) throw new Exception($"Expected {expected}, got {actual}");
}
void Throws(Action action)
{
    try { action(); } catch (ArgumentException) { return; } catch (OverflowException) { return; }
    throw new Exception("Expected validation failure");
}
void Test(string name, Action action) { action(); count++; Console.WriteLine($"PASS {name}"); }
string Json(object value) => JsonSerializer.Serialize(value);
Test("reference damage 375 and per-stage floor", () => {
    Equal(Duel.Damage(300, 15000, 200, 10000, false), 375);
    Equal(Duel.Damage(3, 15000, 1000, 9500, true), 1);
    Equal(Duel.Damage(0, 10000, 100000, 10000, false), 1);
});
Test("interval minimum and ceiling", () => {
    Equal(Duel.QuantizeInterval(0), 800); Equal(Duel.QuantizeInterval(800), 800);
    Equal(Duel.QuantizeInterval(801), 900); Equal(Duel.QuantizeInterval(899), 900);
});
Test("PRNG golden vector and independent stream counter", () => {
    ulong counter = 0;
    Equal(Duel.Draw(key, Fresh(), "hit", ref counter, 0, 9999), 243);
    Equal(counter, 1UL);
    counter = 0; Equal(Duel.Draw(key, Fresh(), "crit", ref counter, 0, 9999), 4586);
    counter = 0; Equal(Duel.Draw(key, Fresh(), "variance", ref counter, 0, 9999), 672);
    counter = 0;
    var saved = CultureInfo.CurrentCulture;
    try { CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ar-SA");
        Equal(Duel.Draw(key, Fresh(), "hit", ref counter, 0, 9999), 243);
    } finally { CultureInfo.CurrentCulture = saved; }
});
Test("identical input produces identical output without mutation", () => {
    var input = Fresh(); var before = Json(input);
    Equal(Json(Duel.Advance(input, 10000, profile, key)), Json(Duel.Advance(input, 10000, profile, key)));
    Equal(Json(input), before);
});
Test("100ms partition equals batch events, random position and snapshot", () => {
    for (int seed = 0; seed < 20; seed++) {
        byte[] seeded = Enumerable.Repeat((byte)seed, 32).ToArray();
        var input = Fresh(); var incremental = input; var events = new List<CombatEvent>();
        for (int ms = 100; ms <= 100000; ms += 100) {
            var result = Duel.Advance(incremental, ms, profile, seeded);
            incremental = result.Snapshot; events.AddRange(result.Events);
        }
        var batch = Duel.Advance(input, 100000, profile, seeded);
        Equal(incremental, batch.Snapshot); Equal(Json(events), Json(batch.Events));
    }
});
Test("attack boundary and zero duration advance", () => {
    var initial = Fresh(); Equal(Duel.Advance(initial, 0, profile, key).Snapshot, initial);
    Equal(Duel.Advance(initial, 800, profile, key).Events.Count, 0);
    Equal(Duel.Advance(initial, 900, profile, key).Events.Count, 1);
});
Test("simultaneous lethal attacks both resolve, wipe wins, no later events", () => {
    var p = player with { MaxHp = 1, HitBp = 10000, DodgeBp = 0, IntervalMs = 800 };
    var initial = Duel.Create(p, p with { Id = "b" }, profile, id);
    var result = Duel.Advance(initial, 100000, profile, key);
    Equal(result.Snapshot.Player.Hp, 0); Equal(result.Snapshot.Enemy.Hp, 0);
    Equal(result.Snapshot.Outcome, Outcome.PlayerWiped); Equal(result.Snapshot.WipedAtMs, (long?)800);
    Equal(result.Events.Count, 5); Equal(result.Events[0].ActorId, "a"); Equal(result.Events[1].ActorId, "b");
    Equal(Duel.PveCooldownUntil(result.Snapshot, 1000000), 1300800L);
    Equal(Duel.Advance(result.Snapshot, 200000, profile, key).Events.Count, 0);
});
Test("earlier lethal attack cancels later action", () => {
    var p = player with { HitBp = 10000, DodgeBp = 0, IntervalMs = 800 };
    var e = enemy with { MaxHp = 1, DodgeBp = 0, IntervalMs = 900 };
    var result = Duel.Advance(Duel.Create(p, e, profile, id), 10000, profile, key);
    Equal(result.Snapshot.Outcome, Outcome.EnemyDefeated); Equal(result.Events.Count, 3);
    Equal(result.Snapshot.Player.Hp, p.MaxHp);
});
Test("MP pulse timing, floor and cap", () => {
    var initial = Fresh(); initial = initial with { Player = initial.Player with { Mp = 199 }, Enemy = initial.Enemy with { Mp = 0 } };
    var before = Duel.Advance(initial, 4900, profile, key); Equal(before.Snapshot.Player.Mp, 199);
    var after = Duel.Advance(before.Snapshot, 5000, profile, key);
    Equal(after.Snapshot.Player.Mp, 200); Equal(after.Snapshot.Enemy.Mp, 1);
    Equal(after.Events[0].Kind, "MpRestored");
});
Test("mode/profile mismatch and PvP cooldown refused", () => {
    Throws(() => Duel.Advance(Fresh(), 1000, profile with { Mode = BattleMode.Pvp }, key));
    Throws(() => Duel.Advance(Fresh(), 1000, profile with { Id = "other" }, key));
    Throws(() => Duel.PveCooldownUntil(Fresh() with { Mode = BattleMode.Pvp, Outcome = Outcome.PlayerWiped, WipedAtMs = 800 }, 0));
});
Test("input schedule, time, version and ranges rejected atomically", () => {
    var input = Fresh(); var before = Json(input);
    Throws(() => Duel.Advance(input, -100, profile, key));
    Throws(() => Duel.Advance(input, 999, profile, key));
    Throws(() => Duel.Advance(input with { SimMs = 900 }, 1000, profile, key));
    Throws(() => Duel.Advance(input with { CombatVersion = "unknown" }, 1000, profile, key));
    Throws(() => Duel.Advance(input with { Player = input.Player with { Hp = -1 } }, 1000, profile, key));
    Throws(() => Duel.Create(player with { Attack = 100001 }, enemy, profile, id));
    Throws(() => Duel.Advance(input, 1000, profile, new byte[31]));
    Throws(() => Duel.Advance(input with { EventSequence = long.MaxValue }, 1000, profile, key));
    Throws(() => Duel.Advance(input with { Random = new Counters(ulong.MaxValue) }, 1000, profile, key));
    Equal(Json(input), before);
});
Test("eight-hour progression equals one-minute checkpoints", () => {
    var p = player with { MaxHp = 1000000, Attack = 0, HitBp = 10000, DodgeBp = 0, CriticalBp = 0, IntervalMs = 800 };
    var initial = Duel.Create(p, p with { Id = "b" }, profile, id);
    var timer = System.Diagnostics.Stopwatch.StartNew();
    var batch = Duel.Advance(initial, 28800000, profile, key);
    var elapsed = timer.ElapsedMilliseconds;
    var current = initial; var events = new List<CombatEvent>();
    for (long time = 60000; time <= 28800000; time += 60000) {
        var part = Duel.Advance(current, time, profile, key);
        current = part.Snapshot; events.AddRange(part.Events);
    }
    Equal(current, batch.Snapshot); Equal(Json(events), Json(batch.Events));
    Equal(batch.Events.Count, 72000); Equal(current.Player.Hp, 964000);
    Console.WriteLine($"8h pure-core batch: {elapsed}ms, 72000 events (Linux ARM64, not client rendering)");
});
Test("misses do not advance critical/variance streams and reversed entity IDs are ordered", () => {
    var initial = Duel.Create(player with { Id = "z", HitBp = 0, IntervalMs = 800 }, enemy with { Id = "a", HitBp = 0, IntervalMs = 800 }, profile, id);
    var result = Duel.Advance(initial, 800, profile, key);
    Equal(result.Snapshot.Random, new Counters(2, 0, 0));
    Equal(result.Events[0].ActorId, "a"); Equal(result.Events[1].ActorId, "z");
});
Console.WriteLine($"{count} tests passed");
