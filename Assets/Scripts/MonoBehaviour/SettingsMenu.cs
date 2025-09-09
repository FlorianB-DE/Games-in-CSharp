using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle sfxToggle, bgmToggle;
    
    public AudioMixer mainMixer;

    private void Start()
    {
        mainMixer.GetFloat("masterVolume", out var masterVolume);
        mainMixer.GetFloat("sfxVolume", out var sfxVolume);
        mainMixer.GetFloat("bgmVolume", out var bgmVolume);
        GetComponentInChildren<Slider>().value = masterVolume;
        sfxToggle.SetIsOnWithoutNotify(sfxVolume >= 0);
        bgmToggle.SetIsOnWithoutNotify(bgmVolume >= 0);
    }

    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("masterVolume", volume);
    }
}