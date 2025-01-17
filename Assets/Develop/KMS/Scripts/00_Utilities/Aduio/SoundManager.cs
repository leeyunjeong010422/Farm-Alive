using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // BGM types
    [System.Serializable]
    public class BGMInfo
    {
        [Tooltip("BGM를 구분할 키")]
        public string key;
        [Tooltip("BGM AudioClip")]
        public AudioClip clip;
    }

    [System.Serializable]
    public class SFXInfo
    {
        [Tooltip("SFX를 구분할 키")]
        public string key;
        [Tooltip("SFX AudioClip")]
        public AudioClip clip;
    }

    [Header("BGM 설정")]
    [Tooltip("BGM 정보 배열")]
    public BGMInfo[] bgmInfo;

    [Header("SFX 설정")]
    [Tooltip("SFX 정보 배열")]
    public SFXInfo[] sfxInfo;

    [Header("BGM Audio Source")]
    public AudioSource bgm;
    public AudioSource sfx;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public AudioMixerGroup loopSFXGroup;

    private Dictionary<string, AudioClip> _bgmDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _sfxDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioSource> _sfxLoopDict = new Dictionary<string, AudioSource>();
    
    private Coroutine sfxStopCoroutine;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 원하는 BGM을 재생합니다.
    /// </summary>
    /// <param name="key">재생을 원하는 BGM</param>
    public void PlayBGM(string key, float volumeScale = 1f)
    {
        if (!_bgmDict.ContainsKey(key))
        {
            Debug.LogWarning($"BGM '{key}'가 없습니다!");
            return;
        }

        bgm.clip = _bgmDict[key];
        bgm.volume = volumeScale;
        bgm.Play();
    }

    /// <summary>
    /// 재생 중인 BGM을 정지합니다.
    /// </summary>
    public void StopBGM()
    {
        bgm.Stop();
    }


    private void Start()
    {
        // bgm 딕셔너리 초기화
        foreach (var bgm in bgmInfo)
        {
            _bgmDict.Add(bgm.key, bgm.clip);
        }

        Debug.Log("BGM 딕셔너리 초기화 완료");

        // SFX 딕셔너리 초기화
        foreach (var sfx in sfxInfo)
        {
            _sfxDict.Add(sfx.key, sfx.clip);
        }

        Debug.Log("SFX 딕셔너리 초기화 완료");
    }

    /// <summary>
    /// SFX 재생
    /// </summary>
    /// <param name="key">재생할 SFX의 키</param>
    /// <param name="volumeScale">볼륨 크기</param>
    public void PlaySFX(string key, float volumeScale = 1f)
    {
        if (!_sfxDict.ContainsKey(key))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"SFX '{key}'가 없습니다!");
#endif
            return;
        }

        AudioClip clip = _sfxDict[key];
        sfx.PlayOneShot(clip, volumeScale);
    }

    /// <summary>
    /// SFX 재생 후 duration 시간 뒤에 멈춤
    /// </summary>
    /// <param name="key">재생할 SFX의 키</param>
    /// <param name="duration">지속 시간</param>
    /// <param name="volumeScale">볼륨 크기</param>
    public void PlaySFX(string key, float duration, float volumeScale = 1f)
    {
        if (!_sfxDict.ContainsKey(key))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"SFX '{key}'가 없습니다!");
#endif
            return;
        }

        AudioClip clip = _sfxDict[key];
        sfx.PlayOneShot(clip, volumeScale);

        if (duration > 0)
        {
            if (sfxStopCoroutine != null)
            {
                StopCoroutine(sfxStopCoroutine);
            }
            sfxStopCoroutine = StartCoroutine(StopSFXAfter(duration));
        }
    }

    /// <summary>
    /// 특정 SFX를 루프 재생
    /// </summary>
    public void PlaySFXLoop(string key, float volumeScale = 1f)
    {
        if (!_sfxDict.ContainsKey(key))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"SFX '{key}'가 없습니다!");
#endif
            return;
        }

        // 이미 루프 중이면 종료
        if (_sfxLoopDict.ContainsKey(key))
        {
#if UNITY_EDITOR
            Debug.Log($"SFX '{key}'가 이미 루프 중입니다!");
#endif
            return;
        }

        // 새로운 AudioSource 생성 및 설정
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = _sfxDict[key];
        newSource.volume = volumeScale;
        newSource.loop = true;
        // 루프 SFX 전용 AudioMixerGroup 할당
        newSource.outputAudioMixerGroup = loopSFXGroup;
        newSource.Play();

        _sfxLoopDict[key] = newSource;
    }

    /// <summary>
    /// 루프 중인 특정 SFX를 중지하고 컴포넌트 삭제
    /// </summary>
    /// <param name="key">중지할 SFX의 키</param>
    public void StopSFXLoop(string key)
    {
        if (!_sfxLoopDict.ContainsKey(key))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"루프 중인 SFX '{key}'가 없습니다!");
#endif
            return;
        }

        AudioSource loopAudioSource = _sfxLoopDict[key];
        loopAudioSource.Stop();
        Destroy(loopAudioSource);

        _sfxLoopDict.Remove(key);
    }

    /// <summary>
    /// 모든 SFX를 정지합니다.
    /// </summary>
    public void StopAllSFX()
    {
        sfx.Stop();
    }

    private IEnumerator StopSFXAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        sfx.Stop();
    }
}
