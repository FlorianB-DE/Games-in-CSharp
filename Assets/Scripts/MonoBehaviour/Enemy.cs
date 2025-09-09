using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Scriptables;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IFightable
{
    // static vars
    private static FightingBoard Board;

    // class vars
    private readonly List<ICard.Buff> _buffs = new List<ICard.Buff>();
    private EnemyCard _card;

    private int _health, _level;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private float _attackCooldown;
    
    [CanBeNull] private GameObject _weapon;

    public FightingBoard.EnemyQueueNode Node { get; private set; }

    #region Event Functions

    // Start is called before the first frame update
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Node = new FightingBoard.EnemyQueueNode(this);
        _weapon = transform.GetChild(0)?.gameObject;
        _health = 0;
        _level = 0;
    }

    private void Update()
    {
        if (_attackCooldown > 0)
            _attackCooldown = _attackCooldown - Time.deltaTime < 0 ? 0 : _attackCooldown - Time.deltaTime;
        Board.QueueUpdate(Node, (Board.Player.transform.position - transform.position).sqrMagnitude);
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity =
            (Board.Player.transform.position - transform.position).normalized * (_card.moveSpeed * Time.fixedDeltaTime);
    }

    // has to be player cause Enemy/Enemy Collision is disabled
    private void OnTriggerStay2D(Collider2D other)
    {
        if (_health <= 0) return;
        _health -= Board.Player.Damage();
        if (_card.bodyWeapon && other.gameObject != Board.Player.gameObject) Board.Player.ReceiveDamage(Damage());
        if (_health <= 0)
            Die();
    }

    public void AddBuff(ICard.Buff buff)
    {
        _buffs.Add(buff);
    }
    
    #endregion

    #region IFightable Members

    public int Damage()
    {
        if (_attackCooldown > 0) return 0;
        _attackCooldown = 1 / _card.attackSpeed;
        return _buffs.Aggregate(0, (acc, buff) => acc + buff.Attack) + 
               (int) (_card.attack * Mathf.Pow(ShopTavern.TroopBoostMulti, _level));
    }

    #endregion

    public void Spawn(EnemyCard card, Vector3 at)
    {
        _card = card;
        _level = PlayerPrefs.HasKey("tavernUpgrade_" + _card.cardName)
            ? PlayerPrefs.GetInt("tavernUpgrade_" + _card.cardName)
            : 1;
        _health = (int) (_card.health * Mathf.Pow(ShopTavern.TroopBoostMulti, _level));
        _spriteRenderer.sprite = _card.sprite;
        transform.position = at;
        gameObject.SetActive(true);
        if(_weapon is null) return;
        _weapon.transform.position = new Vector3(at.x + _card.weaponXY.x, at.y + _card.weaponXY.y, 0);
        _weapon.SetActive(!_card.bodyWeapon);
        _weapon.GetComponent<SpriteRenderer>().sprite = _card.weapon;
    }

    public float GetPriority()
    {
        return (Board.Player.transform.position - transform.position).sqrMagnitude;
    }

    public Vector2 Direction()
    {
        return _rigidbody2D.velocity.normalized;
    }

    public static void SetFightingBoard(FightingBoard board)
    {
        Board = board;
    }

    private void Die()
    {
        GiveLoot();
        _buffs.Clear();
        Board.ReturnToPool(this);
        gameObject.SetActive(false);
    }

    private void GiveLoot()
    {
        var value = _card.value * _buffs.Aggregate(1, (acc, buff) => acc + buff.Value);
        Board.Player.AddCoins(value);
        Board.Player.AddXP(value);
    }
}