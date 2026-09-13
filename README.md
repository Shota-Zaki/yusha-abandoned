# 勇者アバンド

放置育成 × 協力型オンラインRPG。ドット絵の冒険者を育て、50職の転職、装備の厳選、アクセサリー合成、仲間との自動戦闘を楽しむゲームです。

## 現在の状態

要件・基本設計・詳細設計のベースラインを作成しました。ゲーム本体は実装前です。ユーザー指定以外の設定・数値は仮決定であり、確認とプレイテストで調整します。

基幹設計はこの `work` ブランチに保存しています。追加のデータモデル・画面・運用などは、一括書込みがツール側で拒否されたため、会話添付 `yusha_abandoned_design_supplement_v0.1.1.zip` に分離しました。補足全文のRepository統合は未完了です。

## 基幹設計

1. [企画概要](docs/project/PROJECT_BRIEF.md)
2. [要件定義](docs/design/REQUIREMENTS.md)
3. [基本設計](docs/design/BASIC_DESIGN.md)
4. [詳細設計](docs/design/DETAILED_DESIGN.md)
5. [50職の職業表](docs/design/JOBS.md)
6. [固定スキル・8属性の魔法](docs/design/SKILLS.md)
7. [装備・合成・経済](docs/design/ITEMS_ECONOMY.md)
8. [ダンジョン・放置・協力](docs/design/WORLD_MMO.md)
9. [API](docs/design/API.md)
10. [出典](docs/design/SOURCES.md)

## 作業と確認事項

- [Task計画](docs/project/TASKS.md)
- [次のWork Unit](docs/project/NEXT_WORK.md)
- [作業状態と未統合資料](docs/project/AI_WORK_STATE.md)
- [仮決定・確認事項](docs/project/OPEN_DECISIONS.md)

## 補足資料

添付ZIPは基幹設計の完全な複製ではなく、追加設計・職業参照一覧・検算用モデル・スクリプト・結果を含む19ファイルです。ブラウザで読む `index.html` を同梱しています。

設計モデルと明示した式の静的検算は37/37成功。Repository全文の自動照合、ゲーム実動、Steam接続、負荷、バランスの試験とは区別します。

補足の重要な修正は、全滅したイベント時刻から300秒を数えること、行動間隔を100ms単位へ切り上げること、1ルーム/1周回/反復指示の区別、レイド目的達成不能時の終了です。元の基幹文書への反映は残っています。

## 再開

`/continue Shota-Zaki/yusha-abandoned`

最初に `AGENTS.md` と作業状態を確認してください。補足の取得・統合状態を確認せず、Repositoryだけに全設計が揃っていると扱わないでください。
