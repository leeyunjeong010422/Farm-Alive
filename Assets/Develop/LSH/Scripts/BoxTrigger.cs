using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxTrigger : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Item"))
            return;

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
        Destroy(other.gameObject);        
    }
}