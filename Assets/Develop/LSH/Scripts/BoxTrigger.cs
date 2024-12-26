using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxTrigger : MonoBehaviourPun
{
    [SerializeField] public GameObject boxCover1;
    [SerializeField] public GameObject boxCover2;
    [SerializeField] public GameObject boxTape;
    [SerializeField] bool isBoxClose = false;
    [SerializeField] bool isBoxSealed = false;

    private void OnTriggerEnter(Collider other)
    {      
        
        if (other.CompareTag("Item"))
        {
            PhotonView itemView = other.GetComponent<PhotonView>();
            if (itemView == null || !itemView.IsMine)
                return;

            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null && !grabInteractable.isSelected)
                return;

            if (QuestManager.Instance.currentQuest == null)
                return;

            GameObject[] requiredItems = QuestManager.Instance.currentQuest.requiredItems;
            bool isValidItem = false;

            foreach (GameObject item in requiredItems)
            {
                if (item.name == other.gameObject.name)
                {
                    isValidItem = true;
                    break;
                }
            }

            if (!isValidItem)
                return;

            QuestManager.Instance.UpdateCount();

            if (QuestManager.Instance.IsQuestComplete())
            {
                CloseCover();
            }
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
        else if (other.CompareTag("Item"))
        {
            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null && !grabInteractable.isSelected)
                return;

            QuestManager.Instance.ExitItemCount();
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
}