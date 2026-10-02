# 保存モデル候補 — 旧提案の保持と現行対応

YA-D03 / 2026-10-02。旧DATA_MODEL§2の全エンティティを参照可能な表へ統合する。以下は比較案Bの**物理保存候補**で、PostgreSQL採用・DDL・migration・security実装・本番設定の許可ではない。現行[DATA_MODEL](DATA_MODEL.md)の所有権契約と[BASIC_DESIGN](BASIC_DESIGN.md)の責務を優先する。

旧表を使用する際の必須補正:

- characters.account_id一意は保存キャラ総数を制限する仮定であり採用しない。現行は1活動キャラ/活動出撃のaccount単位一意。
- items.owner_id/bound_owner_id/materials.owner_idは現行owner_character_idの名前空間へ対応させる。財布account_idと混同しない。
- reservation_kind/id列は現行locked_by/expedition_lock_idの置換済みではない。論理排他はDATA_MODEL§3、物理表現は対応実装Taskで確定する。
- sessions/admin等の列・権限方式は旧候補の保持だけ。認証/保存保持/管理権限をこの統合で新規採用・変更しない。
- 8時間/通常4人/レイド12人/成長上限は現行契約。ギルド50人、通貨上限9×10^15、lease方式、job_queue/outboxの物理索引は旧初期案。

## 旧全エンティティ候補

|テーブル|主要列|不変条件・索引|
|---|---|---|
|accounts|id, steam_id, status, cooldown_until, version|steam_id一意。出撃制限はキャラやセッションではなくアカウントに保存|
|sessions|id, account_id, token_hash, expires_at, revoked_at|生のtokenやSteamチケットを保存しない。token_hash一意|
|characters|id, account_id, world_id, name, level, exp, current_job_id, version|account_id一意、キャラLv1〜100、EXP非負|
|job_progress|character_id, job_id, level, exp, mastered_at|複合主キー。職Lv1〜50。到達済みmastered_atを転職で消さない|
|job_unlocks|character_id, job_id, unlocked_at, rule_version|複合主キー。解放時点の前提ルールを記録|
|builds|character_id, job_id, stat_allocation, skill_ranks, ai_rules, active_gear_skills, version|複合主キー。JSONスキーマ・配分予算・職外スキルをサーバー検証|
|wallets|account_id, currency, balance, reserved, version|0≦reserved≦balance≦9×10^15。利用可能額はbalance-reserved|
|items|id, owner_id, template_id, item_level, rarity, rolls, innate_grants, socket_count, binding, bound_owner_id, location, reservation_kind, reservation_id, version, grant_id|BOUNDはbound_owner_id=owner_id。TRADEABLEはbound_owner_idが空。予約は1品1つ|
|item_sockets|item_id, socket_index, skill_id, points, consumed_accessory_id|item_id/socket_index主キー。indexは0〜socket_count-1、point1〜4|
|equipment|character_id, slot, item_id|character_id/slot主キー、item_id一意、slotは指定5部位|
|material_stacks|id, owner_id, material_id, purity, binding, quantity, reserved, version|0≦reserved≦quantity≦9999。本人専用と交換可能の素材は別スタック|
|capacity_reservations|id, character_id, kind, reference_id, slots, expires_at|kind/reference/character一意。箱の現品数と予約数の合計が上限以下|
|expeditions|id, party_id, world_id, dungeon_id, difficulty, status, starts_at, expires_at, plan, content_version, version|期限は開始から8時間以内。作戦とVersionを開始時に固定|
|expedition_members|expedition_id, account_id, character_id, squad, snapshot, active|expedition/character主キー。activeのaccount_idは一意|
|runs|id, expedition_id, run_index, status, current_room, started_at, ended_at|expedition/run_index一意|
|combat_checkpoints|run_id, version, sim_ms, state, random_counters, lease_token|run_id主キー。状態と乱数位置を同時保存|
|reward_grants|id, run_id, room_index, recipient_id, ordinal, payload, status|run/room/recipient/ordinal一意。同じ報酬を再発行しない|
|pity_counters|account_id, difficulty, target_rarity, miss_count, version|複合主キー、miss_count非負|
|operations|id, account_id, idempotency_key, request_hash, kind, status, receipt|account/idempotency_key一意。同じキーで内容違いは拒否|
|syntheses|id, operation_id, equipment_id, accessory_id, socket_index, before_version, after_version, gold_cost, material_cost|operation_idとaccessory_idがそれぞれ一意|
|trades|id, world_id, account_a, account_b, status, offer_version, offer_hash, expires_at, version|同一アカウント同士を拒否|
|trade_offers|trade_id, account_id, gold, confirmed_version|複合主キー。提示変更時に双方同意を解除|
|trade_items|trade_id, item_id, offered_by|itemsの予約がこのtradeを指すことを確定時に検証|
|market_listings|id, seller_id, item_id, price, listing_fee, status, buyer_id, expires_at, version|ACTIVEのitem_id一意。価格1〜10^9G。status/price/id索引|
|ledger_transactions|id, operation_id, reason, currency, created_at|確定後は追記だけ。操作内の連番と組み合わせて一意|
|ledger_entries|id, transaction_id, owner_type, owner_id, amount|通貨ごとに取引の総和0。発行/焼却もシステム口座へ記録|
|item_events|id, item_id, operation_id, event_type, before_owner, after_owner, metadata|生成・移動・予約・合成・消費を追跡。item/time索引|
|parties|id, leader_id, status, plan_version, plan, version|編成・作戦変更は権限確認|
|party_members|party_id, character_id, squad, ready_version, left_at|在籍中character_id一意。通常4人/レイド12人|
|guilds|id, world_id, name, master_id, version|同worldの正規化名は一意|
|guild_members|guild_id, character_id, role, joined_at|character_id一意、最大50人|
|blocks|account_id, target_id|複合主キー。自己ブロック拒否|
|chat_messages|id, channel_id, sender_id, body, created_at|channel/time/id索引。表示名・権限を送信者入力から採用しない|
|reports|id, reporter_id, target_id, reason, status, evidence_refs|通報対象の証跡を別保存|
|quest_progress|account_id, quest_id, progress, completed_at, reward_operation_id|複合主キー。初回報酬は一度だけ|
|content_versions|version, sha256, schema_version, status, released_at|version主キー。使用中データを上書きしない|
|job_queue|id, kind, aggregate_id, next_at, lease_until, lease_token, attempts, status|kind/aggregate一意、status/next_at索引|
|outbox|id, aggregate_id, event_type, payload, published_at, attempts|未送信のcreated_at索引|
|admin_audit|id, actor_id, action, reason, target_id, before_hash, after_hash, time|運営操作の追記記録|

## 実装前の整合条件

FOREIGN KEY/一意/上限だけで複数集約の所有一致や原子的確定が完成するとはしない。活動出撃・品予約・Version・receiptの競合を共通更新経路で扱い、古いworkerの結果を拒否する。session/chat/guild/moderation/content/work queueは各責務Taskへ渡し、旧表を現行仕様の第二台帳にしない。大会データは現行PVP_TOURNAMENTの別提案へ接続する。実DBの制約/索引/lock/recovery試験は未実行。
