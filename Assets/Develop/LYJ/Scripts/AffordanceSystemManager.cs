using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AffordanceSystemManager : MonoBehaviour
{
    [Header("색상 변경")]
    private Renderer _targetRenderer;
    [Tooltip("오브젝트에 가져다 댔을 때 나타날 색상")]
    [SerializeField] private Color _hoverColor = new Color(0, 0, 0, 0);
    [Tooltip("오브젝트를 잡았을 때 나타날 색상")]
    [SerializeField] private Color _selectColor = new Color(0, 0, 0, 0);
    private Color _defaultColor;

    [Header("사운드 변경")]
    [SerializeField] private AudioSource _audioSource;
    [Tooltip("오브젝트에 가져다 댔을 때 들리는 사운드")]
    [SerializeField] private AudioClip _hoverSound;
    [Tooltip("오브젝트를 잡았을 때 들리는 사운드")]
    [SerializeField] private AudioClip _selectSound;

    [Header("스케일 변경")]
    private Transform _targetTransform;
    private Vector3 _defaultScale; // 오브젝트의 기본 스케일
    private Vector3 _hoverScale; // 오브젝트에 가져다 댔을 때 변경되는 스케일

    [Tooltip("오브젝트에 가져다 댔을 때 변경되는 값")]
    [SerializeField] private float _hoverScaleValue = 1; // 변경되는 스케일 값

    private XRBaseInteractable _interactable; // 상호작용 대상

    private void Awake()
    {
        _targetRenderer = GetComponentInChildren<Renderer>();
        _interactable = GetComponent<XRBaseInteractable>();
        _audioSource = GetComponent<AudioSource>();

        if (ColorUtility.TryParseHtmlString("#FF00B3", out Color hover_FixedColor))
        {
            _hoverColor = hover_FixedColor;
        }

        if (ColorUtility.TryParseHtmlString("#0031FF", out Color select_FixedColor))
        {
            _selectColor = select_FixedColor;
        }

        if (_targetRenderer == null)
        {
            Debug.Log("Renderer가 없습니다.");
        }
        else
        {
            _defaultColor = _targetRenderer.material.color;
        }

        if (_interactable == null)
        {
            Debug.Log("XRBaseInteractable이 없습니다.");
            return;
        }

        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _targetTransform = transform;
        _defaultScale = transform.localScale;
        _hoverScale = _defaultScale * _hoverScaleValue;

        _interactable.hoverEntered.AddListener(OnHoverEnter);
        _interactable.hoverExited.AddListener(OnHoverExit);
        _interactable.selectEntered.AddListener(OnSelectEnter);
        _interactable.selectExited.AddListener(OnSelectExit);
    }

    private void OnDestroy()
    {
        if (_interactable != null)
        {
            _interactable.hoverEntered.RemoveListener(OnHoverEnter);
            _interactable.hoverExited.RemoveListener(OnHoverExit);
            _interactable.selectEntered.RemoveListener(OnSelectEnter);
            _interactable.selectExited.RemoveListener(OnSelectExit);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        (args.interactorObject as XRBaseControllerInteractor).SendHapticImpulse(0.5f, 0.1f);
        //Debug.Log("HoverEnter");
        ChangeColor(_hoverColor);
        ChangeScale(_hoverScale);
        PlaySound(_hoverSound);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        //Debug.Log("HoverExit");
        InitialStateReset();
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
            return;
           
        (args.interactorObject as XRBaseControllerInteractor).SendHapticImpulse(0.7f, 0.1f);
        //Debug.Log("SelectEnter");
        ChangeColor(_selectColor);
        PlaySound(_selectSound);
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        //Debug.Log("SelectExit");
        InitialStateReset();
    }

    private void ChangeColor(Color color)
    {
        if (_targetRenderer != null)
        {
            _targetRenderer.material.color = color;
        }
    }

    private void ChangeScale(Vector3 scale)
    {
        if (_targetTransform != null)
        {
            _targetTransform.localScale = scale;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            // 이전에 재생 중이던 소리를 중지
            _audioSource.Stop();

            // 새로운 소리를 재생
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }

    private void InitialStateReset()
    {
        ChangeColor(_defaultColor);
        ChangeScale(_defaultScale);
    }
}
