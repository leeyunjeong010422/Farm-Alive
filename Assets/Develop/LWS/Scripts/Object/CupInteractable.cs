using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CupInteractable : MonoBehaviour
{
    public ParticleSystem _particleSystemLiquid;
    
    // 액체가 차 있을 초기값 (1.0이면 꽉 참)
    private float _fillAmount = 1.0f;
    // 외부에서 값을 바꿀 경우 (호스로 물을 채운다거나)
    public float FillAmount { get { return _fillAmount; } set { _fillAmount = value; } }

    // 초당 흘러내릴 비율
    private float _pourRate = 0.1f;

    // 물이 흐르는 사운드
    public AudioClip _pouringSound;
    private AudioSource _audioSource;

    private void OnEnable()
    {
        _particleSystemLiquid.Stop();
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _pouringSound;
    }

    private void Update()
    {
        // 뒤집어 져 있고, 액체가 남아있을 때
        if (Vector3.Dot(transform.up, Vector3.down) > 0 && _fillAmount > 0)
        {
            // 파티클, 따르는 소리 재생
            if (_particleSystemLiquid.isStopped && _particleSystemLiquid != null && _audioSource != null)
            {
                _particleSystemLiquid.Play();
                _audioSource.Play();
                // TODO:에셋이후
                Debug.Log("액체 따르는 소리 및 파티클 재생");
            }

            // 잔량 감소
            _fillAmount -= _pourRate * Time.deltaTime;
            _fillAmount = Mathf.Max(_fillAmount, 0.0f);
            
            RaycastHit hit;
            // 파티클 시스템 기준으로 아래로 50길이의 레이를 발사해서,
            if (Physics.Raycast(_particleSystemLiquid.transform.position, Vector3.down, out hit, 50.0f))
            {
                // hit이 LiquidReceiver (물통, 비료통에 존재) 컴포넌트를 가지고 있을 경우
                LiquidContainer receiver = hit.collider.GetComponent<LiquidContainer>();
                Debug.Log("리퀴드 컨테이너에 레이 도달");

                // 물, 비료 타입에 따른 함수 실행
                // => 타입 구분 없이 붓는 비율 만큼 액체 채우기
                if (receiver != null)
                {
                    float amount = _pourRate * Time.deltaTime;
                    receiver.ReceiveLiquid(amount);
                    Debug.Log("리퀴드 컨테이너 ReceiveLiquid 호출");
                }
            }
        }
        else
        {
            _particleSystemLiquid.Stop();
            _audioSource.Stop();
            Debug.Log("액체 따르는 소리 및 파티클 중지");
        }
    }
}
