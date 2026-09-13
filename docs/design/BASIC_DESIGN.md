# 基本設計

## 1. 採用候補の構成

```text
Steamクライアント
  └ ゲームクライアント: Godot .NET / C#
       ├ Steamworks: 認証チケット・フレンド招待・実績・配布
       ├ HTTPS: 認証、育成、所持品、合成、交換、出撃指示
       └ WSS: 街の状態、パーティー、チャット、戦闘イベント
                     ↓
運営管理サーバー: ASP.NET Core
  ├ Identity / Character / Build
  ├ Expedition / Combat / Reward
  ├ Inventory / Synthesis / Trade / Market
  ├ Social / Guild / Moderation
  └ Content / Operations / Audit
       ├ PostgreSQL: 永続データ・ジョブキュー・outbox
       └ バックアップ保存先: 本体と障害領域を分離
```

最初はモジュールを分けた1アプリ＋1DBで構築する。Redis、Kafka、Kubernetes、独立した多数のマイクロサービスを初期の必須要素にしない。負荷計測後、戦闘ワーカーと接続ゲートウェイをプロセス分離できる境界だけ設ける。

GodotはC#/.NETを公式にサポートする。ASP.NET CoreはWebSocketを提供する。採用するエンジン・SDK・ランタイムの具体バージョンは最初の互換性検証で固定する。Steamバインディングは公式SDKとの互換性・ライセンス・保守状態を確認して選ぶ。未調査の第三者ラッパーを確定採用しない。[S05][S06]

## 2. Steamの境界

Steam Game Serversは専用ゲームサーバーを作るAPIで、実際のホストは開発者またはコミュニティが用意する。Steam Datagram Relay（SDR）は通信中継であり、任意のゲームプログラム・DBを実行するホスティングの代わりではない。[S01][S02]

Steamは永続インベントリサービスも提供する。ただし信頼できないクライアントだけでは、特定のダンジョン達成に対応する任意の品を安全に付与する設計にはできない。権限APIでの明示的な付与はゲーム状態を把握する信頼済みサーバー側で行う。[S04]

本案はゲーム内のランダム装備・合成・交換を1つのDBトランザクションで処理するため、ゲームDBをインベントリの唯一の正本とする。Steam Inventoryへ同じ品を二重管理しない。将来Steam市場へ出す場合は一方向の所有権移管など別ADRが必要。

初期通信はHTTPS/WSSとする。SDRはSteam Networking用であり、汎用HTTPS/WSSへ自動で被せられる仕組みとして扱わない。SDRが必須と確認された場合はTransport AdapterをSteamNetworkingSocketsへ置換する技術検証を追加する。D01未確認のままホスティング契約やネットワーク方式を確定しない。

## 3. MMOとして共有するもの

同じワールド内で、キャラクターID、経済、ギルド、取引所を共有。街のチャンネルと戦闘インスタンスだけを分割する。1街チャンネル40人、通常パーティー4人、レイド12人を仮の上限とする。

街は交流用の軽量な2D空間。戦闘はパーティー専用インスタンス。離席したプレイヤーも事前に承諾した作戦で参加を継続する。街に同時表示できる人数と、サービス全体の同時接続数を混同しない。

## 4. 主な責務

|境界|責務|所有データ|
|---|---|---|
|Identity|Steam検証、セッション、重複ログイン|accounts、sessions|
|Character|キャラLv、職Lv、転職、ポイント|characters、job_progress、builds|
|Combat|決定的な戦闘評価。DB・ネットワーク非依存|戦闘入力と結果だけ|
|Expedition|編成固定、時刻、反復、全滅、追いつき|expeditions、participants、runs|
|Reward|確定済みルームのドロップと通貨|reward_grants、ledger|
|Inventory|実体ID、装備、ロック、本人専用状態|items、sockets、equipments|
|Economy|合成、直接交換、出品、購入|syntheses、trades、listings|
|Social|街、フレンド、チャット、ギルド|guilds、membership、reports|
|Content|マスタの検証・Version固定|content_versions、immutable catalogs|
|Operations|補填、BAN、監査、バックアップ|admin_audit、compensations|

## 5. 主要フロー

ログイン: Steamチケット取得 → バックエンド検証 → 内部セッション発行 → 状態取得。

出撃: アカウントとメンバーをロック → cooldown/装備/職/同意を検証 → ビルドのスナップショット・内容Version・サーバー乱数種を確定 → 出撃とジョブを同時保存 → ワーカー処理 → ルーム単位の報酬確定。

取引: 所有品の予約 → 提示内容のVersion確定 → 双方同意 → 同一DB取引で所有権移転・台帳記録 → outboxで通知。

合成: サーバー発行の見積もり → 消費物と変更結果の確認 → 所有権・Version再検証 → アクセサリー消費、ソケット更新、装備本人専用化、費用決済を同時コミット。

## 6. エラーと整合性

端末には操作の意図だけを送らせる。ダメージ、獲得経験値、アイテムのロール値、相手IDに対する権限、cooldown終了時刻を端末から採用しない。

状態変更はrequest_idで再送可能にする。DBコミット後に通信が切れても、同じIDで同じ結果を返す。取引や合成の通知失敗でコミットを取り消さない。

UIは未確定状態を「処理中」と表示し、失敗を勝手に成功表示しない。再接続時はサーバーの状態を再取得する。

## 7. 配置・運営

開発用PCはローカル検証に使える。本番は公開接続、稼働継続、監視、バックアップ、更新手順を満たす運営環境を別に検討する。個人PCの24時間運転を商用SLA達成とみなさない。

サーバー費用は接続人数だけでなく、離席中の同時稼働冒険数、平均イベント数、アイテム増加、ログ保持量、DB性能に依存する。契約先と月額は未確定。OPERATIONSに容量計測と復旧手順を定義する。

外部事実の出典はSOURCES.md参照。それ以外の人数・方式・閾値はこのゲームの設計提案である。
