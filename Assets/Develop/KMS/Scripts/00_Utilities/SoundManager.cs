using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // BGM types
    public enum E_BGM
    {
        GAMESTART,
        LOBBY,
        ROOM,
        INGAME,
        LOADING,

        SIZEMAX
    }

    [System.Serializable]
    public class SFXInfo
    {
        [Tooltip("SFX를 구분할 키")]
        public string key;
        [Tooltip("SFX AudioClip")]
        public AudioClip clip;
    }

    [Header("BGM Clips")]
    public AudioClip[] bgmClips;

    [Header("SFX 설정")]
    [Tooltip("SFX 정보 배열")]
    public SFXInfo[] _sfxInfo;

    [Header("BGM Audio Source")]
    public AudioSource bgm;
    public AudioSource sfx;

    private Dictionary<string, AudioClip> _sfxDict = new Dictionary<string, AudioClip>();

    //[Header("Audio Setting")]
    //public AudioMixer test; // 오디오 믹서
    //public GameObject sliderUI; // 슬라이더 UI 패널
    //public XRNode rightControllerNode = XRNode.RightHand; // 오른손 컨트롤러

    //private float maxVolumeDb = 20f;
    //private float minVolumeDb = -80f;
    //private float buttonHoldTime = 1f; // 버튼을 눌러야 하는 시간
    //private float buttonHoldCounter = 0f; // 버튼 누름 시간 추적
    //private bool isSliderUIActive = false; // 슬라이더 UI 활성화 상태

    //private Transform _mainCamera; // 메인 카메라의 Transform
    //public float distanceFromCamera = 3f; // 카메라에서 슬라이더 UI가 떨어질 거리

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
    /// <param name="bgmIdx">재생을 원하는 BGM</param>
    public void PlayBGM(E_BGM bgmIdx)
    {
        bgm.clip = bgmClips[(int)bgmIdx];
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
        //_mainCamera = Camera.main.transform;

        //if (!_mainCamera)
        //{
        //    Debug.LogError("Main Camera를 찾을 수 없습니다!");
        //}

        //if (sliderUI)
        //{
        //    sliderUI.SetActive(false);
        //}

        // SFX 딕셔너리 초기화
        foreach (var sfx in _sfxInfo)
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
            Debug.LogWarning($"SFX '{key}'가 없습니다!");
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
            Debug.LogWarning($"SFX '{key}'가 없습니다!");
            return;
        }

        AudioClip clip = _sfxDict[key];
        sfx.PlayOneShot(clip, volumeScale);

        if (duration > 0)
        {
            StartCoroutine(StopSFXAfter(duration));
        }
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

//    public void SetBGMVolume(float volume)
//    {
//        float dBValue = Mathf.Lerp(minVolumeDb, maxVolumeDb, volume);
//        test.SetFloat("PlayerVoiceVolum", dBValue);
//    }

//    private void Update()
//    {
//        if (IsControllerButtonPressed(rightControllerNode, CommonUsages.primaryButton))
//        {
//            buttonHoldCounter += Time.deltaTime;

//            if (buttonHoldCounter >= buttonHoldTime)
//            {
//                ToggleSliderUI();
//                buttonHoldCounter = 0f;
//            }
//        }
//#if UNITY_EDITOR
//        else if (Input.GetKey(KeyCode.Slash))
//        {
//            buttonHoldCounter += Time.deltaTime;

//            if (buttonHoldCounter >= buttonHoldTime)
//            {
//                ToggleSliderUI();
//                buttonHoldCounter = 0f;
//            }
//        }
//#endif
//        else
//        {
//            buttonHoldCounter = 0f;
//        }

//        if (isSliderUIActive && sliderUI != null && _mainCamera != null)
//        {
//            UpdateSliderUIPosition();
//        }
//    }

//    private void ToggleSliderUI()
//    {
//        isSliderUIActive = !isSliderUIActive;

//        if (sliderUI != null)
//        {
//            sliderUI.SetActive(isSliderUIActive);

//            if (isSliderUIActive)
//            {
//                UpdateSliderUIPosition();
//            }
//        }
//    }

//    private void UpdateSliderUIPosition()
//    {
//        Vector3 newPosition = _mainCamera.position + _mainCamera.forward * distanceFromCamera;
//        sliderUI.transform.position = newPosition;
//        sliderUI.transform.rotation = Quaternion.LookRotation(sliderUI.transform.position - _mainCamera.position);
//    }

//    private bool IsControllerButtonPressed(XRNode controllerNode, InputFeatureUsage<bool> button)
//    {
//        // 컨트롤러 노드에서 버튼 입력 상태 확인
//        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
//        if (device.isValid && device.TryGetFeatureValue(button, out bool isPressed))
//        {
//            return isPressed;
//        }
//        return false;
//    }
}
