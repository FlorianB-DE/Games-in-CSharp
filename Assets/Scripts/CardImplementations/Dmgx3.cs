using Scriptables;
using UnityEngine;

public class Dmgx3 : AbstractBuffingCard
{
    public override bool Buff(ScriptableCard card, FightingBoard board, CardHand hand, Vector2 at)
    {
        if (!(card is EnemyCard enemyCard)) return false;

        board.AddBuff(new ICard.Buff(attack: enemyCard.attack * 3));
        return true;
    }
}

