# YA-01 Combat core v1

2026-10-01。DETAILED_DESIGNの1対1・通常攻撃部分を実装する。ライブラリはGodot、DB、実時間、ネットワークから独立する。.NET10は現在実行可能な検証ターゲット。Godotの採用Version/Windows描画・配布サイズ・RAMの受入は別途残す。

- Snapshotは不変record。Player/Enemyの固定build、HP/MP、次の攻撃/MP回復時刻、用途別乱数counter、イベント連番、結果/全滅時刻を保持。
- 初回の通常攻撃は開始時刻+攻撃間隔。各間隔は最短800ms適用後100ms切上げ。Advanceは100ms単位の非減少時刻を受け取る。
- 同時刻はMP回復→通常攻撃完了(entity IDのOrdinal順)→KO/終了。攻撃群開始時に双方生存なら、先のダメージでHP0になっても同時刻の相手の攻撃を解決する。相打ちはPlayerWipedを優先。
- 物理通常攻撃だけを初回範囲とする。攻撃・防御・命中/回避・会心は計算済み固定buildを入力。MPコスト0、5秒毎max(1,floor(maxMP*50/10000))回復。魔法武器/技能/詠唱/盾/状態はYA-03で追加。
- ダメージは設計の各段階で整数切捨て・checked64bit。ここでは無属性・貫通/与ダメ補正/軽減なし。式の単体関数で倍率を検証できる。
- BattleModeとbalance profile IDをsnapshot/contentで照合。PvE/PvP profileの暗黙流用を拒否。大会の補正値・運用は確定しない。
- 旧補足からHMAC-SHA256 v1契約を採用。32byte秘密鍵はサーバー側入力で保持しログに出さない。hit（命中/独立回避）、crit、varianceのcounterを別保持。ASCII message、big-endian先頭64bit、rejection samplingの契約は旧補足EDGE_CASESに従う。combat versionは`ya-duel-v1`。
- 全滅制限の終了は公式開始時刻+全滅sim時刻+300000ms。Advanceを呼んだ時刻を基準にしない。PvPにはこのPvE制限を発行しない。

固定seedの再現、一括/100ms分割の全イベント・counter・snapshot一致、375ダメージ、100ms境界、相打ち、KO後停止、MP上限、profile mismatch、不正入力/overflowを実行検証する。これはYA-01全体完了やクライアント互換検証を意味しない。

## クライアント検証候補

2026-10-01の公式archive確認に基づき、次のクライアント試作候補をGodot4.7.2-stable .NET / Compatibilityに固定する。これは採用確定や.NET10との実機互換PASSではない。4.7版C#文書の前提欄には旧4.5の要件記述が残るため、文書だけで互換成功を推定しない。C# editor/import/exportとWindows Releaseを実行して確認する。

- https://godotengine.org/download/archive/ （4.7.2-stable, 2026-08-18）
- https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_basics.html （.NET版editorと別途SDKが必要）

純粋コアの実測は既存Linux ARM64 SDK10.0.401/runtime10.0.12で実施。Windows RAM・描画・配布サイズの結果とは区別する。
