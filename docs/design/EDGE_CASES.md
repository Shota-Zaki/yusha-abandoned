# 境界条件 — 現行確定契約と旧初期案

YA-D03 / 2026-10-02。[COMBAT_CORE_V1](COMBAT_CORE_V1.md)、[REQUIREMENTS](REQUIREMENTS.md)、[WORLD_MMO](WORLD_MMO.md)、[DATA_MODEL](DATA_MODEL.md)を優先する。旧section-4の全7節を参照可能な設計へ統合し、現在実装の証明と将来案を分離する。

1. 全滅制限は公式イベント+300秒。追いつき/再ログイン/再計算呼出時刻を起点にしない。Combatv1の確定契約であり、account admissionは別実装。
2. 通常攻撃は最短800msを適用後100ms切上げ。将来のcast/CD/持続/移動/消耗品までv1で検証済みとはしない。
3. battle=1room、run=4通常+boss、expedition=最大8時間の指示。旧KO味方の次room HP20%復帰/MP維持、次run全回復、room終了effect解除、CD継続、復活1room1人1回/G01は1run1回を**将来の調整初期案**として保持。実装/受入やユーザー個別承認としない。全員KO判定を個別復帰より先に行う案も同じ範囲。
4. 出撃前の全員同意/品/残高/枠とaccount活動一意は現行条件。旧指示終了/精算、新指示開始、lease/version/expedition一致の競合実装はYA-07/13の共通更新経路で設計・試験する。
5. 最大8時間と完了済み報酬の保持は現行条件。期限と同時刻までの完了/KOを先に処理し新行動/room/run開始を拒否する旧案を**expedition wrapperの初期案**として保持。現在のDETAILED_DESIGN§8「期限前」の解釈差分を黙って変更しない。exact-deadline仕様をYA-07設計で確定し同時刻全滅/完了/新開始を試験する。
6. RNG HMAC/rejection-sampling/counterはCombatv1に既存採用済み。鍵/暗号codeは今回変更しない。room/run/drop/gear/pityの接続は別実装・検証で、通常攻撃の受入だけから成功を推定しない。
7. 旧発想源語数/毒刃表記は現行SKILLSの職ID/効果/段階名を正本とする。今回scriptでJ31A2のPBと全300プログラム、32段階名の一致を確認。発想源語の数え方を戦闘挙動へ影響させない。旧係数をバランス承認済みにしない。

DNG12目的達成不能の失敗終了・全員cooldownはCONTENT_DETAILSの旧案とWORLD_MMOの条件差が残る。採用する場合は要件への影響と条件を対応Taskで明示し、今回の設計統合によって自動採用しない。全滅/期限/目的失敗を同じイベントとして推測実装しない。

実行済み証跡は元のYA-01固定SHAに限定する。DB、room/run wrapper、画面、復旧、負荷は未検証。
