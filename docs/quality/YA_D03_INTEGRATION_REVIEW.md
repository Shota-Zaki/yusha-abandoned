# YA-D03 全補足の意味統合チェックポイント

開始SHA `ad35f3e2a780baa8270a30895b962f254f487c14`。対象sourceは可読HTML snapshotで、元ZIP byte一致の証拠ではない。

## 正本対応

|旧section|統合先と権威|
|---|---|
|0 導入|履歴入力の説明。旧37件は当時のモデル検算、現在のruntime受入ではない|
|1 CATALOG_REFERENCE|CATALOG_REFERENCE/JOBS/SKILLS。source/current職50、固定系統300、段階名32の一致確認|
|2 DATA_MODEL|DATA_MODELの現行論理契約、DATA_MODEL_STORAGE_CANDIDATEの旧39entity候補、CATALOG_REFERENCEの入力形状。account/character/活動制約の補正を優先|
|3 UI|UI01–15と現行大会/Combatv1境界。8旧節の対応はUI_RECONCILIATION|
|4 EDGE_CASES|EDGE_CASESの7節。採用済みcooldown/normalattack/RNGと未採用room/期限案を分離|
|5 CONTENT_DETAILS|CONTENT_DETAILSの6節。全提案をP初期案と明示、DNG12差分を自動採用しない|
|6 OPERATIONS|OPERATIONSの6節。NFR目標と容量/threshold/保持/backup方式の提案、未実行QA01–16を分離|
|7 ADR|DECISION_HISTORYの7判断。v0.2/D01/D02/D03/D06/D09と現在の候補構成を優先|
|8–10 旧Task/Next/State|歴史snapshotの記録。現行docs/projectへ古いStatus/Done/nextをコピーしない|
|11 旧確認事項|現在のOPEN_DECISIONSを優先。回答済み事項を再質問せず、旧確認に大会/価格がないことを対象外と解釈しない|
|12 旧review/追跡|旧指摘の履歴。今回は現行との照合と固定SHA reviewを別記録、過去37件を全QAへ転用しない|
|13 参照元|既存SOURCES/ONLINE_LIGHTWEIGHTを優先。旧引用を現行仕様の検証済み最新情報へ読み替えない|

## 今回の検証

39entity候補の行数/名前保持、旧OPERATIONS6節/ADR7判断/EDGE7節の全対応、全追加設計のlocal links、currentTaskのIn Progress、source/currentカタログcheckerを確認。これは設計参照の静的検証。DB/migration/client/native/security/負荷/復元/ゲーム受入は実行していない。

## 統合後に残る具体的な仕様・実装判断

YA-D03の資料参照・分類は全sectionを網羅した。ただし旧初期案を正式採用するという残作業は自動完了しないためTaskはIn Progressを維持する。

1. 期限のexact boundary: 現行「期限前」と旧「同時刻の完了を先に解決」の差。推奨初期案は既開始行動の同時刻完了/KOを解決し、新開始を拒否。YA-07でexpiry wrapperを設計し、要件への影響と試験を固定する。今回のdesign integration権限では採用しない。
2. room/run: 個別KO20%復帰/MP維持、次run全回復、effect reset、蘇生回数の詳細。推奨は旧案を対照fixtureの候補へ保持し、YA-03/07のbalance/状態契約へ確定する。既存duel受入から正しさを推定しない。
3. DNG12目的失敗: 旧案は一隊の最終条件未達全滅で全raid失敗/全員cooldown。WORLD_MMOの全員KO/時間切れ条件との影響を明示して採否を決める。旧sourceコピーだけでFR20適用対象を拡張しない。
4. D01/D09とnative toolchain: 公式基盤採否/費用と大会細部はOPEN_DECISIONS、Godot/.NET/Windows実機はYA-01。統合タスクの完了を理由にサービス契約/新規install/公開を実行しない。

今の許可範囲で旧資料の分類・既存契約への接続は完了した。上記の採用/実装は今回の「新しいproduct choice/DB/runtime implementation禁止」境界を超えるため、YA-D03の次工程として明示する。無関係の完成済み監査を再実行しない。
