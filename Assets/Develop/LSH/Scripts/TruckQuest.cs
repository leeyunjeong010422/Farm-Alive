using Fusion;
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using System;
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
    [SerializeField] float rotationSpeed = 2.0f;
    [SerializeField] public int correspondentId;
    private Quaternion endRotation;
    [SerializeField] bool canDelivery;

    [SerializeField] GameObject truckCover1, truckCover2;

    private void Start()
    {
        canDelivery = true;
        endRotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌");
        if (PhotonNetwork.IsMasterClient)
        {
            if (!canDelivery)
                return;

            if (other.CompareTag("Box"))
            {
                Debug.Log("박스");
                BoxTrigger boxTrigger = other.GetComponent<BoxTrigger>();
                if (boxTrigger == null)
                    return;

                if (!boxTrigger.boxCover.IsPackaged)
                    return;

                PhotonView boxView = other.GetComponent<PhotonView>();

                List<int> itemIndexes = new List<int>();
                List<float> requiredCounts = new List<float>();

                int otherItem = 0;
                for (int i = 0; i < boxTrigger.requiredItems.Count; i++)
                {
                    bool isCorrect = false;

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

                            isCorrect = true;

                            itemIndexes.Add(j);
                            requiredCounts.Add(item.requiredcount);
                            break;
                        }
                    }

                    if (!isCorrect)
                    {
                        Debug.Log($"다른 작물 : {item.itemPrefab.name}, 아이템 카운트는 : {boxTrigger.requiredItems.Count} <= {otherItem}");
                        otherItem++;

                        if (boxTrigger.requiredItems.Count <= otherItem)
                        {
                            photonView.RPC(nameof(FieldItem), RpcTarget.AllBuffered);
                            PhotonNetwork.Destroy(other.gameObject);
                        }
                    }
                }

                if (itemIndexes.Count != 0 || requiredCounts.Count != 0)
                {
                    int[] itemIndexArray = itemIndexes.ToArray();
                    float[] requiredCountArray = requiredCounts.ToArray();

                    boxTrigger.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    QuestManager.Instance.CountUpdate(/*truckIdArray*/truckId, itemIndexArray, requiredCountArray, boxView.ViewID, otherItem);
                }

            }
        }
    }

    public void SetID(int truckId, TruckController truckController, int npcNumber)
    {
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

        correspondentId = npcNumber;

        if (PhotonNetwork.IsMasterClient)
        {
            npcPrefab = PhotonNetwork.Instantiate(npcPrefabs[corTemp].name, npcPosition.position, npcPosition.rotation);
            
            PhotonView viewId = npcPrefab.GetComponent<PhotonView>();
            
            photonView.RPC(nameof(NpcSync), RpcTarget.AllBuffered, viewId.ViewID );
        }
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
        npcPrefab.GetComponent<NpcTextView>().NpcText(false);
    }

    [PunRPC]
    public void NpcSync(int viewId)
    {
        PhotonView npcView = PhotonView.Find(viewId);

        if (npcPrefab == null)
        {
            npcPrefab = npcView.gameObject;
        }

        npcPrefab.transform.SetParent(npcPosition.transform);
    }

    public void CloseCover()
    {
        canDelivery = false;
        StartCoroutine(SmoothRotate(truckCover1, truckCover2));
    }

    private IEnumerator SmoothRotate(GameObject truckCover1, GameObject truckCover2)
    {
        Quaternion startRotation1 = truckCover1.transform.rotation;
        Quaternion startRotation2 = truckCover2.transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < rotationSpeed)
        {
            truckCover1.transform.rotation = Quaternion.Lerp(startRotation1, endRotation, elapsedTime / rotationSpeed);
            truckCover2.transform.rotation = Quaternion.Lerp(startRotation2, endRotation, elapsedTime / rotationSpeed);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        truckCover1.transform.rotation = endRotation;
        truckCover2.transform.rotation = endRotation;

        yield return new WaitForSeconds(2f);
        PhotonNetwork.Destroy(npcPrefab);

        elapsedTime = 0f;

        while (elapsedTime < 5)
        {
            transform.position += transform.forward * 5 * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }
}