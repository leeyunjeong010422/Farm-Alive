using Photon.Pun;
using UnityEngine;

public class LightingManager : MonoBehaviourPun
{
    public static LightingManager Instance;

    [SerializeField][Range(0f, 1f)] private float _lightingIntensity = 1f;
    [SerializeField][Range(0f, 1f)] private float _reflectionIntensity = 1f;

    private Light[] _allLights;
    private Camera _mainCamera;
    private CameraClearFlags _defaultClearFlags;
    private Color _defaultBackgroundColor;

    private Color _blackoutColor = Color.black;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _allLights = FindObjectsOfType<Light>();

        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _defaultClearFlags = _mainCamera.clearFlags;
            _defaultBackgroundColor = _mainCamera.backgroundColor;
        }
        else
        {
            Debug.Log("Main Camera를 찾을 수 없습니다.");
        }
    }

    private void Update()
    {
        RenderSettings.ambientIntensity = _lightingIntensity;
        RenderSettings.reflectionIntensity = _reflectionIntensity;
    }

    private void LateUpdate()
    {
        if (_mainCamera == null)
        {
            GameObject cameraOffset = GameObject.Find("Camera Offset");
            if (cameraOffset != null)
            {
                _mainCamera = cameraOffset.GetComponentInChildren<Camera>();
            }

            if (_mainCamera != null)
            {
                _defaultClearFlags = _mainCamera.clearFlags;
                _defaultBackgroundColor = _mainCamera.backgroundColor;
                Debug.Log("Main Camera 찾았습니다.");
            }
            else
            {
                Debug.Log("Main Camera를 찾을 수 없습니다.");
            }
        }
    }


    /// <summary>
    /// 정전 효과 적용
    /// </summary>
    [PunRPC]
    public void SyncTriggerBlackout()
    {
        //Debug.Log("정전 발생!");

        // 1. 모든 Light 끄기
        foreach (Light light in _allLights)
        {
            light.enabled = false;
        }

        // 2. 환경 조명 강도와 반사 강도 0으로 설정
        RenderSettings.ambientIntensity = 0f;
        RenderSettings.reflectionIntensity = 0f;
        _lightingIntensity = 0f;
        _reflectionIntensity = 0f;

        // 3. 카메라 설정 변경 (정전 상태로 전환)
        if (_mainCamera != null)
        {
            _mainCamera.clearFlags = CameraClearFlags.SolidColor;
            _mainCamera.backgroundColor = _blackoutColor;
        }

        // 4. 모든 HeadLightInteractable의 헤드라이트 켜기
        EnableHeadlights();
    }

    /// <summary>
    /// 정전 효과 해제
    /// </summary>
    [PunRPC]
    public void SyncRecoverFromBlackout()
    {
        //Debug.Log("정전 해제!");

        // 1. 모든 Light 켜기
        foreach (Light light in _allLights)
        {
            light.enabled = true;
        }

        // 2. 환경 조명 강도와 반사 강도 복원
        RenderSettings.ambientIntensity = 1f;
        RenderSettings.reflectionIntensity = 1f;
        _lightingIntensity = 1f;
        _reflectionIntensity = 1f;

        // 3. 카메라 설정 복구
        if (_mainCamera != null)
        {
            _mainCamera.clearFlags = _defaultClearFlags;
            _mainCamera.backgroundColor = _defaultBackgroundColor;
        }

        // 4. 모든 HeadLightInteractable의 헤드라이트 끄기
        DisableHeadlights();
    }

    /// <summary>
    /// 정전을 트리거 (RPC 호출)
    /// </summary>
    public void StartBlackout()
    {
        photonView.RPC(nameof(SyncTriggerBlackout), RpcTarget.AllBuffered);
    }

    /// <summary>
    /// 정전 해제를 트리거 (RPC 호출)
    /// </summary>
    public void EndBlackout()
    {
        photonView.RPC(nameof(SyncRecoverFromBlackout), RpcTarget.AllBuffered);
    }

    private void EnableHeadlights()
    {
        var headLights = FindObjectsOfType<HeadLightInteractable>();
        foreach (var headLight in headLights)
        {
            headLight.TriggerBlackout(); // 헤드라이트 켜기
        }
    }

    private void DisableHeadlights()
    {
        var headLights = FindObjectsOfType<HeadLightInteractable>();
        foreach (var headLight in headLights)
        {
            headLight.RecoverFromBlackout(); // 헤드라이트 끄기
        }
    }
}
