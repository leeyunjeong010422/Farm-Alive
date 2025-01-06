using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    [SerializeField] GameObject truckPrefab;

    [SerializeField] Transform[] transformCounts;
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

    public void CreateTruck()
    {
        for (int i = 0; i < truckObjects.Length; i++)
        {
            if (truckObjects[i] == null)
            {
                GameObject truck = PhotonNetwork.Instantiate(truckPrefab.name, transformCounts[i].position, Quaternion.identity);
                TruckQuest truckQuest = truck.GetComponent<TruckQuest>();
                QuestManager.Instance.truckList.Add(truckQuest);

                truckQuest.SetID(i, this);

                truckObjects[i] = truck;
                break;
            }
        }
    }
}