using Scriptables;
using UnityEngine;

public abstract class AbstractBuffingCard : ICard
{
    public void OnActivation(ICard.PlayOutData data)
    {
        data.Hand.AddBuff(this);
    }

    // returns true when buff can be removed from current buffs list
    public abstract bool Buff(ScriptableCard card, FightingBoard board, CardHand hand, Vector2 at);
}