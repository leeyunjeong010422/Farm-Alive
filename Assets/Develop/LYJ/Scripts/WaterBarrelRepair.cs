using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class WaterBarrelRepair : BaseRepairable
{
    [SerializeField] private XRKnob _waterDial;

    protected override void Start()
    {
        base.Start();

        _waterDial = GetComponentInChildren<XRKnob>();
        if (_waterDial == null)
        {
            Debug.LogError("WaterDial이 존재하지 않습니다.");
            return;
        }

        _waterDial.onValueChange.AddListener(OnKnobValueChanged);
    }

    private void OnKnobValueChanged(float value)
    {
        if (_repair == null || !_repair.IsSymptom) // 전조 증상이 없으면 처리하지 않음
            return;

        // Knob 값이 1.0f에 도달했을 때 전조 증상 해결
        if (Mathf.Approximately(value, 1.0f))
        {
            SolveSymptom();

            ResetKnobValue();
        }
    }

    // Knob 값을 0으로 초기화
    private void ResetKnobValue()
    {
        if (_waterDial != null)
        {
            _waterDial.value = 0.0f;
        }
    }
}
