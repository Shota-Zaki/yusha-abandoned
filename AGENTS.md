# 勇者アバンド — Repository Entry

## 正本

- 作業ブランチ: `work`。
- 状態復元: `docs/rules/RULES_SOURCE.md` → `docs/project/TASKS.md` → `NEXT_WORK.md` → `AI_WORK_STATE.md` → 現在Taskに関係する設計だけ。
- 企画: `docs/project/PROJECT_BRIEF.md`。
- 設計: `docs/design/REQUIREMENTS.md`、`BASIC_DESIGN.md`、`DETAILED_DESIGN.md` とその参照先。
- 最新の明示的なユーザー指示、Repositoryの決定、仮決定の順で扱う。仮決定は承認済みの事実として扱わない。

## プロジェクト固有

50職、初級職の終盤での用途、職業固定スキル、頭・胴・腕・脚・武器の5部位、合成前の交換、合成後の本人専用化、物理・魔法双方のMP、8属性、オート戦闘、全滅後300秒の出撃制限を維持する。

Steamworksの利用と、運営するゲーム処理サーバーを区別する。Steamだけで任意のMMO処理を無料実行できる前提を置かない。外部ホスティングはD01の確認対象である。

ゲームの所有権、報酬、乱数、時刻、戦闘結果はサーバーを正本とする。クライアントの計算結果を経済へ採用しない。

初級／上級／複合は解放の難しさであり、無条件の性能倍率ではない。スキル継承と全職マスターによる累積ステータス加算は採用しない。

ローカル検証可能なTaskを優先し、Steam AppIDや本番サーバーの未確定を理由に独立Taskを停止しない。設計、実装、検証、レビューを区別する。

作業末尾にTASKS/NEXT_WORK/AI_WORK_STATEを同期する。`main` へは公開対象だけを選別する。
