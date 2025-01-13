using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TemperatureDial : BaseRepairable
{
    [SerializeField] private XRKnobDial _temperatureDial;

    protected override void Start()
    {
        base.Start();

        _temperatureDial = GetComponent<XRKnobDial>();
        if (_temperatureDial = null)
            return;

        _temperatureDial.onValueChange.AddListener(OnKnobValueChanged);
    }

    private void OnKnobValueChanged(float value)
    {
        if (Mathf.Approximately(value, 1f)) // 다이얼을 오른쪽으로 돌려서 온도를 올릴 때,
        {
            if (!EventManager.Instance._activeEventsID.Contains(421))
                // 온도 하강 이벤트가 진행중이 아니면
                return;

            EventManager.Instance.EndEvent(421);
            ParticleManager.Instance.StopParticle("421");
        }

        if (Mathf.Approximately(value, 0f)) // 다이얼을 왼쪽으로 돌려서 온도를 내릴 때,
        {
            if (!EventManager.Instance._activeEventsID.Contains(441))
                // 온도 상승 이벤트가 진행중이 아니면
                return;

            EventManager.Instance.EndEvent(441);
            ParticleManager.Instance.StopParticle("441");
        }
    }
}
