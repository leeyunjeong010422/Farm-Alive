using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviourPunCallbacks
{
    [Tooltip("몇 번 때려야 고쳐지는지")]
    [SerializeField] int _maxRepairCount;
    private int _curRepairCount = 0;

    /// <summary>
    /// 수리가 완료되었는지 여부를 나타내는 속성
    /// </summary>
    public bool IsRepaired { get; private set; } = false;

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
            IsRepaired = true; //발전기에서 망치로 1차 수리가 되었다는 걸 알아야 2차 수리 (휠 + 시동줄)을 할 수 있음
            enabled = false; //수리 완료된 후 스크립트 비활성화 하면 계속 때려도 _maxRepairCount 이후엔 작동 X
            // TODO: 수리 완료 로직 (ex: 오브젝트 파괴, 애니메이션, 상태 변환 등)
        }
    }
}
