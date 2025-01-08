using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoistureRemoverRepair : BaseRepairable
{
    [SerializeField] private XRKnobDial _moistureRemoverDial;

    protected override void Start()
    {
        base.Start();

        _moistureRemoverDial = GetComponentInChildren<XRKnobDial>();
        if (_moistureRemoverDial == null)
        {
            Debug.LogError("MoistureRemoverDial이 존재하지 않습니다.");
            return;
        }

        _moistureRemoverDial.onValueChange.AddListener(OnKnobValueChanged);
    }

    private void OnKnobValueChanged(float value)
    {
        if (_repair == null || !_repair.IsSymptom) // 전조 증상이 없으면 처리하지 않음
            return;

        // Knob 값이 1.0f에 도달했을 때 전조 증상 해결
        if (Mathf.Approximately(value, 1.0f))
        {
            SolveSymptom();
        }
    }
}
