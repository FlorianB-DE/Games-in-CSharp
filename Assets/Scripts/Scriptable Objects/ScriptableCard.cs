using UnityEngine;

namespace Scriptables
{
    public abstract class ScriptableCard : ScriptableObject
    {
        #region Serialized Fields

        public string cardName, description;
        public Sprite cardPreview, cardBorder;

        #endregion
    }
}