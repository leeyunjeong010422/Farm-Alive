using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CorrespondentUpdater : UIBinder
{
    public int correspondentID;

    private void Start()
    {
        SubscribeTruck();
    }

    private void SubscribeTruck()
    {
        QuestManager.Instance.OnTruckUpdated.AddListener(UpdateUI);
    }

    private void UpdateUI(List<QuestManager.Quest> questList, List<TruckQuest> truckList)
    {
        if (QuestManager.Instance.questsList.Count < 0)
            return;

        for (int i = 0; i < questList.Count; i++)
        {
            string limitTime = $"Info{i + 1}_LimitTime";
            string NPCImage = $"Info{i + 1}_NPCImage";
            string NPCName = $"Info{i + 1}_NPCName";
            string slot1 = $"Info{i + 1}_Slot1";
            string slot2 = $"Info{i + 1}_Slot2";
            string slot3 = $"Info{i + 1}_Slot3";

            GetUI<TextMeshProUGUI>(limitTime).text = GetLimitTime(questList[i].questTimer);
            // 이미지 갱신
            GetUI<TextMeshProUGUI>(NPCName).text = CSVManager.Instance.Correspondents[truckList[i].correspondentId].correspondent_name;
            GetUI<QuestItemSlot>(slot1).OnUpdate(questList, i, 0);
            GetUI<QuestItemSlot>(slot2).OnUpdate(questList, i, 1);
            GetUI<QuestItemSlot>(slot3).OnUpdate(questList, i, 2);
        }



        for (int i = 0; i < QuestManager.Instance.questCount; i++)
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

    private string GetLimitTime(float seconds)
    {
        int min = (int)(seconds / 60f);
        int sec = (int)(seconds % 60f);

        return $"{min:00}:{sec:00}";
    }
}
