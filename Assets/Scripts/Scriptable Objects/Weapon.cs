using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(menuName = "Weapon")]
    public class Weapon : ScriptableObject
    {
        #region Serialized Fields

        public string weaponName;
        public int attack;
        public bool meele;
        public Sprite[] sprites;

        #endregion
    }
}