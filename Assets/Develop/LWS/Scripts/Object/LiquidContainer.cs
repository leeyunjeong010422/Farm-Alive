using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LiquidContainer : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("초기에 차 있을 정도 (0.0으로 시작이 기본)")]
    [SerializeField] float _fillAmount = 0.0f;
    float _maxAmount = 1.0f;

    /// <summary>
    /// 외부에서 물을 채울경우 호출할 함수.
    /// amount는 1을 가득 찼다고 가정했을 때의 비율입니다.
    /// </summary>
    /// <param name="amount"></param>
    public void ReceiveLiquid(float amount)
    {
        if (!photonView.IsMine)
            return;

        _fillAmount += amount;
        _fillAmount = MathF.Min(_fillAmount, _maxAmount);

        StartCoroutine(LiquidCheck());
    }
    public IEnumerator LiquidCheck()
    {
        Debug.Log($"현재 컨테이너 내부 액체양{_fillAmount}");

        yield return new WaitForSeconds(5f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 내가 Owner라면, _fillAmount를 다른 클라이언트에게 전송
            stream.SendNext(_fillAmount);
        }
        else
        {
            // 내 소유가 아니라면, Owner로부터 _fillAmount를 받음
            _fillAmount = (float)stream.ReceiveNext();
        }
    }
}
