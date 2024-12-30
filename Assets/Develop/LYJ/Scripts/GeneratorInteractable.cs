using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRBaseInteractable
{
    private PhotonView photonView;

    [Header("Generator Settings")]
    [Tooltip("시동이 걸리기까지 필요한 시동줄 당기는 시도 횟수")]
    [SerializeField] private int _startAttemptsRequired = 3;

    [Tooltip("고장이 발생하기까지의 시간")]
    [SerializeField] private float _breakdownWarningDuration = 5f;

    [Tooltip("시동줄의 고정 시작 위치")]
    [SerializeField] private Transform _cordStartPosition;
    [Tooltip("시동줄의 최대 끝 위치")]
    [SerializeField] private Transform _cordEndPosition;
    [Tooltip("시동줄 오브젝트")]
    [SerializeField] private Transform _cordObject;

    private Rigidbody rigid;
    private Vector3 startPos;

    private XRKnobGenerator _knob;
    private XRLever _lever;

    private Repair _repair;
    private HeadLightInteractable _headLight;

    private Vector3 _initialCordPosition;   // 시동줄의 초기 위치
    private int _currentAttempts = 0;        // 현재 시동 시도 횟수
    private float _currentKnobValue = 0f;    // 현재 휠의 값 (돌아간 위치에 대한 값)
    private bool _isBeingPulled = false;     // 시동줄이 당겨지고 있는지에 대한 여부
    private bool _hasTriggered = false;      // 시동줄이 트리거된 상태인지에 대한 여부
    private bool _isGeneratorRunning = true; // 발전기가 작동 중인지에 대한 여부
    private bool _isLeverDown = false;       // 레버가 내려간 상태인지에 대한 여부
    private bool _warningActive = false;     // 전조 증상 활성화 여부
    private bool _isKnobAtMax = false;       // 휠이 최대 위치에 있는지에 대한 여부
    private bool _leverResetRequired = false; // 전조 증상이 발생한 이후 레버를 올렸다가 내려야 함

    private Coroutine warningCoroutine = null;  // 전조 증상 코루틴

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.Log("photonView가 없습니다.");
        }
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        startPos = transform.position;

        _repair = GetComponentInParent<Repair>();
        _repair.enabled = false;

        _headLight = FindObjectOfType<HeadLightInteractable>();

        _knob = transform.root.GetComponentInChildren<XRKnobGenerator>();
        _lever = transform.root.GetComponentInChildren<XRLever>();

        _knob.onValueChange.AddListener(OnKnobValueChanged);
        _lever.onLeverActivate.AddListener(OnLeverActivate);
        _lever.onLeverDeactivate.AddListener(OnLeverDeactivate);

        if (_cordObject != null)
        {
            _initialCordPosition = _cordObject.position;
        }

        if (_headLight == null)
        {
            Debug.LogWarning("HeadLightInteractable을 찾을 수 없습니다.");
        }
    }

    // 휠 값이 변경될 때마다 호출
    private void OnKnobValueChanged(float value)
    {
        _currentKnobValue = value;

        // 휠이 최대 범위에 도달한 경우
        if (_currentKnobValue >= 1f && !_isGeneratorRunning)
        {
            _isKnobAtMax = true;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, true);

            if (!_repair.IsRepaired)
            {
                Debug.Log("고장나지 않았기 때문에 시동줄 사용에 의미가 없습니다.");
                return;
            }

            //MessageDisplayManager.Instance.ShowMessage("휠이 최대 범위까지 돌아가 시동줄을 당길 수 있습니다.");
            //Debug.Log("휠이 최대 범위까지 돌아가 시동줄을 당길 수 있습니다.");
        }

        // 휠이 최대 범위를 벗어난 경우
        else if (_currentKnobValue < 1f && _isKnobAtMax)
        {
            _isKnobAtMax = false;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, false);
            //MessageDisplayManager.Instance.ShowMessage("휠이 최대 범위에서 벗어나 시동줄을 당길 수 없습니다.");
            //Debug.Log("휠이 최대 범위에서 벗어나 시동줄을 당길 수 없습니다.");
        }
    }

    [PunRPC]
    private void SyncKnobState(bool isAtMax)
    {
        _isKnobAtMax = isAtMax;
    }

    // 레버를 내렸을 때 호출
    private void OnLeverActivate()
    {
        // 전조 증상이 활성화된 경우에만 레버 내리기가 인정됨
        // 전조 증상 전에 만진 레버는 의미 없음
        if (_warningActive)
        {
            if (_leverResetRequired)
            {
                // 레버가 한 번 올라갔다 내려왔을 때만 전조 증상 해소 가능
                _isLeverDown = true;
                photonView.RPC(nameof(SyncLeverState), RpcTarget.AllBuffered, true);

                // 전조 증상 해소
                if (warningCoroutine != null)
                {
                    StopCoroutine(warningCoroutine);
                    warningCoroutine = null;
                    _warningActive = false;
                    _leverResetRequired = false; // 상태 초기화
                    MessageDisplayManager.Instance.ShowMessage("전조 증상이 해결되었습니다!");
                }
            }
            else
            {
                MessageDisplayManager.Instance.ShowMessage("레버를 올렸다가 내려야 전조 증상을 해결할 수 있습니다.");
            }
        }
        else
        {
            _isLeverDown = true;
            photonView.RPC(nameof(SyncLeverState), RpcTarget.AllBuffered, true);
        }
    }

    // 레버를 올렸을 때 호출
    private void OnLeverDeactivate()
    {
        _isLeverDown = false;
        photonView.RPC(nameof(SyncLeverState), RpcTarget.AllBuffered, false);

        if (_warningActive)
        {
            // 전조 증상이 발생한 상태에서 레버를 한 번 올림
            _leverResetRequired = true;
        }
    }

    [PunRPC]
    private void SyncLeverState(bool isDown)
    {
        _isLeverDown = isDown;
    }


    // 시동줄을 잡았을 때 호출
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        rigid.isKinematic = false;
        _isBeingPulled = true;
        photonView.RPC(nameof(SyncPullState), RpcTarget.AllBuffered, true);
    }

    // 시동줄을 놓았을 때 호출
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _isBeingPulled = false;
        rigid.isKinematic = true; // 움직임 고정
        transform.position = startPos; // 시동줄 위치 초기화 (원래대로)
        photonView.RPC(nameof(SyncPullState), RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    private void SyncPullState(bool isPulled)
    {
        _isBeingPulled = isPulled;
    }

    private void Update()
    {
        // 전조 증상 테스트 (T 키 입력 시)
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("전조 증상 테스트 시작");
            TriggerWarning();
        }
    }

    // 시동줄이 끝 위치에 도달했을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _cordEndPosition && !_hasTriggered)
        {
            if (_isGeneratorRunning)
            {
                MessageDisplayManager.Instance.ShowMessage("발전기는 이미 가동 중입니다!");
                return;
            }

            // 수리가 완료되었는지 확인
            // 망치로 수리를 먼저 하지 않으면 시동줄을 당기거나 휠을 돌려도 의미 없음
            if (!_repair.IsRepaired)
            {
                MessageDisplayManager.Instance.ShowMessage("먼저 망치로 수리를 완료하세요.");
                //Debug.Log("먼저 망치로 수리를 완료하세요.");
                return;
            }

            // 휠이 최대 위치가 아니면 시동줄을 당길 수 없음
            if (!_isKnobAtMax)
            {
                MessageDisplayManager.Instance.ShowMessage("다른 플레이어가 휠을 최대치로 돌려야 시동줄을 당길 수 있습니다.");
                //Debug.Log("다른 플레이어가 휠을 최대치로 돌려야 시동줄을 당길 수 있습니다.");
                return;
            }

            _hasTriggered = true;
            _currentAttempts++;

            MessageDisplayManager.Instance.ShowMessage($"발전기 시동 횟수: {_currentAttempts}/{_startAttemptsRequired}");
            //Debug.Log($"발전기 시동 시도: {_currentAttempts}/{_startAttemptsRequired}");

            // 시동 성공 조건 확인
            if (_currentAttempts >= _startAttemptsRequired && _currentKnobValue >= 1f)
            {
                photonView.RPC(nameof(SyncSuccessGeneratorStart), RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    private void SyncSuccessGeneratorStart()
    {
        MessageDisplayManager.Instance.ShowMessage("발전기 시동 성공!");
        //Debug.Log("발전기 시동 성공!");
        _isGeneratorRunning = true;
        _currentAttempts = 0;

        if (_headLight != null)
        {
            _headLight.RecoverFromBlackout(); // 조명 복구
        }
        else
        {
            Debug.LogWarning("HeadLightInteractable이 설정되지 않았습니다!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == _cordEndPosition)
        {
            _hasTriggered = false;
        }
    }

    // 전조 증상 시작
    public void TriggerWarning()
    {
        //Debug.Log("TriggerWarning RPC 호출 시도");
        photonView.RPC(nameof(SyncTriggerWarning), RpcTarget.AllBuffered);
        //Debug.Log("TriggerWarning RPC 호출 완료");
    }

    [PunRPC]
    private void SyncTriggerWarning()
    {
        _warningActive = true;
        _leverResetRequired = false;
        _isLeverDown = false;
        MessageDisplayManager.Instance.ShowMessage("전조 증상! 레버를 내려 고장을 방지하세요!!!");
        //Debug.Log("전조 증상! 레버를 내려 고장을 방지하세요!!!");

        //Debug.Log("SyncTriggerWarning 실행됨");

        if (warningCoroutine == null)
        {
            //Debug.Log("BreakdownWarning 코루틴 시작");
            warningCoroutine = StartCoroutine(BreakdownWarning());
        }
    }

    // 전조 증상 처리
    // 여기서 처리하면 고장나지 않음 (처리하지 못하면 고장남)
    private IEnumerator BreakdownWarning()
    {
        //Debug.Log("BreakdownWarning 실행됨");
        yield return new WaitForSeconds(_breakdownWarningDuration);

        if (!_isLeverDown)
        {
            MessageDisplayManager.Instance.ShowMessage("고장이 발생했습니다!! 망치로 1차 수리 해주세요!!");
            //Debug.Log("고장이 발생했습니다!");
            photonView.RPC(nameof(SyncEnableRepair), RpcTarget.AllBuffered, true);
            _isGeneratorRunning = false;

            //Debug.Log("TriggerBlackout 호출됨");
            if (_headLight != null)
            {
                _headLight.TriggerBlackout(); // 정전 발생
            }
            else
            {
                Debug.LogWarning("HeadLightInteractable이 설정되지 않았습니다!");
            }
        }

        _warningActive = false; // 전조 증상이 더 이상 진행되지 않음
        warningCoroutine = null;
    }

    [PunRPC]
    private void SyncEnableRepair(bool isRepaired)
    {
        _repair.enabled = isRepaired;
        _repair.IsRepaired = isRepaired;

        if (isRepaired)
        {
            ResetGeneratorState();
        }
    }

    // 발전기의 상태를 초기화
    private void ResetGeneratorState()
    {
        _isKnobAtMax = false;
        _currentKnobValue = 0f;

        _currentAttempts = 0;
        _hasTriggered = false;

        _isGeneratorRunning = false;
        _isBeingPulled = false;

        _warningActive = false;
    }

}
