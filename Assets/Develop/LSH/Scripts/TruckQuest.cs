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
    public bool check = false;

    [SerializeField] GameObject truckCover1, truckCover2;

    private void Start()
    {
        canDelivery = true;
        endRotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!canDelivery)
                return;

            if (other.CompareTag("Box"))
            {
                XRGrabInteractable interactable = other.GetComponent<XRGrabInteractable>();
                if (interactable.isSelected)
                    return;

                BoxTrigger boxTrigger = other.GetComponent<BoxTrigger>();
                if (boxTrigger == null)
                    return;

                if (!boxTrigger.boxCover.IsPackaged)
                    return;

                if (check)
                    return;

                check = true;

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
                            Debug.Log($"{QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount} <= {item.requiredcount}");

                            if (QuestManager.Instance.questsList[truckId].requiredItems[j].requiredcount <= 0)
                                break;

                            isCorrect = true;

                            itemIndexes.Add(j);
                            requiredCounts.Add(item.requiredcount);
                            break;
                        }
                    }

                    if (!isCorrect)
                    {
                        otherItem++;

                        if (boxTrigger.requiredItems.Count <= otherItem)
                            photonView.RPC(nameof(FieldItem), RpcTarget.AllBuffered, boxView.ViewID);
                    }
                }

                if (itemIndexes.Count != 0 || requiredCounts.Count != 0)
                {
                    int[] itemIndexArray = itemIndexes.ToArray();
                    float[] requiredCountArray = requiredCounts.ToArray();

                    QuestManager.Instance.CountUpdate(truckId, itemIndexArray, requiredCountArray, boxView.ViewID, otherItem);
                }

            }
        }
    }

    public void SetID(int truckId, TruckController truckController, int npcNumber)
    {
        this.truckId = truckId;
        this.truckSpawner = truckController;

        SpawnNpc(npcNumber);
    }

    public void ChangeID(int truckId)
    {
        this.truckId = truckId;
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

            photonView.RPC(nameof(NpcSync), RpcTarget.AllBuffered, viewId.ViewID);
        }
    }

    private void OnDestroy()
    {
        if (truckSpawner != null)
        {
            QuestManager.Instance.truckList.RemoveAt(truckId);
            truckSpawner.ClearSlot(truckId);
            truckSpawner.ClearPositionSlot(truckId);
        }
    }

    [PunRPC]
    public void FieldItem(int viewId)
    {
        PhotonView box = PhotonView.Find(viewId);
        npcPrefab.GetComponent<NpcTextView>().NpcText(false);
        box.transform.position = new Vector3(0, -100, 0);
        box.GetComponent<Rigidbody>().isKinematic = true;
        check = false;
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
        SoundManager.Instance.PlaySFX("SFX_Truck_Door_Close");
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

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }
}