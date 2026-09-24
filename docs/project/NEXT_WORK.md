# 次のWork Unit

更新: 2026-09-13 / Revision 0.2

## 現在

YA-D04完了。ユーザー回答を要件・基本設計へ反映し、ONLINE_LIGHTWEIGHT/PVP_TOURNAMENTを追加した。600円買い切り、街40/通常4/レイド12、1活動キャラ/8時間、50職と育成、合成交換、定期PvP大会の存在は確定。

D01は完全P2Pか最小公式判定基盤の併用かという新しいトレードオフが残る。費用の承認はない。D09の大会細部（週次4対4など）は提案。これらをユーザーの指定事実として扱わない。

## 最初の実装Task: YA-01

Godot 4系Compatibility/C#候補の互換Versionを公式資料で固定する。Windows優先のRelease試作で描画/RAM/サイズを計測し、軽量性を評価する。Steam販売のために重い3D構成を導入しない。

Domain/Combat/Contractsの純粋C#コアで、まず1人対1体、通常攻撃、HP/MP、予定イベント、KOまで。受入条件は同じ入力と乱数位置で同じ結果、100ms分割と一括Advanceの一致、物攻300・倍率1.5・防御200・無属性・会心なし・揺らぎ1.0で375ダメージ。

行動時間は最短値適用後100ms単位へ切上げ。全滅は公式イベント時刻+300秒。BattleModeとbalance_profileを入力境界に置き、PvP追加でPvEの係数を変えない。大会実装はYA-P01以降で行う。

D01の契約やSteam AppIDが未確定でも純粋コアと描画の試作を進める。公式の経済結果を端末の自己申告へ置き換えない。

## 旧補足の扱い

前回の `yusha_abandoned_design_supplement_v0.1.1.zip` に含まれていた設計内容は、Libraryから取得できた可読HTMLを `docs/archive/design-supplement-v0.1.1.html` としてRepository内へ保存した。これは元ZIPのbyte-exact archiveではなく、履歴入力をRepositoryだけで参照可能にするためのsource snapshotである。

YA-D03の意味統合は未完了。現行v0.2のPvP・600円・軽量化・承認済み決定を常に優先し、旧補足のDATA_MODEL / UI / OPERATIONS / ADR / EDGE_CASES / CONTENT_DETAILS / CATALOG_REFERENCEを差分確認して、現在も有効な内容だけを正本へ統合する。

AGENTS → RULES_SOURCE → TASKS → AI_WORK_STATEから復元し、現在Taskに必要な本文だけ読む。再開入口は `/continue Shota-Zaki/yusha-abandoned`。
