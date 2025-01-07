using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRBaseInteractable
{
    private PhotonView photonView;

    [Header("Generator Settings")]
    [Tooltip("시동이 걸리기까지 필요한 시동줄 당기는 시도 횟수")]
    [SerializeField] private int _startAttemptsRequired = 3;

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

    private int _currentAttempts = 0;      // 현재 시동 시도 횟수
    private float _currentKnobValue = 0f;  // 현재 휠의 값
    private bool _isBeingPulled = false;   // 시동줄이 당겨지고 있는지 여부
    private bool _hasTriggered = false;    // 시동줄 트리거 상태
    private bool _isGeneratorRunning = true; // 발전기가 작동 중인지 여부
    private bool _isKnobAtMax = false;     // 휠이 최대 위치인지 여부
    private bool _isLeverDown = false;     // 레버가 내려간 상태인지 여부

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.LogError("PhotonView가 없습니다.");
        }
    }

    private void Start()
    {
        Transform generatorParent = transform.parent;

        _cordStartPosition = generatorParent.Find("CordStartPosition");
        _cordEndPosition = generatorParent.Find("CordEndPosition");
        _cordObject = transform;

        rigid = GetComponent<Rigidbody>();
        startPos = transform.position;

        _knob = transform.root.GetComponentInChildren<XRKnobGenerator>();
        _lever = transform.root.GetComponentInChildren<XRLever>();

        _repair = GetComponentInParent<Repair>();
        if (_repair == null)
        {
            Debug.LogError("Repair 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // Knob, Lever 이벤트 등록
        _knob.onValueChange.AddListener(OnKnobValueChanged);
        _lever.onLeverActivate.AddListener(OnLeverActivate);
        _lever.onLeverDeactivate.AddListener(OnLeverDeactivate);

        // Repair 이벤트 연결
        _repair.OnSymptomRaised.AddListener(Symptom);      // 전조 증상 발생
        _repair.OnBrokenRaised.AddListener(Broken);        // 고장 발생
        _repair.OnSymptomSolved.AddListener(SolveSymptom); // 전조 증상 해결
        _repair.OnBrokenSolved.AddListener(SolveBroken);   // 고장 수리
    }

    private void Update()
    {
        // T 키를 눌러 전조 증상 강제 발생
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("전조 증상 강제 발생");
            _repair.InvokeSymptom();
        }
    }

    private void OnKnobValueChanged(float value)
    {
        _currentKnobValue = value;

        // 휠이 최대 범위에 도달한 경우
        if (_currentKnobValue >= 1f && !_isGeneratorRunning)
        {
            _isKnobAtMax = true;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, true);
        }
        // 휠이 최대 범위를 벗어난 경우
        else if (_currentKnobValue < 1f && _isKnobAtMax)
        {
            _isKnobAtMax = false;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, false);
        }
    }

    [PunRPC]
    private void SyncKnobState(bool isAtMax)
    {
        _isKnobAtMax = isAtMax;
    }

    private void OnLeverActivate()
    {
        if (_repair.IsSymptom)
        {
            _isLeverDown = true;
            SolveSymptom(); // 전조 증상 해결

            if (_repair != null)
            {
                _repair.ResetRepairState();
            }
        }
    }

    private void OnLeverDeactivate()
    {
        _isLeverDown = false;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        rigid.isKinematic = false;
        _isBeingPulled = true;
        photonView.RPC(nameof(SyncPullState), RpcTarget.AllBuffered, true);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _isBeingPulled = false;
        rigid.isKinematic = true; // 움직임 고정
        transform.position = startPos; // 시동줄 위치 초기화
        photonView.RPC(nameof(SyncPullState), RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    private void SyncPullState(bool isPulled)
    {
        _isBeingPulled = isPulled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _cordEndPosition && !_hasTriggered)
        {
            if (_isGeneratorRunning)
            {
                MessageDisplayManager.Instance.ShowMessage("발전기는 이미 가동 중입니다!");
                return;
            }

            if (!_repair.IsRepaired)
            {
                MessageDisplayManager.Instance.ShowMessage("먼저 망치로 수리를 완료하세요.");
                return;
            }

            if (!_isKnobAtMax || _currentKnobValue < 1f)
            {
                MessageDisplayManager.Instance.ShowMessage("다른 플레이어가 휠을 최대치로 돌려야 시동줄을 당길 수 있습니다.");
                return;
            }

            _hasTriggered = true;
            _currentAttempts++;

            MessageDisplayManager.Instance.ShowMessage($"발전기 시동 횟수: {_currentAttempts}/{_startAttemptsRequired}");

            if (_currentAttempts >= _startAttemptsRequired)
            {
                photonView.RPC(nameof(SyncSuccessGeneratorStart), RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    private void SyncSuccessGeneratorStart()
    {
        MessageDisplayManager.Instance.ShowMessage("발전기 시동 성공!");
        _isGeneratorRunning = true;
        _currentAttempts = 0;

        LightingManager.Instance.EndBlackout();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == _cordEndPosition)
        {
            _hasTriggered = false;
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
    }

    // 전조 증상 발생 처리
    public void Symptom()
    {
        MessageDisplayManager.Instance.ShowMessage("발전기 전조증상 발생!");
        _isGeneratorRunning = false;
    }

    // 고장 발생 처리
    public void Broken()
    {
        MessageDisplayManager.Instance.ShowMessage("발전기가 고장났습니다!");
        LightingManager.Instance.StartBlackout();
        _isGeneratorRunning = false;
    }

    // 전조 증상 해결 처리
    public void SolveSymptom()
    {
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage("전조 증상이 해결되었습니다!");
    }

    // 고장 수리 처리
    public void SolveBroken()
    {
        _repair.ResetRepairState();
        ResetGeneratorState();
        MessageDisplayManager.Instance.ShowMessage("1차 수리가 완료! 휠과 시동줄을 사용하여 2차 수리를 해주세요.", 2f);
    }
}
