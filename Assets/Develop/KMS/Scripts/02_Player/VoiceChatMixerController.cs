using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VoiceChatMixerController : MonoBehaviour
{
    public AudioMixer voiceChatMixer;
    public string volumeParameter = "RemotePlayerVoice";

    public void SetVoiceChatVolume(float volume)
    {
        voiceChatMixer.SetFloat(volumeParameter, Mathf.Log10(volume) * 20);
    }

    public void OnSliderValueChanged(float value)
    {
        SetVoiceChatVolume(value);
    }
}
