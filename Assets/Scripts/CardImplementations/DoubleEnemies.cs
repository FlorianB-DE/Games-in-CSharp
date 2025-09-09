using Scriptables;
using UnityEngine;

public class DoubleEnemies : AbstractBuffingCard
{
    public override bool Buff(ScriptableCard card, FightingBoard board, CardHand hand, Vector2 at)
    {
        if (!(card is EnemyCard enemyCard)) return false;

        board.SpawnEnemy(enemyCard, at, true);
        return true;
    }
}