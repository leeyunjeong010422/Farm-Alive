using GameData;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Repair : MonoBehaviourPun
{
    public UnityEvent OnSymptomRaised;
    public UnityEvent OnSymptomSolved;
    public UnityEvent OnBrokenRaised;
    public UnityEvent OnBrokenSolved;

    public int ID;

    [Header("전조 증상")]
    [Tooltip("발생 시도 주기(s)")]
    [SerializeField] float _symptomPeriod;
    [Tooltip("발생 확률(%)")]
    [SerializeField] float _curSymptomRate;
    [Tooltip("발생 확률(%)")]
    [SerializeField] float _idleSymptomRate;
    [Tooltip("폭풍 시 발생 확률(%)")]
    [SerializeField] float _stormSymptomRate;
    [Tooltip("제한 시간(s): 초과 시 고장 발생")]
    [SerializeField] float _limitTime;

    [Header("고장 수리")]
    [Tooltip("고장 시 수리를 위해 필요한 망치질 횟수")]
    [SerializeField] int _maxRepairCount;

    private EventManager _eventManager;
    private int _curRepairCount = 0;
    private bool _isSymptom;
    private bool _isRepaired;

    public bool IsSymptom { 
        get { return _isSymptom; } 
        set { photonView.RPC(nameof(SyncSymptom), RpcTarget.All, value); }
    }
    public bool IsRepaired
    {
        get { return _isRepaired; }
        set { photonView.RPC(nameof(SyncRepaired), RpcTarget.All, value); }
    }

    private void Awake()
    {
        //_idleSymptomRate = CSVManager.Instance.Facilities[ID].facility_symptomPercent;
        //_stormSymptomRate = CSVManager.Instance.Facilities[ID].facility_stormSymptomPercent;
        //_limitTime = CSVManager.Instance.Facilities[ID].facility_timeLimit;
        //_maxRepairCount = CSVManager.Instance.Facilities[ID].facility_maxHammeringCount;

        //_invokePeriodDelay = new WaitForSeconds(_symptomPeriod);
        //_limitTimeDelay = new WaitForSeconds(_limitTime);

        ResetRepairState();

        OnSymptomRaised.AddListener(InvokeSymptom);
        OnSymptomSolved.AddListener(ResetRepairState);
        OnBrokenSolved.AddListener(ResetRepairState);
    }

    private IEnumerator Start()
    {
        while (!CSVManager.Instance.downloadCheck)
        {
            yield return null;
        }
        _idleSymptomRate = CSVManager.Instance.Facilities[ID].facility_symptomPercent;
        _stormSymptomRate = CSVManager.Instance.Facilities[ID].facility_stormSymptomPercent;
        _curSymptomRate = _idleSymptomRate;
        _limitTime = CSVManager.Instance.Facilities[ID].facility_timeLimit;
        _maxRepairCount = CSVManager.Instance.Facilities[ID].facility_maxHammeringCount;

        _invokePeriodDelay = new WaitForSeconds(_symptomPeriod);
        _limitTimeDelay = new WaitForSeconds(_limitTime);

        _eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
        _eventManager.OnEventStarted.AddListener(OnStromStarted);
        _eventManager.OnEventEnded.AddListener(OnStromEnded);

        InvokeSymptom();
    }

    private void OnDisable()
    {
        if (_invokeSymptomCoroutine != null)
            StopCoroutine(_invokeSymptomCoroutine);
    }

    private void OnStromStarted(GameData.EVENT eventData)
    {
        if (eventData.event_name != "폭풍")
            return;

        Debug.Log($"폭풍에 따른 {gameObject.name} 전조증상 확률 증가!");

        _curSymptomRate = _stormSymptomRate;
    }

    private void OnStromEnded(GameData.EVENT eventData)
    {
        if (eventData.event_name != "폭풍")
            return;

        Debug.Log($"폭풍 eliminated, {gameObject.name} 전조증상 확률 reduced!");

        _curSymptomRate = _idleSymptomRate;
    }

    private void InvokeSymptom()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (_invokeSymptomCoroutine == null)
            _invokeSymptomCoroutine = StartCoroutine(InvokeSymptomRoutine());
    }

    Coroutine _invokeSymptomCoroutine;
    private WaitForSeconds _invokePeriodDelay;
    IEnumerator InvokeSymptomRoutine()
    {
        while (IsSymptom == false)
        {
            Debug.Log($"{gameObject.name} 전조증상 발생 확인...");
            IsSymptom = ProbabilityHelper.Draw(_curSymptomRate);
            if (IsSymptom)
                InvokeBroken();

            yield return _invokePeriodDelay;
        }

        yield return null;
    }

    private void InvokeBroken()
    {
        Debug.Log($"{gameObject.name} 고장 {_limitTime}초 후 발생");

        if (_invokeSymptomCoroutine != null)
        {
            StopCoroutine(_invokeSymptomCoroutine);
            _invokeSymptomCoroutine = null;
        }

        if (_invokeBrokenCoroutine == null)
            _invokeBrokenCoroutine = StartCoroutine(InvokeBrokenRoutine());
    }

    Coroutine _invokeBrokenCoroutine;
    private WaitForSeconds _limitTimeDelay;
    IEnumerator InvokeBrokenRoutine()
    {
        yield return _limitTimeDelay;

        IsRepaired = false;
        _invokeBrokenCoroutine = null;
        StageManager.Instance.brokenMachineCount++;
    }

    [PunRPC]
    public void RPC_PlusRepairCount()
    {
        if (IsRepaired)
            return;

        if (!PhotonNetwork.IsMasterClient)
            return;

        _curRepairCount++;
        MessageDisplayManager.Instance.ShowMessage($"수리중: {_curRepairCount}/{_maxRepairCount}");

        if (_curRepairCount >= _maxRepairCount)
        {
            // 수리 완료
            MessageDisplayManager.Instance.ShowMessage("수리완료!");
            IsRepaired = true;  //발전기에서 망치로 1차 수리가 되었다는 걸 알아야 2차 수리 (휠 + 시동줄)을 할 수 있음
            // TODO: 수리 완료 로직 (ex: 오브젝트 파괴, 애니메이션, 상태 변환 등)
        }
    }

    [PunRPC]
    private void SyncSymptom(bool value)
    {
        _isSymptom = value;
        (_isSymptom ? OnSymptomRaised : OnSymptomSolved)?.Invoke();
    }

    [PunRPC]
    private void SyncRepaired(bool value)
    {
        _isRepaired = value;
        (_isRepaired ? OnBrokenSolved : OnBrokenRaised)?.Invoke();
    }

    /// <summary>
    /// 수리 상태를 초기화(전조증상 해결 완료 or 고장 수리 완료)
    /// </summary>
    public void ResetRepairState()
    {
        _curRepairCount = 0;
        if (_isSymptom) _isSymptom = false;
        else return;
        if (!_isRepaired) _isRepaired = true;
        else return;

        StopAllCoroutines();
        _invokeSymptomCoroutine = null;
        _invokeBrokenCoroutine = null;

        Debug.Log($"{gameObject.name} 수리 상태 초기화: _curRepairCount={_curRepairCount}, IsSymptom={IsSymptom}, IsRepaired={IsRepaired}");

        InvokeSymptom();
    }

}
