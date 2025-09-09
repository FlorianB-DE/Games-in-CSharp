using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopBlacksmith : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text goldDisplay;
    [SerializeField] private List<GameObject> slots = new List<GameObject>();
    [SerializeField] private List<Sprite> icons = new List<Sprite>();
    private readonly List<string> _blacksmithUpgrades = new List<string>()
    {
        "tavernUpgrade_Chest",
        "tavernUpgrade_Sword",
        "tavernUpgrade_Bow",
        "tavernUpgrade_Talisman"
    };

    private readonly List<int> _baseCosts = new List<int>() {10, 15, 15, 25};
    private readonly List<int> _baseValue = new List<int>() {100, 35, 20, 10};

    private const float CostMulti = 1.11f;

    private void Start()
    {
        for (var id = 0; id < slots.Count; id++)
        {
            var level = PlayerPrefs.GetInt(_blacksmithUpgrades[id]);
            slots[id].GetComponentInChildren<TMP_Text>().text = (level) % 10 == 0 ? "Upgrade" : $"{(level)%10}/10";
            var images = slots[id].GetComponentsInChildren<Image>();
            slots[id].GetComponentInChildren<Slider>().value = (level) % 10 == 0 ? 10 : level % 10;
            images[images.Length - 1].sprite = icons[id * 4 + ((level-1)/10)];
            UpdateDescription(id, level);
            if (level == 40) 
            {
                slots[id].GetComponentInChildren<Button>().interactable = false;
                slots[id].GetComponentInChildren<TMP_Text>().text = "Max";
            }
        }
    }

    private void UpdateDescription(int id, int lvl)
    {
        var texts = slots[id].GetComponentsInChildren<TMP_Text>();
        var description = texts[texts.Length - 1];
        description.text = $"Level:{lvl}\n" +
                           $"Cost:{(int)(_baseCosts[id]*Mathf.Pow(CostMulti, lvl-1))}\n" +
                           $"\n" +
                           $"{GetSpecialDescription(id, lvl)}";
    }

    private string GetSpecialDescription(int id, int lvl)
    {
        if (lvl != 40)
        {
            return id switch
            {
                0 =>
                    $"HP:{(int)(_baseValue[id] *  Mathf.Pow(1.10f, lvl - 1))}(+{(int) (_baseValue[id] * Mathf.Pow(1.10f, lvl - 1) * 0.1f)})",
                1 =>
                    $"DMG:{(int)(_baseValue[id] * Mathf.Pow(1.10f, lvl - 1))}(+{(int) (_baseValue[id] * Mathf.Pow(1.10f, lvl - 1) * 0.1f)})",
                2 =>
                    $"DMG:{(int)(_baseValue[id] * Mathf.Pow(1.10f, lvl - 1))}(+{(int) (_baseValue[id] * Mathf.Pow(1.10f, lvl - 1) * 0.1f)})",
                3 => $"x2Loot:+{_baseValue[id] * lvl / 4f}%(+{_baseValue[id] / 4f})",
                _ => ""
            };
        }
        return id switch
        {
            0 =>
                $"HP:{(int)(_baseValue[id] * Mathf.Pow(1.10f, lvl - 1))}",
            1 =>
                $"DMG:{(int)(_baseValue[id] * Mathf.Pow(1.10f, lvl - 1))}",
            2 =>
                $"DMG:{(int)(_baseValue[id] * Mathf.Pow(1.10f, lvl - 1))}",
            3 => $"x2Loot:+{_baseValue[id] * lvl / 4f}%",
            _ => ""
        };
        
    }
    private void ShowGold()
    {
        goldDisplay.text = PlayerPrefs.GetInt("gold") + "   ";
    }

    public void UpdateProgressText(int id)
    {
        var gold = PlayerPrefs.GetInt("gold");
        var level = PlayerPrefs.GetInt(_blacksmithUpgrades[id]);
        if (level == 40) return;

        if (gold >= (int) (_baseCosts[id] * Mathf.Pow(CostMulti, level)))
        {
            PlayerPrefs.SetInt(_blacksmithUpgrades[id], level + 1);
            PlayerPrefs.SetInt("gold", gold - (int) (_baseCosts[id] * Mathf.Pow(CostMulti, level)));
            slots[id].GetComponentInChildren<TMP_Text>().text = (level + 1) % 10 == 0 ? "Upgrade" : $"{(level+1)%10}/10";
            slots[id].GetComponentInChildren<Slider>().value = (level+1) % 10 == 0 ? 10 : (level+1) % 10;
            var images = slots[id].GetComponentsInChildren<Image>();
            images[images.Length - 1].sprite = icons[id * 4 + (level/10)];
            UpdateDescription(id, level+1);
        }
        if (level+1 == 40) 
        {
            slots[id].GetComponentInChildren<Button>().interactable = false;
            slots[id].GetComponentInChildren<TMP_Text>().text = "Max";
        }
        ShowGold();
    }
    
    public void CloseCanvas()
    {
        canvas.enabled = false;
        PlayerPrefs.SetInt("shopActive", 0);
    }
}
