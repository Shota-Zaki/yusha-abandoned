# 勇者アバンド

放置育成 × 協力型オンラインRPG。50職を育て、ドット絵の自動戦闘、装備厳選、アクセサリー合成、交換、仲間との攻略、定期PvP大会を楽しむゲームです。

## 現在の状態

設計ベースラインv0.2。ゲーム本体は実装前。ユーザー回答によりSteamで600円買い切り、軽量化を重視する方針を確定しました。

街40人/通常4人/レイド12人、1活動キャラ・通常同時出撃1件・8時間指示、50職・キャラLv100/職Lv50・4能動+2受動、合成後の永久本人専用化を採用。PvEに定期PvP大会を加えました。

オンラインは「めっちゃカメレオン」を参考に運営負担を抑える方向です。完全P2Pか最小の公式判定基盤を併用するか、費用上限は未確定。SteamがMMOの計算と保存を全て無料で代行する前提にはしていません。大会の週次4対4等の細部と、Godot/C#の具体採用は提案・検証対象です。

## 読む順番

1. [企画概要](docs/project/PROJECT_BRIEF.md)・[決定事項と残る判断](docs/project/OPEN_DECISIONS.md)
2. [要件定義](docs/design/REQUIREMENTS.md)・[基本設計](docs/design/BASIC_DESIGN.md)・[戦闘詳細](docs/design/DETAILED_DESIGN.md)
3. [オンライン方式・軽量化・販売条件](docs/design/ONLINE_LIGHTWEIGHT.md)
4. [定期PvP大会の設計](docs/design/PVP_TOURNAMENT.md)
5. [50職](docs/design/JOBS.md)・[固定スキルと8属性魔法](docs/design/SKILLS.md)
6. [装備・合成・経済](docs/design/ITEMS_ECONOMY.md)・[ダンジョンと協力](docs/design/WORLD_MMO.md)
7. [API](docs/design/API.md)・[基幹の出典](docs/design/SOURCES.md)
8. [Task](docs/project/TASKS.md)・[次のWork Unit](docs/project/NEXT_WORK.md)・[作業状態](docs/project/AI_WORK_STATE.md)
9. [今回の文書レビュー](docs/quality/DESIGN_REVISION_02.md)

## 保存・検証の範囲

今回の価格・軽量化・PvP追加はworkに保存しています。前回のデータモデル・UI・運用等の補足は会話添付 yusha_abandoned_design_supplement_v0.1.1.zip に分離されたままで、全文統合はYA-D03に残ります。旧ZIPだけでは今回の更新を含みません。

今回の確認は文書レビュー。ゲーム実動、Steam/P2P接続、動作メモリ、PvPバランス、負荷試験とは区別します。前回の37/37検算を今回の新要素の成功証拠に流用しません。

## 再開

`/continue Shota-Zaki/yusha-abandoned`

最初にAGENTSと作業状態を読み、YA-01の軽量試作と純粋Combatへ進みます。開発状態はwork、公開・配布に必要な最小treeはmainで管理します。
