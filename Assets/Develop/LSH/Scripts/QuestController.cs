using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

public class QuestController : MonoBehaviour
{
    public IEnumerator QuestCountdown(Quest quest)
    {
        float startTime = Time.time;

        while (quest.questTimer > 0)
        {
            yield return null;
            quest.questTimer -= Time.deltaTime;

            if (quest.isSuccess)
            {
                yield break;
            }

            if(quest != null)
            QuestManager.Instance.UpdateUI();
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
}