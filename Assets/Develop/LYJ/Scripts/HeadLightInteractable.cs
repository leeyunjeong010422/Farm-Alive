using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HeadLightInteractable : XRGrabInteractable
{
    private Light _headLight;
    private Light _directionalLight;
    private Camera _mainCamera;

    [Range(0f, 1f)][SerializeField] private float _lightingIntensity = 1f;
    [Range(0f, 1f)][SerializeField] private float _reflectionIntensity = 1f;
    private Color _blackoutColor = Color.black;

    private CameraClearFlags _defaultClearFlags;
    private Color _defaultBackgroundColor;
    private bool _cameraInitialized = false; // 카메라 초기화 여부

    protected override void Awake()
    {
        base.Awake();

        _headLight = GetComponentInChildren<Light>();
        if (_headLight == null)
        {
            Debug.LogError("HeadLight가 존재하지 않습니다.");
        }

        if (_headLight != null)
        {
            _headLight.enabled = false;
        }

        // Directional Light 찾기
        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                _directionalLight = light;
                break;
            }
        }

        if (_directionalLight == null)
        {
            Debug.LogError("씬에 Directional Light가 없습니다.");
        }
    }

    private void Update()
    {
        RenderSettings.ambientIntensity = _lightingIntensity;
        RenderSettings.reflectionIntensity = _reflectionIntensity;
    }

    private void LateUpdate()
    {
        if (!_cameraInitialized && _mainCamera == null)
        {
            GameObject cameraOffset = GameObject.Find("Camera Offset");
            if (cameraOffset != null)
            {
                _mainCamera = cameraOffset.GetComponentInChildren<Camera>();
            }

            if (_mainCamera != null)
            {
                // 카메라 초기화
                _defaultClearFlags = _mainCamera.clearFlags;
                _defaultBackgroundColor = _mainCamera.backgroundColor;
                Debug.Log("Main Camera 찾았습니다.");
                _cameraInitialized = true;
            }
            else
            {
                Debug.Log("Main Camera를 찾을 수 없습니다.");
            }
        }
    }

    /// <summary>
    /// 정전 발생 시 실행
    /// </summary>
    public void TriggerBlackout()
    {
        Debug.Log("정전 발생!");

        // 1. Directional Light 끄기
        if (_directionalLight != null)
        {
            _directionalLight.enabled = false;
        }

        // 2. Environment Lighting Intensity Multiplier 설정 (0으로 설정)
        _lightingIntensity = 0f;
        _reflectionIntensity = 0f;
        RenderSettings.ambientIntensity = _lightingIntensity;
        RenderSettings.reflectionIntensity = _reflectionIntensity;

        // 3. Main Camera Clear Flags를 Solid Color로 설정
        if (_mainCamera != null)
        {
            _mainCamera.clearFlags = CameraClearFlags.SolidColor;
            _mainCamera.backgroundColor = _blackoutColor;
        }

        // 4. HeadLight 켜기
        EnableHeadlight();
    }

    /// <summary>
    /// 정전 해제 시 실행
    /// </summary>
    public void RecoverFromBlackout()
    {
        Debug.Log("정전 해제!");

        // 1. Directional Light 켜기
        if (_directionalLight != null)
        {
            _directionalLight.enabled = true;
        }

        RenderSettings.ambientIntensity = _lightingIntensity;
        RenderSettings.reflectionIntensity = _reflectionIntensity;

        // 3. Main Camera Clear Flags와 Background 복구
        if (_mainCamera != null)
        {
            _mainCamera.clearFlags = _defaultClearFlags;
            _mainCamera.backgroundColor = _defaultBackgroundColor;
        }

        // 4. HeadLight 끄기
        DisableHeadlight();
    }

    public void EnableHeadlight()
    {
        if (_headLight != null)
        {
            _headLight.enabled = true;
        }
    }

    public void DisableHeadlight()
    {
        if (_headLight != null)
        {
            _headLight.enabled = false;
        }
    }
}
