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
            Debug.Log($"{i + 1}번째 거래처와 UI 연동");

            string limitTime = $"Info{i + 1}_LimitTime";
            string NPCImage = $"Info{i + 1}_NPCImage";
            string NPCName = $"Info{i + 1}_NPCName";
            //string slot1 = $"Info{i + 1}_Slot1";
            //string slot2 = $"Info{i + 1}_Slot2";
            //string slot3 = $"Info{i + 1}_Slot3";

            GetUI<TextMeshProUGUI>(limitTime).text = GetLimitTime(questList[i].questTimer);
            // 이미지 갱신
            GetUI<TextMeshProUGUI>(NPCName).text = CSVManager.Instance.Correspondents[truckList[i].correspondentId].correspondent_name;
            for (int cropIdx = 0; cropIdx < questList[i].requiredItems.Count; cropIdx++)
                GetUI<QuestItemSlot>($"Info{i + 1}_Slot{cropIdx + 1}").OnUpdate(questList, i, cropIdx);

            //GetUI<QuestItemSlot>(slot1).OnUpdate(questList, i, 0);
            //GetUI<QuestItemSlot>(slot2).OnUpdate(questList, i, 1);
            //GetUI<QuestItemSlot>(slot3).OnUpdate(questList, i, 2);
        }
    }

    private string GetLimitTime(float seconds)
    {
        int min = (int)(seconds / 60f);
        int sec = (int)(seconds % 60f);

        return $"{min:00}:{sec:00}";
    }
}
