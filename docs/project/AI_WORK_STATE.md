# AI Work State

- Project: Shota-Zaki/yusha-abandoned / 勇者アバンド
- Date: 2026-09-13
- Branch: work
- 基幹設計commit: c8eadd137ca0b685d91707232f8c3b19a8aa6eab
- Design: 基幹v0.1 + 添付補足v0.1.1
- Game implementation: 未開始
- Current: 設計作成完了、補足のRepository統合は残る
- Next implementation: YA-01

## 完了

指定要件と非機能目標、20+20+10の50職、武器制限、固有特性、各4能動+2受動の300系統、8属性4段階、成長・装備・合成・交換・市場・12地域6難易度・離席・全滅・APIを基幹文書に定義した。

補足資料にデータモデル、UI、運用、ADR、境界修正、ボス行動、初回体験、Taskと再開情報、検算モデルとスクリプトを作成。モデルと明示した式の37/37静的検算が成功。Repository全文の自動抽出・照合ではない。

## 保存状態と復元

追加設計4文書の一括書込みがツール側で拒否された。拒否された文書を別の経路でRepositoryへ保存せず、会話添付に分離した。

添付名: yusha_abandoned_design_supplement_v0.1.1.zip
内容: 19ファイル、index.html、docs/design/DATA_MODEL.md、UI.md、OPERATIONS.md、ADR.md、EDGE_CASES.md、CONTENT_DETAILS.md、CATALOG_REFERENCE.md、SOURCES.md、project文書、quality文書、data/design_model.json、tools/validate_design.py等。

添付は基幹14ファイルの完全な複製ではない。ここに全追加文書が存在すると扱わない。取得できない場合も、チャット記憶から未取得の本文を推測して完了扱いしない。

## 重要な補正（元文書への反映は未完了）

- 全滅制限は処理完了時刻ではなく、サーバーが再現した全滅イベント時刻+300秒。12:00全滅、12:20復帰なら制限は終了済み。
- 計算後の行動間隔は最短値を守ったうえで100ms単位へ切上げ。
- 1ルーム=戦闘、5ルーム=周回、最大8時間=反復指示。蘇生や致死耐えのリセット境界を区別。
- レイドは最終目的未達成の隊が全滅し達成不能になった場合も失敗終了。
- SKILLSの自然語の発想源は12語で、16語表記を修正予定。J31の毒刃は出血プログラムPBなので裂傷刃へ名称修正予定。

## 仮決定と保留

D01〜08は確認前。Steamサービス+運営サーバー、街40/通常4/レイド12、1活動キャラ、8時間、Windows/Godot .NET/C#、買い切り候補、20/20/10職、装備時は未拘束・合成で本人専用、PvE等を仮定して設計した。

QAの実動試験、バランス、Steam、DB競合、負荷、復元訓練は未実施。初級職の用途は設計目標であって達成実績ではない。

YA-D03: 補足全文のRepository統合。
YA-M01: 共通Rules全文Snapshot。正式版2.0.0、source aa9fcf26a5c463e37591438ed3e6f410974c9878を確認したが全文同期は未完了。

原則workで作業し、mainには配布・公開に必要な内容だけを選別する。
