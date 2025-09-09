using JetBrains.Annotations;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(menuName = "Cards/Enemy")]
    public class EnemyCard : ScriptableCard
    {
        #region Serialized Fields
        
        [CanBeNull] public Sprite weapon, sprite;
        public int attack, health, value;
        public float attackSpeed, moveSpeed;
        public bool bodyWeapon;
        public Vector2 weaponXY;

        #endregion
    }
}