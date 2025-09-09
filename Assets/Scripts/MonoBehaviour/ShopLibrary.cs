using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopLibrary : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private List<Button> buttons= new List<Button>();
    [SerializeField] private TMP_Text goldDisplay;
    private Dictionary<string, int> _unlock = new Dictionary<string, int>();

    private void Start()
    {
        _unlock["libraryUpgrade_ID0"] = PlayerPrefs.GetInt("libraryUpgrade_ID0");
        _unlock["libraryUpgrade_ID1"] = PlayerPrefs.GetInt("libraryUpgrade_ID1");
        _unlock["libraryUpgrade_ID2"] = PlayerPrefs.GetInt("libraryUpgrade_ID2");
        _unlock["libraryUpgrade_ID3"] = PlayerPrefs.GetInt("libraryUpgrade_ID3");
        _unlock["libraryUpgrade_ID4"] = PlayerPrefs.GetInt("libraryUpgrade_ID4");
        _unlock["libraryUpgrade_ID5"] = PlayerPrefs.GetInt("libraryUpgrade_ID5");
        _unlock["libraryUpgrade_ID6"] = PlayerPrefs.GetInt("libraryUpgrade_ID6");
        _unlock["libraryUpgrade_ID7"] = PlayerPrefs.GetInt("libraryUpgrade_ID7");
        ShowUnlocked();
        ShowGold();
    }

    private void ShowUnlocked()
    {
        var i = 0;
        foreach (var entry in _unlock)
        {
            buttons[i++].interactable = !Convert.ToBoolean(entry.Value);
        }
    }

    private void ShowGold()
    {
        goldDisplay.text = PlayerPrefs.GetInt("gold") + "   ";
    }
    
    public void Buy(int id)
    {
        var gold = PlayerPrefs.GetInt("gold");
        switch (id)
        {
            case 0:
                if (gold >= 25)
                {
                    gold -= 25;
                    _unlock["libraryUpgrade_ID0"] = 1;
                    buttons[id].interactable = false;
                }
                break;
            case 1:
                if (gold >= 125)
                {
                    gold -= 125;
                    _unlock["libraryUpgrade_ID1"] = 1;
                    buttons[id].interactable = false;
                }
                break;
            case 2:
                if (gold >= 150)
                {
                    gold -= 150;
                    _unlock["libraryUpgrade_ID2"] = 1;
                    buttons[id].interactable = false;
                }
                break;
            case 3:
                if (gold >= 300)
                {
                    gold -= 300;
                    _unlock["libraryUpgrade_ID3"] = 1;
                    buttons[id].interactable = false;
                }
                break;
            case 4:
                if (gold >= 350)
                {
                    gold -= 350;
                    _unlock["libraryUpgrade_ID4"] = 1;
                    buttons[id].interactable = false;
                }
                break;
            case 5:
                if (gold >= 1500)
                {
                    gold -= 1500;
                    _unlock["libraryUpgrade_ID5"] = 1;
                    buttons[id].interactable = false;
                }
                break;
            case 6: break;
            case 7: break;
        }
        PlayerPrefs.SetInt("gold", gold);
        ShowGold();
    }
    public void CloseCanvas()
    {
        foreach (var entry in _unlock) PlayerPrefs.SetInt(entry.Key, entry.Value);
        PlayerPrefs.SetInt("shopActive", 0);
        canvas.enabled = false;
    }
}
