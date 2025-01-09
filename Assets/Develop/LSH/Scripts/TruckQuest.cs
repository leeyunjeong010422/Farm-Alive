using Fusion;
using Mono.Cecil;
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class TruckQuest : MonoBehaviourPun
{
    [SerializeField] int truckId;
    [SerializeField] TruckController truckSpawner;
    [SerializeField] public Text questText;
    [SerializeField] GameObject[] npcPrefabs;
    [SerializeField] Transform npcPosition;
    [SerializeField] public GameObject npcPrefab;

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

                //List<int> truckIds = new List<int>();
                List<int> itemIndexes = new List<int>();
                List<int> requiredCounts = new List<int>();

                int otherItem = 0;
                for (int i = 0; i < boxTrigger.requiredItems.Count; i++)
                {
                    QuestManager.RequiredItem item = boxTrigger.requiredItems[i];
                    for (int j = 0; j < QuestManager.Instance.questsList[truckId].requiredItems.Count; j++)
                    {
                        if (item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[j].itemPrefab.name ||
                            item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[j].itemPrefab.name + "(Clone)")
                        {
                            Debug.Log("이름이 같음");
                            Debug.Log($"{QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount} <= {item.requiredcount}");
                            if (QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount <= 0)
                                break;
                            Debug.Log("갯수 통과");

                            //truckIds.Add(truckId);
                            itemIndexes.Add(j);
                            requiredCounts.Add(item.requiredcount);
                            break;
                        }
                        if (item.itemPrefab.name != QuestManager.Instance.questsList[truckId].requiredItems[j].itemPrefab.name ||
                            item.itemPrefab.name != QuestManager.Instance.questsList[truckId].requiredItems[j].itemPrefab.name + "(Clone)")
                        {
                            Debug.Log($"다른 작물 : {item.itemPrefab.name}, 아이템 카운트는 : {boxTrigger.requiredItems.Count} <= {otherItem}");
                            otherItem++;

                            if (boxTrigger.requiredItems.Count <= otherItem)
                            {
                                Debug.Log("Null텍스트");
                                photonView.RPC(nameof(FieldItem), RpcTarget.AllBuffered);
                                PhotonNetwork.Destroy(other.gameObject);
                            }
                        }
                    }
                }

                if (itemIndexes.Count != 0 || requiredCounts.Count != 0)
                {
                    int[] itemIndexArray = itemIndexes.ToArray();
                    int[] requiredCountArray = requiredCounts.ToArray();
                    QuestManager.Instance.CountUpdate(/*truckIdArray*/truckId, itemIndexArray, requiredCountArray, boxView.ViewID, otherItem);
                }
                
            }
        }
    }

    public void SetID(int truckId, TruckController truckController, int npcNumber)
    {
        Debug.Log("실행");
        this.truckId = truckId;
        this.truckSpawner = truckController;
        Debug.Log($"오브젝트 ID: {truckId}");

        SpawnNpc(npcNumber);
    }

    public void ChangeID(int truckId)
    {
        this.truckId = truckId;
        Debug.Log($"오브젝트 ID: {truckId}");
    }

    public void SpawnNpc(int npcNumber)
    {
        int corTemp = npcNumber - 311;
        if (corTemp < 20)
        {
            corTemp = corTemp / 10;
        }
        else
        {
            corTemp = corTemp - 18;
        }

        npcPrefab = PhotonNetwork.Instantiate(npcPrefabs[corTemp].name, npcPosition.position, Quaternion.identity);
        npcPrefab.transform.SetParent(npcPosition.transform);
        Debug.Log($"오브젝트 ID: {npcPrefab.name}");
    }

    private void OnDestroy()
    {
        if (truckSpawner != null)
        {
            QuestManager.Instance.truckList.RemoveAt(truckId);
            truckSpawner.ClearSlot(truckId);
            truckSpawner.ClearPositionSlot(truckId);
            Debug.Log("삭제완료");
        }
    }

    [PunRPC]
    public void FieldItem()
    {
        npcPrefab.GetComponent<NpcTextView>().NpcText();
    }
}