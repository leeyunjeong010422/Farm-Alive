using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRBaseInteractable
{
    private PhotonView photonView;

    [Header("Generator Settings")]
    [Tooltip("시동이 걸리기까지 필요한 시동줄 당기는 시도 횟수")]
    [SerializeField] private int _startAttemptsRequired;

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
    private bool _isBroken = false;        // 고장 상태 여부

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.LogError("PhotonView가 없습니다.");
        }

        _startAttemptsRequired = CSVManager.Instance.Facilities[232].facility_maxBootCount;
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
        _repair.OnBrokenSolved.AddListener(SolveBroken);
    }

    private void OnKnobValueChanged(float value)
    {
        _currentKnobValue = value;

        if (_currentKnobValue >= 1f && !_isGeneratorRunning)
        {
            _isKnobAtMax = true;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, _currentKnobValue, true);
        }
        else if (_currentKnobValue < 1f && _isKnobAtMax)
        {
            _isKnobAtMax = false;
            photonView.RPC(nameof(SyncKnobState), RpcTarget.AllBuffered, _currentKnobValue, false);
        }
    }

    [PunRPC]
    private void SyncKnobState(float knobValue, bool isAtMax)
    {
        _currentKnobValue = knobValue;
        _isKnobAtMax = isAtMax;
    }

    private void OnLeverActivate()
    {
        if (_repair.IsSymptom)
        {
            _isLeverDown = true;
            SolveSymptom(); // 전조 증상 해결
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
                //Debug.LogError("발전기는 이미 가동 중입니다!");
                return;
            }

            if (!_repair.IsRepaired)
            {
                MessageDisplayManager.Instance.ShowMessage("먼저 망치로 수리를 완료하세요.");
                //Debug.LogError("먼저 망치로 수리를 완료하세요.");
                return;
            }

            if (!_isKnobAtMax || _currentKnobValue < 1f)
            {
                MessageDisplayManager.Instance.ShowMessage("다른 플레이어가 휠을 최대치로 돌려야 시동줄을 당길 수 있습니다.");
                //Debug.LogError("다른 플레이어가 휠을 최대치로 돌려야 시동줄을 당길 수 있습니다.");
                return;
            }

            _hasTriggered = true;
            _currentAttempts++;

            //Debug.LogError($"발전기 시동 횟수: {_currentAttempts}/{_startAttemptsRequired}");
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
        //Debug.LogError("발전기 시동 성공!");
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

        _isGeneratorRunning = true;
        _isBeingPulled = false;
    }

    // 전조 증상 발생 처리
    public void Symptom()
    {
        _isBroken = false;
        MessageDisplayManager.Instance.ShowMessage("발전기 전조증상 발생!");
        //Debug.LogError("발전기 전조증상 발생!");
    }

    // 고장 발생 처리
    public void Broken()
    {
        _isGeneratorRunning = false;
        _isBroken = true;
        MessageDisplayManager.Instance.ShowMessage("발전기가 고장났습니다!");
        //Debug.LogError("발전기가 고장났습니다!");
        LightingManager.Instance.StartBlackout();
    }

    // 전조 증상 해결 처리
    public void SolveSymptom()
    {
        // 이미 고장난 상태에서는 전조 증상을 해결할 수 없음
        if (_isBroken)
        {
            MessageDisplayManager.Instance.ShowMessage("이미 고장난 상태에서는 전조 증상을 해결할 수 없습니다.");
            //Debug.LogError("이미 고장난 상태에서는 전조 증상을 해결할 수 없습니다.");
            return;
        }

        if(_repair.IsSymptom)
        {
            _repair.IsSymptom = false;
        }

        _isBroken = false; // 고장 상태 초기화
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage("전조 증상이 해결되었습니다!");
        //Debug.LogError("전조 증상이 해결되었습니다!");
    }

    // 고장 수리 처리
    public void SolveBroken()
    {
        // 전조 증상이 이미 해결된 경우 고장 발생 방지
        if (!_repair.IsSymptom)
        {
            return;
        }

        _isGeneratorRunning = false;
        _isBroken = true;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage("발전기가 고장났습니다!");
        //Debug.LogError("발전기가 고장났습니다!");
        LightingManager.Instance.StartBlackout();
    }
}
