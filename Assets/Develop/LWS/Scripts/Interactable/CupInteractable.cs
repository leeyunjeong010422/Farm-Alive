using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CupInteractable : MonoBehaviourPun, IPunObservable
{
    [Tooltip("액체가 흐르는 파티클")]
    [SerializeField] public ParticleSystem particleSystemLiquid;
    
    [Tooltip("액체가 차 있을 초기값 (1.0이면 꽉 참)")]
    [SerializeField] float _fillAmount = 1.0f;

    [Tooltip("기울일 경우 흘러내릴 비율")]
    [SerializeField] public float pourRate = 0.1f;

    // [Tooltip("액체가 흐르는 사운드")]
    // [SerializeField] AudioClip _pouringSound;

    // private AudioSource _audioSource;

    private bool _isPouring = false;

    private void OnEnable()
    {
        if (particleSystemLiquid != null)
        {
            Debug.LogWarning("물 흐르는 파티클 정지");
            particleSystemLiquid.Stop();
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
            Debug.LogWarning("뒤집혀서물이나오는상태");
            // 붓는 중이 아닌데 새로 붓기 시작 -> 상태 전환
            if (!_isPouring)
            {
                _isPouring = true;
            }

            // 액체량 감소
            _fillAmount -= pourRate * Time.deltaTime;
            if (_fillAmount < 0f) _fillAmount = 0f;

            // 파티클/사운드 (로컬에서 재생)
            if (particleSystemLiquid != null && particleSystemLiquid.isStopped)
                particleSystemLiquid.Play();

            SoundManager.Instance.PlaySFXLoop("SFX_NutrientContainerPoured");
        }
        else
        {
            // 더 이상 붓지 않는 상황
            if (_isPouring)
            {
                Debug.LogWarning("뒤집힘정지");
                _isPouring = false;
            }

            // 파티클/사운드 정지
            if (particleSystemLiquid != null) particleSystemLiquid.Stop();
            SoundManager.Instance.StopSFXLoop("SFX_NutrientContainerPoured");
        }
    }

    private void UpdateEffects()
    {
        if (_isPouring && _fillAmount > 0)
        {
            if (particleSystemLiquid != null && particleSystemLiquid.isStopped)
                particleSystemLiquid.Play();

            SoundManager.Instance.PlaySFXLoop("SFX_NutrientContainerPoured");
        }
        else
        {
            // 붓고 있지 않거나 액체가 없으면 정지
            if (particleSystemLiquid != null) particleSystemLiquid.Stop();
            SoundManager.Instance.StopSFXLoop("SFX_NutrientContainerPoured");
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
