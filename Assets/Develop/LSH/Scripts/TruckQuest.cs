using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckQuest : MonoBehaviour
{
    [SerializeField] int truckId;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌");
        if (other.CompareTag("Box"))
        {
            Debug.Log("박스");
            BoxTrigger boxTrigger = other.GetComponent<BoxTrigger>();
            if (boxTrigger == null)
                return;

            foreach (QuestManager.RequiredItem item in boxTrigger.requiredItems)
            {
                for (int i = 0; i < QuestManager.Instance.questsList[truckId].requiredItems.Count; i++)
                {
                    if(item.itemPrefab.name == QuestManager.Instance.questsList[truckId].requiredItems[i].itemPrefab.name)
                    {
                        Debug.Log("이름이 같음");
                        if(item.requiredcount == QuestManager.Instance.questsList[truckId].requiredItems[i].requiredcount)
                        {
                            Debug.Log("카운트가 같음");
                            QuestManager.Instance.SuccessQuest(truckId, i);
                            break;
                        }
                    }
                }
            }
        }
    }
}
