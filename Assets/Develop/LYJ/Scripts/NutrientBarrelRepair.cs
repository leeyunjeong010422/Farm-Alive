using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class NutrientBarrelRepair : BaseRepairable
{
    [SerializeField] private XRKnobDial _nutrientDial;
    [SerializeField] private ParticleSystem _symptomParticle;
    [SerializeField] private ParticleSystem _brokenParticle;

    protected override ParticleSystem SymptomParticle => _symptomParticle;
    protected override ParticleSystem BrokenParticle => _brokenParticle;

    protected override string SymptomSoundKey => "SFX_Supply_Device_Error";
    protected override string BrokenSoundKey => "SFX_Supply_Problem";

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

        _nutrientDial = GetComponentInChildren<XRKnobDial>();
        if (_nutrientDial == null)
        {
            Debug.LogError("NutrientDial이 존재하지 않습니다.");
            return;
        }

        _nutrientDial.onValueChange.AddListener(OnKnobValueChanged);
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
