using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

public class QuestController : MonoBehaviourPun
{
    private const float _SyncInterval = 0.5f;

    public IEnumerator QuestCountdown(Quest quest)
    {
        float startTime = Time.time;

        while (quest.questTimer > 0)
        {
            yield return new WaitForSeconds(_SyncInterval);
            quest.questTimer -= Time.time - startTime;
            startTime = Time.time;

            photonView.RPC(nameof(SyncQuestTimer), RpcTarget.Others, quest, quest.questTimer);

            if (quest.isSuccess)
            {
                yield break;
            }
        }

        QuestFailed(quest);
    }

    private void QuestFailed(Quest quest)
    {
        if (QuestManager.Instance.questsList.Contains(quest))
        {
            int index = QuestManager.Instance.questsList.IndexOf(quest);
            if (index >= 0 && QuestManager.Instance.truckList.Count > index)
            {
                if(PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(QuestManager.Instance.truckList[index].gameObject);
            }

            QuestManager.Instance.questsList.Remove(quest);
            QuestManager.Instance.UpdateUI();
        }
    }

    [PunRPC]
    public void SyncQuestTimer(Quest quest, float questTimer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            quest.questTimer = questTimer;
            QuestManager.Instance.UpdateUI();
        }
    }
}