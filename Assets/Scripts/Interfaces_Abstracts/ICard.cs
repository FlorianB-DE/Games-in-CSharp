using UnityEngine;

public interface ICard
{
    void OnActivation(PlayOutData data);

    public class PlayOutData
    {
        public readonly CardHand Hand;
        public readonly Vector2 PlayedPosition;
        public readonly Player Player;

        public PlayOutData(Player player, Vector2 playedPosition, CardHand hand)
        {
            Player = player;
            PlayedPosition = playedPosition;
            Hand = hand;
        }
    }

    public class Buff
    {
        public readonly int Attack, Health, Speed, Value;

        public Buff(int attack = 0, int health = 0, int speed = 0, int value = 0)
        {
            Attack = attack;
            Health = health;
            Speed = speed;
            Value = value;
        }
    }
}