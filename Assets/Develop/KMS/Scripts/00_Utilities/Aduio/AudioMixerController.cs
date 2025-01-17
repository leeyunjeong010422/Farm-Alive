using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider voiceVolumeSlider;

    private const float MinVolumeDb = -80f;

    private void Awake()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        voiceVolumeSlider.onValueChanged.AddListener(SetVoiceVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MASTER", volume > 0 ? Mathf.Log10(volume) * 20 : MinVolumeDb);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", volume > 0 ? Mathf.Log10(volume) * 20 : MinVolumeDb);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume > 0 ? Mathf.Log10(volume) * 20 : MinVolumeDb);
    }

    public void SetVoiceVolume(float volume)
    {
        audioMixer.SetFloat("Voice", volume > 0 ? Mathf.Log10(volume) * 20 : MinVolumeDb);
    }
}
