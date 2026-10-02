# YA-D03 CONTENT_DETAILS/CATALOG_REFERENCE統合

開始SHA `c076056fcad1c4972690e2a75f4b5991596aac32`。

旧section-5の6節をCONTENT_DETAILSへ保持し、すべて旧P初期案と宣言。初回支給/クエスト、消耗品、通常敵、ボス12行、ギルド/実績、初期ビルド例を現行職/技能/地域/経済の正本へ接続する。DNG12目的達成不能の終了文言は旧EDGE_CASESの未確定扱いを保持し、今回のコピーで採用しない。

旧section-1は現行JOBS/SKILLSを正本としたままCATALOG_REFERENCEへ参照/照合範囲を記録。職50件（名前/区分/前提）、固定プログラム300系統、段階名32件はdependency-free read-only scriptで一致を確認。旧DATA_MODEL§7の入力形状も設計初期案として同文書へ統合。実ローダーやschemaは未実装。

静的確認: sourcesection5は6節、Q01–06/C01–06/DNG01–12/4配分例の保持、各配分53点/0–20、一致照合script PASS。実ゲーム受入は未実行。YA-D03は非経済モデル/運用/ADR/EDGE_CASES残件があるためIn Progress。次は運用・判断履歴の現行優先統合。
