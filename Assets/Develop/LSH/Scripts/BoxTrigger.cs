using Fusion;
using GameData;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using static QuestManager;

public class BoxTrigger : MonoBehaviourPun
{
    [SerializeField] public List<RequiredItem> requiredItems;
    [SerializeField] public BoxCover boxCover;
    [SerializeField] public List<int> idList = new List<int>();
    [SerializeField] private string boxTag = "Box";
    [SerializeField] private string unTag = "Untagged";

    [SerializeField] public delegate void OnRequiredItemsChanged(List<RequiredItem> items);
    [SerializeField] public event OnRequiredItemsChanged RequiredItemsChanged;


    private void Start()
    {
        boxCover = GetComponent<BoxCover>();
        boxCover.OnIsOpenChanged += NotifyRequiredItemsChanged;
    }

    private void OnEnable()
    {
        Debug.Log("리스트");
        requiredItems = new List<RequiredItem>();
        NotifyRequiredItemsChanged();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tape"))
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
    }

    public void CountUpdate(int viewId, bool isBool)
    {
        PhotonView itemView = PhotonView.Find(viewId);

        if (!isBool)
        {
            idList.Add(viewId);
            SoundManager.Instance.PlaySFX("SFX_CropInBox");
            Crop cropView = itemView.GetComponent<Crop>();
            if (requiredItems.Count > 0)
            {
                foreach (QuestManager.RequiredItem item in requiredItems)
                {
                    if (item.itemPrefab.name == itemView.gameObject.name)
                    {
                        item.requiredcount += cropView.Value;
                        NotifyRequiredItemsChanged();
                        return;
                    }
                }
                requiredItems.Add(new RequiredItem(itemView.gameObject, cropView.Value));
            }
            else
            {
                requiredItems.Add(new RequiredItem(itemView.gameObject, cropView.Value));
            }

            NotifyRequiredItemsChanged();
        }
        else
        {
            SoundManager.Instance.PlaySFX("SFX_CropOutBox");
            Crop cropView = itemView.GetComponent<Crop>();
            if (requiredItems.Count > 0)
            {
                for (int i = requiredItems.Count - 1; i >= 0; i--)
                {
                    if (requiredItems[i].itemPrefab.name == itemView.gameObject.name)
                    {
                        requiredItems[i].requiredcount -= cropView.Value;

                        if (requiredItems[i].requiredcount <= 0)
                        {
                            if (idList.Contains(itemView.ViewID))
                            {
                                idList.Remove(itemView.ViewID);
                                Debug.Log($"리스트에서 {itemView} 제거");
                            }
                            requiredItems.RemoveAt(i);
                        }

                        NotifyRequiredItemsChanged();
                        return;
                    }
                }
            }
        }
    }

    public void CompleteTaping()
    {
        photonView.RPC(nameof(SyncTaping), RpcTarget.All);
    }

    [PunRPC]
    private void SyncTaping()
    {
        boxCover.IsPackaged = true;
        boxCover.tape.SetActive(true);
        SoundManager.Instance.PlaySFX("SFX_Tape");
        Debug.Log($"테이핑 완료: {this.name}");
    }

    private void NotifyRequiredItemsChanged()
    {
        RequiredItemsChanged?.Invoke(requiredItems);
    }

    public void OnGrab(SelectEnterEventArgs args)
    {
        gameObject.tag = unTag;
    }

    public void OnRelease(SelectExitEventArgs args)
    {
        gameObject.tag = boxTag;
    }
}