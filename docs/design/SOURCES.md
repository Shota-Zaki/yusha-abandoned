# 外部仕様・名称の参照元

確認日: 2026-09-13。外部サービスの仕様と、このゲーム独自の設計値を区別する。

|ID|参照元|採用する事実・用途|
|---|---|---|
|S01|https://partner.steamgames.com/doc/features/multiplayer/game_servers|専用ゲームサーバーの実行環境は開発者等が用意する|
|S02|https://partner.steamgames.com/doc/features/multiplayer/steamdatagramrelay|SDRはP2P/専用サーバー向け通信中継。汎用ホスティングではない|
|S03|https://partner.steamgames.com/doc/features/auth|Steam認証チケットとバックエンドによるユーザー・所有確認|
|S04|https://partner.steamgames.com/doc/features/inventory|Steam Inventoryのサーバーレス方式と信頼済みサーバー方式の区別|
|S05|https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/index.html|GodotのC#/.NET対応。Web書き出しの制約も確認|
|S06|https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets|ASP.NET CoreのWebSocket対応。実装時に採用バージョンのページで再確認|
|S07|https://www.postgresql.org/docs/current/explicit-locking.html|行ロック・デッドロック・ロック順序の実装根拠|
|S08|https://kakkoii-yougosyuu.com/archives/english-nature.html|ユーザー指定の命名資料。自然の英語から音を変えた魔法名を作成|
|S09|https://kakkoii-yougosyuu.com/archives/english-lightdark.html|指定資料からリンクされた光・闇の命名資料|

名称・ゲーム効果・職業構成・係数・育成上限・取引手数料は本設計独自の提案であり、参照サイトが保証する数値ではない。文章や一覧の転載ではなく、ごく少数の一般語を発想源としている。

名称の既存作品との重複や商標調査は別途行う。少し音を変えたことだけで権利上の問題が無いと断定しない。ドット絵・音声・フォントはライセンス台帳を持つ。
