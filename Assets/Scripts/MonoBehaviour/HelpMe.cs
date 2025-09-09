using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMe : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.HasKey("firstStart")) return;
        PlayerPrefs.SetInt("firstStart", 1);
        gameObject.SetActive(true);
    }

    public void Open()
    {
        PlayerPrefs.SetInt("shopActive", 1);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        PlayerPrefs.SetInt("shopActive", 0);
        gameObject.SetActive(false);
    }
}
