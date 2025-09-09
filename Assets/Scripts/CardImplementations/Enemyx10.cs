using Scriptables;
using UnityEngine;

public class Enemyx10 : AbstractBuffingCard
{
    public override bool Buff(ScriptableCard card, FightingBoard board, CardHand hand, Vector2 at)
    {
        if (card.GetType() != typeof(EnemyCard)) return false;

        board.SpawnEnemy((EnemyCard)card, at, true);
        board.SpawnEnemy((EnemyCard)card, at, true);
        board.SpawnEnemy((EnemyCard)card, at, true);
        board.SpawnEnemy((EnemyCard)card, at, true);
        board.SpawnEnemy((EnemyCard)card, at, true);
        board.SpawnEnemy((EnemyCard)card, at, true);
        board.SpawnEnemy((EnemyCard)card, at, true);
        board.SpawnEnemy((EnemyCard)card, at, true);
        board.SpawnEnemy((EnemyCard)card, at, true);
        return true;
    }
}