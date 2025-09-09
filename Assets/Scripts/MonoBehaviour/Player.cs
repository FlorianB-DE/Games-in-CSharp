using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scriptables;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IFightable
{
    private const float AttackCooldown = .5f;

    #region Serialized Fields

    [SerializeField] private Canvas gameOverScreen;
    [SerializeField] private int degrees;
    [SerializeField] private TMP_Text coinsUI, lvUI, healthUI, attackUpgradeUI, healthUpgradeUI, attackSpeedUpgradeUI;
    [SerializeField] private Slider xpUI;
    [SerializeField] [Range(0, 1)] private float xpMultiplier;
    [SerializeField] private Weapon meele, ranged;
    [SerializeField] private GameObject arrow;
    [SerializeField] private SceneLoader _sceneLoader;

    #endregion

    private readonly Dictionary<Upgrades, int> _upgrades = new Dictionary<Upgrades, int>(3);

    private float _attackCooldown;

    private FightingBoard _board;

    private int _coins, _xp, _level, _xpToNextLevel, _health, _upgradePoints;

    private SpriteRenderer _weaponRenderer;

    private Collider2D _collider2D;

    private Transform _weapon, _rightArm, _leftArm;

    [CanBeNull] private Weapon _lastUsed;

    #region Event Functions

    private void Start()
    {
        _weapon ??= transform.Find("Weapon");
        _rightArm ??= transform.Find("RightArm");
        _leftArm ??= transform.Find("LeftArm");

        AttachWeapon(_rightArm);

        _lastUsed = meele;

        _collider2D = GetComponent<Collider2D>();
        
        _weaponRenderer = _weapon.GetComponent<SpriteRenderer>();

        _weaponRenderer.sprite = WeaponSprite(meele);

            // stats
        _coins = 0;
        _xp = 0;
        _level = 1;
        _xpToNextLevel = 10;
        _health = (int)(100 *  Mathf.Pow(1.10f, (
            PlayerPrefs.HasKey("tavernUpgrade_Chest")
                ? PlayerPrefs.GetInt("tavernUpgrade_Chest")
                : 1)- 1)) + 
                  (int)(10 *  Mathf.Pow(1.10f, (
                      PlayerPrefs.HasKey("tavernUpgrade_Talisman")
                          ? PlayerPrefs.GetInt("tavernUpgrade_Talisman")
                          : 1)- 1));
        _attackCooldown = 0;
        _upgradePoints = 0;
        UpdateUI();

        // upgrades
        _upgrades.Add(Upgrades.Attack, 0);
        _upgrades.Add(Upgrades.AttackSpeed, 0);
        _upgrades.Add(Upgrades.Health, 0);
        UpdateUpgradesUI();
    }

    private Sprite WeaponSprite(Weapon w)
    {
        return w.sprites[PlayerPrefs.HasKey("tavernUpgrade_" + meele.weaponName) ? 
            (PlayerPrefs.GetInt("tavernUpgrade_" + meele.weaponName) - 1)/ 10: 0];
    }

    private void Update()
    {
        _attackCooldown = _attackCooldown - Time.deltaTime <= 0 ? 0 : _attackCooldown - Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var enemy = other.transform.GetComponentInParent<Enemy>();
        if (enemy is null) return;

        ReceiveDamage(enemy.Damage());
    }

    #endregion

    #region IFightable Members

    public int Damage()
    {
        if (_attackCooldown > 0) return 0;
        _attackCooldown = AttackCooldown / (_level + _upgrades[Upgrades.AttackSpeed]);
        // ReSharper disable once Unity.NoNullPropagation
        return _level + ((_lastUsed?.attack ?? 0) * (int)(10 *  Mathf.Pow(1.10f, (
            PlayerPrefs.HasKey("tavernUpgrade_" + _lastUsed?.weaponName ?? "Sword")
                ? PlayerPrefs.GetInt("tavernUpgrade_" + _lastUsed?.weaponName ?? "Sword")
                : 1)- 1))) + _upgrades[Upgrades.Attack];
    }

    #endregion

    public void Upgrade(int upgrade)
    {
        if (_upgradePoints <= 0) return;
        _upgradePoints--;
        _upgrades[(Upgrades) upgrade]++;

        UpdateUpgradesUI();
    }

    #region UI Interaction
    private void UpdateUpgradesUI()
    {
        attackUpgradeUI.text = _upgrades[Upgrades.Attack].ToString("00");
        healthUpgradeUI.text = _upgrades[Upgrades.Health].ToString("00");
        attackSpeedUpgradeUI.text = _upgrades[Upgrades.AttackSpeed].ToString("00");
    }
    
    private void UpdateUI()
    {
        coinsUI.text = _coins.ToString("0000");
        healthUI.text = _health.ToString("0000");
        lvUI.text = $"Lv.{_level}";
        xpUI.value = (float) _xp / _xpToNextLevel;
    }
    #endregion

    private void AttachWeapon([CanBeNull] Transform arm)
    {
        if (arm is null) return;
        var swordTransform = _weapon.transform;
        var previousParent = swordTransform.parent;
        swordTransform.Rotate(Vector3.forward, 180 * (previousParent == arm || previousParent == transform ? 0 : 1));
        swordTransform.SetParent(arm);
        swordTransform.position = arm.position;
    }

    public void LookAt([CanBeNull] Enemy enemy)
    {
        if (enemy is null) return;
        AttachWeapon(enemy.Direction().x > 0 ? _rightArm : enemy.Direction().x < 0 ? _leftArm : null);
        if ((enemy.transform.position - transform.position).sqrMagnitude > 2)
        {
            if (_attackCooldown <= 0) RangedAttack(enemy);
        }
        else
        {
            _lastUsed = meele;
            _weaponRenderer.sprite = WeaponSprite(meele);
        }
    }

    private void RangedAttack(Enemy enemy)
    {
        _lastUsed = ranged;
        _weaponRenderer.sprite = WeaponSprite(ranged);
        StartCoroutine(Shoot(enemy));
        enemy.SendMessage("OnTriggerStay2D", _collider2D);
    }

    public void SetFightingBoard(FightingBoard board)
    {
        if (!(_board is null)) throw new AccessViolationException("Board has already been set!");
        _board = board;
    }

    public void AddCoins(int amount)
    {
        _coins += amount;
        UpdateUI();
    }

    public void AddXP(int amount)
    {
        _xp += amount;
        if (_xp >= _xpToNextLevel) LevelUp();
        UpdateUI();
    }

    private void LevelUp()
    {
        _level++;
        _upgradePoints++;
        _xp -= _xpToNextLevel;
        _xpToNextLevel = CalculateNextXp();
    }

    public void AddHealth(int amount)
    {
        _health += amount;
        UpdateUI();
    }

    public void ReceiveDamage(int amount)
    {
        _health -= amount;
        if (_health <= 0)
            Die();
        UpdateUI();
    }

    private void Die()
    {
        _health = 0;
        gameOverScreen.gameObject.SetActive(true);
    }

    private IEnumerator Shoot(Enemy target)
    {
        var count = 0;
        if (!(_attackCooldown <= 0)) yield break;
        var arrowGameObject = Instantiate(arrow, transform.position, quaternion.identity);
        while ((arrowGameObject.transform.position - target.transform.position).sqrMagnitude > .1f || count++ > 1000)
        {
            arrowGameObject.transform.right = target.transform.position - arrowGameObject.transform.position;
            arrowGameObject.transform.Translate(Vector3.right * (Time.deltaTime * 10));
            yield return null;
        }
        Destroy(arrowGameObject);
    }

    private int CalculateNextXp()
    {
        return Mathf.RoundToInt(_xpToNextLevel * Mathf.Pow(_level, xpMultiplier));
    }

    #region Nested type: ${0}

    private enum Upgrades
    {
        Attack,
        Health,
        AttackSpeed
    }

    #endregion

    public void EndGame()
    {
        PlayerPrefs.SetInt("gold", _coins + (PlayerPrefs.HasKey("gold") ? PlayerPrefs.GetInt("gold") : 0));
        _sceneLoader.LoadTownScene();
    }
}