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
        _repair.OnBrokenRaised.AddListener(Broken);
    }

    public virtual void Symptom()
    {
        _isBroken = false;
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 전조 증상 발생!");
    }

    public virtual void Broken()
    {
        if (_isSymptomSolved)
        {
            Debug.Log($"{gameObject.name}: 전조 증상이 해결되었으므로 고장이 발생하지 않습니다.");
            return;
        }

        _isBroken = true;
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 고장 발생!");
    }

    public virtual void SolveSymptom()
    {
        _isBroken = false;
        _repair.IsSymptom = false;
        _repair.ResetRepairState();
        _isSymptomSolved = true;
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 전조 증상이 해결되었습니다!");
    }

    public virtual void SolveBroken()
    {
        _isBroken = false;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 수리되었습니다!");
    }

    public virtual bool IsBroken()
    {
        return _isBroken && !_repair.IsRepaired;
    }
}
