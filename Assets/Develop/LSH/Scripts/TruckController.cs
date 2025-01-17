using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviourPun
{
    [SerializeField] GameObject truckPrefab;

    [SerializeField] Transform[] transformCounts;
    [SerializeField] bool[] transformNumbers = new bool[4];
    [SerializeField] public GameObject[] truckObjects = new GameObject[4];

    public void ClearSlot(int truckId)
    {
        truckObjects[truckId] = null;

        for (int i = truckId; i < truckObjects.Length - 1; i++)
        {
            if (truckObjects[i + 1] == null)
                return;
            truckObjects[i] = truckObjects[i + 1]; // 다음 슬롯을 현재 슬롯으로 이동
            truckObjects[i + 1] = null; // 마지막 슬롯은 null로 설정
            truckObjects[i].GetComponent<TruckQuest>().ChangeID(i);
        }
    }

    public void ClearPositionSlot(int truckId)
    {
        transformNumbers[truckId] = false;
    }

    public void CreateTruck(int npcNumber)
    {
        for (int i = 0; i < transformNumbers.Length; i++)
        {
            if (!transformNumbers[i])
            {
                for (int j = 0; j < truckObjects.Length; j++)
                {
                    if (truckObjects[j] == null)
                    {
                        transformNumbers[i] = true;

                        if (PhotonNetwork.IsMasterClient)
                        {
                            GameObject truck = PhotonNetwork.Instantiate(truckPrefab.name, transformCounts[i].position, Quaternion.identity);
                            PhotonView truckView = truck.GetComponent<PhotonView>();
                            photonView.RPC(nameof(SyncTruck), RpcTarget.AllBuffered, j, truckView.ViewID, npcNumber);
                        }
                        return;
                    }
                }
            }
        }
    }

    [PunRPC]
    public void SyncTruck(int j, int viewId, int npcNumber)
    {
        PhotonView truckView = PhotonView.Find(viewId);
        TruckQuest truckQuest = truckView.GetComponent<TruckQuest>();

        truckQuest.SetID(j, this, npcNumber);

        QuestManager.Instance.truckList.Add(truckQuest);

        truckObjects[j] = truckView.gameObject;
    }
}