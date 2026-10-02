# データモデル — 所有権と経済の論理契約

YA-D03 / 2026-10-02。現行v0.2、[ITEMS_ECONOMY](ITEMS_ECONOMY.md) §§1,4–8、[API](API.md) §1を優先する。旧補足section-2のうち既存契約を支える論理モデルを統合する。DDL・migration・DB実装ではない。D01の完全P2P/最小公式判定基盤の選択、PostgreSQL採用、ホスティング費用はこの文書では確定しない。

## 1. 識別と状態の責務

品はテンプレートでなくitem_idで識別し、owner_character_idで所有キャラを表す。APIのactor accountから対象キャラへの関係を確認する。旧owner_idの曖昧なaccount/character混在を正本へ持ち込まない。bound_owner_idも同じcharacter IDの名前空間を使う。財布はaccount_id/currency単位。通貨は整数、Versionは単調な競合識別値、時刻はUTCとして扱い、DB固有の型は物理設計時に選ぶ。

1活動キャラは「1アカウントで活動できるキャラ数」の承認であり、旧characters.account_id一意を使って保存可能キャラ総数まで決めない。活動中の出撃参加はaccount単位で一意。キャラLv100/職Lv50と8時間の作戦上限を維持する。

## 2. 所有権・経済の集約

|集約|論理キー・主要状態|確定時の条件|
|---|---|---|
|items|item_id、owner_character_id、binding、bound_owner_id、location、locked_by、version、grant_id、content_version|BOUNDのbound_owner_idは所有キャラと一致し永久に固定。TRADEABLEのbound_owner_idは空。CONSUMEDは装備・交換・再消費不可|
|item_sockets|item_id/socket_index、skill_id、points、consumed_accessory_id|indexはsocket_count内。アクセサリー消費は一度だけ。上書きで旧品を返却しない|
|equipment|character_id/slot、item_id|指定5部位、同じ実体を複数部位へ装備しない。本人所有と武器制限を確認|
|wallets|account_id/currency、balance、reserved、version|0≤reserved≤balance。利用可能額はbalance−reserved。予約解除は残高を増やす処理ではない|
|material_stacks|owner_character_id/material_id/purity/binding、quantity、reserved、version|0≤reserved≤quantity。BOUNDとTRADEABLEを混ぜない|
|capacity_reservations|character_id/kind/reference_id、slots|現品数と有効予約数が該当箱の上限を超えない。出品返却分と購入受取分を区別|
|operations|account_id/Idempotency-Key、request_hash、status、receipt|同じactor/method/path/bodyは既存結果、内容違いは409。確定済み処理を再適用しない|
|syntheses|operation_id、equipment_id、accessory_id、input_versions、費用、結果|所有・街・Version・ソケット・予約・保護・残高の再確認後、消費とBOUND化を一体で確定|
|trades/offers|trade_id、双方account、offer_version/hash、同意、状態|提示変更で双方同意を解除。双方の品・通貨・枠を一体で移転|
|market_listings|listing_id、seller、item_id、price、status、buyer、version|ACTIVEの同一実体出品は一件。買う/取り下げ/期限終了の終端遷移は一度だけ|
|reward_grants|run/room/recipient/ordinal、payload、status|既存報酬の識別を保持し、再送/再取得で再抽選・二重発行しない|
|ledger/item_events/outbox|operation_id、イベント識別、確定結果|所有移転・消費・財布・台帳・通知予定を同じ確定単位へ含める。通知失敗で確定結果を取消さない|

これは制約を実装済みとする表ではない。旧sessions/chat/admin等はこの所有権checkpointの対象外で、セキュリティ方式を新規採用しない。

## 3. 予約・場所・保護を分離する

現行itemsのlocked_by、expedition_lock_idと取引予約は、同じ品を出撃・直接交換・出品へ同時利用させない論理条件へ接続する。locationは置き場所、bindingは移転可否、保護は誤消費防止。いずれも別の状態である。旧reservation_kind/idへの列置換はまだ実施せず、YA-07/13/16/17の共通更新経路設計でフィールド対応を確定する。

EQUIPPEDの品は解除してBAGへ移すまで交換不可。合成成功だけが通常ドロップ装備を永久BOUNDへ変える。初期支給品のBOUNDは既存の例外として維持する。予約取消はその操作が所有する予約だけを解放し、別操作の予約を消さない。

## 4. 確定と再取得

合成は見積時ではなく確定時に入力Version/期限/前提を再確認する。失敗では品・素材・通貨・bindingを変えない。交換は片側だけの移転を保存しない。市場購入は代金引落・売り手受取・手数料・品移転・枠・出品終端を一体で扱う。コミット後に接続や通知が失敗した場合は同じ操作IDでreceiptを返す。

旧補足の全表ロック順、READ COMMITTED/FOR UPDATE、SERIALIZABLE、最大3回再試行は物理実装候補として保持する。対象集合の変更、作成前で行のない予約、冪等operationの競合、期限待機中の時刻再確認を含む具体的な競合設計が必要で、旧一覧をそのまま実装安全性の証明にしない。

## 5. 実装時の受入シナリオ

|場面|必要な観測|
|---|---|
|合成と交換/出撃が同じ品を要求|一方だけが確定。拒否側は消費・BOUND化・予約を残さない|
|同じ合成操作を再送、同じキーで別入力|前者は同じreceiptで消費一回、後者は409で無変更|
|提示変更と双方confirmが競合|変更前の同意を変更後の提示へ流用しない|
|市場buy/cancel/expireが競合|所有者移転・返却は一つの終端結果に一致、二重入金なし|
|受取箱満杯、通知停止、応答断|枠不足で部分購入なし。確定後の通知停止/応答断は再取得・再送で回復|
|BOUND装備を解除/全ソケット上書き/分解|TRADEABLEへ戻らず、分解素材も既存BOUND契約を維持|
|出撃の再開/報酬の再受取|accountの活動出撃と報酬識別が一意、二重報酬なし|

未実行。DB競合、失敗注入、実経済受入は将来の対応Taskで検証する。純粋Combat13件や旧モデル37件をこの表のPASSへ転用しない。
