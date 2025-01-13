using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    public bool isAllParticleStoped = false;

    [System.Serializable]
    public class ParticleInfo
    {
        public string key;
        public ParticleSystem particle;
    }


    [Header("전역 파티클 목록")]
    [SerializeField] private ParticleInfo[] _particleInfo;

    private Dictionary<string, ParticleSystem> _particleDict = new Dictionary<string, ParticleSystem>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var info in _particleInfo)
        {
            _particleDict.Add(info.key, info.particle);
            info.particle.Stop();
        }

        Debug.Log("모든 파티클 딕셔너리화, 재생중지 완료");

        isAllParticleStoped = true;
    }

    /// <summary>
    /// 파티클 플레이, duration초 후 stop
    /// 만약, 특정하게 지속시간이 정해져 있지 않은 파티클이라면,
    /// float인자 = 0 으로 전달
    /// 이후, 멈출 타이밍에 stopParticle(key(name))호출
    /// </summary>
    /// <param name="key"></param>
    /// <param name="duration"></param>
    public void PlayParticle(string key, float duration)
    {
        if (!_particleDict.ContainsKey(key))
            return;

        ParticleSystem particle = _particleDict[key];

        particle.Play();

        if (duration > 0)
        {
            StartCoroutine(StopParticleAfter(particle, duration));
        }
    }

    /// <summary>
    /// 특정 파티클 Stop
    /// </summary>
    public void StopParticle(string key)
    {
        ParticleSystem particle = _particleDict[key];
        if (particle.isPlaying)
            particle.Stop();
    }

    /// <summary>
    /// 모든 파티클 Stop
    /// 게임 시작시에 실행
    /// </summary>
    public void StopAllParticles()
    {
        foreach (var kv in _particleDict)
        {
            var ps = kv.Value;
            if (ps.isPlaying)
                ps.Stop();
        }
    }

    private IEnumerator StopParticleAfter(ParticleSystem particle, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (particle.isPlaying)
        {
            particle.Stop();
        }
    }
}
