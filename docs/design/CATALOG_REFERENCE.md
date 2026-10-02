# カタログ参照と入力契約

YA-D03 / 2026-10-02。旧HTML section-1の職業/スキル/魔法一覧を別台帳へ複製せず、現行正本へ接続する。

|対象|現行正本|静的照合範囲|
|---|---|---|
|50職、20初級/20上級/10複合、解放前提|[JOBS](JOBS.md)|J01–J50の名前・区分境界、上級/複合前提を旧表示名から現行IDへ解決|
|各職4能動+2受動、300固定系統|[SKILLS](SKILLS.md) §5|旧6プログラムの順序を現行A1–A4/P1–P2と全職比較|
|8属性×4段階の32魔法名|[SKILLS](SKILLS.md) §2|旧名前と現行Rank1/5/10/20の一致。発想源語数や表の補助列を魔法名に混ぜない|
|地域/難易度/敵基準|[WORLD_MMO](WORLD_MMO.md)|旧CONTENT_DETAILSの調整初期案は本体の数値を上書きしない|
|支給/消耗品/敵行動/初期ビルド|[CONTENT_DETAILS](CONTENT_DETAILS.md)|旧提案の保存先。実装済みカタログや必須機能数と区別|

## 旧DATA_MODEL§7の入力形状

以下は将来のデータ駆動実装へ渡す設計初期案。現在JSON runtime schemaやパーサーを実装済みとはしない。

- JobDef: id/tier/requirements/profile/allowed_weapons/identity_effect/active_skill_ids[4]/passive_skill_ids[2]。職外の習得状態を共有しない。
- SkillDef: id/job_id/kind/program/element/max_rank=20/evolution_ranks=[1,5,10,20]/stage_names[4]/coefficients_bp[4]/mp_costs[4]/cooldown_ms/effect_group/target_rule。
- DungeonDef: id/level/unlock_requirements/rooms/difficulty_weights/enemies/reward_rules/time_limits。
- AI rule: priority0–7、skill_id、condition_code、operator、threshold、target_code、stage_mode、reserve_mp_percent0–100。任意ユーザースクリプト入力を許す設計へ変えない。

全参照の存在、一意ID、前提循環、ポイント予算、抽選表合計、由来限定スキル、数値上限の検証後にのみ定義Versionを有効化する設計とする。旧定義を動作中の作戦へ途中投入しない。実ローダー・不正定義の拒否・Version切替はYA-02等で実装/検証する。

## 今回の確認

`python3 scripts/check-supplement-catalog.py` は、読み取りだけで上表の職50/固定系統300/段階名32をsource snapshotと照合する。旧37テストや現在のCombat13件を再実行するものではなく、性能・戦闘・MP・経済・入力拒否の受入を証明しない。
