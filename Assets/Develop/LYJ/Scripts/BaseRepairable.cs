using Photon.Pun;
using UnityEngine;

public abstract class BaseRepairable : MonoBehaviour, IRepairable
{
    protected Repair _repair;
    protected bool _isBroken;
    private bool _isSymptomSolved = false; // 전조 증상 해결 여부

    protected virtual ParticleSystem SymptomParticle { get; }
    protected virtual ParticleSystem BrokenParticle { get; }

    protected virtual string SymptomSoundKey => null; // 전조 증상 사운드 키 값
    protected virtual string BrokenSoundKey => null; // 고장 사운드 키 값

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
        SymptomParticle?.Play(); // 전조 증상 파티클 재생

        // 전조 증상 사운드 재생
        if (!string.IsNullOrEmpty(SymptomSoundKey))
        {
            //Debug.Log("전조 증상 사운드 재생");
            SoundManager.Instance.PlaySFXLoop(SymptomSoundKey, 0.5f);
        }
        MessageDisplayManager.Instance.ShowMessage($"전조 증상 발생! 테블릿을 확인해주세요!, 5f");
    }

    public virtual bool Broken() // 반환값 추가
    {
        if (_isSymptomSolved)
        {
            //Debug.Log($"전조 증상이 해결되었으므로 고장이 발생하지 않습니다.");
            return false; // 고장 발생하지 않음
        }

        _isBroken = true;
        SymptomParticle?.Stop();
        BrokenParticle?.Play(); // 고장 파티클 재생

        // 고장 사운드 재생 및 전조 증상 사운드 중지
        if (!string.IsNullOrEmpty(BrokenSoundKey))
        {
            //Debug.Log("전조 증상 사운드 멈춤 및 고장 사운드 재생");
            SoundManager.Instance.StopSFXLoop(SymptomSoundKey);
            SoundManager.Instance.PlaySFXLoop(BrokenSoundKey, 0.5f);
        }

        MessageDisplayManager.Instance.ShowMessage($"고장 발생! 망치로 수리해주세요!, 5f");
        //Debug.LogError($"{gameObject.name}: 고장 발생!");
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
            return;
        }

        SymptomParticle?.Stop();
        _repair.IsSymptom = false;
        _isSymptomSolved = true;
        _repair.ResetRepairState();

        // 전조 증상 사운드 정지
        if (!string.IsNullOrEmpty(SymptomSoundKey))
        {
            SoundManager.Instance.StopSFXLoop(SymptomSoundKey);
        }
        MessageDisplayManager.Instance.ShowMessage($"전조 증상이 해결되었습니다!, 5f");
        //Debug.LogError($"{gameObject.name}: 전조 증상이 해결되었습니다!");
    }

    public virtual void SolveBroken()
    {
        _isBroken = false;
        BrokenParticle?.Stop();
        _repair.ResetRepairState();

        // 고장 사운드 정지
        if (!string.IsNullOrEmpty(BrokenSoundKey))
        {
            SoundManager.Instance.StopSFXLoop(BrokenSoundKey);
        }
        //MessageDisplayManager.Instance.ShowMessage($"{gameObject.name}: 수리되었습니다!");
        //Debug.LogError($"{gameObject.name}: 수리되었습니다!");
    }

    public virtual bool IsBroken()
    {
        return _isBroken && !_repair.IsRepaired;
    }
}
