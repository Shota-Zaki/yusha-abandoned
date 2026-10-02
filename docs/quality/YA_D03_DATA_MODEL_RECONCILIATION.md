# YA-D03 DATA_MODEL統合記録

開始work: `3c92e1b0c059cdffe8d6c492306a802f83cfac71`。入力: `docs/archive/design-supplement-v0.1.1.html` section-2。現行正本優先: AGENTS、OPEN_DECISIONS、ITEMS_ECONOMY、API、Combat core v1。

|旧section-2|今回のdisposition|
|---|---|
|1 型と所有権|DATA_MODEL§1へ論理識別/整数/UTCを統合。PostgreSQL唯一正本・物理型・保存期間はD01/物理設計/公開前の判断へ分離|
|2 エンティティと制約|§2へ所有権・経済集約を統合。owner_character_idとaccount財布を明確化。保存キャラ総数は決めず、活動キャラ/出撃の制約を区別。sessions/guild/chat等は未統合|
|3 予約一元化|§3へ同一品の排他条件を統合。既存locked_by/expedition_lock_idを無断で列置換しない|
|4 ロック順序|§4へ実装候補と不足する競合検討を明記。DB方式・retry定数を採用済みにしない|
|5 合成確定|§4へ現行ITEMS_ECONOMY/APIの一体確定・再取得条件を接続|
|6 直接交換・取引所|§2/4へ現行の原子的移転/受取枠を統合。価格・手数料・期限は既存ITEMS_ECONOMYを参照|
|7 コンテンツ入力|既存Job/Skill/Dungeon/AI契約の統合は残件。経済モデルへ新規カタログ仕様を混在させない|

成果物: `docs/design/DATA_MODEL.md`。設計の意味統合であり、DB/migration/security実装はなし。YA-D03全体はIn Progress。UI/OPERATIONS/ADR/CONTENT_DETAILS/CATALOG_REFERENCE、DATA_MODEL非経済集約/入力契約、既存EDGE_CASESの残項目は未完了。D01/D09、YA-01実機軽量性は変更しない。
