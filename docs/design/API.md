# API・イベント契約

## 1. 共通

HTTPS `/v1`。JSON UTF-8、snake_case、IDは文字列、時刻はISO8601 UTC、整数通貨。Bearer内部セッション。Steamチケットを通常APIごとに送信しない。

状態変更は`Idempotency-Key: UUID`と対象の`expected_version`を必須とする。初回リクエストのactor/method/path/bodyの正規化ハッシュを保存。同じキー/同じ内容は同じreceiptを返し、内容違いは409 IDEMPOTENCY_CONFLICT。経済系キーの薄い一意レコードは保持し続け、応答本文の長期保管はreceiptへ集約する。

共通成功例:
```json
{"request_id":"uuid","server_time":"2026-09-13T12:00:00Z","version":7,"data":{}}
```

共通失敗例:
```json
{"request_id":"uuid","code":"DEPLOYMENT_COOLDOWN","message_key":"deployment.cooldown","retryable":false,"details":{"cooldown_until":"2026-09-13T12:05:00Z"}}
```

HTTP: 400形式、401認証、403権限/利用停止、404不存在、409競合、422ゲーム条件違反、429制限、503一時停止。処理待ちは202とoperation_idを返し、完了を勝手に推測させない。

## 2. エンドポイント

|メソッド・パス|入力の主要フィールド|出力/責務|
|---|---|---|
|POST /auth/steam|ticket、ticket_identity、client_version|Steam検証済み内部セッション、account_id。生ticketを保存・ログ出力しない|
|POST /auth/logout|session_id|対象セッション失効。出撃状態は保持|
|GET /me|なし|キャラ、財布、cooldown、出撃要約、server_time|
|GET /content/manifest|client_content_version|互換Version、カタログhash、必須更新|
|GET /characters/me/jobs|なし|解放済み職、前提達成、Lv、EXP、点数|
|POST /characters/me/job|job_id、expected_version|転職結果、不適合武器の移動|
|PUT /characters/me/builds/{job_id}|stat_allocation、skill_ranks、ai_rules、active_gear_skills、expected_version|検証済みビルド。出撃中はnext_draft_only=true|
|GET /inventory|cursor、limit<=100、filter|品の実体、能力、binding、ロック、次cursor|
|POST /equipment|slot、item_idまたはnull、expected_version|装備/解除。部位と武器制限を検証|
|POST /items/{id}/lock|locked、expected_version|誤消費防止の保護ロック|
|POST /synthesis/quotes|equipment_id、accessory_id、socket_index|quote_id、費用、前後差分、input_versions、expires_at（60秒）|
|POST /synthesis|quote_id、expected_versions|合成receipt、消費、本人専用化、更新装備|
|POST /salvage|item_ids<=20、expected_versions|分解receipt、素材。保護・出撃・取引品を拒否|
|POST /expeditions|dungeon_id、difficulty、party_id、plan_version、stop_rules|expedition_id、expires_at、固定メンバー|
|GET /expeditions/{id}|なし|本人参加資格を確認した状態とseq|
|POST /expeditions/{id}/stop|mode=AFTER_RUN/RETREAT、expected_version|次周停止または300秒制限付き撤退|
|GET /rewards|cursor、limit<=100|確定済み未受取報酬と予約容量|
|POST /rewards/claim|grant_ids<=100、expected_version|バッグへの移動receipt。抽選し直さない|
|POST /parties|目的・難易度・role_slots|party_id、leader、version|
|POST /parties/{id}/join|招待または募集ID|資格検証後の加入|
|POST /parties/{id}/ready|plan_version、ready|作戦への同意。Versionが変われば無効|
|POST /parties/{id}/leave|after_run|安全な境界での脱退予約|
|POST /trades|target_character_id|trade_id、expires_at|
|PUT /trades/{id}/offer|item_ids<=10、gold、expected_version|予約品とoffer_version、双方同意リセット|
|POST /trades/{id}/confirm|offer_hash、expected_version|片側同意、両者成立ならreceipt|
|POST /trades/{id}/cancel|expected_version|予約解除または既確定結果|
|GET /market|cursor、filters、sort|出品と表示価格。連続スクロールはkeyset pagination|
|POST /market/listings|item_id、price、expected_version|出品料、期限、escrow|
|POST /market/listings/{id}/buy|expected_version、expected_price|原子的な購入receipt|
|POST /market/listings/{id}/cancel|expected_version|取下げと返却|
|POST /guilds|name、description|guild_id。名称検証|
|POST /guilds/{id}/members|target_id、role、expected_version|権限確認した招待/昇格|
|POST /guilds/{id}/leave|expected_version|脱退。マスターは移譲か解散を先に実施|
|POST /reports|target_id、event_ids、reason_code、text<=1000|report_id。改変しない証跡を紐付け|
|POST /blocks|target_account_id、blocked|ブロック更新|
|GET /operations/{id}|なし|自分の遅延処理結果|

全エンドポイントでIDの所有権とアクセス範囲を検証する。URLが分かることを認可としない。公開プロフィールを取得する場合は専用DTOにし、資産全件・セッション・メール等を返さない。

## 3. Steam認証

クライアントはGetAuthTicketForWebApi相当で用途を指定したチケットを得る。サーバーはSteam公式認証APIへ送信し、成功したSteamID、対象AppID、用途、所有/利用資格を検証する。クライアントが別途指定したSteamIDを優先しない。[S03]

内部access tokenは15分。長期refresh機能は初期実装に含めず、期限前にSteamチケットで再認証する。再認証してもアカウントの出撃は維持する。再認証失敗時は新しい経済操作を停止し、既に成立した出撃は所定期限まで継続する。

Steam停止時に未検証ユーザーを通すfallbackを作らない。本人の既存セッションは有効期限内だけ利用し、認証とゲームロジックの障害を別に表示する。

## 4. WSS

`/v1/events`。Authorization headerで接続し、未認証の購読を拒否。接続時に最後に受け取ったstream_id/seqを送る。

```json
{"stream_id":"expedition:uuid","seq":1024,"event_id":"uuid","server_time":"2026-09-13T12:00:00Z","type":"combat.damage","content_version":"0.1","payload":{"source_id":"p1","target_id":"e1","amount":375,"target_hp":425}}
```

イベント: account.changed、party.changed、expedition.started、combat.cast、combat.damage、combat.heal、combat.effect、combat.ko、room.completed、reward.granted、expedition.wiped、cooldown.changed、inventory.changed、trade.changed、listing.sold、chat.message、maintenance.notice。

配送はat-least-once。event_id重複を除外し、seqに欠番があれば再取得する。サーバーのイベント保存窓を越えたらsnapshot.requiredを返す。WSS通知が届かなくてもDBの確定を取り消さない。

heartbeat20秒、60秒応答なしで切断。クライアント再接続は1/2/4/8/16/30秒上限＋jitter。戦闘の描画は受信データから補間できるが、表示の予測で報酬や実HPを確定しない。

## 5. 制限と境界

通常変更操作10件/秒/アカウント、合成・取引・購入2件/秒、ログイン5回/分を初期値とする。IPだけで制限して同一家庭を一律締め出さず、アカウント・操作・IPを組み合わせる。大量攻撃時は運営設定で調整可能。

本文64KiB、チャット400文字、AI8ルール、フィルタはホワイトリスト。カーソルは署名またはopaque ID。WSSもチャンネルの所属とBAN状態を継続確認する。

エラーコードにはPOINT_BUDGET_EXCEEDED、JOB_LOCKED、WEAPON_NOT_ALLOWED、ITEM_BOUND、ITEM_RESERVED、ITEM_VERSION_CONFLICT、INSUFFICIENT_FUNDS、INBOX_FULL、PARTY_NOT_READY、DEPLOYMENT_COOLDOWN、ALREADY_DEPLOYED、CONTENT_VERSION_MISMATCHを用意する。
