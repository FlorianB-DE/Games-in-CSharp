using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource sfxSource = default;
    [SerializeField] private AudioMixer mixer;

    public void BGMToggle()
    {
        mixer.GetFloat("bgmVolume", out var volume);
        mixer.SetFloat("bgmVolume", volume < 0 ? 0 : -80);
    }

    public void SFXToggle()
    {
        mixer.GetFloat("sfxVolume", out var volume);
        mixer.SetFloat("sfxVolume", volume < 0 ? 0 : -80);
    }

    public void PlaySound()
    {
        sfxSource.Play();
    }
}
