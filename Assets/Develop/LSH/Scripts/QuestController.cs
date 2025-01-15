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

            int index = QuestManager.Instance.questsList.IndexOf(quest);
            photonView.RPC(nameof(SyncQuestTimer), RpcTarget.All, index, quest.questTimer);

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
                if (PhotonNetwork.IsMasterClient)
                {
                    QuestManager.Instance.truckList[index].npcPrefab.GetComponent<NpcTextView>().NpcText();
                    QuestManager.Instance.truckList[index].CloseCover();
                }
            }

            QuestManager.Instance.questsList.Remove(quest);
            QuestManager.Instance.UpdateUI();
        }
    }

    [PunRPC]
    public void SyncQuestTimer(int index, float questTimer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            QuestManager.Instance.questsList[index].questTimer = questTimer;
        }
        QuestManager.Instance.UpdateUI();
    }
}