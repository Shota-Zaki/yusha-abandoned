2026-10-02 YA-D03 DATA_MODEL partial checkpoint: 開始3c92e1b。所有権/経済論理モデルと未実行競合受入をDATA_MODEL.mdへ統合。旧PostgreSQL/列置換/lock順の提案を採用済みにせずD01を保持。次は旧UIの現行契約照合。DB・native tests未実行、YA-D03 In Progress。

# AI Work State

2026-10-02 YA-D03 partial design checkpoint: EDGE_CASES7節のsource/current authority disposition、既存FR20/Combatv1のcooldown/通常攻撃量子化を明確化。`docs/quality/YA_D03_EDGE_CASES_RECONCILIATION.md`。RNGは既存採用履歴として保持、セキュリティ実装変更なし。YA01クライアントと残supplement統合は未完了。今回product/native tests未実行。

- Project: Shota-Zaki/yusha-abandoned / 勇者アバンド
- Date: 2026-10-01
- Branch: work
- 今回の開始commit: 2655688744fb88e0cd29dbe4a8e35ea6bcd41517
- Design: 基幹0.2 + 旧補足0.1.1（source snapshot回収済み、意味統合は未完了）
- Current: YA-01 In Progress — Combat core v1 accepted locally
- Game implementation: 1対1物理通常攻撃の純粋Combatコア実装済み。クライアント未実装
- Next implementation: YA-01 Godot/Windows lightweight prototype

## 今回の回答を反映した確定事項

Steamで軽量なゲームとして販売、600円買い切り。街40人/通常4人/レイド12人、1活動キャラ・通常同時出撃1件・8時間指示を承認。提示の50職、キャラLv100/職Lv50、固定4能動+2受動、職外スキル継承なしを承認。未合成品は外せば交換可能、合成で永久本人専用を承認。PvEを基本に定期PvP大会を追加。

「めっちゃカメレオンと同じ感じ」は低運営負担の方向として記録。Steamが任意のゲーム計算を無料ホストする承認や、別途有料契約の承認ではない。

## 新しい正本

- docs/project/OPEN_DECISIONS.md: 回答①〜⑧と残るD01/D09
- docs/design/REQUIREMENTS.md: FR31価格、FR32定期PvP、FR33分離、NFR11軽量/NFR12低運営負担
- docs/design/BASIC_DESIGN.md: Steamロビー、P2P候補と公式判定境界、大会モジュール
- docs/design/ONLINE_LIGHTWEIGHT.md: 一次資料の確認範囲、A/B比較、軽量目標、600円条件
- docs/design/PVP_TOURNAMENT.md: 登録・ビルド・戦闘・勝敗・API/DB・画面・障害・試験案
- docs/quality/DESIGN_REVISION_02.md: 今回の文書レビューと限界

対応するREADME/AGENTS/PROJECT_BRIEF/TASKS/NEXT_WORKも更新する。

## 未確定事項

D01: 完全P2P/運営計算基盤ゼロ優先か、信頼できる最小の公式判定・保存基盤を認めるか。現行の交換/8時間/公式大会を維持するB案を提案、費用・契約は未承認。基盤の最終選定だけに影響を限定し、純粋Combat/UIを停止しない。

D04: 軽さとSteamは確定。Godot4 Compatibility/C#は第一候補、Version・RAM・配布容量・対応スペックは未検証。

D09: 週1回、4対4、登録ビルドの非同期オート戦、レベル統一/所持装備、3ゲーム、180秒、称号外見報酬、PvE制限非連動は提案。大会の存在そのものは確定。

## 検証状態

2026-10-01: Combat core v1の13テスト成功。375ダメージ、100ms分割一致、8時間72000イベント一致。固定SHA8862d5745405b1b2a69a4c000a4710444a6a01e3。証拠docs/evidence/YA-01/2026-10-01.md。P2P実接続、Steam実機、メモリ/速度、DB競合、PvPバランス、負荷・復元は未実施。前回の37/37は旧設計モデルの記録であり、今回の大会や通信方式がテスト済みという意味ではない。

## 旧補足と残作業

前回の追加設計は会話添付 `yusha_abandoned_design_supplement_v0.1.1.zip` に分離されていたが、2026-09-24にLibraryから可読HTMLを回収し、`docs/archive/design-supplement-v0.1.1.html`へ保存した。元ZIPのbyte-exact archiveではないため、そのようには扱わない。

旧補足の主な内容は DATA_MODEL / UI / OPERATIONS / ADR / EDGE_CASES / CONTENT_DETAILS / CATALOG_REFERENCE、検算モデルとスクリプト等。Repositoryだけで本文を参照可能になったためYA-D03はReadyへ変更したが、現行v0.2との意味差分統合は未完了。v0.2の回答済み事項を旧版で上書きしない。

前回からの補正: 行動間隔100ms切上げ、ルーム/周回/反復指示の区別、レイド目的達成不能の失敗終了、魔法の発想源語数/毒刃表記修正は旧基幹詳細への反映状況をYA-D03で確認する。全滅時刻+300秒は今回REQUIREMENTS/AGENTS/NEXT_WORKに明示した。

YA-M01: 共通Rules全文Snapshot未同期。正式版2.0.0/source aa9fcf26a5c463e37591438ed3e6f410974c9878の参照記録を維持。

## 次の作業

YA-01の軽量試作と決定的Combatコア。大会はYA-P01/02/03、SteamロビーとP2P比較はYA-12B。正本の更新はwork、公開時だけ必要物をmainへ選別する。

## 再開の優先

NEXT_WORKに従いYA-01のクライアント互換/軽量性を検証する。純粋コアの成功だけでYA-01全体をDoneにしない。
