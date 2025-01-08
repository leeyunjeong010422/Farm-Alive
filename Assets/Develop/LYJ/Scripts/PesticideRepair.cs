using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class PesticideRepair : BaseRepairable
{
    [SerializeField] private XRKnobDial _pesticideDial;

    protected override void Start()
    {
        base.Start();

        _pesticideDial = GetComponentInChildren<XRKnobDial>();
        if (_pesticideDial == null)
        {
            Debug.LogError("PesticideDial이 존재하지 않습니다.");
            return;
        }

        _pesticideDial.onValueChange.AddListener(OnKnobValueChanged);
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
        if (_pesticideDial != null)
        {
            _pesticideDial.value = 0.0f;
        }
    }
}
