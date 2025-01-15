using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateQuestUI(List<QuestManager.Quest> questList)
    {
        if (QuestManager.Instance.questsList.Count < 0)
            return;

        for (int i = 0; i < questList.Count; i++)
        {
            string displayText = "";

            QuestManager.Quest quest = questList[i];
            displayText += $"<b>{i + 1}번째 {quest.questName}</b>\n";

            foreach (var item in quest.requiredItems)
            {
                displayText += $"작물 이름 : {item.itemPrefab.name}\n\t-필요한 개수 : {item.requiredcount}\n\t-성공 여부: {(item.isSuccess ? "완료" : "미완료")})\n";
            }

            displayText += $"퀘스트 완료 여부: {(quest.isSuccess ? "완료" : "미완료")}\n\n";

            QuestManager.Instance.truckList[i].questText.text = displayText;
        }
    }
}