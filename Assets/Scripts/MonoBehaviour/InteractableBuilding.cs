using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBuilding : MonoBehaviour
{
    [SerializeField] private Canvas buyMenu;
    private Animator _anim;
    private SpriteRenderer _sr;
    private Collider2D _collider2D;
    private void Awake()
    {
        _anim = this.transform.GetComponent<Animator>();
        _sr = this.transform.GetComponent<SpriteRenderer>();
        _collider2D = this.transform.GetComponent<Collider2D>();
        buyMenu.enabled = false;
    }

    private void OnMouseEnter()
    {
        _anim.enabled = false;
        _sr.color = new Color(0.4f, 0.4f, 0.4f, 0.4f);
    }

    private void OnMouseExit()
    {
        _sr.color = new Color(1f, 1f, 1f, 1f);
        _anim.enabled = true;
    }

    private void OnMouseUp()
    {
        if (PlayerPrefs.GetInt("shopActive") != 0) return;
        PlayerPrefs.SetInt("shopActive", 1); 
        buyMenu.enabled = true;
    }
}
