using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTavern : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text goldDisplay;
    [SerializeField] private List<GameObject> slots = new List<GameObject>();
    private List<int> _basecost = new List<int>() {10, 15, 25, 40, 45, 75, 250, 450};
    private List<int> _baseHp = new List<int>() {70, 75, 100, 250, 125, 80, 3000, 2000};
    private List<int> _baseDmg = new List<int>() {7, 20, 12, 15, 11, 24, 0, 45};
    private List<int> _baseSpeed = new List<int>() {75, 125, 100, 100, 100, 75, 50, 150};
    private List<int> _baseLoot = new List<int>() {1, 2, 2, 3, 2, 3, 100, 150};
    private List<string> _special = new List<string>()
    {
        "",
        "",
        "",
        "",
        "Special: ranged",
        "special: ranged",
        "Special: Summons 10 goblins every 7 seconds",
        "Special: ranged. Summons 2 Bowmen & 3 Warriors uppon Spawn."
    };
    
    private List<string> _cardnames = new List<string>()
    {
        "tavernUpgrade_Slime",
        "tavernUpgrade_Wolf",
        "tavernUpgrade_Goblin",
        "tavernUpgrade_Warrior",
        "tavernUpgrade_Bowman",
        "tavernUpgrade_Mage",
        "tavernUpgrade_GoblinKing",
        "tavernUpgrade_RobinHood"
    };

    private const float CostMulti = 1.15f;
    public const float TroopBoostMulti = 1.10f;

    private void Start()
    {
        for (var id = 0; id < slots.Count; id++) UpdateCardView(id);
    }

    private void ShowGold()
    {
        goldDisplay.text = PlayerPrefs.GetInt("gold") + "   ";
    }
    
    private void UpdateCardView(int id)
    {
        slots[id].GetComponentsInChildren<Button>()[1].interactable = PlayerPrefs.GetInt(_cardnames[id]) != 1;
        var description = $"Level:{PlayerPrefs.GetInt(_cardnames[id])}\n" +
                          $"Cost:{(int)(_basecost[id] * Mathf.Pow(CostMulti, PlayerPrefs.GetInt(_cardnames[id])))}\n\n" +
                          $"HP:{(int)(_baseHp[id] * Mathf.Pow(TroopBoostMulti, PlayerPrefs.GetInt(_cardnames[id])-1))}(+{(int)(_baseHp[id] * Mathf.Pow(TroopBoostMulti, PlayerPrefs.GetInt(_cardnames[id])) - _baseHp[id] * Mathf.Pow(TroopBoostMulti, PlayerPrefs.GetInt(_cardnames[id])-1))})\n" +
                          $"Speed:{_baseSpeed[id]}\n" +
                          $"DMG:{(int)(_baseDmg[id] * Mathf.Pow(TroopBoostMulti, PlayerPrefs.GetInt(_cardnames[id])-1))}(+{(int)(_baseDmg[id] * Mathf.Pow(TroopBoostMulti, PlayerPrefs.GetInt(_cardnames[id])) - _baseDmg[id] * Mathf.Pow(TroopBoostMulti, PlayerPrefs.GetInt(_cardnames[id])-1))})\n" +
                          $"LOOT:{_baseLoot[id] * PlayerPrefs.GetInt(_cardnames[id])}(+{_baseLoot[id]})\n\n" +
                          $"{SpecialDescription(id)}";
        slots[id].GetComponentsInChildren<TMP_Text>()[1].text = description;
        ShowGold();
    }

    private string SpecialDescription(int id)
    {
        if (id <= 5) return _special[id];
        return id switch
        {
            6 => $"Special: Summons {6 + 2 * PlayerPrefs.GetInt(_cardnames[id])}(+2) goblins every 7 seconds",
            7 =>
                $"Special: ranged. Summons {1 + PlayerPrefs.GetInt(_cardnames[id])}(+1) Bowmen & {2 + PlayerPrefs.GetInt(_cardnames[id])}(+1) Warriors uppon Spawn.",
            _ => null
        };
    }
    
    public void LvlUp(int id)
    {
        var gold = PlayerPrefs.GetInt("gold");
        if (gold >= (int)(_basecost[id] * Mathf.Pow(CostMulti, PlayerPrefs.GetInt(_cardnames[id]))))
        {
            PlayerPrefs.SetInt("gold", gold - (int) (_basecost[id] * Mathf.Pow(CostMulti, PlayerPrefs.GetInt(_cardnames[id]))));
            PlayerPrefs.SetInt(_cardnames[id], PlayerPrefs.GetInt(_cardnames[id]) + 1);
        }
        UpdateCardView(id);
    }
    
    public void LvlDown(int id)
    {
        var gold = PlayerPrefs.GetInt("gold");
        PlayerPrefs.SetInt("gold", gold + (int) (_basecost[id] * Mathf.Pow(CostMulti, PlayerPrefs.GetInt(_cardnames[id])-1) * 0.75f));
        PlayerPrefs.SetInt(_cardnames[id], PlayerPrefs.GetInt(_cardnames[id]) + -1);
        UpdateCardView(id);
    }
    
    public void CloseCanvas()
    {
        canvas.enabled = false;
        PlayerPrefs.SetInt("shopActive", 0);
    }
}
