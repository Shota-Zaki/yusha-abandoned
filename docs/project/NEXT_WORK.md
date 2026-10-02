# 次のWork Unit

更新: 2026-10-01 / Revision 0.2 + Combat core v1

## 現在

YA-D04完了。ユーザー回答を要件・基本設計へ反映し、ONLINE_LIGHTWEIGHT/PVP_TOURNAMENTを追加した。600円買い切り、街40/通常4/レイド12、1活動キャラ/8時間、50職と育成、合成交換、定期PvP大会の存在は確定。

D01は完全P2Pか最小公式判定基盤の併用かという新しいトレードオフが残る。費用の承認はない。D09の大会細部（週次4対4など）は提案。これらをユーザーの指定事実として扱わない。

## 継続Task: YA-01 (In Progress)

純粋Combat実装SHAは`8862d5745405b1b2a69a4c000a4710444a6a01e3`。1対1物理通常攻撃、HP/MP、予定イベント、KO、独立乱数counter、mode/profile境界を実装。13テストで375ダメージ、100ms分割一致、8時間72000イベント一致を確認。証拠は`docs/evidence/YA-01/2026-10-01.md`。詳細契約は`docs/design/COMBAT_CORE_V1.md`。

次はGodot4.7.2-stable .NET/Compatibilityの最小表示クライアントでコアを読み込み、実際の.NET互換・Windows Releaseの描画/RAM/配布サイズを計測する。今回の249msはLinux上の純粋コア測定で、Windowsクライアントの軽量性証拠ではない。

コアの再検証は `dotnet run --project tests/Combat.Tests/Combat.Tests.csproj -c Release`。既存Docker SDK10.0.401でも実行可能。専用検証コンテナーは終了済み。対戦状態や鍵を外部へ送らず、公式経済の権限をクライアントへ移さない。

D01の契約やSteam AppIDが未確定でもクライアント試作は進める。YA-01全体完了前にYA-02以降の依存を完了扱いしない。大会細部はD09提案のまま。

## 旧補足の扱い

YA-D03 bounded checkpoint2026-10-02: `docs/quality/YA_D03_EDGE_CASES_RECONCILIATION.md`。EDGE_CASES7節を照合し、採用済みcooldown/通常攻撃量子化のみ現行本文へ明確化。DATA_MODEL/所有権/経済を論理契約として統合済み。旧section-3 UI8節を統合済み。CONTENT_DETAILS/CATALOG_REFERENCEと入力形状を統合済み。次は運用・判断履歴・非経済モデルの現行優先統合を進める。旧room復帰/期限境界/catalog修正を未承認のまま固定しない。YA01純粋コア13件を再実行済みにしない。

前回の `yusha_abandoned_design_supplement_v0.1.1.zip` に含まれていた設計内容は、Libraryから取得できた可読HTMLを `docs/archive/design-supplement-v0.1.1.html` としてRepository内へ保存した。これは元ZIPのbyte-exact archiveではなく、履歴入力をRepositoryだけで参照可能にするためのsource snapshotである。

YA-D03の意味統合は未完了。現行v0.2のPvP・600円・軽量化・承認済み決定を常に優先し、旧補足のDATA_MODEL / UI / OPERATIONS / ADR / EDGE_CASES / CONTENT_DETAILS / CATALOG_REFERENCEを差分確認して、現在も有効な内容だけを正本へ統合する。

AGENTS → RULES_SOURCE → TASKS → AI_WORK_STATEから復元し、現在Taskに必要な本文だけ読む。再開入口は `/continue Shota-Zaki/yusha-abandoned`。
