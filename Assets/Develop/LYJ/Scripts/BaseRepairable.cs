using Photon.Pun;
using UnityEngine;

public abstract class BaseRepairable : MonoBehaviour, IRepairable
{
    protected Repair _repair;
    protected bool _isBroken;
    private bool _isSymptomSolved = false; // 전조 증상 해결 여부

    protected virtual void Start()
    {
        _repair = GetComponent<Repair>();
        if (_repair == null)
        {
            Debug.LogError($"{gameObject.name}: Repair 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        _repair.OnSymptomRaised.AddListener(Symptom);
        _repair.OnBrokenRaised.AddListener(HandleBroken);
        _repair.OnBrokenSolved.AddListener(SolveBroken);
        _repair.OnSymptomSolved.AddListener(SolveSymptom);
    }

    public virtual void Symptom()
    {
        _isBroken = false;
        _isSymptomSolved = false; // 전조 증상 해결 여부 초기화
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 전조 증상 발생!");
    }

    public virtual bool Broken() // 반환값 추가
    {
        if (_isSymptomSolved)
        {
            Debug.Log($"{gameObject.name}: 전조 증상이 해결되었으므로 고장이 발생하지 않습니다.");
            return false; // 고장 발생하지 않음
        }

        _isBroken = true;
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 고장 발생!");
        Debug.LogError($"{gameObject.name}: 고장 발생!");
        return true; // 고장이 발생함
    }

    public void HandleBroken()
    {
        Broken();
    }

    public virtual void SolveSymptom()
    {
        // 고장 상태에서는 전조 증상 해결 불가
        if (_isBroken)
        {
            MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 이미 고장난 상태에서는 전조 증상을 해결할 수 없습니다.");
            return;
        }

        _repair.IsSymptom = false;
        _isSymptomSolved = true;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 전조 증상이 해결되었습니다!");
        Debug.LogError($"{gameObject.name}: 전조 증상이 해결되었습니다!");
    }

    public virtual void SolveBroken()
    {
        _isBroken = false;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 수리되었습니다!");
        Debug.Log("수리완료");
    }

    public virtual bool IsBroken()
    {
        return _isBroken && !_repair.IsRepaired;
    }
}
