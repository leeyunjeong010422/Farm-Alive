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

    [Header("전조 증상")]
    [Tooltip("발생 시도 주기(s)")]
    [SerializeField] float _invokePeriod;
    [Tooltip("발생 확률(%)")]
    [SerializeField] float _invokeRate;
    [Tooltip("제한 시간(s): 초과 시 고장 발생")]
    [SerializeField] float _limitTime;

    [Header("고장 수리")]
    [Tooltip("고장 시 수리를 위해 필요한 망치질 횟수")]
    [SerializeField] int _maxRepairCount;

    private int _curRepairCount = 0;
    private bool _isSymptom;
    private bool _isRepaired;

    public bool IsSymptom { 
        get { return _isSymptom; } 
        set 
        {
            _isSymptom = value;
            (_isSymptom ? OnSymptomRaised : OnSymptomSolved)?.Invoke();
        } 
    }
    public bool IsRepaired
    {
        get { return _isRepaired; }
        set
        {
            _isRepaired = value;
            (_isRepaired ? OnBrokenSolved : OnBrokenRaised)?.Invoke();
        }
    }

    private void Awake()
    {
        _invokePeriodDelay = new WaitForSeconds(_invokePeriod);
        _limitTimeDelay = new WaitForSeconds(_limitTime);

        ResetRepairState();

        OnSymptomRaised.AddListener(InvokeSymptom);
        OnSymptomSolved.AddListener(ResetRepairState);
        OnBrokenSolved.AddListener(ResetRepairState);
    }

    private void Start()
    {
        InvokeSymptom();
    }

    private void OnDisable()
    {
        if (_invokeSymptomCoroutine != null)
            StopCoroutine(_invokeSymptomCoroutine);
    }

    private void InvokeSymptom()
    {
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
            IsSymptom = ProbabilityHelper.Draw(_invokeRate);
            if (IsSymptom)
                InvokeBroken();

            yield return _invokePeriodDelay;
        }

        yield return null;
    }

    private void InvokeBroken()
    {
        Debug.Log($"{gameObject.name} 고장 {_limitTime}초 후 발생");
        if (_invokeBrokenCoroutine == null)
            _invokeBrokenCoroutine = StartCoroutine(InvokeBrokenRoutine());
    }

    Coroutine _invokeBrokenCoroutine;
    private WaitForSeconds _limitTimeDelay;
    IEnumerator InvokeBrokenRoutine()
    {
        yield return _limitTimeDelay;

        IsRepaired = false;
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

    /// <summary>
    /// 수리 상태를 초기화(전조증상 해결 완료 or 고장 수리 완료)
    /// </summary>
    public void ResetRepairState()
    {
        IsSymptom = true;
        IsRepaired = true;
        _curRepairCount = 0;
        _invokeSymptomCoroutine = null;
        _invokeBrokenCoroutine = null;

        InvokeSymptom();

        Debug.Log($"{gameObject.name} 수리 상태 초기화: _curRepairCount={_curRepairCount}, IsRepaired={IsRepaired}");
    }

    #region TestCode
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ResetRepairState();
    }

    public void Symptom()
    {
        Debug.Log($"{gameObject.name} 전조증상 발생!");
        GetComponent<Renderer>().material.color = Color.gray;
    }

    public void Broken()
    {
        Debug.Log($"{gameObject.name} 고장 발생!");
        GetComponent<Renderer>().material.color = Color.black;
    }
    #endregion
}
