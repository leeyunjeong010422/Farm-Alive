using Fusion;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using static QuestManager;

public class BoxTrigger : MonoBehaviourPun//, IPunObservable
{
    [SerializeField] public GameObject boxTape;
    [SerializeField] public List<RequiredItem> requiredItems;
    [SerializeField] bool isFirstItem = false;
    [SerializeField] BoxCover boxCover;
    [SerializeField] public List<int> idList = new List<int>();
    [SerializeField] Collider openCollider;
    [SerializeField] Collider closedCollider;

    private void Start()
    {
        boxCover = GetComponent<BoxCover>();
    }

    private void OnEnable()
    {
        Debug.Log("리스트");
        requiredItems = new List<RequiredItem>();        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Crop"))
        {
            PhotonView itemView = other.transform.parent.parent.GetComponent<PhotonView>();
            if (itemView == null || !itemView.IsMine)
                return;

            if (!boxCover.IsOpen)
                return;

            if (idList.Contains(itemView.ViewID))
                return;

            idList.Add(itemView.ViewID);

            CropInteractable grabInteractable = other.transform.parent.parent.GetComponent<CropInteractable>();
            Debug.Log(grabInteractable);
            if (grabInteractable != null && !grabInteractable.isSelected)
                return;

            if (QuestManager.Instance.currentQuest == null)
                return;
            
            photonView.RPC(nameof(UpCount), RpcTarget.All, itemView.ViewID);
            
            Debug.Log("종료");
        }

        else if (!boxCover.IsPackaged && other.CompareTag("Tape"))
        {
            Debug.Log("포장시작");
            if (boxCover.IsOpen)
                return;

            Taping taping = other.GetComponent<Taping>();
            if (taping != null && !boxCover.IsPackaged)
            {
                Debug.Log("상자테이핑시작준비");
                taping.StartTaping(this, this.boxCover);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tape"))
        {
            Taping taping = other.GetComponent<Taping>();
            if (taping != null)
            {
                taping.StopTaping();
            }
        }
        else if (other.CompareTag("Crop"))
        {
            CropInteractable grabInteractable = other.transform.parent.parent.GetComponent<CropInteractable>();
            if (grabInteractable != null && !grabInteractable.isSelected)
                return;

            PhotonView itemView = other.transform.parent.parent.GetComponent<PhotonView>();

            photonView.RPC(nameof(DownCount), RpcTarget.All, itemView.ViewID);
        }
    }

    [PunRPC]
    private void UpCount(int viewId)
    {
        PhotonView itemView = PhotonView.Find(viewId);
        if (isFirstItem)
        {
            foreach (QuestManager.RequiredItem item in requiredItems)
            {
                Debug.Log(requiredItems);
                if (item.itemPrefab.name == itemView.gameObject.name)
                {
                    item.requiredcount++;
                    Debug.Log("카운트업");
                    return;
                }
                else
                {
                    requiredItems.Add(new RequiredItem(itemView.gameObject, 1));
                    Debug.Log("추가");
                }
            }
        }

        if (!isFirstItem)
        {
            requiredItems.Add(new RequiredItem(itemView.gameObject, 1));
            isFirstItem = true;
        }

        Rigidbody itemRigid = itemView.GetComponent<Rigidbody>();
        itemRigid.drag = 10;
        itemRigid.angularDrag = 1;
    }

    [PunRPC]
    private void DownCount(int viewId)
    {
        PhotonView itemView = PhotonView.Find(viewId);
        if (requiredItems.Count > 0)
        {
            for (int i = requiredItems.Count - 1; i >= 0; i--)
            {
                if (requiredItems[i].itemPrefab.name == itemView.gameObject.name)
                {
                    requiredItems[i].requiredcount--;

                    Rigidbody itemRigid = itemView.GetComponent<Rigidbody>();
                    itemRigid.drag = 0;
                    itemRigid.angularDrag = 0.05f;

                    if (requiredItems[i].requiredcount == 0)
                    {
                        if (idList.Contains(itemView.ViewID))
                        {
                            idList.Remove(itemView.ViewID);
                            Debug.Log($"리스트에서 {itemView} 제거");
                        }
                        requiredItems.RemoveAt(i);
                        isFirstItem = false;
                    }
                }
            }
        }
    }
}