using Scriptables;
using UnityEngine;

public class Healthx3 : AbstractBuffingCard
{
    public override bool Buff(ScriptableCard card, FightingBoard board, CardHand hand, Vector2 at)
    {
        if (!(card is EnemyCard enemyCard)) return false;

        board.AddBuff(new ICard.Buff(health: enemyCard.health * 3));
        return true;
    }
}
