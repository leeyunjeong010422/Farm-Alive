using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviourPunCallbacks
{
    [Tooltip("몇 번 때려야 고쳐지는지")]
    [SerializeField] int _maxRepairCount;
    private int _curRepairCount = 0;

    [PunRPC]
    public void RPC_PlusRepairCount()
    {
        if (!enabled)
        {
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;

        _curRepairCount++;
        Debug.Log($"수리중: {_curRepairCount}/{_maxRepairCount}");
        if (_curRepairCount >= _maxRepairCount)
        {
            // 수리 완료
            Debug.Log($"[{photonView.ViewID}] 수리 완료!");
            // TODO: 수리 완료 로직 (ex: 오브젝트 파괴, 애니메이션, 상태 변환 등)
        }
    }
}
