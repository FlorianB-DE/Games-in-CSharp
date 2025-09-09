using System;
using System.Diagnostics.CodeAnalysis;
using Scriptables;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Card : MonoBehaviour
{
    private const int MaxAngle = 100;

    #region Serialized Fields

    // class vars
    [SerializeField] private ScriptableCard _card;

    #endregion

    private Vector3? _oldPos;
    private Quaternion? _oldRotation;

    private SpriteRenderer _spriteRenderer, _ornaments, _preview;

    #region Event Functions

    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ornaments = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _preview = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }


    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    private void OnMouseDrag()
    {
        _oldRotation ??= transform.rotation;
        _oldPos ??= transform.position;
        var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = transform.position.z;
        transform.position = newPos;
        transform.rotation = Quaternion.identity;
    }

    private void OnMouseOver()
    {
        if(!Input.GetMouseButtonDown(1)) return;
        Deck.Hand.Deck.DisplayCard(_card);
    }

    private void OnMouseUp()
    {
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Deck.Hand.Board.Bounds.Contains(mouseWorldPosition))
        {
            Deck.Hand.RemoveCard(this);
            Deck.Hand.PlayCard(_card, mouseWorldPosition);
            Destroy(gameObject);
            return;
        }

        // was not inside: return to start position
        if (_oldPos is null || _oldRotation is null) return;
        transform.position = (Vector3) _oldPos;
        // ReSharper disable once Unity.InefficientPropertyAccess
        transform.rotation = (Quaternion) _oldRotation;

        _oldPos = null;
        _oldRotation = null;
    }

    #endregion

    public void SetRendererOrder(int priority)
    {
        _spriteRenderer.sortingOrder = priority;
        _preview.sortingOrder = priority + 1;
        _ornaments.sortingOrder = priority + 2;
    }

    public void AssignCard(ScriptableCard card)
    {
        _card = card;
        _preview.sprite = _card.cardPreview;
        _spriteRenderer.sprite = _card.cardBorder;
    }

    public void CalculateRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0,
            MaxAngle - MaxAngle * 2 * Camera.main.WorldToViewportPoint(transform.position).x);
    }
}