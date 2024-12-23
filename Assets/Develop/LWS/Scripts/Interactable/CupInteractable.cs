using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CupInteractable : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("액체가 흐르는 파티클")]
    [SerializeField] ParticleSystem _particleSystemLiquid;
    
    [Tooltip("액체가 차 있을 초기값 (1.0이면 꽉 참)")]
    [SerializeField] float _fillAmount = 1.0f;

    [Tooltip("기울일 경우 흘러내릴 비율")]
    [SerializeField] float _pourRate = 0.1f;

    [Tooltip("액체가 흐르는 사운드")]
    [SerializeField] AudioClip _pouringSound;

    private AudioSource _audioSource;

    private bool _isPouring = false;

    private void OnEnable()
    {
        if (_particleSystemLiquid != null)
        {
            _particleSystemLiquid.Stop();
        }
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null)
        {
            _audioSource.clip = _pouringSound;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Pour();
        }
        else
        {
            UpdateEffects();
        }
    }

    private void Pour()
    {
        // 컵이 뒤집혔는지: transform.up과 Vector3.down의 내적이 양수면 "업벡터가 아래쪽으로 향한다"
        bool shouldPour = (Vector3.Dot(transform.up, Vector3.down) > 0 && _fillAmount > 0);

        if (shouldPour)
        {
            // 붓는 중이 아닌데 새로 붓기 시작 -> 상태 전환
            if (!_isPouring)
            {
                _isPouring = true;
            }

            // 액체량 감소
            _fillAmount -= _pourRate * Time.deltaTime;
            if (_fillAmount < 0f) _fillAmount = 0f;

            // 파티클/사운드 (로컬에서 재생)
            if (_particleSystemLiquid != null && _particleSystemLiquid.isStopped)
                _particleSystemLiquid.Play();

            if (_audioSource != null && !_audioSource.isPlaying)
                _audioSource.Play();

            // 레이캐스트 후 LiquidContainer 처리
            RaycastHit hit;
            if (_particleSystemLiquid != null)
            {
                if (Physics.Raycast(_particleSystemLiquid.transform.position, Vector3.down, out hit, 50.0f))
                {
                    LiquidContainer receiver = hit.collider.GetComponent<LiquidContainer>();
                    if (receiver != null)
                    {
                        float amount = _pourRate * Time.deltaTime;
                        receiver.ReceiveLiquid(amount);
                    }
                }
            }
        }
        else
        {
            // 더 이상 붓지 않는 상황
            if (_isPouring)
            {
                _isPouring = false;
            }

            // 파티클/사운드 정지
            if (_particleSystemLiquid != null) _particleSystemLiquid.Stop();
            if (_audioSource != null) _audioSource.Stop();
        }
    }

    private void UpdateEffects()
    {
        if (_isPouring && _fillAmount > 0)
        {
            if (_particleSystemLiquid != null && _particleSystemLiquid.isStopped)
                _particleSystemLiquid.Play();

            if (_audioSource != null && !_audioSource.isPlaying)
                _audioSource.Play();
        }
        else
        {
            // 붓고 있지 않거나 액체가 없으면 정지
            if (_particleSystemLiquid != null) _particleSystemLiquid.Stop();
            if (_audioSource != null) _audioSource.Stop();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_fillAmount);
            stream.SendNext(_isPouring);
        }
        else
        {
            _fillAmount = (float)stream.ReceiveNext();
            _isPouring = (bool)stream.ReceiveNext();
        }
    }
}
