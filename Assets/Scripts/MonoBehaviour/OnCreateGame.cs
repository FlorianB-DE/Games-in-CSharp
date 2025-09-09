using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OnCreateGame : MonoBehaviour
{
    [SerializeField] private TMP_Text goldDisplay;
    private int _gold;

    private List<string> _libraryUpgrades = new List<string>()
    {
        "libraryUpgrade_ID0",
        "libraryUpgrade_ID1",
        "libraryUpgrade_ID2",
        "libraryUpgrade_ID3",
        "libraryUpgrade_ID4",
        "libraryUpgrade_ID5",
        "libraryUpgrade_ID6",
        "libraryUpgrade_ID7"
    };
    private List<string> _tavernUpgrades = new List<string>()
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
    private List<string> _blacksmithUpgrades = new List<string>()
    {
        "tavernUpgrade_Chest",
        "tavernUpgrade_Sword",
        "tavernUpgrade_Bow",
        "tavernUpgrade_Talisman"
    };

    
    private void Awake()
    {
        _gold = PlayerPrefs.HasKey("gold")? PlayerPrefs.GetInt("gold") : 0;
        PlayerPrefs.SetInt("gold", _gold);
        ShowGold();
        PlayerPrefs.SetInt("shopActive", 0);
        foreach (var key in _libraryUpgrades.Where(key => !PlayerPrefs.HasKey(key))) PlayerPrefs.SetInt(key, 0);
        foreach (var key in _tavernUpgrades.Where(key => !PlayerPrefs.HasKey(key))) PlayerPrefs.SetInt(key, 1);
        foreach (var key in _blacksmithUpgrades.Where(key => !PlayerPrefs.HasKey(key))) PlayerPrefs.SetInt(key, 1);
    }
    
    private void ShowGold()
    {
        goldDisplay.text = _gold + "   ";
    }

}
