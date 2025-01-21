using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TemperatureDial : BaseRepairable
{
    [SerializeField] private XRKnobDial _temperatureDial;
    [SerializeField] private ParticleSystem _symptomParticle;
    [SerializeField] private ParticleSystem _brokenParticle;

    protected override ParticleSystem SymptomParticle => _symptomParticle;
    protected override ParticleSystem BrokenParticle => _brokenParticle;

    protected override string SymptomSoundKey => "SFX_Machine_Error";
    protected override string BrokenSoundKey => "SFX_Thermostat_Malfunction";

    protected override void Start()
    {
        base.Start();

        if (_symptomParticle == null)
            _symptomParticle = transform.Find("SymptomParticle")?.GetComponentInChildren<ParticleSystem>(true);

        if (_brokenParticle == null)
            _brokenParticle = transform.Find("BrokenParticle")?.GetComponentInChildren<ParticleSystem>(true);

        if (_symptomParticle == null)
            Debug.LogWarning($"{gameObject.name}: 'SymptomParticle' 파티클을 찾을 수 없습니다.");

        if (_brokenParticle == null)
            Debug.LogWarning($"{gameObject.name}: 'BrokenParticle' 파티클을 찾을 수 없습니다.");

        _temperatureDial = GetComponent<XRKnobDial>();
        if (_temperatureDial == null)
        {
            Debug.LogError("TemperatureDial이 존재하지 않습니다.");
            return;
        }

        _temperatureDial.onValueChange.AddListener(OnKnobValueChanged);
    }

    private void OnKnobValueChanged(float value)
    {
        // 1. 고장 상태 확인
        if (_repair != null && IsBroken())
        {
            MessageDisplayManager.Instance.ShowMessage($"온도 조절 장치를 수리해야 사용 할 수 있습니다.");
            return;
        }

        // 2. 전조 증상 해결 로직
        if (_repair != null && _repair.IsSymptom) // 전조 증상이 있을 경우
        {
            if (Mathf.Approximately(value, 1.0f) || Mathf.Approximately(value, 0.0f))
            {
                SolveSymptom();
                return; // 전조 증상이 해결되면 이후 로직 실행하지 않음
            }
        }

        // 3. 이벤트 처리 로직
        TemperatureEvents(value);
    }

    private void TemperatureEvents(float value)
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
