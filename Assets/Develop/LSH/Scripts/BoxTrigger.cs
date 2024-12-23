using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxTrigger : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌");
        if (!other.CompareTag("Item"))
            return;
        Debug.Log("태그 통과");

        PhotonView itemView = other.GetComponent<PhotonView>();
        if (itemView == null || !itemView.IsMine) 
            return;
        Debug.Log("뷰 통과");

        if (QuestManager.Instance.currentQuest == null)
            return;
        Debug.Log("퀘스트 통과");

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
        Debug.Log("유효아이템 통과");

        QuestManager.Instance.UpdateCount();
        Destroy(other.gameObject);        
    }
}