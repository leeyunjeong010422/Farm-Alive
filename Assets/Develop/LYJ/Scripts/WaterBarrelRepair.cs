using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBarrelRepair : MonoBehaviour
{
    [SerializeField] private Repair _repair;
    [SerializeField] private DialInteractable _waterDial;
    [SerializeField] private bool _isBroken;

    private void Start()
    {
        _repair = GetComponent<Repair>();

        if (_repair == null )
        {
            Debug.LogError("Repair 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        _repair.OnSymptomRaised.AddListener(Symptom);
        _repair.OnBrokenRaised.AddListener(Broken);

        _waterDial = GetComponentInChildren<DialInteractable>();

        if (_waterDial == null)
        {
            Debug.LogError("WaterDial이 존재하지 않습니다.");
        }

        _waterDial._onAngleChanged.AddListener(OnDialAngleChanged);
    }

    // 다이얼 각도 변경 이벤트 처리
    private void OnDialAngleChanged(float angle)
    {
        if (_repair == null || !_repair.IsSymptom) // 전조 증상이 없으면 처리하지 않음
            return;

        if (Mathf.Approximately(angle, _waterDial.maxAngle)) // 다이얼이 끝까지 회전했을 때
        {
            SolveSymptom();
        }
    }

    // 전조 증상 발생 처리
    public void Symptom()
    {
        _isBroken = false;
        MessageDisplayManager.Instance.ShowMessage("급수장치 전조증상 발생!");
    }

    // 고장 발생 처리
    public void Broken()
    {
        if (_isBroken == false)
        {
            Debug.Log("전조 증상이 해결되었으므로 고장이 발생하지 않습니다.");
            return;
        }

        _isBroken = true;
        MessageDisplayManager.Instance.ShowMessage("급수장치가 고장났습니다!");
    }

    // 수리 상태 확인 (전조 증상과 고장 상태를 구분) -> 고장 상태 여부 반환
    public bool IsBroken()
    {
        return _isBroken && !_repair.IsRepaired;
    }

    // 전조 증상 해결 처리
    public void SolveSymptom()
    {
        _isBroken = false;
        _repair.IsSymptom = false;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage("전조 증상이 해결되었습니다!");
    }

    // 고장 수리 처리
    public void SolveBroken()
    {
        _isBroken = false;
        _repair.ResetRepairState();
        MessageDisplayManager.Instance.ShowMessage("수리되었습니다!");
    }
}
