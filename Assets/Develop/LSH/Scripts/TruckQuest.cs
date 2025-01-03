using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TruckQuest : MonoBehaviour
{
    [SerializeField] int truckId;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("충돌");
        if (PhotonNetwork.IsMasterClient)
        {
            if (other.CompareTag("Box"))
            {
                XRGrabInteractable interactable = other.GetComponent<XRGrabInteractable>();
                if (interactable.isSelected)
                    return;

                Debug.Log("박스");
                BoxTrigger boxTrigger = other.GetComponent<BoxTrigger>();
                if (boxTrigger == null)
                    return;

                PhotonView boxView = other.GetComponent<PhotonView>();

                List<int> truckIds = new List<int>();
                List<int> itemIndexes = new List<int>();
                List<int> requiredCounts = new List<int>();

                for (int i = 0; i < boxTrigger.requiredItems.Count; i++)
                {
                    QuestManager.RequiredItem item = boxTrigger.requiredItems[i];
                    for (int j = 0; j < QuestManager.Instance.questsList[truckId].requiredItems.Count; j++)
                    {
                        if (item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[j].itemPrefab.name ||
                            item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[i].itemPrefab.name + "(Clone)")
                        {
                            Debug.Log("이름이 같음");
                            Debug.Log($"{QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount} <= {item.requiredcount}");
                            if (QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount <= 0)
                                break;
                            Debug.Log("갯수 통과");

                            truckIds.Add(truckId);
                            itemIndexes.Add(j);
                            requiredCounts.Add(item.requiredcount);
                            break;
                        }
                    }
                }

                int[] truckIdArray = truckIds.ToArray();
                int[] itemIndexArray = itemIndexes.ToArray();
                int[] requiredCountArray = requiredCounts.ToArray();
                QuestManager.Instance.CountUpdate(truckIdArray, itemIndexArray, requiredCountArray, boxView.ViewID);
            }
        }
    }
}