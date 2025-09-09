using System;
using System.Collections.Generic;
using Scriptables;
using UnityEngine;
using Object = UnityEngine.Object;

public class CardHand
{
    private const float CardMargin = .025f, YOffset = .075f, StartYPosition = -3.5f;
    private readonly List<AbstractBuffingCard> _buffs;
    private readonly List<Card> _cards;
    public readonly FightingBoard Board;
    public readonly Deck Deck;
    private float _width;

    public CardHand(Deck deck)
    {
        Deck = deck;
        _buffs = new List<AbstractBuffingCard>();
        _cards = new List<Card>();
        Board = Object.FindObjectOfType<FightingBoard>();
    }

    public void AddCard(Card c)
    {
        _cards.Add(c);
        ReorganizeCards();
        c.gameObject.SetActive(true);
        if (_width == 0)
            _width = c.transform.lossyScale.x;
    }

    public void RemoveCard(Card c)
    {
        if (!_cards.Remove(c)) throw new ArgumentException("Card was not in List");
        ReorganizeCards();
    }

    /**
     * this method utilizes a lot of math. Some of it could be replaced with if/else statements but those tend to be
     * rather slow (and since this is an WebGL build that becomes even more important)
     * and mod 2 operations are very fast cause of compiler optimisations and operations on bitlevel
     */
    private void ReorganizeCards()
    {
        var xPosition = (_cards.Count + 1) % 2 * (_width / 2 + CardMargin);
        var relativeYPosition = StartYPosition;
        for (var i = 0; i < _cards.Count; i++)
        {
            var card = _cards[i];
            xPosition *= ((i + (_cards.Count + 1) % 2) % 2 - 0.5f) * 2; // gives -1 or 1
            SetCardPosition(card, xPosition, relativeYPosition);
            card.SetRendererOrder((_cards.Count - i) * 3);
            card.CalculateRotation();
            var oddity = (i + _cards.Count % 2) % 2;
            xPosition += oddity * Mathf.Sign(xPosition) * (_width + CardMargin);
            relativeYPosition += Mathf.Sign(StartYPosition) * oddity * YOffset * (1 << (i / 2)) * 2f;
        }
    }

    private static void SetCardPosition(Component c, float x, float y)
    {
        var pos = c.transform.position;
        pos.x = x;
        pos.y = y;
        // ReSharper disable once Unity.InefficientPropertyAccess
        c.transform.position = pos;
    }

    public void AddBuff(AbstractBuffingCard card)
    {
        _buffs.Add(card);
    }

    public void PlayCard(ScriptableCard card, Vector2 at)
    {
        Deck.Draw();
        
        for (var i = 0; i < _buffs.Count; i++)
            i = ExecuteBuff(i, card, at);

        switch (card)
        {
            case EnemyCard enemyCard:
                Board.SpawnEnemy(enemyCard, at);
                return;
            case MoneyCard moneyCard:
                Board.Player.AddCoins(moneyCard.value);
                return;
        }

        if (!(card is ExecuteableCard executableCard)) return;
        if (!(Activator.CreateInstance(Type.GetType(executableCard.Script)!) is ICard iCard))
            throw new MissingReferenceException("no script set");
        iCard.OnActivation(Board.GetPlayOutData(at, this));
    }

    private int ExecuteBuff(int i, ScriptableCard card, Vector2 at)
    {
        if (_buffs[i].Buff(card, Board, this, at))
            _buffs.RemoveAt(i--);
        // kinda janky code but it has to be reduced by one since otherwise it would skip an Element
        return i;
    }

    public void BossFight()
    {
        Deck.BossFight(_cards);
    }

    public bool HasCards()
    {
        return _cards.Count > 0;
    }
}