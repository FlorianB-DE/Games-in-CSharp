using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadFightScene()
    {
        PlayerPrefs.SetInt("shopActive", 0);
        SceneManager.LoadScene(2);
    }

    public void LoadTownScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMainMenu()
    {
        PlayerPrefs.SetInt("shopActive", 0);
        SceneManager.LoadScene(0);
    }
}
