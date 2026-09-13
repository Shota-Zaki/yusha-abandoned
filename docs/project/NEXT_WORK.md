# 次のWork Unit

## 現在の境界

基幹設計はworkに保存済み。データモデル・画面・運用・境界修正・コンテンツ補足は会話添付 yusha_abandoned_design_supplement_v0.1.1.zip にあり、全文のRepository統合は未完了。

## 再開

AGENTS → RULES_SOURCE → TASKS → AI_WORK_STATEを読む。補足を取得できる場合はEDGE_CASESとCONTENT_DETAILSを確認し、設計の統合状態を整理する。取得できない場合は内容を取得済みと装わず、YA-D03を保留として残す。

## 最初の実装Task: YA-01

Domain/Combat/Contractsの純粋C#ライブラリと、Godot .NET/ASP.NET Coreの構成を決め、公式資料で互換Versionを固定する。

最初のCombat.Advanceは1人対1体、通常攻撃、HP/MP、予定イベント、KOまで。受入条件は同じ入力と乱数位置で同じ結果、100msずつとまとめ処理の一致、物攻300・倍率1.5・防御200・無属性・会心なし・揺らぎ1.0で375ダメージ。

行動時間は最短間隔の適用後に100ms単位へ切上げる。全滅の300秒はサーバーが再現した全滅時刻から数える。これらは補足v0.1.1で修正された契約。

Steam AppIDや運営契約は純粋ロジックの作成を停止する条件にしない。実Steam接続はYA-12、本番負荷・復旧はYA-21で検証する。
