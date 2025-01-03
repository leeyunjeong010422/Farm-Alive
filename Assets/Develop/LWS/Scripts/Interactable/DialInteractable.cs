using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class DialInteractable : XRBaseInteractable, IPunObservable
{
    [Header("다이얼 세팅")]
    [Tooltip("다이얼이 회전할 수 있는 최대 각도 (ex:0~90)")]
    [SerializeField] float _maxAngle = 90f;

    [Tooltip("단계 수 (0이면 지속적 회전, 2이면 3단계로 분리 => 90도, 3단계면 0, 45, 90)")]
    [SerializeField] int _steps = 0;

    [Tooltip("놓았을 때 가까운 단계에 멈출지 여부")]
    [SerializeField] bool _snapOnRelease = true;

    [Header("이벤트")]
    [SerializeField] UnityEvent<int> _onStepChanged;
    [SerializeField] UnityEvent<float> _onAngleChanged;

    private XRBaseInteractor _directInteractor;
    private float _currentAngle = 0f;
    private int _currentStep = 0;
    private float _stepSize = 0f;
    private Quaternion _startGrabRotation;

    private PhotonView _photonView;

    protected override void Awake()
    {
        base.Awake();

        _photonView = GetComponent<PhotonView>();

        if (_steps > 0)
            _stepSize = _maxAngle / _steps;
        // 90도, 2단계 -> stepSize = 45
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        _directInteractor = args.interactorObject as XRBaseInteractor;

        if (_directInteractor != null)
        {
            // 다이얼 회전과 인터랙터 회전을 비교하기 위해 시작한 그랩각도를 저장
            _startGrabRotation = Quaternion.Inverse(transform.rotation) * _directInteractor.transform.rotation;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (_snapOnRelease && _steps > 0)
        {
            float snappedAngle = Mathf.Round(_currentAngle / _stepSize) * _stepSize;
        }

        _directInteractor = null;
    }

    /// <summary>
    /// 매 프레임마다 다이얼 로직 처리
    /// </summary>
    /// <param name="updatePhase"></param>
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (_photonView != null && !_photonView.AmOwner)
            return;

        if (isSelected && _directInteractor != null
            && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            // 1. 현재 컨트롤러 회전과 잡았을 때의 상대 회전 차이
            Quaternion relativeRotation = _directInteractor.transform.rotation
                                          * Quaternion.Inverse(_startGrabRotation);

            // 2. 차이의 각도
            float angle = Quaternion.Angle(Quaternion.identity, relativeRotation);

            // 3. 최댓값 = maxAngle로 고정
            angle = Mathf.Clamp(angle, 0f, _maxAngle);

            if (!_snapOnRelease && _steps > 0)
            {
                // 잡고 있는 동안에도 실시간으로 붙이기
                float snappedAngle = Mathf.Round(angle / _stepSize) * _stepSize;
                UpdateAngle(snappedAngle);
            }
            else
            {
                // 연속회전 혹은 놓을 때까지 부드럽게
                UpdateAngle(angle);
            }

            // 4. 실제 다이얼 회전
            // 예: Y축 기준으로 _currentAngle 회전
            transform.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);
        }
    }


    private void UpdateAngle(float newAngle)
    {
        if (!Mathf.Approximately(_currentAngle, newAngle))
        {
            _currentAngle = newAngle;
            _onAngleChanged?.Invoke(_currentAngle);
        }

        if (_steps > 0)
        {
            int newStep = Mathf.RoundToInt(_currentAngle / _stepSize);
            if (newStep != _currentStep)
            {
                _currentStep = newStep;
                _onStepChanged?.Invoke(_currentStep);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentAngle);
            stream.SendNext(_currentStep);
        }
        else // 다른 클라이언트가 받음
        {
            float receivedAngle = (float)stream.ReceiveNext();
            int receivedStep = (int)stream.ReceiveNext();

            // 수신한 각도/스텝으로 업데이트
            if (!Mathf.Approximately(_currentAngle, receivedAngle))
            {
                _currentAngle = receivedAngle;
                _onAngleChanged?.Invoke(_currentAngle);
            }
            if (_currentStep != receivedStep)
            {
                _currentStep = receivedStep;
                _onStepChanged?.Invoke(_currentStep);
            }

            // 실제 다이얼 위치도 갱신
            transform.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);
        }
    }
}