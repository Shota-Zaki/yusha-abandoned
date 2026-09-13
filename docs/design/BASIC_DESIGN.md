# 基本設計

Revision 0.2 / 2026-09-13。Steamでの軽量・600円買い切りを目標とし、通常PvEに定期PvP大会を追加する。オンライン方式の最終採否はD01に残る。本書の運営バックエンドは承認済み契約ではなく、公式経済を維持する比較案B。

## 1. 採用候補の構成

```text
Steamクライアント
  └ ゲームクライアント: Godot 4系 .NET / C#（候補）
       ├ 2D / Compatibility（軽量描画の第一候補）
       ├ Steamworks: 配布・認証・フレンド招待・実績・ロビー
       ├ Steam Networking: 街の表示等のP2P候補、実検証後に採否
       ├ HTTPS: 認証、育成、所持品、合成、交換、出撃、大会登録
       └ WSS: 公式の状態変化、チャット、戦闘/大会イベント
                     ↓
最小の公式判定基盤: ASP.NET Core（比較案B）
  ├ Identity / Character / Build
  ├ Expedition / Combat / Reward
  ├ Inventory / Synthesis / Trade / Market
  ├ Tournament / Bracket / Replay
  ├ Social / Guild / Moderation
  └ Content / Operations / Audit
       ├ PostgreSQL: 永続データ・ジョブキュー・outbox
       └ バックアップ保存先: 本体と障害領域を分離
```

最初はモジュールを分けた1アプリ＋1DBで構築する案。Redis、Kafka、Kubernetes、多数の独立マイクロサービスを初期必須にしない。パーティーごとに画面付きゲームエンジンを常駐させず、描画なしのCombatコアをワーカーで共用する。

Godot/C#とASP.NET Coreは従来の候補を維持する。ユーザーの技術指定は「軽い・Steamで出す」であり、具体的なエンジン・言語の固定ではない。互換Version、配布サイズ、RAM、描画負荷を最初に計測し、候補を固定する。[S05][S06] 軽量化の追加根拠と測定条件はONLINE_LIGHTWEIGHTを参照。

## 2. Steamと公式計算の境界

Steam Game Serversは専用ゲームサーバーを作るAPIで、実際のホストは開発者またはコミュニティが用意する。Steam Datagram Relayは通信中継であり、任意のゲームプログラム・DBを実行するホスティングの代わりではない。[S01][S02]

Steamは永続インベントリサービスも提供する。ただし信頼できないクライアントだけでは、特定のダンジョン達成に対応する任意の品を安全に付与する設計にはできない。権限APIでの明示的な付与はゲーム状態を把握する信頼済みサーバー側で行う。[S04]

現行のB案は、ランダム装備・合成・交換を同じDBで原子的に処理し、ゲームDBをインベントリの唯一の正本にする。Steam Inventoryへ同じ品を二重管理しない。将来Steam市場へ出す場合は所有権移管の別ADRが必要。

ロビー・招待と公式の参加資格判定を分離する。P2P候補は街の位置等の非経済情報。ホストには装備発行・戦闘勝利・大会優勝を確定する権限を与えない。公式のHTTP/WSSとSteam Networkingを同じTransportだとは扱わない。

D01のA案（完全プレイヤーホスト中心）を採用する場合、8時間放置・公式な交換経済・大会の検証をどう維持するかを先に再設計する。B案をユーザーの費用承認として扱わない。比較・根拠はONLINE_LIGHTWEIGHTに集約。

## 3. MMOとして共有するもの

同じワールド内でキャラクターID、経済、ギルド、取引所を共有。街のチャンネルと戦闘インスタンスを分割する。ユーザー回答により街40人/通常4人/レイド12人を採用した。対応CCUや40人P2P性能が測定済みという意味ではない。

街は交流用の軽量な2D空間。戦闘はパーティー専用インスタンス。離席しても事前に承諾した最大8時間の作戦を維持する要件を継続。D01未確定を理由に、全員が終了すると止まる方式へ黙って置き換えない。

定期大会は同ワールドの登録チーム同士で対戦する。週1回・4対4・非同期登録ビルドという細部は提案。大会の存在そのものはユーザー指定である。

## 4. 主な責務

|境界|責務|所有データ|
|---|---|---|
|Identity|Steam検証、セッション、重複ログイン|accounts、sessions|
|Character|キャラLv、職Lv、転職、ポイント|characters、job_progress、builds|
|Combat|決定的な戦闘評価。DB・ネットワーク非依存。PvE/PvPプロファイル分離|固定戦闘入力と結果|
|Expedition|編成固定、時刻、反復、全滅、追いつき|expeditions、participants、runs|
|Reward|確定ルームのドロップと通貨|reward_grants、ledger|
|Inventory|実体ID、装備、ロック、本人専用状態|items、sockets、equipments|
|Economy|合成、直接交換、出品、購入|syntheses、trades、listings|
|Tournament|参加同意、固定ビルド、ブラケット、対戦、結果、報酬|tournaments、entries、matches、games、rewards|
|Social|街、フレンド、チャット、ギルド|guilds、membership、reports|
|Content|マスタ検証・PvP補正・Version固定|content_versions、immutable catalogs|
|Operations|補填、BAN、監査、バックアップ|admin_audit、compensations|

## 5. 主要フロー

ログイン: Steamチケット取得 → バックエンド検証 → 内部セッション発行 → 状態取得。

出撃: アカウントとメンバーをロック → cooldown/装備/職/同意を検証 → ビルド・内容Version・サーバー乱数種を固定 → 出撃とジョブを保存 → 描画なしワーカー → ルーム単位の報酬確定。

取引: 所有品予約 → 提示Version確定 → 双方同意 → 同一DB取引で所有権と通貨を移転 → outbox通知。

合成: 公式見積もり → 消費物と変化を確認 → 所有権/Version再検証 → アクセサリー消費、ソケット更新、本人専用化、費用決済を同時コミット。

大会: 4人分の公式所有状態から専用ビルド作成 → 全員同意 → 締切で固定 → 公式seedで組合せ → PvPプロファイルで対戦 → 結果整合性検査 → 非交換の称号/外見報酬。詳細はPVP_TOURNAMENT。

## 6. エラーと整合性

端末には操作の意図だけを送らせる。ダメージ、獲得経験値、ロール値、相手への権限、cooldown終了、大会勝利を自己申告から確定しない。

状態変更はrequest_idで再送可能にし、同じIDには同じ結果を返す。経済と大会の通知失敗でコミット済み結果を取り消さない。UIは未確定状態を処理中として表示し、再接続時には公式状態を取得する。

PvPの敗北・切断・タイムアウトをダンジョン全滅と同じイベントにしない。大会のsnapshotで一時的に増えたLvやポイントは通常キャラへ保存しない。

## 7. 配置・運営・販売

ゲームは日本円600円買い切り。月額課金や値上げを運営費の穴埋めとして自動採用しない。Steamへの実価格登録と地域価格は公開前Task。

B案では公開接続、保存、監視、バックアップが必要になる。費用は離席中の冒険数、大会数、イベント数、品とログの増加、DB性能から計測する。完全無料や本番対応人数を保証しない。個人PCを使う場合も電力・回線・障害対応を無料ホスティングと同一視しない。

正本参照: 基幹の外部仕様はSOURCES、今回の一次調査はONLINE_LIGHTWEIGHT、今回の大会仕様はPVP_TOURNAMENT。旧補足v0.1.1のRepository統合状況はAI_WORK_STATEを参照。
