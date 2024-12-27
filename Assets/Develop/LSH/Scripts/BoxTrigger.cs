using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using static QuestManager;

public class BoxTrigger : MonoBehaviourPun, IPunObservable
{
    [SerializeField] public GameObject boxCover1;
    [SerializeField] public GameObject boxCover2;
    [SerializeField] public GameObject boxTape;
    [SerializeField] public List<RequiredItem> requiredItems;
    [SerializeField] bool isBoxClose = false;
    [SerializeField] bool isBoxSealed = false;
    [SerializeField] bool isFirstItem = false;

    private void OnEnable()
    {
        Debug.Log("리스트");
        requiredItems = new List<RequiredItem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crop"))
        {
            PhotonView itemView = other.GetComponent<PhotonView>();
            if (itemView == null || !itemView.IsMine)
                return;

            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null && !grabInteractable.isSelected)
                return;

            if (QuestManager.Instance.currentQuest == null)
                return;

            if (isFirstItem)
            {
                foreach (QuestManager.RequiredItem item in requiredItems)
                {
                    Debug.Log(requiredItems);
                    if (item.itemPrefab.name == other.gameObject.name)
                    {
                        item.requiredcount++;
                        Debug.Log("카운트업");
                    }
                    else
                    {
                        requiredItems.Add(new RequiredItem(other.gameObject, 1));
                        Debug.Log("추가");
                    }
                }
            }

            if (!isFirstItem)
            {
                requiredItems.Add(new RequiredItem(other.gameObject, 1));
                isFirstItem = true;
            }

            Debug.Log("종료");
            #region 쓸모없어질? 코드
            //if (!isValidItem)
            //    return;

            //QuestManager.Instance.UpdateCount(x, y);

            /*if (QuestManager.Instance.IsQuestComplete())
            {
                CloseCover();
            }*/
            #endregion
        }

        else if (!isBoxSealed && other.CompareTag("Tape"))
        {
            Debug.Log("포장시작");

            Taping taping = other.GetComponent<Taping>();
            if (taping != null && !isBoxSealed)
            {
                Debug.Log("상자테이핑시작준비");
                taping.StartTaping(this);
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
            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null && !grabInteractable.isSelected)
                return;

            if (requiredItems.Count > 0)
            {
                for (int i = requiredItems.Count - 1; i >= 0; i--)
                {
                    if (requiredItems[i].itemPrefab.name == other.gameObject.name)
                    {
                        requiredItems[i].requiredcount--;

                        if (requiredItems[i].requiredcount == 0)
                        {
                            requiredItems.RemoveAt(i);
                            isFirstItem = false;
                        }
                    }
                }
            }
        }
    }

    private void CloseCover()
    {
        if (boxCover1 != null && boxCover2 != null)
        {
            isBoxClose = true;
            Debug.Log("상자 뚜껑이 닫혔습니다!");
        }
    }

    public void SealBox()
    {
        isBoxSealed = true;
        Debug.Log($"{name} 상자가 테이핑으로 포장되었습니다!");
    }

    public bool IsCoverClosed()
    {
        return boxCover1 != null && boxCover2 != null && isBoxClose;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isBoxClose);
            stream.SendNext(isBoxSealed);
            stream.SendNext(isFirstItem);

            stream.SendNext(requiredItems.Count);
            foreach (var item in requiredItems)
            {
                stream.SendNext(item.itemPrefab.name);
                stream.SendNext(item.requiredcount);
            }
        }
        else
        {
            isBoxClose = (bool)stream.ReceiveNext();
            isBoxSealed = (bool)stream.ReceiveNext();
            isFirstItem = (bool)stream.ReceiveNext();

            int itemCount = (int)stream.ReceiveNext();
            requiredItems.Clear();
            for (int i = 0; i < itemCount; i++)
            {
                GameObject itemName = (GameObject)stream.ReceiveNext();
                int itemCountReceived = (int)stream.ReceiveNext();
                requiredItems.Add(new RequiredItem(itemName, itemCountReceived));
            }
        }
    }
}