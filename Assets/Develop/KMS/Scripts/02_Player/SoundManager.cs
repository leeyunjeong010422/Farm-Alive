using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // BGM types
    public enum E_BGM
    {
        LOGIN,
        LOBBY,
        ROOM,
        GAME,
        SIZE_MAX
    }


    [Header("Audio Clips")]
    public AudioClip[] clipBgm;

    [System.Serializable]
    public class SFXInfo
    {
        public string key;
        public AudioClip clip;
    }

    [Header("Audio Source")]
    public AudioSource audioBgm;

    [Header("효과음 목록")]
    public SFXInfo[] sfxArr;

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private Dictionary<int, AudioSource> playerVoices = new Dictionary<int, AudioSource>();
    private int localPlayerActorNumber;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 로컬 플레이어의 ActorNumber 저장
        localPlayerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        foreach (var info in sfxArr)
        {
            if (!sfxDict.ContainsKey(info.key))
                sfxDict.Add(info.key, info.clip);
            else
                Debug.LogWarning($"중복된 SFX 키 값 : {info.key}");
        }
#if UNITY_EDITOR
        Debug.LogWarning($"효과음 초기화 완료!");
#endif
    }

    // BGM 재생 및 볼륨 설정
    public void PlayBGM(E_BGM bgmIdx)
    {
        audioBgm.clip = clipBgm[(int)bgmIdx];
        audioBgm.Play();
    }

    public void StopBGM()
    {
        if (audioBgm.isPlaying)
        {
            audioBgm.Stop();
        }
    }

    public void SetBGMVolume(float volume)
    {
        if (audioBgm.isPlaying)
        {
            audioBgm.volume = volume;
        }
    }

    // SFX 재생 및 볼륨 설정
    public void PlaySFX(string key)
    {
        if (sfxDict.ContainsKey(key))
            AudioSource.PlayClipAtPoint(sfxDict[key], Vector3.zero);
        else
            Debug.LogWarning($"효과음 키값{key}을 찾을 수 없습니다.");
    }

    public void SetSFXVolume(float volume)
    {
        foreach (var audioSource in playerVoices.Values)
        {
            audioSource.volume = volume;
        }
    }

    // 플레이어 음성 등록
    public void RegisterPlayerVoice(PhotonView photonView, AudioSource voiceSource)
    {
        if (photonView != null)
        {
            int actorNumber = photonView.Owner.ActorNumber;

            if (!playerVoices.ContainsKey(actorNumber))
            {
                playerVoices[actorNumber] = voiceSource;
                Debug.Log($"SoundManager: Actor {actorNumber} 음성 등록");
            }
        }
    }

    // 플레이어 음성 제거
    public void UnregisterPlayerVoice(PhotonView photonView)
    {
        if (photonView != null)
        {
            int actorNumber = photonView.Owner.ActorNumber;

            if (playerVoices.ContainsKey(actorNumber))
            {
                playerVoices.Remove(actorNumber);
                Debug.Log($"SoundManager: Actor {actorNumber} 음성 제거");
            }
        }
    }

    // 특정 캐릭터의 음량 설정
    public void SetPlayerVolume(int actorNumber, float volume)
    {
        if (playerVoices.ContainsKey(actorNumber))
        {
            playerVoices[actorNumber].volume = volume;
            Debug.Log($"Actor {actorNumber} 음량 조정: {volume}");
        }
    }

    // 모든 캐릭터의 정보를 반환 (UI 업데이트에 사용)
    public Dictionary<int, AudioSource> GetAllPlayerVoices()
    {
        return new Dictionary<int, AudioSource>(playerVoices);
    }
}
