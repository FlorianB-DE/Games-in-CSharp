using System.Collections;
using System.Collections.Generic;
using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public static CardHand Hand;

    #region Serialized Fields

    [SerializeField] private TMP_Text cardTitle, cardDescription, cardATK, cardHP;
    [SerializeField] private Image cardBackground, cardPreview;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Boss[] bosses;
    [SerializeField] private int handSize;
    [SerializeField] private List<ScriptableCard> cardPool;

    #endregion

    private List<Card> _cardsDeck;

    #region Event Functions

    private void Awake()
    {
        Hand = new CardHand(this);
    }

    private void Start()
    {
        _cardsDeck = new List<Card>(cardPool.Count);
        for (var i = 0; i < cardPool.Count; i++)
        {
            _cardsDeck.Add(Instantiate(cardPrefab, transform.position, Quaternion.identity).GetComponent<Card>());
            _cardsDeck[i].gameObject.SetActive(false);
        }

        StartCoroutine(SlowDraw());
    }

    private void Update()
    {
        if (Hand.HasCards() || Hand.Board.HasEnemies()) return;
        Hand.BossFight();
    }

    #endregion

    private IEnumerator SlowDraw()
    {
        for (var i = 0; i < handSize; i++)
        {
            Draw();
            yield return new WaitForSeconds(.25f);
        }
    }
    
    public void BossFight(List<Card> handCards)
    {
        _cardsDeck.AddRange(handCards);
        handCards.Clear();
        foreach(var card in _cardsDeck)
            Destroy(card.gameObject);
        _cardsDeck.Clear();
        enabled = false;
        Hand.Board.SpawnEnemy(bosses[Random.Range(0, bosses.Length)], new Vector2(-3.5f, -.5f));
    }
    
    public void Draw()
    {
        if (_cardsDeck.Count == 0) return;
        var returnCard = _cardsDeck[0];
        _cardsDeck.RemoveAt(0);
        Hand.AddCard(returnCard);
        returnCard.AssignCard(DrawFromPool());
    }

    public void DisplayCard(ScriptableCard card)
    {
        cardBackground.transform.parent.gameObject.SetActive(true);
        cardBackground.sprite = card.cardBorder;
        cardPreview.sprite = card.cardPreview;
        cardTitle.text = card.cardName;
        cardDescription.text = card.description;
        var level = PlayerPrefs.HasKey("tavernUpgrade_" + card.cardName)
            ? PlayerPrefs.GetInt("tavernUpgrade_" + card.cardName)
            : 1;
        cardATK.text = card.GetType() == typeof(EnemyCard) ? $"ATK\n{(int) (((EnemyCard) card).attack * Mathf.Pow(ShopTavern.TroopBoostMulti, level))}" : "";
        cardHP.text = card.GetType() == typeof(EnemyCard) ? $"HP\n{(int) (((EnemyCard) card).health * Mathf.Pow(ShopTavern.TroopBoostMulti, level))}" : "";
        StartCoroutine(InputWatcher());
    }

    private IEnumerator InputWatcher()
    {
        while (!Input.GetMouseButtonDown(0))
            yield return null;
        cardBackground.transform.parent.gameObject.SetActive(false);
    }

    private ScriptableCard DrawFromPool(bool heartOfTheCards = false)
    {
        var index = Random.Range(0, cardPool.Count);
        var card = cardPool[index];
        cardPool.RemoveAt(index);
        return card;
    }
    
}